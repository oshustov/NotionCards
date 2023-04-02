using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

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
      _httpClient.BaseAddress = new Uri(_options.ApiUrl, UriKind.Absolute);
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
      _httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_options.NotionSecretId}");
    }

    public async Task<NotionResponse> FetchBlocks(string parentId)
    {
      var query = HttpUtility.ParseQueryString(string.Empty);
      query["page_size"] = "50";
      var queryString = query.ToString();

      var message = new HttpRequestMessage(HttpMethod.Get, $"blocks/{parentId}/children?{queryString}");

      using var response = await _httpClient.SendAsync(message);
      var body = await response.Content.ReadAsStringAsync();

      var deserialized = JsonSerializer.Deserialize<NotionResponse>(body);
      return deserialized;
    }

    public async Task<NotionResponse> FetchDbPages(int size)
    {
      var databaseId = _options.DatabaseId;

      var requestUri = $"databases/{databaseId}/query";
      var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

      var requestBody = JsonSerializer.Serialize(new
      {
        page_size = size
      });

      message.Content = new StringContent(requestBody, new MediaTypeHeaderValue("application/json"));

      using var response = await _httpClient.SendAsync(message);
      var body = await response.Content.ReadAsStringAsync();

      var deserialized = JsonSerializer.Deserialize<NotionResponse>(body);
      return deserialized;
    }
  }
}
