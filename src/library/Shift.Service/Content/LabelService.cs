using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace Shift.Service.Content;

public class LabelService : ILabelService
{
    private readonly Guid ContainerIdentifier = new Guid("5d3b4c71-f3f7-4e41-9945-0f83cf922e2f");
    private volatile List<TInputEntity> _items = [];

    private readonly TInputReader _reader;
    private readonly TInputWriter _writer;

    public LabelService(TInputReader reader, TInputWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public async Task Refresh()
    {
        var labels = await _reader.CollectAsync(new CollectInputs
        {
            ContainerIdentifier = ContainerIdentifier,
            ContainerType = ContentContainerType.Application
        });

        _items = labels.ToList();
    }

    public string? GetTranslation(string name, string language, Guid organization, bool allowNull, bool allowDefault)
    {
        var labels = _items;

        var contents = labels
            .Where(x => string.Equals(x.ContentLabel, name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var translation = GetTranslation(language, organization, contents);

        if (!string.IsNullOrEmpty(translation))
            return translation;

        if (allowDefault)
        {
            translation = GetTranslation("en", organization, contents);

            if (!string.IsNullOrEmpty(translation))
                return translation;
        }

        return allowNull ? null : name;
    }

    private static string? GetTranslation(string language, Guid? organizationIdentifier, List<TInputEntity> contents)
    {
        var content = contents.FirstOrDefault(x =>
            string.Equals(x.ContentLanguage, language, StringComparison.OrdinalIgnoreCase)
            && (organizationIdentifier == null && x.OrganizationIdentifier == OrganizationIdentifiers.Global || x.OrganizationIdentifier == organizationIdentifier)
        );

        if (content == null && organizationIdentifier.HasValue && organizationIdentifier != OrganizationIdentifiers.Global)
        {
            content = contents.FirstOrDefault(x =>
                string.Equals(x.ContentLanguage, language, StringComparison.OrdinalIgnoreCase)
                && x.OrganizationIdentifier == OrganizationIdentifiers.Global
            );
        }

        return content?.ContentText;
    }

    public async Task SaveTranslation(string language, string label, string source, string target)
    {
        label = StringHelper.Snip(label, 100);

        await _writer.CreateAsync(new TInputEntity
        {
            ContentIdentifier = UuidFactory.CreateV7(),
            ContainerType = "Application",
            ContainerIdentifier = ContainerIdentifier,
            ContentLabel = label,
            ContentText = source,
            ContentSnip = ContentContainerItem.GetSnip(source, null),
            ContentLanguage = "en",
            OrganizationIdentifier = OrganizationIdentifiers.Global
        });

        await _writer.CreateAsync(new TInputEntity
        {
            ContentIdentifier = UuidFactory.CreateV7(),
            ContainerType = "Application",
            ContainerIdentifier = ContainerIdentifier,
            ContentLabel = label,
            ContentText = target,
            ContentSnip = ContentContainerItem.GetSnip(target, null),
            ContentLanguage = language,
            OrganizationIdentifier = OrganizationIdentifiers.Global
        });
    }
}
