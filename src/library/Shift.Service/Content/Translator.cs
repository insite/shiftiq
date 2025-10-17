using Shift.Common.Integration.Google;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Content;

public class Translator : ITranslator
{
    private readonly ITranslationClient _translationClient;
    private readonly ILabelService _labelService;
    private readonly Guid _organization;
    private readonly string _language;

    public Translator(ITranslationClient translationClient, ILabelService labelService, IShiftIdentityService identityService)
    {
        _translationClient = translationClient;
        _labelService = labelService;

        var principal = identityService.GetPrincipal();

        _organization = principal.Organization.Identifier;
        _language = principal.User.Language;
    }

    public string? Translate(string source) => Translate(source, source);

    public string? Translate(string label, string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return source;

        var target = _labelService.GetTranslation(label, _language, _organization, true, false);
        if (string.IsNullOrWhiteSpace(target))
        {
            if (_language == "en")
                return source;

            target = _translationClient.Translate("en", _language, source);

            _labelService.SaveTranslation(_language, label, source, target);

            _labelService.Refresh();
        }
        return target;
    }
}
