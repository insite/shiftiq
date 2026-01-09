using Shift.Common;

namespace Shift.Test.Common;

public class RelativePathTests
{
    [Fact]
    public void Constructor_ValueAndDefaultSeparator()
    {
        var rp = new RelativePath("a/b/c");
        Assert.Equal("a/b/c", rp.Value);
        Assert.Equal('/', rp.Separator);
    }

    [Fact]
    public void Constructor_NonDefaultSeparator()
    {
        var rp = new RelativePath("a.b.c", '.');
        Assert.Equal("a.b.c", rp.Value);
        Assert.Equal('.', rp.Separator);
    }

    [Fact]
    public void HasSegments_EmptyPath_ReturnsFalse()
    {
        var rp = new RelativePath(string.Empty);
        Assert.False(rp.HasSegments());
    }

    [Fact]
    public void HasSegments_NonEmptyPath_ReturnsTrue()
    {
        var abc = new RelativePath("a/b/c");
        Assert.True(abc.HasSegments());
        Assert.Equal(3, abc.CountSegments());

        var a = new RelativePath("a");
        Assert.True(a.HasSegments());
        Assert.Equal(1, a.CountSegments());
    }

    [Fact]
    public void IsCurrentRelative_ReturnsTrue()
    {
        var rp = new RelativePath("a/b/c");
        Assert.True(rp.IsCurrentRelative);
    }

    [Fact]
    public void IsCurrentRelative_ReturnsFalse()
    {
        var rp = new RelativePath("/a/b/c");
        Assert.False(rp.IsCurrentRelative);
    }

    [Fact]
    public void IsRootRelative_ReturnsTrue()
    {
        var rp = new RelativePath("/a/b/c");
        Assert.True(rp.IsRootRelative);
    }

    [Fact]
    public void IsRootRelative_ReturnsFalse()
    {
        var rp = new RelativePath("a/b/c");
        Assert.False(rp.IsRootRelative);
    }

    [Fact]
    public void RemoveLastSegment_Success()
    {
        var rp = new RelativePath("a/b/c");
        Assert.Equal("a/b/c", rp.Value);

        rp.RemoveLastSegment();
        Assert.Equal("a/b", rp.Value);

        rp.RemoveLastSegment();
        Assert.Equal("a", rp.Value);

        rp.RemoveLastSegment();
        Assert.Null(rp.Value);
    }
}