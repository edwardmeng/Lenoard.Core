using System;
using System.Globalization;

namespace Lenoard.Core.UnitTests
{
    public class RangeTest
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseWithParenthesis()
        {
            Assert.Equal(new Range<int>(0, 3), Range<int>.Parse("[0,3]", int.Parse));
            Assert.Equal(new Range<int>(0, 3, false), Range<int>.Parse("(0,3]", int.Parse));
            Assert.Equal(new Range<int>(0, 3, true, false), Range<int>.Parse("[0,3)", int.Parse));
            Assert.Equal(new Range<int>(0, 3, false, false), Range<int>.Parse("(0,3)", int.Parse));

            Assert.Equal(new Range<int?>(null, 3), Range<int?>.Parse("[,3]", ParseNullable));
            Assert.Equal(new Range<int?>(0, null), Range<int?>.Parse("[0,]", ParseNullable));

            Assert.Throws<FormatException>(() => Range<int>.Parse("[0,3", int.Parse));
            Assert.Throws<FormatException>(() => Range<int>.Parse("0,3]", int.Parse));
            Assert.Throws<FormatException>(() => Range<int>.Parse("[0]", int.Parse));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseWithParenthesis()
        {
            Range<int> result;
            Assert.True(Range<int>.TryParse("[0,3]", int.TryParse, out result));
            Assert.False(Range<int>.TryParse("[0,]", int.TryParse, out result));
            Assert.False(Range<int>.TryParse("[,3]", int.TryParse, out result));
            Assert.False(Range<int>.TryParse("[0,3", int.TryParse, out result));
            Assert.False(Range<int>.TryParse("0,3]", int.TryParse, out result));
            Assert.False(Range<int>.TryParse("[0]", int.TryParse, out result));

            Range<int?> nullableResult;
            Assert.True(Range<int?>.TryParse("[0,3]", TryParseNullable, out nullableResult));
            Assert.True(Range<int?>.TryParse("[0,]", TryParseNullable, out nullableResult));
            Assert.True(Range<int?>.TryParse("[,3]", TryParseNullable, out nullableResult));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseWithHyphen()
        {
            Assert.Equal(new Range<int>(0, 3), Range<int>.Parse("0~3", int.Parse));
            Assert.Equal(new Range<int?>(null, -3), Range<int?>.Parse("~-3", ParseNullable));
            Assert.Equal(new Range<int?>(-3, null), Range<int?>.Parse("-3~", ParseNullable));

            Assert.Equal(new Range<DateTime>(new DateTime(2000, 1, 1), new DateTime(2016, 1, 1)),
                Range<DateTime>.Parse("2000-01-01 - 2016-01-01", x => DateTime.ParseExact(x, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces)));
            Assert.Equal(new Range<DateTime>(new DateTime(2000, 1, 1), new DateTime(2016, 1, 1)),
               Range<DateTime>.Parse("2000-01-01 ~ 2016-01-01", x => DateTime.ParseExact(x, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces)));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseWithHyphen()
        {
            Range<int> result;
            Assert.True(Range<int>.TryParse("0~3", int.TryParse, out result));
            Range<int?> nullableResult;
            Assert.True(Range<int?>.TryParse("~3", TryParseNullable, out nullableResult));
            Assert.True(Range<int?>.TryParse("-3~", TryParseNullable, out nullableResult));
            Assert.True(Range<int?>.TryParse("~-3", TryParseNullable, out nullableResult));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseSingleValue()
        {
            Assert.Equal(new Range<int>(3, 3), Range<int>.Parse("3", int.Parse));
            Assert.Equal(new Range<int>(-3, -3), Range<int>.Parse("-3", int.Parse));
        }

        private static bool TryParseNullable(string text, out int? value)
        {
            if (string.IsNullOrEmpty(text))
            {
                value = null;
                return true;
            }
            int i;
            if (int.TryParse(text, out i))
            {
                value = i;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        private static int? ParseNullable(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            return int.Parse(text);
        }
    }
}
