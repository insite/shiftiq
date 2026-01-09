using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Json;
using Shift.Common.Linq;
using Shift.Sdk.UI;

namespace InSite.Api.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonFileListResult : JsonResult
    {
        public JsonFileListResult()
            : base("FileList")
        {
        }

        [JsonProperty(PropertyName = "path")]
        public string CurrentPath { get; set; }

        [JsonProperty(PropertyName = "folders")]
        public JsonFolderModel[] Folders { get; set; }

        [JsonProperty(PropertyName = "files")]
        public JsonFileModel[] Files { get; set; }

        public static JsonFileListResult Create(FolderModel<JsonFileModel> root, string type, string name, DateTimeOffset? since, DateTimeOffset? before)
        {
            var filter = BuildFilter(type, name, since, before);

            var result = new JsonFileListResult
            {
                CurrentPath = root.Path.EndsWith("/")
                    ? root.Path
                    : root.Path + "/"
            };

            result.Folders = root.Folders
                .Select(
                    x => new JsonFolderModel
                    {
                        Name = x.Name,
                        FilesCount = x.CountFiles(filter).ToString("n0")
                    })
                .OrderBy(x => x.Name)
                .ToArray();

            var excludeEmptyFolders = name.IsNotEmpty() || since.HasValue || before.HasValue;

            if (excludeEmptyFolders)
                result.Folders = result.Folders
                    .Where(x => x.FilesCount != "0")
                    .ToArray();

            result.Files = root.Files
                .Where(filter)
                .OrderBy(x => x.Path)
                .ToArray();

            return result;
        }

        private static Func<JsonFileModel, bool> BuildFilter(string type, string name, DateTimeOffset? since, DateTimeOffset? before)
        {
            var filterExpr = PredicateBuilder.True<JsonFileModel>();

            if (type.IsNotEmpty())
            {
                var includes = new HashSet<string>(type.Split(','), StringComparer.OrdinalIgnoreCase);

                filterExpr = filterExpr.And(x => includes.Contains(x.Extension));
            }

            if (name.IsNotEmpty())
                filterExpr = filterExpr.And(f => Path.GetFileNameWithoutExtension(f.Path).Contains(name, StringComparison.OrdinalIgnoreCase));

            if (since.HasValue)
                filterExpr = filterExpr.And(f => f.Date >= since.Value);

            if (before.HasValue)
                filterExpr = filterExpr.And(f => f.Date < before.Value);

            return filterExpr.Compile();
        }
    }
}