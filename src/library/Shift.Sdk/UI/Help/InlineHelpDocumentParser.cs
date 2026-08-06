using System;
using System.Collections.Generic;
using System.Text;

using Shift.Common;

namespace Shift.Sdk.UI.Help
{
    public class InlineHelpDocumentParser
    {
        private static readonly MemoryCache<string, Lexicon> LexiconCache = new MemoryCache<string, Lexicon>();

        private static readonly MemoryCache<string, Instructions> MarkdownCache = new MemoryCache<string, Instructions>();

        /// <summary>
        /// The number of seconds an unreadable document stays cached as empty before it is downloaded again. Inline
        /// help is decoration, so a missing or broken URL must not break the page that asks for it.
        /// </summary>
        private const int FailureCacheSeconds = 300;

        public Lexicon ParseLexicon(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new Lexicon();

            if (LexiconCache.TryGet(url, out Lexicon lexicon))
                return lexicon;

            try
            {
                var input = Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.GetStringAsync, url);

                var output = Lexicon.FromJson(input);

                LexiconCache.Add(url, output);

                return output;
            }
            catch (Exception)
            {
                var empty = new Lexicon();

                LexiconCache.Add(url, empty, FailureCacheSeconds);

                return empty;
            }
        }

        /// <summary>
        /// Reads the Markdown file from the specified URL and parses it into a list of MarkdownSection objects.
        /// Each section contains a heading (identified by leading "# ") and its associated body content.
        /// </summary>
        /// <returns>A list of MarkdownSection objects</returns>
        public Instructions ParseInstructions(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new Instructions();

            if (MarkdownCache.TryGet(url, out Instructions instructions))
                return instructions;

            try
            {
                var input = Shift.Common.TaskRunner.RunSync(StaticHttpClient.Client.GetStringAsync, url);

                var output = ParseMarkdownContent(input);

                MarkdownCache.Add(url, output);

                return output;
            }
            catch (Exception)
            {
                var empty = new Instructions();

                MarkdownCache.Add(url, empty, FailureCacheSeconds);

                return empty;
            }
        }

        /// <summary>
        /// Parses markdown content string into MarkdownSection objects.
        /// </summary>
        /// <param name="content">The markdown content as a string</param>
        /// <returns>A list of MarkdownSection objects</returns>
        private Instructions ParseMarkdownContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new Instructions();

            var sections = new List<MarkdownSection>();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.None);

            string currentHeading = null;
            var currentBody = new StringBuilder();

            foreach (string line in lines)
            {
                // Check if the line is a heading (starts with "# ")
                if (line.StartsWith("# "))
                {
                    // If we have a previous section, add it to the list
                    if (!string.IsNullOrEmpty(currentHeading))
                    {
                        sections.Add(new MarkdownSection(currentHeading, currentBody.ToString().Trim()));
                    }

                    // Start a new section
                    currentHeading = line.Substring(2).Trim(); // Remove "# " prefix
                    currentBody.Clear();
                }
                else
                {
                    // Add line to current body (only if we have a current heading)
                    if (!string.IsNullOrEmpty(currentHeading))
                    {
                        if (currentBody.Length > 0)
                        {
                            currentBody.AppendLine();
                        }
                        currentBody.Append(line);
                    }
                }
            }

            // Add the last section if it exists
            if (!string.IsNullOrEmpty(currentHeading))
            {
                sections.Add(new MarkdownSection(currentHeading, currentBody.ToString().Trim()));
            }

            return new Instructions { Sections = sections };
        }
    }
}