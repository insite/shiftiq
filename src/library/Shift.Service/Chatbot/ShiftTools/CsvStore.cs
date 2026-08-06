using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

using DocumentFormat.OpenXml.Spreadsheet;

using InSite.Application.Files.Read;
using InSite.Domain.Reports;

using Shift.Common;
using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot.ShiftTools;

internal record ExportCsvResult(Guid FileId, string FileName, string FileUrl, string[] Columns, int RowCount);

internal class CsvStore(IStorageServiceAsync storageService)
{
    public async Task<ExportCsvResult> ExportAsync(ToolContext toolContext, string fileNameBase, IReadOnlyCollection<object> list)
    {
        var columns = GetColumns(list);
        
        if (list.Count == 0)
            return new ExportCsvResult(Guid.Empty, "", "", columns, 0);

        var fileName = CreateFileName(fileNameBase);
        var csv = Encoding.UTF8.GetBytes(SerializeCsv(list, columns));

        await using var stream = new MemoryStream(csv);

        var file = await storageService.CreateAsync(
            stream,
            fileName,
            toolContext.Principal.OrganizationId,
            toolContext.Principal.UserId,
            ObjectIdentifiers.Temporary,
            FileObjectType.Temporary,
            new FileProperties { DocumentName = fileName },
            [new FileClaim { ObjectIdentifier = toolContext.Principal.UserId, ObjectType = FileClaimObjectType.Person }]
        );

        var fileUrl = storageService.GetFileUrl(file);

        return new ExportCsvResult(file.FileIdentifier, file.FileName, fileUrl, columns, list.Count);
    }

    public async Task<string?> ConvertToTableAsync(IPrincipal principal, Guid fileId, string[] columns, int limit)
    {
        var (_, _, stream) = await storageService.GetFileStreamAndAuthorizeAsync(principal, fileId);
        if (stream == null)
            throw new NotSupportedException();

        string[][] values;

        using (stream)
            values = CsvImportHelper.GetValues(stream, null, false, Encoding.UTF8, limit + 1);

        if (values.Length < 2)
            return null;

        var columnIndexes = GetColumnIndexes(values[0], columns);

        var result = new StringBuilder();

        foreach (var columnIndex in columnIndexes)
            result.Append($"|{values[0][columnIndex]}");

        result.AppendLine("|");
        result.AppendLine($"{string.Join("", Enumerable.Repeat("|-", columnIndexes.Count))}|");

        for (int i = 1; i < values.Length; i++)
        {
            foreach (var columnIndex in columnIndexes)
                result.Append($"|{values[i][columnIndex]}");

            result.AppendLine("|");
        }

        return result.ToString();
    }

    private static List<int> GetColumnIndexes(string[] allColumns, string[] visibleColumns)
    {
        var result = new List<int>();

        foreach (var name in visibleColumns)
        {
            var index = Array.IndexOf(allColumns, name);
            if (index < 0)
                throw new ArgumentOutOfRangeException($"Column {name} is not found");

            result.Add(index);
        }

        return result;
    }

    private static string[] GetColumns(IEnumerable list)
    {
        var itemType = list.GetType().GetGenericArguments()[0];
        var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return [..properties.Select(x => x.Name)];
    }

    private static string CreateFileName(string fileNameBase)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
        return $"{fileNameBase}-{timestamp}.csv";
    }

    private static string SerializeCsv(IReadOnlyCollection<object> list, string[] columns)
    {
        var csv = new CsvExportHelper(list);

        foreach (var columnName in columns)
            csv.AddMapping(columnName, columnName);

        return csv.GetString();
    }
}