using Shift.Common.Integration.Google;
using Shift.Contract;

namespace Shift.Service.Content;

public class TranslatorService : ITranslatorService
{
    private readonly ITranslationClient _translationClient;
    private readonly ILabelService _labelService;

    public TranslatorService(ITranslationClient translationClient, ILabelService labelService)
    {
        _translationClient = translationClient;
        _labelService = labelService;
    }

    public async Task<string?> TranslateAsync(string source, string language, Guid organization)
        => await TranslateAsync(source, source, language, organization);

    public async Task<string?> TranslateAsync(string label, string? source, string language, Guid organization)
    {
        if (string.IsNullOrWhiteSpace(source))
            return source;

        var target = _labelService.GetTranslation(label, language, organization, true, false);
        if (string.IsNullOrWhiteSpace(target))
        {
            if (language == "en")
                return source;

            target = _translationClient.Translate("en", language, source);

            await _labelService.SaveTranslationAsync(language, label, source, target);

            await _labelService.RefreshAsync();
        }
        return target;
    }

    public string? Translate(string source, string language, Guid organization)
        => TranslateAsync(source, language, organization).GetAwaiter().GetResult();
}
