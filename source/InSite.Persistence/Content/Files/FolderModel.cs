using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shift.Common.Trees;

namespace InSite.Persistence
{
    public class FolderModel<TFile> where TFile : IFile
    {
        #region Properties

        public string Name => _name;

        public string Path => (_parent == null ? _path : _parent.Path) + "/" + _name;

        public List<TFile> Files => _files;

        public List<FolderModel<TFile>> Folders => _folders.Values.ToList();

        #endregion

        #region Fields

        private string _path;
        private FolderModel<TFile> _parent;

        private string _name;
        private List<TFile> _files;
        private Dictionary<string, FolderModel<TFile>> _folders;

        #endregion

        #region Construction

        private FolderModel()
        {
            _files = new List<TFile>();
            _folders = new Dictionary<string, FolderModel<TFile>>(StringComparer.OrdinalIgnoreCase);
        }

        public FolderModel(string path)
            : this()
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.StartsWith("/"))
                throw new ApplicationException($"Invalid path: {path}");

            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);

            var lastIndex = path.LastIndexOf('/');

            if (lastIndex < 0)
            {
                _path = string.Empty;
                _name = string.Empty;
            }
            else if (lastIndex == 0)
            {
                _path = string.Empty;
                _name = path.Substring(lastIndex + 1);
            }
            else
            {
                _path = path.Substring(0, lastIndex);
                _name = path.Substring(lastIndex + 1);
            }
        }

        private FolderModel(string name, FolderModel<TFile> parent)
            : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        #endregion

        #region Methods

        public void Add(TFile file)
        {
            if (!file.Path.StartsWith(Path, StringComparison.OrdinalIgnoreCase))
                throw new ApplicationException($"Invalid file path: {file.Path}");

            var folder = this;
            var parts = file.Path.Substring(Path.Length).Split(new[] { '/' });

            for (var i = parts[0] == string.Empty ? 1 : 0; i < parts.Length - 1; i++)
            {
                var name = parts[i];

                if (!folder._folders.ContainsKey(name))
                    folder._folders.Add(name, new FolderModel<TFile>(name, folder));

                folder = folder._folders[name];
            }

            folder._files.Add(file);
        }

        public bool HasFiles() => HasFiles(file => true);

        public bool HasFiles(Func<TFile, bool> filter)
        {
            foreach (var file in Files)
            {
                if (filter(file))
                    return true;
            }

            foreach (var folder in Folders)
            {
                if (folder.HasFiles(filter))
                    return true;
            }

            return false;
        }

        public int CountFiles() => CountFiles(file => true);

        public int CountFiles(Func<TFile, bool> filter)
        {
            var count = 0;

            foreach (var folder in Folders)
                count += folder.CountFiles(filter);

            foreach (var file in Files)
            {
                if (filter(file))
                    count++;
            }

            return count;
        }

        public override string ToString()
        {
            var tree = NodeTree<string>.NewTree();

            BuildTree(this, tree.Root);

            return ToString(tree.Root, "    ", false);
        }

        #endregion

        #region Helpers (tree building)

        private static void BuildTree(FolderModel<TFile> folder, INode<string> node)
        {
            foreach (var file in folder.Files)
                node.AddChild(System.IO.Path.GetFileName(file.Path).ToLower() + " *");

            foreach (var sub in folder.Folders)
            {
                var branch = node.AddChild(sub.Path.ToUpper());

                BuildTree(sub, branch);
            }
        }

        private static string ToString(INode<string> node, string indent, bool last)
        {
            var sb = new StringBuilder();

            if (node != node.Root)
            {
                sb.Append(indent);

                if (last)
                {
                    sb.Append("\\-");
                    indent += "  ";
                }
                else
                {
                    sb.Append("|-");
                    indent += "| ";
                }

                sb.Append(" " + node.Data);
                sb.AppendLine();
            }

            foreach (var child in node.DirectChildren.Nodes)
                sb.Append(ToString(child, indent, child.IsLast));

            return sb.ToString();
        }

        #endregion
    }
}