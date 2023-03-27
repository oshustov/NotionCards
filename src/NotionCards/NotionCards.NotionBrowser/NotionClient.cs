using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace NotionCards.NotionBrowser
{
  public class NotionClient
  {
    private readonly HttpClient _httpClient;
    private readonly NotionOptions _options;

    public NotionClient(HttpClient httpClient, IOptions<NotionOptions> options)
    {
      _httpClient = httpClient;
      _options = options.Value;
      _httpClient.BaseAddress = new Uri(_options.ApiUrl);
    }

    public async Task QueryDatabase()
    {
      var databaseId = _options.DatabaseId;

      var message = new HttpRequestMessage(HttpMethod.Get, $"/v1/databases/{databaseId}");
      
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
      _httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_options.NotionSecretId}");

      using var response = await _httpClient.SendAsync(message);
      var body = await response.Content.ReadAsStringAsync();
    }
  }
}
