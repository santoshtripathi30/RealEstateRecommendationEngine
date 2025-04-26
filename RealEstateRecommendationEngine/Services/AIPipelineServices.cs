using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Trainers;

using RealEstateRecommendationEngine.Infrastructure;
using RealEstateRecommendationEngine.Model;

namespace RealEstateRecommendationEngine.Services
{
    public interface IAIPipelineServices
    {
        void CreateModelZipFile();
        ITransformer GetModel();
        ITransformer LoadModel();
        void ReloadModel();
        PredictionEngine<PropertyRating, PropertyPredictionWithId> GetPredictionEngine();
    }

    public class AIPipelineServices : IAIPipelineServices
    {
        private readonly string _connectionString;
        private readonly MLContext _mlContext;
        private readonly ILogger<AIPipelineServices> _logger;

        private static DataViewSchema? _modelInputSchema;
        private IDataView? _trainingData;
        private static Lazy<ITransformer>? _lazyModel;

        public AIPipelineServices(IConfiguration configuration, ILogger<AIPipelineServices> logger)
        {
            _connectionString = configuration.GetConnectionString(ConfigurationKeys.DefaultConnection)
                ?? throw new ArgumentNullException("Connection string not found.");

            _mlContext = new MLContext();
            _logger = logger;

            _lazyModel = new Lazy<ITransformer>(LoadModel, isThreadSafe: true);
        }

        public void CreateModelZipFile()
        {
            var ratings = LoadRatingsFromDatabase();
            _trainingData = _mlContext.Data.LoadFromEnumerable(ratings);

            // Transform user & property IDs into keys
            var dataPipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("userIdEncoded", nameof(PropertyRating.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("propertyIdEncoded", nameof(PropertyRating.PropertyId)));

            var transformer = dataPipeline.Fit(_trainingData);
            var transformedData = transformer.Transform(_trainingData);

            var trainerOptions = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "propertyIdEncoded",
                LabelColumnName = nameof(PropertyRating.Rating),
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var trainer = _mlContext.Recommendation().Trainers.MatrixFactorization(trainerOptions);
            var trainedModel = trainer.Fit(transformedData);

            var fullModel = transformer.Append(trainedModel);

            var fileName = $"model_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
            var modelPath = Path.Combine(FileHelper.GetDataFileDirectory(), fileName);

            _mlContext.Model.Save(fullModel, _trainingData.Schema, modelPath);

            _logger.LogInformation("Model saved to {Path}", modelPath);
        }

        public ITransformer LoadModel()
        {
            var modelDirectory = FileHelper.GetDataFileDirectory();

            var latestModelFile = Directory.GetFiles(modelDirectory, "model_*.zip")
                .OrderByDescending(f => f)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(latestModelFile) && File.Exists(latestModelFile))
            {
                _logger.LogInformation("Loading ML model from: {File}", latestModelFile);
                return _mlContext.Model.Load(latestModelFile, out _modelInputSchema);
            }

            _logger.LogWarning("No model file found in {Directory}", modelDirectory);
            throw new FileNotFoundException("No trained model found.");
        }

        public ITransformer GetModel()
        {
            if (_lazyModel == null || !_lazyModel.IsValueCreated)
            {
                _logger.LogWarning("Model not initialized. Reloading...");
                ReloadModel();
            }

            return _lazyModel.Value;
        }

        public PredictionEngine<PropertyRating, PropertyPredictionWithId> GetPredictionEngine()
        {
            var model = GetModel();
            return _mlContext.Model.CreatePredictionEngine<PropertyRating, PropertyPredictionWithId>(model);
        }

        public void ReloadModel()
        {
            _lazyModel = new Lazy<ITransformer>(LoadModel, isThreadSafe: true);
            _logger.LogInformation("ML model has been reloaded.");
        }

        private List<PropertyRating> LoadRatingsFromDatabase()
        {
            var ratings = new List<PropertyRating>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("SELECT UserId, PropertyId, Rating FROM vw_UserPropertyRating", connection);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ratings.Add(new PropertyRating
                {
                    UserId = reader.GetString(0),
                    PropertyId = reader.GetString(1),
                    Rating = Convert.ToSingle(reader.GetInt32(2))
                });
            }

            return ratings;
        }
    }
}
