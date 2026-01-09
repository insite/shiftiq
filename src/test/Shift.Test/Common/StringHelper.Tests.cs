using Shift.Common;

namespace Shift.Test.Common
{
    public class StringHelperTests
    {
        [Fact]
        public void Snip_MaxLengthGreaterThanStringLength_ReturnsOriginalString()
        {
            var original = "12345";
            var snipped = StringHelper.Snip(original, 10);
            Assert.Equal(original, snipped);
        }

        [Fact]
        public void Snip_MaxLengthEqualsStringLength_ReturnsOriginalString()
        {
            var original = "12345";
            var snipped = StringHelper.Snip("12345", 5);
            Assert.Equal(original, snipped);
        }

        [Fact]
        public void Snip_MaxLengthLessThanStringLength_ReturnsSnippedOriginalWithEllipsis()
        {
            var original = "12345";
            var snipped = StringHelper.Snip(original, 4);
            Assert.Equal("1...", snipped);
        }

        [Fact]
        public void Snip_SnippedStringCannotFitEllipsis_ThrowsException()
        {
            var original = "123";
            Assert.Throws<ArgumentOutOfRangeException>(() => StringHelper.Snip(original, 2));
        }

        [Fact]
        public void IsFileNameMatch_IgnoreUrlEncoding()
        {
            var a = "AbC APP-0016416-John DOE-20230204-025210.pdf";
            var b = "aBc%20APP-0016416-John%20doe-20230204-025210.pdf";

            Assert.True(StringHelper.IsFileNameMatch(a, b));
        }
    }
}