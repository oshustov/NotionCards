using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using NotionCards.NotionBrowser.Entities;

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
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
      _httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_options.NotionSecretId}");
    }

    public async Task FetchPageBlocks(string pageId)
    {
      var requestUri = $"/v1/blocks/{pageId}/children";
      var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

      var requestBody = JsonSerializer.Serialize(new
      {
        page_size = 1
      });

      message.Content = new StringContent(requestBody, new MediaTypeHeaderValue("application/json"));

      using var response = await _httpClient.SendAsync(message);
      var body = await response.Content.ReadAsStringAsync();
    }

    public async Task<NotionDbPagesResponse> FetchDbPages()
    {
      var databaseId = _options.DatabaseId;

      var requestUri = $"/v1/databases/{databaseId}/query";
      var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
      
      var requestBody = JsonSerializer.Serialize(new
      {
        page_size = 1
      });

      message.Content = new StringContent(requestBody, new MediaTypeHeaderValue("application/json"));

      using var response = await _httpClient.SendAsync(message);
      var body = await response.Content.ReadAsStringAsync();

      var deserialized = JsonSerializer.Deserialize<NotionDbPagesResponse>(body);
      return deserialized;
    }
  }
}
