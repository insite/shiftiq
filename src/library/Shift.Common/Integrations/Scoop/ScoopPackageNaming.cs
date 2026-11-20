using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shift.Common.Integration.Scoop
{
    /// <summary>
    /// Which SCORM package handle a lesson should launch for a given learner.
    /// </summary>
    /// <remarks>
    /// An administrator stores one base SCORM Package ID against the lesson and
    /// ticks "Append a language code to SCORM Package ID". Scoop then holds
    /// one package per language under that base: i-100-intro-en and
    /// i-100-intro-fr. This decides which of them to launch.
    ///
    /// Pure on purpose: no HTTP, no settings, no HttpContext. Every decision
    /// this feature makes is here, so every decision is unit-testable.
    /// </remarks>
    public static class ScoopPackageNaming
    {
        /// <summary>
        /// The language a learner falls back to when their own has no package.
        /// </summary>
        public const string FallbackLanguage = "en";

        /// <summary>
        /// The longest handle Scoop accepts.
        /// </summary>
        public const int MaxHandleLength = 300;

        private static readonly Regex HandlePattern = new Regex(
            "^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// The handle to launch, given the base handle, the learner's portal
        /// language and the handles Scoop reports.
        /// </summary>
        /// <param name="availableHandles">
        /// What Scoop answered. An EMPTY collection means it answered and
        /// holds nothing under this base, so the bare base handle is launched
        /// and Scoop renders its own not-found page, exactly as it does
        /// today for a mistyped ID. NULL means the lookup could not be made at
        /// all, and the English variant is launched on the assumption that the
        /// administrator set the flag because the variants exist. The
        /// difference between the two is the whole failure policy.
        /// </param>
        public static string Resolve(string baseHandle, string language, bool isMultilingual, ICollection<string> availableHandles)
        {
            if (string.IsNullOrWhiteSpace(baseHandle))
                return baseHandle;

            // A single-language lesson never reaches the network. This is the
            // guard that keeps the feature off the hot path for everyone who
            // has not turned it on.
            if (!isMultilingual)
                return baseHandle;

            var root = Normalize(baseHandle);
            var code = NormalizeLanguage(language);

            if (availableHandles == null)
                return Variant(root, FallbackLanguage);

            var candidates = new List<string>();
            if (code != FallbackLanguage)
                candidates.Add(Variant(root, code));
            candidates.Add(Variant(root, FallbackLanguage));
            candidates.Add(root);

            var available = new HashSet<string>(
                availableHandles.Where(h => !string.IsNullOrWhiteSpace(h)),
                StringComparer.OrdinalIgnoreCase);

            foreach (var candidate in candidates)
            {
                if (available.Contains(candidate))
                    return candidate;
            }

            // Nothing matched. Launch the base handle and let Scoop say so,
            // rather than inventing a variant that is known not to exist.
            return root;
        }

        /// <summary>The handle for one language under a base handle.</summary>
        public static string Variant(string baseHandle, string language)
        {
            var root = Normalize(baseHandle);
            var code = NormalizeLanguage(language);

            return string.IsNullOrEmpty(root) ? root : root + "-" + code;
        }

        /// <summary>
        /// Whether a value is a handle Scoop accepts: letters, digits and single hyphens, at most
        /// 300 characters.
        /// </summary>
        /// <remarks>
        /// Scoop's own rule says lowercase, but every lookup lowercases the handle before sending
        /// it, so case is not held against a stored ID here. Anything else Scoop answers with a 400.
        /// </remarks>
        public static bool IsHandle(string value)
        {
            var isEmpty = string.IsNullOrEmpty(value);
            if (isEmpty || value.Length > MaxHandleLength)
                return false;

            return HandlePattern.IsMatch(value);
        }

        /// <summary>
        /// The package handle in what an administrator entered as a SCORM Package ID.
        /// </summary>
        /// <remarks>
        /// Administrators copy links from the Scoop library, and a link to a package ends in its
        /// handle: https://app.openscorm.com/lobby/enroll/keyera/i-100-intro. Saved as entered, a
        /// link reaches Scoop as the handle itself, which no lookup or launch can resolve. Anything
        /// that is not a web link is returned trimmed and otherwise as entered, for IsHandle to
        /// judge.
        /// </remarks>
        public static string ParseHandle(string value)
        {
            var isBlank = string.IsNullOrWhiteSpace(value);
            if (isBlank)
                return value;

            var text = value.Trim();

            var isAbsolute = Uri.TryCreate(text, UriKind.Absolute, out var uri);
            var isWebLink = isAbsolute && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            if (!isWebLink)
                return text;

            var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return text;

            return Uri.UnescapeDataString(segments[segments.Length - 1]);
        }

        // A stored ID may carry stray whitespace or a trailing hyphen from an
        // administrator's copy and paste. Neither should produce "base--fr".
        private static string Normalize(string baseHandle)
        {
            return string.IsNullOrWhiteSpace(baseHandle)
                ? baseHandle
                : baseHandle.Trim().TrimEnd('-');
        }

        // Portal languages arrive as a culture token that may be capitalized
        // ("FR", "fr-CA"). Scoop handles are lowercase, and its comparison
        // is case-insensitive either way, so lowercase here and compare
        // case-insensitively above.
        private static string NormalizeLanguage(string language)
        {
            return string.IsNullOrWhiteSpace(language)
                ? FallbackLanguage
                : language.Trim().ToLowerInvariant();
        }
    }
}
