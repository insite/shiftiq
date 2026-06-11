using System;
using System.Collections.Generic;
using System.Linq;

namespace InSite.Domain.Foundations.Navigation
{
    public class Link
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public List<Link> Links { get; set; }

        public Link()
        {
            Links = new List<Link>();
        }

        public bool ShouldSerializeLinks()
        {
            return Links.Any();
        }
    }

    public class SitemapConfiguration
    {
        public List<PageConfiguration> Pages { get; set; }

        public SitemapConfiguration()
        {
            Pages = new List<PageConfiguration>();
        }
    }

    public class PageConfiguration
    {
        public string Slug { get; set; }
        public List<BlockConfiguration> Blocks { get; set; }

        public PageConfiguration()
        {
            Blocks = new List<BlockConfiguration>();
        }

        public bool ShouldSerializeLinks()
        {
            return Blocks.Any();
        }
    }

    public class BlockConfiguration
    {
        public string Type { get; set; }
        public string Control { get; set; }
        public string File { get; set; }
    }

    public class NavigationMenu
    {
        public static Link Create(PageTree tree)
        {
            var root = new Link();

            if (tree != null)
            {
                var node = tree.Node;
                if (node == null || node.IsHidden)
                    return root;

                root.Text = node.Name;

                var url = node.NavigateUrl ?? tree.Path;
                if (!string.IsNullOrWhiteSpace(url))
                    root.Url = url;

                foreach (var child in tree.Children)
                {
                    var branch = Create(child);
                    if (!string.IsNullOrWhiteSpace(branch.Text))
                        root.Links.Add(branch);
                }
            }

            return root;
        }
    }

    public class Sitemap
    {
        public static SitemapConfiguration Create(PageTree tree)
        {
            var items = tree.Flatten();
            var pages = items.Where(n => n.Node.Type == "Page").ToList();
            var blocks = items.Where(n => n.Node.Type == "Block").ToList();

            var sitemap = new SitemapConfiguration();
            
            foreach (var page in pages)
            {
                var config = new PageConfiguration
                {
                    Slug = page.Path,
                    Blocks = CreateBlocks(page.Node.Identifier, page.Path, blocks)
                };

                if (config.Slug != null && config.Slug.StartsWith("/"))
                    config.Slug = config.Slug.Substring(1);

                sitemap.Pages.Add(config);
            }

            return sitemap;
        }

        private static List<BlockConfiguration> CreateBlocks(Guid pageId, string pagePath, List<PageTree> source)
        {
            var blocks = new List<BlockConfiguration>();

            var subs = source.Where(x => x.Node.Parent == pageId && x.Node.Type == "Block");
            if (subs.Any())
            {
                foreach (var sub in subs)
                {
                    var config = new BlockConfiguration
                    {
                        Type = sub.Node.Type,
                        Control = GetClassName(sub.Node.Control),
                        File = SanitizeFilePath(sub.Path) + ".json"
                    };
                    blocks.Add(config);
                }
            }
            else
            {
                var config = new BlockConfiguration
                {
                    Type = "Html",
                    File = SanitizeFilePath(pagePath) + ".html"
                };
                blocks.Add(config);
            }

            return blocks;

            string SanitizeFilePath(string input)
            {
                if (input.StartsWith("/"))
                    input = input.Substring(1);
                return input.Replace("/", "__");
            }
        }

        private static string GetClassName(string qualifiedName)
        {
            return qualifiedName?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault();
        }
    }
}
