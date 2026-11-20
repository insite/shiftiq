using Shift.Common;

namespace Shift.Test.Common
{
    public class FilePathTests
    {
        [Fact]
        public void PathType_NetworkShare()
        {
            var unc = @"\\server\share\file.txt";
            var path = new FilePath(unc);
            Assert.Equal(FilePathType.RemoteFile, path.PathType);
        }

        [Fact]
        public void PathType_LocalPath()
        {
            var local = @"C:\Users\Username\file.txt";
            var path = new FilePath(local);
            Assert.Equal(FilePathType.LocalFile, path.PathType);
        }

        [Fact]
        public void PathType_AbsoluteUrl()
        {
            var url = @"http://www.example.com/page.html";
            var path = new FilePath(url);
            Assert.Equal(FilePathType.AbsoluteUrl, path.PathType);
        }

        [Fact]
        public void PathType_RelativeUrl()
        {
            var url = @"/page.html";
            var path = new FilePath(url);
            Assert.Equal(FilePathType.RelativeUrl, path.PathType);
        }

        [Fact]
        public void PathType_RelativeUrlStartingWithTilde()
        {
            var url = @"~/page";
            var path = new FilePath(url);
            Assert.Equal(FilePathType.RelativeUrl, path.PathType);
        }
    }
}