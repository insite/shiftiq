using InSite.Application.Files.Read;

using Shift.Common;
using Shift.Contract;
using Shift.Service.Content;

namespace Shift.Service.Directory;

public class PersonImportReporter(FileReader fileReader, IStorageServiceAsync storageService) : IPersonImportReporter
{
    private const string Section4Name = "Section 4";

    public async Task<FileStorageModel?> SaveReportAsync(Guid organizationId, Guid userId, string timeZone, IEnumerable<ImportPersonResult> imports, bool isAutoImport)
    {
        var sections = imports
            .Where(x => x.Status != ImportPersonResult.StatusEnum.Error)
            .GroupBy(x => GetSectionName(x, isAutoImport))
            .OrderBy(x => x.Key)
            .ToList();

        if (sections.Count == 0)
            return null;

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        writer.AutoFlush = true;

        foreach (var section in sections)
        {
            var sectionImports = section.OrderBy(x => x.Input.PersonCode).ToList();
            
            WriteSection(section.Key, sectionImports, writer);
        }

        stream.Position = 0;

        var documentName = await CreateDocumentNameAsync(organizationId, timeZone);

        var file = await storageService.CreateAsync(
            stream,
            documentName,
            organizationId,
            userId,
            organizationId,
            FileObjectType.Organization,
            new FileProperties { DocumentName = documentName, Tag = FileTag.PersonImport },
            null
        );

        return file;
    }

    private async Task<string> CreateDocumentNameAsync(Guid organizationId, string timeZone)
    {
        var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZones.GetInfo(timeZone));
        var documentNameStart = $"Import Report {now.Year} {now.Month.ToString().PadLeft(2, '0')} {now.Day.ToString().PadLeft(2, '0')}";
        var documentName = (string?)null;
        var fileIndex = 0;

        var criteria = new CollectFiles { OrganizationId = organizationId, ObjectId = organizationId, DocumentNameContains = documentNameStart, FileTag = FileTag.PersonImport };
        criteria.DisablePaging();

        var files = await fileReader.CollectAsync(criteria);

        while (documentName == null || files.Any(x => string.Equals(x.DocumentName, documentName, StringComparison.OrdinalIgnoreCase)))
        {
            var prefix = fileIndex == 0 ? "" : $"_{fileIndex}";

            documentName = $"{documentNameStart}{prefix}.txt";

            fileIndex++;
        }

        return documentName;
    }

    private static void WriteSection(string name, List<ImportPersonResult> imports, StreamWriter writer)
    {
        var isSection4 = name == Section4Name;

        writer.WriteLine(name);
        writer.WriteLine();

        foreach (var import in imports)
        {
            writer.WriteLine(import.Input.PersonCode);

            if (isSection4)
                continue;

            if (import.Status == ImportPersonResult.StatusEnum.Modified && import.Original == null)
                writer.WriteLine("Only status was changed");
            else
                WriteChangedValues(import.Input, import.Original, writer);

            writer.WriteLine();
        }
    }

    private static void WriteChangedValues(ImportPerson input, ImportPerson? original, StreamWriter writer)
    {
        var isNew = original == null;

        WriteChangedValue("Email", isNew, original?.UserEmail, input.UserEmail, writer);
        WriteChangedValue("First Name", isNew, original?.UserFirstName, input.UserFirstName, writer);
        WriteChangedValue("Last Name", isNew, original?.UserLastName, input.UserLastName, writer);
        WriteChangedValue("Middle Name", isNew, original?.UserMiddleName, input.UserMiddleName, writer);
        WriteChangedValue("Job Division", isNew, original?.JobDivision, input.JobDivision, writer);
        WriteChangedValue("Job Title", isNew, original?.JobTitle, input.JobTitle, writer);
        WriteChangedValue("Work Address Street 1", isNew, original?.WorkAddressStreet1, input.WorkAddressStreet1, writer);
        WriteChangedValue("Work Address Street 2", isNew, original?.WorkAddressStreet2, input.WorkAddressStreet2, writer);
        WriteChangedValue("Work Address City", isNew, original?.WorkAddressCity, input.WorkAddressCity, writer);
        WriteChangedValue("Work Address Province", isNew, original?.WorkAddressProvince, input.WorkAddressProvince, writer);
        WriteChangedValue("Work Address Postal Code", isNew, original?.WorkAddressPostalCode, input.WorkAddressPostalCode, writer);
    }

    private static void WriteChangedValue(string name, bool isNew, string? oldValue, string? newValue, StreamWriter writer)
    {
        if (isNew)
        {
            if (!string.IsNullOrEmpty(newValue))
                writer.WriteLine($"- {name}: {newValue}");

            return;
        }

        if (string.Equals(oldValue ?? "", newValue ?? ""))
            return;

        writer.WriteLine($"- {name} (Old): {oldValue}");
        writer.WriteLine($"- {name} (New): {newValue}");
    }

    private static string GetSectionName(ImportPersonResult import, bool isAutoImport)
    {
        return import.Status switch
        {
            ImportPersonResult.StatusEnum.Modified => "Section 1",
            ImportPersonResult.StatusEnum.Created => isAutoImport ? "Section 2" : "Section 3",
            ImportPersonResult.StatusEnum.NotChanged or ImportPersonResult.StatusEnum.Pending => Section4Name,
            _ => throw new ArgumentException($"Unknown status: {import.Status}"),
        };
    }
}