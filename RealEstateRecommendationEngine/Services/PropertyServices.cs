using Microsoft.Data.SqlClient;

using RealEstateRecommendationEngine.Infrastructure;
using RealEstateRecommendationEngine.Model;

using System.Text.Json;

namespace RealEstateRecommendationEngine.Services
{
    public interface IPropertyServices
    {
        public void ExportPropertiesToJson();
        public void LoadPropertiesFromJson();

        public List<PropertyInfo> GetAllProperties();
    }

    public class PropertyServices(IConfiguration configuration) : IPropertyServices
    {
        private readonly string _connectionString = configuration.GetConnectionString(ConfigurationKeys.DefaultConnection);
        private List<PropertyInfo> _propertyInfos = [];
        private readonly string jsonPath = Path.Combine(FileHelper.GetDataFileDirectory(), "properties.json");
        public void ExportPropertiesToJson()
        {
            try
            {
                List<PropertyInfo> properties = [];
                PropertyInfo obj;

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand(@"select * from vw_UserPropertyInfo", connection);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        obj = new PropertyInfo();

                        obj.PropertyID = DBToInt(reader["PropertyID"]);
                        obj.Address = DBToString(reader["Address"]);
                        obj.BedRoom = DBToInt(reader["BedRoom"]);
                        obj.CarpetAreaSft = DBToDouble(reader["CarpetAreaSft"]);
                        obj.ConstructionStatus = DBToString(reader["ConstructionStatus"]);
                        obj.Description = DBToString(reader["Description"]);
                        obj.FacingDirection = DBToString(reader["FacingDirection"]);
                        obj.PricePsftTentative = DBToDecimal(reader["PricePsftTentative"]);
                        obj.ProjectName = DBToString(reader["ProjectName"]);
                        obj.SpecBathroom = DBToString(reader["SpecBathroom"]);
                        obj.SpecKitchen = DBToString(reader["SpecKitchen"]);
                        obj.SpecWindows = DBToString(reader["SpecWindows"]);

                        properties.Add(obj);
                    }
                }

                string json = JsonSerializer.Serialize(properties, options: new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
                File.WriteAllText(jsonPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting properties: {ex.Message}");
                throw; // or handle gracefully
            }




        }

        private static string DBToString(object obj)
        {
            return obj == null || obj == DBNull.Value ? string.Empty : obj.ToString()?.Trim() ?? string.Empty;
        }

        private static int DBToInt(object obj)
        {
            return obj != null && obj != DBNull.Value && int.TryParse(obj.ToString(), out int result) ? result : 0;
        }

        private static double DBToDouble(object obj)
        {
            return obj != null && obj != DBNull.Value && double.TryParse(obj.ToString(), out double result) ? result : 0.0;
        }

        private static decimal DBToDecimal(object obj)
        {
            return obj != null && obj != DBNull.Value && decimal.TryParse(obj.ToString(), out decimal result) ? result : 0.0m;
        }


        public void LoadPropertiesFromJson()
        {

            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("Property JSON file not found.");

            var json = File.ReadAllText(jsonPath);
            _propertyInfos = JsonSerializer.Deserialize<List<PropertyInfo>>(json);
        }

        public List<PropertyInfo> GetAllProperties()
        {
            if (_propertyInfos == null || _propertyInfos.Count==0)
            {
                LoadPropertiesFromJson();
            }
            return _propertyInfos;
        }
    }
}
