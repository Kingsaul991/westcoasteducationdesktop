using System.Text.Json.Serialization;
 
namespace WestcoastEducation.Desktop.Models;
 
public abstract class Person : IEntity
{
    [JsonPropertyName("_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }
 
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string PersonalIdentityNumber { get; set; } = "";
    public string Street { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string City { get; set; } = "";
 
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
 
    [JsonIgnore]
    public string Address => $"{Street}, {PostalCode} {City}";
 
    public override string ToString() => FullName;
}