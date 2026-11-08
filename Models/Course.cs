using System.Text.Json.Serialization;
 
namespace WestcoastEducation.Desktop.Models;
 
public class Course : IEntity

{

    [JsonPropertyName("_id")]

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

    public string? Id { get; set; }
 
    public string CourseNumber { get; set; } = "";

    public string Title { get; set; } = "";

    public int Duration { get; set; }

    public string DurationUnit { get; set; } = "Weeks";

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;

    public bool IsDistance { get; set; }

    public string Teacher { get; set; } = "";
 
    [JsonIgnore]

    public string DeliveryMode => IsDistance ? "Distance" : "Classroom";
 
    [JsonIgnore]

    public string Period => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
 
    public override string ToString() => $"{CourseNumber} - {Title}";

}
 