namespace RealEstateRecommendationEngine.Model
{
    public class PropertyInfo
    {
        public int PropertyID { get; set; }
        public string Address { get; set; } = string.Empty;
        public int BedRoom { get; set; }
        public double CarpetAreaSft { get; set; }
        public string ConstructionStatus { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FacingDirection { get; set; } = string.Empty;
        public decimal PricePsftTentative { get; set; } 
        public string ProjectName { get; set; } = string.Empty;
        public string SpecBathroom { get; set; } = string.Empty;
        public string SpecKitchen { get; set; } = string.Empty;
        public string SpecWindows { get; set; } = string.Empty;
    }
}
