using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Shift.Common.Integration.Google;
using Shift.Contract.Presentation;

namespace Shift.Api;

[Route("content/translations")]
[ApiController()]
[HybridAuthorize()]
[ApiExplorerSettings(GroupName = "Content API: Translations")]
public class TranslationController(
    IShiftIdentityService shiftIdentity,
    IReactService reactService,
    ITranslationClient translationClient
    ) : ControllerBase
{
    [HttpPost("translate")]
    public async Task<ActionResult<List<Dictionary<string, string>>>> TranslateAsync(string[] englishTexts)
    {
        var principal = shiftIdentity.GetPrincipal();
        var settings = await reactService.RetrieveSiteSettingsAsync(principal, false);

        var result = new List<Dictionary<string, string>>();
        for (int i = 0; i < englishTexts.Length; i++)
            result.Add(new Dictionary<string, string>());

        foreach (var language in settings.SupportedLanguages)
        {
            var translatedTexts = string.Equals(language, "en")
                ? englishTexts
                : await translationClient.TranslateAsync("en", language, englishTexts);

            for (int i = 0; i < englishTexts.Length; i++)
                result[i].Add(language, translatedTexts[i]);
        }

        return result;
    }
}
