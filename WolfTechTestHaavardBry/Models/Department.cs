using System.Text.Json.Serialization;

namespace WolfTechTestHaavardBry.Models
{
    public class Department
    {
        [JsonPropertyName("OID")]
        public int OID { get; set; }

        [JsonPropertyName("Title")]
        public string ?Title { get; set; }

        [JsonPropertyName("NumDecendants")]
        public int NumDecendants { get; set; }

        [JsonPropertyName("Color")]
        public string ?Color { get; set; }

        [JsonIgnore]
        public int? DepartmentParent_OID { get; set; }

        [JsonPropertyName("Departments")]
        public List<Department> ?Departments { get; set; }
    }
}
