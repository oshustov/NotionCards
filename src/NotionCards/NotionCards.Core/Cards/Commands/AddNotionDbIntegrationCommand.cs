using System.Text.Json;
using MediatR;
using Notion.Client;
using NotionCards.Core.Dto;
using NotionCards.Core.Entities;
using NotionCards.Core.Notion;
using NotionCards.Storage;

namespace NotionCards.Core.Cards.Commands;

public record AddNotionDbIntegrationCommandResponse(bool IsOkay, string DatabaseTitle);
public class AddNotionDbIntegrationCommand : IRequest<AddNotionDbIntegrationCommandResponse>
{
  public string NotionDbId { get; set; } = "1a1fd67d82db48e18936feeb6866aa84";
  public int? PullingIntervalDays { get; set; }
  public NotionDbFieldMappings FieldMappings { get; set; }

  public class Handler: IRequestHandler<AddNotionDbIntegrationCommand, AddNotionDbIntegrationCommandResponse>
  {
    private readonly AppDbContext _appDbContext;
    private readonly NotionClient _notionClient;
    private readonly NotionClientAdapter _notionClientAdapter;

    public Handler(AppDbContext appDbContext, NotionClient notionClient, NotionClientAdapter notionClientAdapter)
    {
      _appDbContext = appDbContext;
      _notionClient = notionClient;
      _notionClientAdapter = notionClientAdapter;
    }

    public async Task<AddNotionDbIntegrationCommandResponse> Handle(AddNotionDbIntegrationCommand request, CancellationToken cancellationToken)
    {
      try
      {
        var db = await _notionClient.Databases.RetrieveAsync(request.NotionDbId, cancellationToken);

        var currentSetup = await _appDbContext.NotionDbs.FindAsync(db.Id);
        if (currentSetup != null)
        {
          currentSetup.NotionDbName = db.Title[0].PlainText;
          currentSetup.PullingInterval =
            request.PullingIntervalDays.HasValue ? TimeSpan.FromDays(request.PullingIntervalDays.Value) : null;

          await _appDbContext.SaveChangesAsync(cancellationToken);
          return new AddNotionDbIntegrationCommandResponse(true, db.Title[0].PlainText);
        }

        var setup = new NotionDbSetup()
        {
          NotionDbId = db.Id,
          FieldMappings = JsonSerializer.Serialize(request.FieldMappings),
          NotionDbName = db.Title[0].PlainText,
          PullingInterval = request.PullingIntervalDays.HasValue
            ? TimeSpan.FromDays(request.PullingIntervalDays.Value)
            : null
        };

        await _appDbContext.NotionDbs.AddAsync(setup, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        await _notionClientAdapter.ReadEntireDb(request.NotionDbId);

        return new AddNotionDbIntegrationCommandResponse(true, db.Title[0].PlainText);
      }
      catch (Exception e)
      {
        return new AddNotionDbIntegrationCommandResponse(false, null);
      }
    }
  }
}
/*
 * {
  "Title": [
    {
      "PlainText": "My expressions",
      "Href": null,
      "Annotations": {
        "IsBold": false,
        "IsItalic": false,
        "IsStrikeThrough": false,
        "IsUnderline": false,
        "IsCode": false,
        "Color": 0
      },
      "Type": 1
    }
  ],
  "Properties": {
    "Kind": {
      "Id": "%3EPrU",
      "Type": 4,
      "Name": "Kind"
    },
    "Meaning/translation": {
      "Id": "%60k%3CU",
      "Type": 2,
      "Name": "Meaning/translation"
    },
    "Date": {
      "Id": "qQV%5B",
      "Type": 6,
      "Name": "Date"
    },
    "Expression": {
      "Id": "title",
      "Type": 1,
      "Name": "Expression"
    }
  },
  "Parent": {
    "Type": 2
  },
  "Icon": null,
  "Cover": {
    "Caption": null,
    "Type": "external"
  },
  "Url": "https://www.notion.so/1a1fd67d82db48e18936feeb6866aa84",
  "Archived": false,
  "IsInline": true,
  "Description": [],
  "Object": 1,
  "Id": "1a1fd67d-82db-48e1-8936-feeb6866aa84",
  "CreatedTime": "2023-11-20T17:36:00Z",
  "LastEditedTime": "2024-01-09T16:27:00Z",
  "CreatedBy": {
    "Id": "b7f1f7a1-93c1-4428-b884-08d4f79649a9",
    "Object": 3
  },
  "LastEditedBy": {
    "Id": "b7f1f7a1-93c1-4428-b884-08d4f79649a9",
    "Object": 3
  },
  "PublicUrl": null
}
 */ 