using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Sdk.UI.Help
{
    public class MarkdownSection
    {
        public string Path { get; set; }
        public string Body { get; set; }

        public MarkdownSection(string heading, string body)
        {
            Path = heading;
            Body = body;
        }

        public override string ToString()
        {
            return $"Heading: {Path}\nBody: {Body}\n";
        }
    }

    public class HelpDocumentParser
    {
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Reads the Markdown file from the specified URL and parses it into a list of MarkdownSection objects.
        /// Each section contains a heading (identified by leading "# ") and its associated body content.
        /// </summary>
        /// <returns>A list of MarkdownSection objects</returns>
        public async Task<List<MarkdownSection>> ParseMarkdownAsync(string documentUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(documentUrl))
                    return new List<MarkdownSection>();

                // Download the markdown content
                string markdownContent = await httpClient.GetStringAsync(documentUrl);

                // Parse the content
                return ParseMarkdownContent(markdownContent);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to download markdown file: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error parsing markdown: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parses markdown content string into MarkdownSection objects.
        /// </summary>
        /// <param name="content">The markdown content as a string</param>
        /// <returns>A list of MarkdownSection objects</returns>
        public List<MarkdownSection> ParseMarkdownContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return new List<MarkdownSection>();
            }

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

            return sections;
        }

        /// <summary>
        /// Synchronous version that reads from a local file path instead of URL.
        /// </summary>
        /// <param name="filePath">Path to the markdown file</param>
        /// <returns>A list of MarkdownSection objects</returns>
        public List<MarkdownSection> ParseMarkdownFromFile(string filePath)
        {
            try
            {
                string content = System.IO.File.ReadAllText(filePath);
                return ParseMarkdownContent(content);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading file '{filePath}': {ex.Message}", ex);
            }
        }

        // Dispose of HttpClient when the class is disposed
        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }

    public class EmbeddedHelp
    {
        public EmbeddedHelp(string documentUrl)
        {
            DocumentUrl = documentUrl;
        }

        public string DocumentUrl { get; }

        public async Task<string> GetContentAsync(string topicPath)
        {
            var parser = new HelpDocumentParser();

            var sections = await parser.ParseMarkdownAsync(DocumentUrl);

            var section = sections.FirstOrDefault(x => StringHelper.Equals(x.Path, topicPath));

            return section != null
                ? Markdown.ToHtml(section.Body)
                : null;
        }
    }
}