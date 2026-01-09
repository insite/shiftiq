using Microsoft.AspNetCore.Mvc;

using Endpoints = Shift.Common.Integration.Google.Endpoints;

namespace Engine.Api.Translation
{
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private const string GoogleApiErrorCause = "The payment method for this Google Cloud account may be an expired credit card. Please refer to the documentation for Google API integration in Confluence.";
        private const string GoogleApiErrorEffect = "RESOURCE_EXHAUSTED Quota exceeded for quota metric";

        private readonly ITranslationService _translator;
        private readonly IMonitor _monitor;

        public TranslationController(ITranslationService translator, IMonitor monitor)
        {
            _translator = translator;
            _monitor = monitor;
        }

        [HttpPost(Endpoints.Translation.Translate)]
        [ProducesResponseType<string[]>(StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [EndpointName("translations")]
        public async Task<ActionResult<List<string>>> TranslateAsync(string from, string to, [FromBody] string[] contents)
        {
            try
            {
                var translations = new List<string>();

                foreach (var content in contents)
                    translations.Add(await _translator.TranslateAsync(content, from, to));

                return translations;
            }
            catch (LanguageNotSupportedException ex)
            {
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(GoogleApiErrorEffect, StringComparison.CurrentCultureIgnoreCase))
                    _monitor.Warning(GoogleApiErrorCause);

                _monitor.Error(ex.Message);

                return Problem(ex.Message);
            }
        }

        [HttpPost("content/translations/translate")] // TODO: Deprecate old URL
        [ProducesResponseType<string[]>(StatusCodes.Status200OK, "application/json")]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [EndpointName("translations_deprecate")]
        public async Task<ActionResult<List<string>>> TranslateDeprecateAsync(string from, string to, [FromBody] string[] contents) =>
            await TranslateAsync(from, to, contents);
    }

}