using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lenoard.Core
{
    /// <summary>
    /// Reprensents a version object, compliant with the Semantic Version 2.0(https://github.com/npm/node-semver)
    /// </summary>
    public class SemanticVersionComparator
    {
        private class ComparatorSemanticVersion
        {
            private static readonly Regex VersionExpression =
                new Regex(@"^(?<major>\d+)" +
                    @"(\.(?<minor>\d+|x|X|\*))?" +
                    @"(\.(?<patch>\d+|x|X|\*))?" +
                    @"(\-(?<pre>[0-9A-Za-z\-\.]+))?$",
#if NetCore
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
#else
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
#endif

            private ComparatorSemanticVersion(int major, int? minor, int? patch, string prerelease = null)
            {
                Major = major;
                Minor = minor;
                Patch = patch;
                Prerelease = prerelease ?? string.Empty;
            }

            public int Major { get; }

            public int? Minor { get; }

            public int? Patch { get; }

            public string Prerelease { get; }

            public override int GetHashCode()
            {
                var hashCode = Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ Patch.GetHashCode();
                if (!string.IsNullOrWhiteSpace(Prerelease))
                {
                    hashCode = (hashCode * 397) ^ Prerelease.GetHashCode();
                }
                return hashCode;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null)) return false;
                if (ReferenceEquals(this, obj)) return true;
                var other = obj as ComparatorSemanticVersion;
                if (other == null) return false;
                return Major == other.Major && Minor == other.Minor && Patch == other.Patch && string.Equals(Prerelease, other.Prerelease, StringComparison.OrdinalIgnoreCase);
            }

            public static bool TryParse(string versionString, out ComparatorSemanticVersion version)
            {
                version = null;
                if (versionString == null) return false;
                versionString = versionString.Trim();
                if (versionString.Length == 0) return false;
                if (versionString[0] == 'v' || versionString[0] == 'V') versionString = versionString.Substring(1);
                if (versionString.Length == 0) return false;
                var versionMatch = VersionExpression.Match(versionString);

                if (!versionMatch.Success)
                    return false;
                int? major,patch, minor;
                if (!TryParseNumber(versionMatch.Groups["major"].Value, false, out major)) return false;
                if (!TryParseNumber(versionMatch.Groups["minor"].Value, true, out minor)) return false;
                if (!TryParseNumber(versionMatch.Groups["patch"].Value, true, out patch)) return false;
                var pre = versionMatch.Groups["pre"].Value;
                version = new ComparatorSemanticVersion(major??0, minor, patch, pre);
                return true;
            }

            private static bool TryParseNumber(string value, bool allowWildcast, out int? result)
            {
                result = null;
                if (string.IsNullOrEmpty(value)|| value != "x" && value != "X" && value != "*")
                {
                    return allowWildcast;
                }
                int versionNumber;
                if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out versionNumber))
                {
                    return false;
                }
                result = versionNumber;
                return true;
            }

            public SemanticVersion ToLowerBound()
            {
                return new SemanticVersion(Major, Minor ?? 0, Patch ?? 0, Prerelease);
            }

            public SemanticVersion ToUpperBound()
            {
                return new SemanticVersion(Major, Minor ?? int.MaxValue, Patch ?? int.MaxValue, Prerelease);
            }
        }

        private readonly string _originalString;
        private readonly Func<SemanticVersion, bool> _calculator;

        private static readonly Func<SemanticVersion, bool> _anyCalculator = version => true;

        private SemanticVersionComparator(string originalString, Func<SemanticVersion, bool> calculator)
        {
            _originalString = originalString;
            _calculator = calculator;
        }

        /// <summary>
        /// Gives a normalized representation of the semantic version comparator.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this semantic version comparator.
        /// </returns>
        public override string ToString()
        {
            return _originalString;
        }

        /// <summary>
        /// Determines whether the specified semantic version satisfies the current comparator.
        /// </summary>
        /// <param name="version">The semantic version that is being checked to determine whether it matches the comparator.</param>
        /// <returns>
        /// <c>true</c> if the semantic version satisfies the current comparator; otherwise, <c>false</c>.
        /// </returns>
        public bool Match(SemanticVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }
            return _calculator(version);
        }

        /// <summary>Parses the specified string to a semantic version comparator.</summary>
        /// <param name="value">The version comparator string.</param>
        /// <returns>A new <see cref="SemanticVersion" /> object that has the specified values.</returns>
        /// <exception cref="ArgumentNullException">Raised when the input string is null.</exception>
        /// <exception cref="ArgumentException">Raised when the the input string is in an invalid format.</exception>
        public static SemanticVersionComparator Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value), Strings.CannotNullOrEmpty);
            SemanticVersionComparator comparator;
            if (!TryParse(value, out comparator))
                throw new ArgumentException(Strings.InvalidVersion, nameof(value));
            return comparator;
        }

        /// <summary>Tries to parse the specified string into a semantic version comparator.</summary>
        /// <param name="value">The version comparator string.</param>
        /// <param name="comparator">
        ///     When the method returns, contains a <see cref="SemanticVersionComparator"/> instance equivalent
        ///     to the version comparator string passed in, if the version comparator string was valid, or <c>null</c> if the
        ///     version comparator string was not valid.
        /// </param>
        /// <returns><c>False</c> when a invalid version comparator string is passed, otherwise <c>true</c>.</returns>
        public static bool TryParse(string value, out SemanticVersionComparator comparator)
        {
            comparator = null;
            if (string.IsNullOrEmpty(value))
            {
                comparator = new SemanticVersionComparator(value, _anyCalculator);
                return true;
            }
            value = value.Trim();
            if (value.Length == 0)
            {
                comparator = new SemanticVersionComparator(value, _anyCalculator);
                return true;
            }
            Func<SemanticVersion, bool> expr;
            if (ParseUnionSet(value, out expr))
            {
                comparator = new SemanticVersionComparator(value, expr);
                return true;
            }
            return false;
        }

        private static bool ParseUnionSet(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Trim();
            if (value.Length == 0) return false;
            var parts = new List<Func<SemanticVersion, bool>>();
            foreach (var text in Regex.Split(value, @"\|\|?"))
            {
                Func<SemanticVersion, bool> comparator;
                if (!ParseIntersectSet(text, out comparator))
                {
                    return false;
                }
                parts.Add(comparator);
            }
            expr = version => parts.Any(part => part(version));
            return true;
        }

        private static bool ParseIntersectSet(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Trim();
            if (value.Length == 0) return false;
            var parts = new List<Func<SemanticVersion, bool>>();
            foreach (var text in Regex.Split(value, @"(&&?)|\s+"))
            {
                Func<SemanticVersion, bool> comparator;
                if (!ParseRange(text, out comparator))
                {
                    return false;
                }
                parts.Add(comparator);
            }
            expr = version => parts.All(part => part(version));
            return true;
        }

        private static bool ParseRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Trim();
            if (value.Length == 0) return false;
            return ParseHyphenRange(value, out expr) ||
                   ParseCaretRange(value, out expr) ||
                   ParseTildeRange(value, out expr) ||
                   ParseParenthesesRange(value, out expr) ||
                   ParsePrimitiveRange(value, out expr) ||
                   ParseSimpleComparator(value, out expr);
        }

        /// <summary>
        /// Parses the Hyphen Ranges like X.Y.Z - A.B.C
        /// </summary>
        private static bool ParseHyphenRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            var boundaries = Regex.Split(value, @"\s+-\s+");
            if (boundaries.Length != 2)
            {
                return false;
            }
            ComparatorSemanticVersion lowerBoundary, upperBoundary;
            if (!ComparatorSemanticVersion.TryParse(boundaries[0], out lowerBoundary))
            {
                return false;
            }
            if (!ComparatorSemanticVersion.TryParse(boundaries[1], out upperBoundary))
            {
                return false;
            }
            expr = CreateComparator(lowerBoundary.ToLowerBound(), upperBoundary.ToUpperBound(), true, true);
            return true;
        }

        /// <summary>
        /// Parses the Caret Ranges like ^X.Y.Z
        /// </summary>
        private static bool ParseCaretRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            if (value[0] != '^') return false;
            ComparatorSemanticVersion version;
            if (!ComparatorSemanticVersion.TryParse(value.Substring(1), out version)) return false;
            SemanticVersion upperBound;
            if (version.Major != 0 || !version.Minor.HasValue)
            {
                upperBound = new SemanticVersion(version.Major + 1);
            }
            else if (version.Minor != 0 || !version.Patch.HasValue)
            {
                upperBound = new SemanticVersion(version.Major, version.Minor.Value + 1);
            }
            else
            {
                upperBound = new SemanticVersion(version.Major, version.Minor.Value, version.Patch.Value + 1);
            }
            expr = CreateComparator(version.ToLowerBound(), upperBound, true, false);
            return true;
        }

        /// <summary>
        /// Parses the Tilde Ranges like ~X.Y.Z
        /// </summary>
        /// <remarks>
        /// Allows patch-level changes if a minor version is specified on the comparator. Allows minor-level changes if not.
        /// </remarks>
        private static bool ParseTildeRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            if (value[0] != '~') return false;
            ComparatorSemanticVersion version;
            if (!ComparatorSemanticVersion.TryParse(value.Substring(1), out version)) return false;
            var upperBound = !version.Minor.HasValue
                ? new SemanticVersion(version.Major + 1)
                : new SemanticVersion(version.Major, version.Minor.Value + 1);
            expr = CreateComparator(version.ToLowerBound(), upperBound, true, false);
            return true;
        }

        /// <summary>
        /// Parses the Parentheses Range like [X.Y.Z, A.B.C] or (X.Y.Z, A.B.C)
        /// </summary>
        private static bool ParseParenthesesRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            bool includeLowerBound, includeUpperBound;
            char firstChar = value[0], lastChar = value[value.Length - 1];
            if (firstChar == '(')
            {
                includeLowerBound = false;
            }
            else if (firstChar == '[')
            {
                includeLowerBound = true;
            }
            else
            {
                return false;
            }
            if (lastChar == ')')
            {
                includeUpperBound = false;
            }
            else if (lastChar == ']')
            {
                includeUpperBound = true;
            }
            else
            {
                return false;
            }
            value = value.Substring(1, value.Length - 2);
            var boundaries = value.Split(',');
            if (boundaries.Length != 2) return false;
            SemanticVersion lowerBound, upperBound;
            if (string.IsNullOrWhiteSpace(boundaries[0]))
            {
                lowerBound = null;
            }
            else if (!SemanticVersion.TryParse(boundaries[0].Trim(), out lowerBound))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(boundaries[1]))
            {
                upperBound = null;
            }
            else if (!SemanticVersion.TryParse(boundaries[1].Trim(), out upperBound))
            {
                return false;
            }
            expr = CreateComparator(lowerBound, upperBound, includeLowerBound, includeUpperBound);
            return true;
        }

        /// <summary>
        /// Parses the primitive ranges.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>The set of primitive operators is:</listheader>
        ///     <item>
        ///         <term>&lt; </term>
        ///         <description>Less than</description>
        ///     </item>
        ///     <item>
        ///         <term>&lt;= </term>
        ///         <description>Less than or equal to</description>
        ///     </item>
        ///     <item>
        ///         <term>&gt; </term>
        ///         <description>Greater than</description>
        ///     </item>
        ///     <item>
        ///         <term>&gt;= </term>
        ///         <description>Greater than or equal to</description>
        ///     </item>
        ///     <item>
        ///         <term>= </term>
        ///         <description>Equal</description>
        ///     </item>
        /// </list>
        /// </remarks>
        private static bool ParsePrimitiveRange(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            Func<ComparatorSemanticVersion, Func<SemanticVersion, bool>> comparatorCreator;
            int compareLength;
            if (value.StartsWith(">="))
            {
                comparatorCreator =
                    comparator => CreateComparator(comparator.ToLowerBound(), null, true, false);
                compareLength = 2;
            }
            else if (value.StartsWith("<="))
            {
                comparatorCreator =
                    comparator =>
                        CreateComparator(null, comparator.ToUpperBound(), false, true);
                compareLength = 2;
            }
            else if (value.StartsWith("!=") || value.StartsWith("<>"))
            {
                comparatorCreator = comparator =>
                {
                    var compareVersion = comparator.ToLowerBound();
                    return version => version != compareVersion;
                };
                compareLength = 2;
            }
            else if (value.StartsWith(">"))
            {
                comparatorCreator = comparator => CreateComparator(comparator.ToLowerBound(), null, false, false);
                compareLength = 1;
            }
            else if (value.StartsWith("<"))
            {
                comparatorCreator =
                    comparator => CreateComparator(null, comparator.ToUpperBound(), false, false);
                compareLength = 1;
            }
            else if (value.StartsWith("="))
            {
                comparatorCreator = comparator =>
                {
                    var compareVersion = comparator.ToLowerBound();
                    return version => version == compareVersion;
                };
                compareLength = 1;
            }
            else
            {
                return false;
            }
            ComparatorSemanticVersion compare;
            if (!ComparatorSemanticVersion.TryParse(value.Substring(compareLength), out compare))
            {
                return false;
            }
            var compareExpr = comparatorCreator(compare);
            expr = version => compareExpr(version);
            return true;
        }

        private static bool ParseSimpleComparator(string value, out Func<SemanticVersion, bool> expr)
        {
            expr = null;
            ComparatorSemanticVersion version;
            if (!ComparatorSemanticVersion.TryParse(value, out version)) return false;
            SemanticVersion upperBound;
            if (!version.Minor.HasValue)
            {
                upperBound = new SemanticVersion(version.Major + 1);
            }
            else if (!version.Patch.HasValue)
            {
                upperBound = new SemanticVersion(version.Major, version.Minor.Value + 1);
            }
            else
            {
                expr = x => x == version.ToLowerBound();
                return true;
            }
            expr = CreateComparator(version.ToLowerBound(), upperBound, true, false);
            return true;
        }

        private static Func<SemanticVersion, bool> CreateComparator(SemanticVersion lowerBound, SemanticVersion upperBound, bool includeLowerBound, bool includeUpperBound)
        {
            Func<SemanticVersion, bool> lowerComparator, upperComparator;
            if (lowerBound == null)
            {
                lowerComparator = version => true;
            }
            else if (!string.IsNullOrEmpty(lowerBound.Prerelease))
            {
                if (includeLowerBound)
                {
                    lowerComparator = version => string.IsNullOrEmpty(version.Prerelease)
                        ? version >= lowerBound
                        : VersionNumbersSame(version, lowerBound) &&
                          SemanticVersion.ComparePreRelease(version.Prerelease, lowerBound.Prerelease) >= 0;
                }
                else
                {
                    lowerComparator = version => string.IsNullOrEmpty(version.Prerelease)
                        ? version > lowerBound
                        : VersionNumbersSame(version, lowerBound) &&
                          SemanticVersion.ComparePreRelease(version.Prerelease, lowerBound.Prerelease) > 0;
                }
            }
            else
            {
                if (includeLowerBound)
                {
                    lowerComparator = version => string.IsNullOrEmpty(version.Prerelease) && version >= lowerBound;
                }
                else
                {
                    lowerComparator = version => string.IsNullOrEmpty(version.Prerelease) && version > lowerBound;
                }
            }
            if (upperBound == null)
            {
                upperComparator = version => true;
            }
            else if (!string.IsNullOrEmpty(upperBound.Prerelease))
            {
                if (includeUpperBound)
                {
                    upperComparator = version => string.IsNullOrEmpty(version.Prerelease)
                        ? version <= upperBound
                        : VersionNumbersSame(version, upperBound) &&
                          SemanticVersion.ComparePreRelease(version.Prerelease, upperBound.Prerelease) <= 0;
                }
                else
                {
                    upperComparator = version => string.IsNullOrEmpty(version.Prerelease)
                        ? version < upperBound
                        : VersionNumbersSame(version, upperBound) &&
                          SemanticVersion.ComparePreRelease(version.Prerelease, upperBound.Prerelease) > 0;
                }
            }
            else
            {
                if (includeUpperBound)
                {
                    upperComparator = version => string.IsNullOrEmpty(version.Prerelease) && version <= upperBound;
                }
                else
                {
                    upperComparator = version => string.IsNullOrEmpty(version.Prerelease) && version < upperBound;
                }
            }
            return version => lowerComparator(version) && upperComparator(version);
        }

        private static bool VersionNumbersSame(SemanticVersion x, SemanticVersion y)
        {
            return x.Major == y.Major && x.Minor == y.Minor && x.Patch == y.Patch;
        }
    }
}
