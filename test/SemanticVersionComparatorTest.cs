namespace Lenoard.Core.UnitTests
{
    public class SemanticVersionComparatorTest
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseAnyComparator()
        {
            var comparator = SemanticVersionComparator.Parse("");
            Assert.True(comparator.Match("1.0.0"));
            Assert.True(comparator.Match("2.0.0"));

            comparator = SemanticVersionComparator.Parse(" ");
            Assert.True(comparator.Match("1.0.0"));
            Assert.True(comparator.Match("2.0.0"));

            comparator = SemanticVersionComparator.Parse("*");
            Assert.True(comparator.Match("1.0.0"));
            Assert.True(comparator.Match("2.0.0"));

            comparator = SemanticVersionComparator.Parse(" * ");
            Assert.True(comparator.Match("1.0.0"));
            Assert.True(comparator.Match("2.0.0"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void SimpleComparatorTest()
        {
            var comparator = SemanticVersionComparator.Parse("1.2.x");
            Assert.False(comparator.Match("1.1.9"));
            Assert.True(comparator.Match("1.2.0"));
            Assert.False(comparator.Match("1.2.0-alpha"));
            Assert.False(comparator.Match("1.3.0"));

            comparator = SemanticVersionComparator.Parse("1.2");
            Assert.False(comparator.Match("1.1.9"));
            Assert.True(comparator.Match("1.2.0"));
            Assert.False(comparator.Match("1.2.0-alpha"));
            Assert.False(comparator.Match("1.3.0"));

            comparator = SemanticVersionComparator.Parse("1.2.3");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.3-alpha"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterThanPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse(">1.2.3");
            Assert.False(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha"));

            comparator = SemanticVersionComparator.Parse(">1.2.x");
            Assert.False(comparator.Match("1.2.0"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha"));

            comparator = SemanticVersionComparator.Parse(">1.2.3-alpha.1");
            Assert.True(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.3-alpha.2"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterThanOrEqualToPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse(">=1.2.3");
            Assert.False(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.3-alpha"));
            Assert.True(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha"));

            comparator = SemanticVersionComparator.Parse(">=1.2.x");
            Assert.False(comparator.Match("1.1.9"));
            Assert.True(comparator.Match("1.2.0"));
            Assert.False(comparator.Match("1.2.0-alpha"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha"));

            comparator = SemanticVersionComparator.Parse(">=1.2.3-alpha.1");
            Assert.True(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.3-Alpha.1"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.4-alpha.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessThanPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse("<1.2.3");
            Assert.False(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.3-alpha"));
            Assert.True(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.2-alpha"));

            comparator = SemanticVersionComparator.Parse("<1.2.x");
            Assert.False(comparator.Match("1.3.0"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.2.9-alpha"));
            Assert.True(comparator.Match("1.1.9"));
            Assert.False(comparator.Match("1.1.9-alpha"));

            comparator = SemanticVersionComparator.Parse("<1.2.3-alpha.1");
            Assert.False(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.3-alpha"));
            Assert.True(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.2-alpha.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessThanOrEqualToPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse("<=1.2.3");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.3-alpha"));
            Assert.True(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.2-alpha"));

            comparator = SemanticVersionComparator.Parse("<=1.2.x");
            Assert.False(comparator.Match("1.3.0"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.2.9-alpha"));
            Assert.True(comparator.Match("1.1.9"));
            Assert.False(comparator.Match("1.1.9-alpha"));

            comparator = SemanticVersionComparator.Parse("<=1.2.3-alpha.1");
            Assert.False(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.3-alpha.1"));
            Assert.True(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.2-alpha.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualToPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse("=1.2.3");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.4"));
            Assert.False(comparator.Match("1.2.3-alpha"));

            comparator = SemanticVersionComparator.Parse("=1.2.x");
            Assert.False(comparator.Match("1.3.0"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.2.9-alpha"));

            comparator = SemanticVersionComparator.Parse("=1.2.3-alpha.1");
            Assert.False(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.3-alpha.1"));
            Assert.False(comparator.Match("1.2.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void NotEqualToPrimitiveComparator()
        {
            var comparator = SemanticVersionComparator.Parse("!=1.2.3");
            Assert.False(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("1.2.4"));
            Assert.True(comparator.Match("1.2.3-alpha"));

            comparator = SemanticVersionComparator.Parse("<>1.2.x");
            Assert.True(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.2.9"));
            Assert.True(comparator.Match("1.2.9-alpha"));

            comparator = SemanticVersionComparator.Parse("!=1.2.3-alpha.1");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.3-alpha.1"));
            Assert.True(comparator.Match("1.2.2"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParenthesesComparator()
        {
            var comparator = SemanticVersionComparator.Parse("[ 1.2.3,2.0.1)");
            Assert.False(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("2.0.1"));
            Assert.True(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("2.0.0"));
            Assert.False(comparator.Match("2.0.0-alpha"));

            comparator = SemanticVersionComparator.Parse("[1.2.3 ,2.0.1-alpha.1)");
            Assert.False(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("2.0.1"));
            Assert.True(comparator.Match("1.2.3"));
            Assert.True(comparator.Match("2.0.0"));
            Assert.False(comparator.Match("2.0.0-alpha"));
            Assert.True(comparator.Match("2.0.1-alpha"));

            comparator = SemanticVersionComparator.Parse(" [ 1.2.3-alpha.1 , 1.2.3-alpha.9 ) ");
            Assert.False(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.2-alpha.1"));
            Assert.False(comparator.Match("1.2.4-alpha.1"));
            Assert.True(comparator.Match("1.2.3-alpha.1"));
            Assert.True(comparator.Match("1.2.3-alpha.8"));
            Assert.False(comparator.Match("1.2.3-alpha.9"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TildeRangeComparator()
        {
            var comparator = SemanticVersionComparator.Parse("~1.2.3");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.3-alpha"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.2.2"));

            comparator = SemanticVersionComparator.Parse("~ 1.2");
            Assert.True(comparator.Match("1.2.0"));
            Assert.False(comparator.Match("1.2.0-alpha"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.9"));

            comparator = SemanticVersionComparator.Parse("~1");
            Assert.True(comparator.Match("1.0.0"));
            Assert.False(comparator.Match("1.0.0-alpha"));
            Assert.True(comparator.Match("1.9.9"));
            Assert.False(comparator.Match("2.0.0"));
            Assert.False(comparator.Match("0.9.9"));

            comparator = SemanticVersionComparator.Parse("~1.2.3-beta.2");
            Assert.True(comparator.Match("1.2.3-beta.2"));
            Assert.False(comparator.Match("1.2.4-beta.2"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.True(comparator.Match("1.2.3-beta.3"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CaretRangeComparator()
        {
            var comparator = SemanticVersionComparator.Parse("^1.2.3");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.4-alpha"));
            Assert.True(comparator.Match("1.9.9"));
            Assert.False(comparator.Match("2.0.0"));
            Assert.False(comparator.Match("1.2.2"));

            comparator = SemanticVersionComparator.Parse("^ 0.2.3");
            Assert.True(comparator.Match("0.2.3"));
            Assert.False(comparator.Match("0.2.4-alpha"));
            Assert.True(comparator.Match("0.2.9"));
            Assert.False(comparator.Match("0.3.0"));
            Assert.False(comparator.Match("0.2.2"));

            comparator = SemanticVersionComparator.Parse("^0.0.3");
            Assert.True(comparator.Match("0.0.3"));
            Assert.False(comparator.Match("0.0.3-alpha"));
            Assert.False(comparator.Match("0.0.4-alpha"));
            Assert.False(comparator.Match("0.0.4"));
            Assert.False(comparator.Match("0.0.2"));

            comparator = SemanticVersionComparator.Parse("^1.2.3-beta.2");
            Assert.True(comparator.Match("1.2.3-beta.2"));
            Assert.False(comparator.Match("1.2.4-beta.2"));
            Assert.True(comparator.Match("1.9.9"));
            Assert.False(comparator.Match("2.0.0"));
            Assert.True(comparator.Match("1.2.3-beta.3"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void HyphenRangeComparator()
        {
            var comparator = SemanticVersionComparator.Parse("1.2.3 - 2.3.4");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.3-alpha.1"));
            Assert.False(comparator.Match("1.2.2"));
            Assert.True(comparator.Match("2.3.4"));
            Assert.False(comparator.Match("2.3.5"));
            Assert.False(comparator.Match("2.3.4-alpha.1"));

            comparator = SemanticVersionComparator.Parse("1.2 - 2.3");
            Assert.True(comparator.Match("1.2.0"));
            Assert.False(comparator.Match("1.2.0-alpha.1"));
            Assert.False(comparator.Match("1.1.9"));
            Assert.True(comparator.Match("2.3.9"));
            Assert.False(comparator.Match("2.3.9-alpha.1"));
            Assert.False(comparator.Match("2.4.0"));

            comparator = SemanticVersionComparator.Parse("1.2.3-alpha.1 - 2.3.4");
            Assert.True(comparator.Match("1.2.3-alpha.1"));
            Assert.True(comparator.Match("1.2.3-alpha.2"));
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("1.2.4-alpha.1"));
            Assert.False(comparator.Match("2.3.4-alpha.1"));
            Assert.True(comparator.Match("2.3.4"));
            Assert.False(comparator.Match("2.3.5"));

            comparator = SemanticVersionComparator.Parse("1.2.3 - 2.3.4-alpha.1");
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.3-alpha"));
            Assert.False(comparator.Match("1.2.2"));
            Assert.True(comparator.Match("2.3.4-alpha"));
            Assert.False(comparator.Match("2.3.3-alpha"));
            Assert.False(comparator.Match("2.3.5"));

            comparator = SemanticVersionComparator.Parse("1.2.3-alpha.1 - 2.3.4-alpha.1");
            Assert.True(comparator.Match("1.2.3-alpha.1"));
            Assert.True(comparator.Match("1.2.3-alpha.2"));
            Assert.True(comparator.Match("1.2.3"));
            Assert.False(comparator.Match("1.2.2"));
            Assert.False(comparator.Match("2.3.4"));
            Assert.False(comparator.Match("2.3.5"));
            Assert.True(comparator.Match("2.3.4-alpha.1"));
            Assert.True(comparator.Match("2.3.4-alpha"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void IntersectSetComparator()
        {
            var comparator = SemanticVersionComparator.Parse(">=1.2.7 <1.3.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));

            comparator = SemanticVersionComparator.Parse(">=1.2.7 && <1.3.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));

            comparator = SemanticVersionComparator.Parse(">=1.2.7 & <1.3.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));

            comparator = SemanticVersionComparator.Parse(">=1.2.7 1.2.6 - 1.3.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.True(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));

            comparator = SemanticVersionComparator.Parse(">=1.2.7 [1.2.6 , 1.3.0)");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));

            comparator = SemanticVersionComparator.Parse(">=1.2.7 (1.2.6 , 1.3.0)");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.8"));
            Assert.True(comparator.Match("1.2.99"));
            Assert.False(comparator.Match("1.2.6"));
            Assert.False(comparator.Match("1.3.0"));
            Assert.False(comparator.Match("1.1.0"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void UnionSetComparator()
        {
            var comparator = SemanticVersionComparator.Parse("1.2.7 || >=1.2.9 <2.0.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.True(comparator.Match("1.4.6"));
            Assert.False(comparator.Match("1.2.8"));
            Assert.False(comparator.Match("2.0.0"));

            comparator = SemanticVersionComparator.Parse("1.2.7 | >=1.2.9 <2.0.0");
            Assert.True(comparator.Match("1.2.7"));
            Assert.True(comparator.Match("1.2.9"));
            Assert.True(comparator.Match("1.4.6"));
            Assert.False(comparator.Match("1.2.8"));
            Assert.False(comparator.Match("2.0.0"));
        }
    }
}
