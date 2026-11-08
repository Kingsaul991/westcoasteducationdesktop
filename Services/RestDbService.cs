using System.Net.Http.Json;

using System.Text.Json;

using WestcoastEducation.Desktop.Models;
 
namespace WestcoastEducation.Desktop.Services;

public class RestDbService<T> : IRestDbService<T> where T : IEntity

{

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
 
    private readonly HttpClient _http;

    private readonly string _collection;
 
    public RestDbService(HttpClient http, string collection)

    {

        _http = http;

        _collection = collection;

    }
 
    public async Task<List<T>> GetAllAsync()

    {

        var items = await _http.GetFromJsonAsync<List<T>>(_collection, Options);

        return items ?? new List<T>();

    }

    public async Task<T> AddAsync(T item)

    {

        var response = await _http.PostAsJsonAsync(_collection, item, Options);

        response.EnsureSuccessStatusCode();
 

        var saved = await response.Content.ReadFromJsonAsync<T>(Options);

        return saved ?? item;

    }

    public async Task UpdateAsync(T item)

    {

        if (string.IsNullOrEmpty(item.Id))

        {

            throw new InvalidOperationException("Cannot update a record that has not been saved yet.");

        }
 
        var response = await _http.PutAsJsonAsync($"{_collection}/{item.Id}", item, Options);

        response.EnsureSuccessStatusCode();

    }

    public async Task DeleteAsync(string id)

    {

        var response = await _http.DeleteAsync($"{_collection}/{id}");

        response.EnsureSuccessStatusCode();

    }

}
 