using System.Text.Json;
 
namespace WestcoastEducation.Desktop.Services;
 
public class RestDbSettings

{

    public string BaseUrl { get; set; } = "";

    public string ApiKey { get; set; } = "";
 

    public static RestDbSettings Load()

    {

        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(path))

        {

            throw new FileNotFoundException(

                "appsettings.json is missing. Copy appsettings.example.json and fill in your RestDb URL and API key.");

        }
 
        var json = File.ReadAllText(path);

        var settings = JsonSerializer.Deserialize<RestDbSettings>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
 
        if (settings == null || settings.BaseUrl == "" || settings.ApiKey == "")

        {

            throw new InvalidOperationException("appsettings.json must contain baseUrl and apiKey.");

        }
 
        return settings;

    }

    public HttpClient CreateHttpClient()

    {

        var http = new HttpClient

        {

            BaseAddress = new Uri(BaseUrl.TrimEnd('/') + "/rest/")

        };

        http.DefaultRequestHeaders.Add("x-apikey", ApiKey);

        http.DefaultRequestHeaders.Add("cache-control", "no-cache");

        return http;

    }

}
 