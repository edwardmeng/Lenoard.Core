using System;

namespace Lenoard.Core.UnitTests
{
    public class SemanticVersionTest
    {
#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseMajorVersion()
        {
            Assert.Equal(new SemanticVersion("1.2.3").Major, 1);
            Assert.Equal(new SemanticVersion(" 1.2.3 ").Major, 1);
            Assert.Equal(new SemanticVersion(" 2.2.3-4 ").Major, 2);
            Assert.Equal(new SemanticVersion(" 3.2.3-pre ").Major, 3);
            Assert.Equal(new SemanticVersion("v5.2.3").Major, 5);
            Assert.Equal(new SemanticVersion(" v8.2.3 ").Major, 8);
            Assert.Equal(new SemanticVersion("\t13.2.3").Major, 13);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseMinorVersion()
        {
            Assert.Equal(new SemanticVersion("1.2.3").Minor, 2);
            Assert.Equal(new SemanticVersion(" 1.2.3 ").Minor, 2);
            Assert.Equal(new SemanticVersion(" 2.2.3-4 ").Minor, 2);
            Assert.Equal(new SemanticVersion(" 3.2.3-pre ").Minor, 2);
            Assert.Equal(new SemanticVersion("v5.2.3").Minor, 2);
            Assert.Equal(new SemanticVersion(" v8.2.3 ").Minor, 2);
            Assert.Equal(new SemanticVersion("\t13.2.3").Minor, 2);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParsePatchVersion()
        {
            Assert.Equal(new SemanticVersion("1.2.3").Patch, 3);
            Assert.Equal(new SemanticVersion(" 1.2.3 ").Patch, 3);
            Assert.Equal(new SemanticVersion(" 2.2.3-4 ").Patch, 3);
            Assert.Equal(new SemanticVersion(" 3.2.3-pre ").Patch, 3);
            Assert.Equal(new SemanticVersion("v5.2.3").Patch, 3);
            Assert.Equal(new SemanticVersion(" v8.2.3 ").Patch, 3);
            Assert.Equal(new SemanticVersion("\t13.2.3").Patch, 3);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest1()
        {
            var version = SemanticVersion.Parse("1.2.45-alpha+nightly.23");

            Assert.Equal(1, version.Major);
            Assert.Equal(2, version.Minor);
            Assert.Equal(45, version.Patch);
            Assert.Equal("alpha", version.Prerelease);
            Assert.Equal("nightly.23", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest2()
        {
            var version = SemanticVersion.Parse("1");

            Assert.Equal(1, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal("", version.Prerelease);
            Assert.Equal("", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest3()
        {
            var version = SemanticVersion.Parse("1.2.45-alpha-beta+nightly.23.43-bla");

            Assert.Equal(1, version.Major);
            Assert.Equal(2, version.Minor);
            Assert.Equal(45, version.Patch);
            Assert.Equal("alpha-beta", version.Prerelease);
            Assert.Equal("nightly.23.43-bla", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest4()
        {
            var version = SemanticVersion.Parse("2.0.0+nightly.23.43-bla");

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal("", version.Prerelease);
            Assert.Equal("nightly.23.43-bla", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest5()
        {
            var version = SemanticVersion.Parse("2.0+nightly.23.43-bla");

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal("", version.Prerelease);
            Assert.Equal("nightly.23.43-bla", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest6()
        {
            var version = SemanticVersion.Parse("2.1-alpha");

            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal("alpha", version.Prerelease);
            Assert.Equal("", version.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ParseTest7()
        {
            Assert.Throws<ArgumentException>(() => SemanticVersion.Parse("ui-2.1-alpha"));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseTest1()
        {
            SemanticVersion v;
            Assert.True(SemanticVersion.TryParse("1.2.45-alpha-beta+nightly.23.43-bla", out v));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseTest2()
        {
            SemanticVersion v;
            Assert.False(SemanticVersion.TryParse("ui-2.1-alpha", out v));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseTest3()
        {
            SemanticVersion v;
            Assert.False(SemanticVersion.TryParse("", out v));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseTest4()
        {
            SemanticVersion v;
            Assert.False(SemanticVersion.TryParse(null, out v));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TryParseTest5()
        {
            SemanticVersion v;
            Assert.True(SemanticVersion.TryParse("1.2", out v));
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CreateVersionTest()
        {
            var v = new SemanticVersion(1, 2, 3, "a", "b");

            Assert.Equal(1, v.Major);
            Assert.Equal(2, v.Minor);
            Assert.Equal(3, v.Patch);
            Assert.Equal("a", v.Prerelease);
            Assert.Equal("b", v.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CreateVersionTestWithNulls()
        {
            var v = new SemanticVersion(1, 2, 3, null, null);

            Assert.Equal(1, v.Major);
            Assert.Equal(2, v.Minor);
            Assert.Equal(3, v.Patch);
            Assert.Equal("", v.Prerelease);
            Assert.Equal("", v.Build);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CreateVersionTestWithSystemVersion1()
        {
            var nonSemanticVersion = new Version(0, 0);
            var v = new SemanticVersion(nonSemanticVersion);

            Assert.Equal(0, v.Major);
            Assert.Equal(0, v.Minor);
            Assert.Equal(0, v.Patch);
            Assert.Equal("", v.Build);
            Assert.Equal("", v.Prerelease);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CreateVersionTestWithSystemVersion3()
        {
            var nonSemanticVersion = new Version(1, 2, 0, 3);
            var v = new SemanticVersion(nonSemanticVersion);

            Assert.Equal(1, v.Major);
            Assert.Equal(2, v.Minor);
            Assert.Equal(0, v.Patch);
            Assert.Equal("3", v.Build);
            Assert.Equal("", v.Prerelease);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CreateVersionTestWithSystemVersion4()
        {
            var nonSemanticVersion = new Version(1, 2, 4, 3);
            var v = new SemanticVersion(nonSemanticVersion);

            Assert.Equal(1, v.Major);
            Assert.Equal(2, v.Minor);
            Assert.Equal(4, v.Patch);
            Assert.Equal("3", v.Build);
            Assert.Equal("", v.Prerelease);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void ToStringTest()
        {
            var version = new SemanticVersion(1, 2, 0, "beta", "dev-mha.120");

            Assert.Equal("1.2.0-beta+dev-mha.120", version.ToString());
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest1()
        {
            var v1 = new SemanticVersion(1, 2, 0, null, "nightly");
            var v2 = new SemanticVersion(1, 2, 0, null, "nightly");

            var r = v1.Equals(v2);
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest2()
        {
            var v1 = new SemanticVersion(1, 2, 0, "alpha", "dev");
            var v2 = new SemanticVersion(1, 2, 0, "alpha", "dev");

            var r = v1.Equals(v2);
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest3()
        {
            var v1 = SemanticVersion.Parse("1.2-nightly+dev");
            var v2 = SemanticVersion.Parse("1.2.0-nightly");

            var r = v1.Equals(v2);
            Assert.False(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest4()
        {
            var v1 = SemanticVersion.Parse("1.2-nightly");
            var v2 = SemanticVersion.Parse("1.2.0-nightly2");

            var r = v1.Equals(v2);
            Assert.False(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest5()
        {
            var v1 = SemanticVersion.Parse("1.2.1");
            var v2 = SemanticVersion.Parse("1.2.0");

            var r = v1.Equals(v2);
            Assert.False(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualTest6()
        {
            var v1 = SemanticVersion.Parse("1.4.0");
            var v2 = SemanticVersion.Parse("1.2.0");

            var r = v1.Equals(v2);
            Assert.False(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest1()
        {
            var v1 = SemanticVersion.Parse("1.0.0");
            var v2 = SemanticVersion.Parse("2.0.0");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest2()
        {
            var v1 = SemanticVersion.Parse("1.0.0-beta+dev.123");
            var v2 = SemanticVersion.Parse("1-beta+dev.123");

            var r = v1.CompareTo(v2);
            Assert.Equal(0, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest3()
        {
            var v1 = SemanticVersion.Parse("1.0.0-alpha+dev.123");
            var v2 = SemanticVersion.Parse("1-beta+dev.123");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest4()
        {
            var v1 = SemanticVersion.Parse("1.0.0-alpha");
            var v2 = SemanticVersion.Parse("1.0.0");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest5()
        {
            var v1 = SemanticVersion.Parse("1.0.0");
            var v2 = SemanticVersion.Parse("1.0.0-alpha");

            var r = v1.CompareTo(v2);
            Assert.Equal(1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest6()
        {
            var v1 = SemanticVersion.Parse("1.0.0");
            var v2 = SemanticVersion.Parse("1.0.1-alpha");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest7()
        {
            var v1 = SemanticVersion.Parse("0.0.1");
            var v2 = SemanticVersion.Parse("0.0.1+build.12");

            var r = v1.CompareTo(v2);
            Assert.Equal(0, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest8()
        {
            var v1 = SemanticVersion.Parse("0.0.1+build.13");
            var v2 = SemanticVersion.Parse("0.0.1+build.12.2");

            var r = v1.CompareTo(v2);
            Assert.Equal(0, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest9()
        {
            var v1 = SemanticVersion.Parse("0.0.1-13");
            var v2 = SemanticVersion.Parse("0.0.1-b");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest10()
        {
            var v1 = SemanticVersion.Parse("0.0.1+uiui");
            var v2 = SemanticVersion.Parse("0.0.1+12");

            var r = v1.CompareTo(v2);
            Assert.Equal(0, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest11()
        {
            var v1 = SemanticVersion.Parse("0.0.1+bu");
            var v2 = SemanticVersion.Parse("0.0.1");

            var r = v1.CompareTo(v2);
            Assert.Equal(0, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest12()
        {
            var v1 = SemanticVersion.Parse("0.1.1+bu");
            var v2 = SemanticVersion.Parse("0.2.1");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest13()
        {
            var v1 = SemanticVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemanticVersion.Parse("0.1.1-gamma.12.88");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest14()
        {
            var v1 = SemanticVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemanticVersion.Parse("0.1.1-gamma.12.87.1");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest15()
        {
            var v1 = SemanticVersion.Parse("0.1.1-gamma.12.87.99");
            var v2 = SemanticVersion.Parse("0.1.1-gamma.12.87.X");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareTest16()
        {
            var v1 = SemanticVersion.Parse("0.1.1-gamma.12.87");
            var v2 = SemanticVersion.Parse("0.1.1-gamma.12.87.X");

            var r = v1.CompareTo(v2);
            Assert.Equal(-1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void CompareNullTest()
        {
            var v1 = SemanticVersion.Parse("0.0.1+bu");
            var r = v1.CompareTo(null);
            Assert.Equal(1, r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TestHashCode()
        {
            var v1 = SemanticVersion.Parse("1.0.0-1+b");
            var v2 = SemanticVersion.Parse("1.0.0-1+c");

            var h1 = v1.GetHashCode();
            var h2 = v2.GetHashCode();

            Assert.NotEqual(h1, h2);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TestStringConversion()
        {
            SemanticVersion v = "1.0.0";
            Assert.Equal(1, v.Major);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void TestUntypedCompareTo()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var c = v1.CompareTo((object)v1);

            Assert.Equal(0, c);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void EqualsOperatorTest()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            var r = v1 == v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void UnequalOperatorTest()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(2, 0, 0);

            var r = v1 != v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterOperatorTest()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(2, 0, 0);

            var r = v2 > v1;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterOperatorTest2()
        {
            var v1 = new SemanticVersion(1, 0, 0, "alpha");
            var v2 = new SemanticVersion(1, 0, 0, "rc");

            var r = v2 > v1;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterOperatorTest3()
        {
            var v1 = new SemanticVersion(1, 0, 0, "-ci.1");
            var v2 = new SemanticVersion(1, 0, 0, "alpha");

            var r = v2 > v1;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterOrEqualOperatorTest1()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            var r = v1 >= v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void GreaterOrEqualOperatorTest2()
        {
            var v1 = new SemanticVersion(2, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            var r = v1 >= v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessOperatorTest()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(2, 0, 0);

            var r = v1 < v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessOperatorTest2()
        {
            var v1 = new SemanticVersion(1, 0, 0, "alpha");
            var v2 = new SemanticVersion(1, 0, 0, "rc");

            var r = v1 < v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessOperatorTest3()
        {
            var v1 = new SemanticVersion(1, 0, 0, "-ci.1");
            var v2 = new SemanticVersion(1, 0, 0, "alpha");

            var r = v1 < v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessOrEqualOperatorTest1()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            var r = v1 <= v2;
            Assert.True(r);
        }

#if NetCore
        [Xunit.Fact]
#else
        [NUnit.Framework.Test]
#endif
        public void LessOrEqualOperatorTest2()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(2, 0, 0);

            var r = v1 <= v2;
            Assert.True(r);
        }

#if !NetCore
        [NUnit.Framework.Test]
        public void TestSerialization()
        {
            var semVer = new SemanticVersion(1, 2, 3, "alpha", "dev");
            SemanticVersion semVerSerializedDeserialized;
            using (var ms = new System.IO.MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, semVer);
                ms.Position = 0;
                semVerSerializedDeserialized = (SemanticVersion)bf.Deserialize(ms);
            }
            Assert.Equal(semVer, semVerSerializedDeserialized);
        }
#endif
    }
}
