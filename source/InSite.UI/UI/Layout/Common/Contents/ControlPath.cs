using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Contents
{
    public static class ControlPath
    {
        private static readonly PageContentControlTypeInfo[] _pageControls;
        private static readonly BlockContentControlTypeInfo[] _blockControls;

        public static IReadOnlyCollection<PageContentControlTypeInfo> PageControlTypes => _pageControls;
        public static IReadOnlyCollection<BlockContentControlTypeInfo> BlockControlTypes => _blockControls;

        static ControlPath()
        {
            {
                var templates = new List<PageContentControlTypeInfo>
                {
                    new PageContentControlTypeInfo("Article", "Article", null, 0),
                    new PageContentControlTypeInfo("Catalog", "Catalog", null, 0),
                    new PageContentControlTypeInfo("Course", "Course", null, 0),
                    new PageContentControlTypeInfo("Folder", "Folder", null, 0),
                    new PageContentControlTypeInfo("Newsletter", "Newsletter", null, 0),
                    new PageContentControlTypeInfo("Survey", "Survey", null, 0),
                    new PageContentControlTypeInfo("Program", "Program", null, 0),
                };

                AppendTemplateInfo(typeof(UserControl), typeof(IPageTemplate), "GetTemplateInfo", templates);

                _pageControls = templates.OrderBy(x => x.Sequence).ThenBy(x => x.Title).ToArray();
            }

            {
                var blocks = new List<BlockContentControlTypeInfo>();

                AppendTemplateInfo(typeof(UserControl), typeof(IBlockControl), "GetBlockInfo", blocks);

                _blockControls = blocks.OrderBy(x => x.Sequence).ThenBy(x => x.Title).ToArray();
            }

            void AppendTemplateInfo<T>(Type type1, Type type2, string methodName, List<T> list)
            {
                foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (!type1.IsAssignableFrom(t) || !type2.IsAssignableFrom(t))
                        continue;

                    var methodInfo = t.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                    if (methodInfo == null)
                        throw ApplicationError.Create($"{methodName} static property is not defind for {t.FullName}");

                    var info = (T)methodInfo.Invoke(null, null);

                    list.Add(info);
                }
            }
        }

        public static PageContentControlTypeInfo GetPageTemplate(string name)
        {
            if (name.IsEmpty())
                throw new ArgumentNullException(nameof(name));

            return _pageControls.FirstOrDefault(x => x.Name == name)
                ?? throw new PageContentControlNotFound($"There is no page content control named \"{name}\".");
        }

        public static BlockContentControlTypeInfo GetPageBlock(string name)
        {
            if (name.IsEmpty())
                throw new ArgumentNullException(nameof(name));

            return _blockControls.FirstOrDefault(x => x.Name == name)
                ?? throw new BlockContentControlNotFound($"There is no block content control named \"{name}\".");
        }
    }
}