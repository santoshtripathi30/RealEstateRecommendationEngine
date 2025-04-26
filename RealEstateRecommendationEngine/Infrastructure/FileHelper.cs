namespace RealEstateRecommendationEngine.Infrastructure
{
    public static class FileHelper
    {
        private const string DataFolderName = "DataFiles";

        public static string GetDataFileDirectory()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrWhiteSpace(baseDirectory))
                throw new InvalidOperationException("Base directory is not available.");

            string fullPath = Path.GetFullPath(Path.Combine(baseDirectory, DataFolderName));

            // Create directory if it doesn't exist
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }
    }

    public static class ConfigurationKeys
    {
        public const string DefaultConnection = "DefaultConnection";
    }
}
