using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using InSite.UI;

using Shift.Common;
using Shift.Constant;

using IoFile = System.IO.File;
using IoPath = System.IO.Path;

namespace InSite.Common.Web.UI
{
    public static class ResourceHelper
    {
        private static readonly Regex _jsMapLinkPattern = new Regex("^\\s*//#\\s*sourceMappingURL=[^=]+\\.map\\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

        static ResourceHelper()
        {
            _bundleStoragePath = IoPath.Combine(ServiceLocator.FilePaths.TempFolderPath, "Bundles");
            _bundleStorageCache = new ConcurrentDictionary<string, BundleFileData>(StringComparer.OrdinalIgnoreCase);

            InitBundleStorage(_bundleStoragePath);
        }

        #region Bundle

        public class BundleContent
        {
            public string Name { get; }

            public string Content { get; set; }

            public BundleContent(string name)
            {
                Name = name;
            }
        }

        internal class BundleFileData
        {
            public string Type { get; }
            public byte[] Body { get; }
            public DateTime Timestamp { get; }

            public BundleFileData(string type, byte[] body, DateTime timestamp)
            {
                Type = type;
                Body = body;
                Timestamp = timestamp;
            }
        }

        private static readonly string _bundleStoragePath;
        private static readonly ConcurrentDictionary<string, BundleFileData> _bundleStorageCache;

        private static void InitBundleStorage(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                dir.Create();
                return;
            }

            foreach (var file in dir.EnumerateFiles())
            {
                if ((DateTime.UtcNow - file.LastWriteTimeUtc).TotalDays <= 7)
                    continue;

                try
                {
                    file.Delete();
                }
                catch (IOException ioex)
                {
                    if (!ioex.Message.StartsWith("The process cannot access the file"))
                        throw;
                }
            }

            foreach (var subDir in dir.EnumerateDirectories())
                subDir.Delete(true);
        }

        private static BundleFileData GetBundleFileData(string name)
        {
            if (name.Length > 60)
                return null;

            for (var i = 0; i < name.Length; i++)
            {
                var ch = name[i];
                if ((ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9') && ch != '-' && ch != '_' && ch != '=')
                    return null;
            }

            string fileName, contentType;

            if (name[0] == 'j')
            {
                fileName = name + ".js";
                contentType = "application/x-javascript; charset=utf-8";
            }
            else if (name[0] == 's')
            {
                fileName = name + ".css";
                contentType = "text/css; charset=utf-8";
            }
            else
            {
                return null;
            }

            var filePath = System.IO.Path.Combine(_bundleStoragePath, fileName);

            return IoFile.Exists(filePath)
                ? new BundleFileData(contentType, IoFile.ReadAllBytes(filePath), IoFile.GetCreationTimeUtc(filePath))
                : null;
        }

        internal static BundleFileData GetBundleFile(string name)
        {
            return _bundleStorageCache.GetOrAdd(name, GetBundleFileData);
        }

        internal static string[] CreateBundle(ResourceLink.ResourceType type, IList<BundleContent> items)
        {
            const int maxBundleSize = 256000; // 250 KB

            var index = 0;
            var bundleSize = 0;
            var bundleItems = new List<BundleContent>();
            var bundleNames = new List<string>();

            while (true)
            {
                var item = items[index++]
                    ?? throw new ArgumentNullException("item");

                if (item.Content == null)
                    throw new ArgumentNullException($"{item.Name}::item.Content");

                var isLastItem = items.Count == index;

                if (bundleSize + item.Content.Length > maxBundleSize)
                {
                    if (bundleItems.Count == 0)
                    {
                        Add(item);
                        Flush();
                    }
                    else
                    {
                        Flush();
                        Add(item);
                    }
                }
                else
                {
                    Add(item);
                }

                if (isLastItem)
                {
                    Flush();
                    break;
                }
            }

            return bundleNames.ToArray();

            void Add(BundleContent item)
            {
                bundleSize += item.Content.Length;
                bundleItems.Add(item);
            }

            void Flush()
            {
                if (bundleItems.Count == 0)
                    return;

                if (bundleItems.Count > 1)
                {
                    var name = WriteBundle(type, bundleItems);

                    bundleNames.Add(InSite.Web.Persistence.ResourceBundle.Path + $"?r={name}");
                }
                else
                {
                    bundleNames.Add(bundleItems[0].Name);
                }

                bundleSize = 0;
                bundleItems.Clear();
            }
        }

        private static string WriteBundle(ResourceLink.ResourceType type, IEnumerable<BundleContent> items)
        {
            byte[] bundleHash;

            var tempFilePath = IoPath.Combine(_bundleStoragePath, $"{Guid.NewGuid()}.temp");

            using (var fileStream = IoFile.Open(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var hashAlgorithm = SHA256.Create())
                {
                    using (var outputStream = new CryptoStream(fileStream, hashAlgorithm, CryptoStreamMode.Write))
                    {
                        using (var writer = new StreamWriter(outputStream, Encoding.UTF8, 1024, true))
                        {
                            if (type == ResourceLink.ResourceType.Css)
                                WriteCssBundle(writer, items);
                            else
                                WriteJavaScriptBundle(writer, items);
                        }

                        outputStream.FlushFinalBlock();
                    }

                    bundleHash = hashAlgorithm.Hash;
                }
            }

            var bundleName = HttpServerUtility.UrlTokenEncode(bundleHash);
            string bundleFilePath;

            if (type == ResourceLink.ResourceType.JavaScript)
            {
                bundleName = "j" + bundleName;
                bundleFilePath = bundleName + ".js";
            }
            else if (type == ResourceLink.ResourceType.Css)
            {
                bundleName = "s" + bundleName;
                bundleFilePath = bundleName + ".css";
            }
            else
                throw new NotSupportedException($"Resource type is not supported: {type.GetName()}");

            bundleFilePath = IoPath.Combine(_bundleStoragePath, bundleFilePath);

            if (IoFile.Exists(bundleFilePath))
            {
                var creationTime = IoFile.GetCreationTimeUtc(bundleFilePath);

                IoFile.Delete(bundleFilePath);
                IoFile.Move(tempFilePath, bundleFilePath);
                IoFile.SetCreationTimeUtc(bundleFilePath, creationTime);
            }
            else
            {
                IoFile.Move(tempFilePath, bundleFilePath);
            }

            return bundleName;
        }

        private static void WriteJavaScriptBundle(StreamWriter writer, IEnumerable<BundleContent> items)
        {
            foreach (var item in items)
            {
                if (item.Name.IsEmpty())
                    continue;

                var content = item.Content;
                if (content.HasNoValue())
                    continue;

                content = _jsMapLinkPattern.Replace(content, "");

                writer.Write("// ");
                writer.WriteLine(item.Name);
                writer.WriteLine();

                writer.Write(content);
                writer.Write(";");

                writer.WriteLine();
                writer.WriteLine();
            }
        }

        private static readonly Regex CssCharsetPatternRegex = new Regex("@charset .+?;", RegexOptions.Compiled);
        private static readonly Regex CssImportPatternRegex = new Regex("@import .+?;", RegexOptions.Compiled);
        private static readonly Regex CssUrlPatternRegex = new Regex("(?<start>url\\(['\"]?)(?<url>[^)]+?)(?<end>['\"]?\\))", RegexOptions.Compiled);

        private static void WriteCssBundle(StreamWriter writer, IEnumerable<BundleContent> items)
        {
            writer.WriteLine("@charset \"UTF-8\";");

            foreach (var item in items)
            {
                item.Content = CssCharsetPatternRegex.Replace(item.Content, string.Empty);

                var dirUrl = VirtualPathUtility.GetDirectory(item.Name);
                if (dirUrl.HasValue())
                {
                    item.Content = CssUrlPatternRegex.Replace(item.Content, m =>
                    {
                        var url = m.Groups["url"].Value;

                        if (!url.HasValue() || url.StartsWith("/") || url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                            return m.Value;

                        if (!dirUrl.EndsWith("/"))
                            url = "/" + url;

                        url = VirtualPathUtility.ToAbsolute(dirUrl + url);

                        return m.Groups["start"].Value + url + m.Groups["end"].Value;
                    });
                }

                item.Content = CssImportPatternRegex.Replace(item.Content, m =>
                {
                    writer.WriteLine(m.Value);

                    return string.Empty;
                });
            }

            writer.WriteLine();

            foreach (var item in items)
            {
                writer.Write("/* ");
                writer.Write(item.Name);
                writer.WriteLine(" */");
                writer.WriteLine();

                writer.WriteLine(item.Content);
                writer.WriteLine();
                writer.WriteLine();
            }
        }

        #endregion

        #region File

        private static readonly ConcurrentDictionary<string, IResourceFileData> _resourceFileStorageCache = new ConcurrentDictionary<string, IResourceFileData>(StringComparer.OrdinalIgnoreCase);

        internal interface IResourceFileData
        {
            string ID { get; }
            string Url { get; }
        }

        internal class ResourceFileData : IResourceFileData
        {
            public string ID { get; set; }

            public string Url { get; set; }
        }

        internal class DebugResourceFileData : IResourceFileData
        {
            public string ID
            {
                get
                {
                    if (DateTime.UtcNow > _nextCheck)
                    {
                        _id = IoFile.GetLastWriteTimeUtc(Path).Ticks.ToString();
                        _nextCheck = DateTime.UtcNow.AddSeconds(5);
                    }

                    return _id;
                }
            }

            public string Url { get; }

            public string Path { get; }

            private string _id = "unknown";
            private DateTime _nextCheck = DateTime.MinValue;

            public DebugResourceFileData(IResourceFileData data)
            {
                Url = data.Url;
                Path = HttpContext.Current.Server.MapPath(Url);
            }
        }

        internal static IResourceFileData GetResourceFile(string url)
        {
            return _resourceFileStorageCache.GetOrAdd(url, GetResourceFileData);
        }

        internal static IResourceFileData GetDebugResourceFile(string url)
        {
            return _resourceFileStorageCache.GetOrAdd(url, x =>
            {
                var item = GetResourceFileData(x);

                return new DebugResourceFileData(item);
            });
        }

        private static IResourceFileData GetResourceFileData(string url)
        {
            try
            {
                if (url.StartsWith(InSite.Web.Persistence.ResourceBundle.Path, StringComparison.OrdinalIgnoreCase))
                    return new ResourceFileData
                    {
                        Url = url
                    };

                var filePath = HttpContext.Current.Server.MapPath(url);
                if (!IoFile.Exists(filePath))
                    return null;

                filePath = GetMinFilePath(filePath);

                var fileContent = IoFile.ReadAllBytes(filePath);
                var hash = EncryptionHelper.ComputeHashMd5(fileContent);

                return new ResourceFileData
                {
                    ID = HttpServerUtility.UrlTokenEncode(hash),
                    Url = PathHelper.PhysicalToRelativePath(filePath)
                };
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                return null;
            }
        }

        #endregion

        #region Other

        internal static string GetMinFilePath(string filePath)
        {
            var minFilePath = IoPath.GetFileNameWithoutExtension(filePath);
            if (!minFilePath.EndsWith(".min", StringComparison.OrdinalIgnoreCase))
            {
                minFilePath = IoPath.Combine(IoPath.GetDirectoryName(filePath), minFilePath + ".min" + IoPath.GetExtension(filePath));
                if (IoFile.Exists(minFilePath))
                    return minFilePath;
            }

            return filePath;
        }

        #endregion
    }
}