using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace kursovaya;

public class SheetAccess
{
    private static int num = 0;
    public static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};
    public static readonly string AppicationName = "Kursach";
    private static readonly string SpreadsheetId = "1bo0GKcxshwQdxYkCQ2st8Y671TIHu5ZNtsEXlLujSEE";
    private static readonly string sheet = "table";
    public static SheetsService service;

    public static void InitializeConnection()
    {
        GoogleCredential credential;

        using (var stream = new FileStream("sheets_service_account.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AppicationName
            });
        }
    }

    public static IList<IList<object>> ReadEntries(string cell)
    {
        var range = $"{sheet}!{cell}";
        var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

        var response = request.Execute();
        var values = response.Values;

        return values;
    }

    public static void WriteCell(string cell, string data)
    {
        var range = $"{sheet}!{cell}";
        var valueRange = new ValueRange();

        var oblist = new List<object> {$"{data}"};
        valueRange.Values = new List<IList<object>> {oblist};

        var writeRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
        writeRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var writeResponse = writeRequest.Execute();
    }

    public static void WriteOwner(string cell, string name, string number)
    {
        var range = $"people!{cell}";
        var valueRange = new ValueRange();

        var oblist = new List<object> {$"{name}", $"{number}"};
        valueRange.Values = new List<IList<object>> {oblist};

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
        appendRequest.ValueInputOption =
            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = appendRequest.Execute();
    }
    
    public static void DeleteEntry(string cell)
    {
        var range = $"{sheet}!{cell}";
        var requestBody = new ClearValuesRequest();

        var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
        var deleteReponse = deleteRequest.Execute();
    }

    public static void Download(string filetype)
    {
        using (var client = new WebClient())
        using (var completedSignal = new AutoResetEvent(false))
        {
            client.DownloadFileAsync(
                new Uri(
                    $"https://www.docs.google.com/feeds/download/spreadsheets/Export?key=1bo0GKcxshwQdxYkCQ2st8Y671TIHu5ZNtsEXlLujSEE&exportFormat={filetype}"),
                $"kursach.{filetype}");
        }
    }
}