using Shift.Common;

namespace Shift.Test.Common;

public class RelativePathCollectionTests
{
    [Fact]
    public void Constructor_RehydrateImpliedSubPaths()
    {
        var abc = new RelativePath("ABC", "a/b/c", '/');

        var def = new RelativePath("DEF", "/d/e/f", '/');

        var paths = new RelativePathCollection([abc, def]);

        Assert.Equal(6, paths.Count());
    }
}