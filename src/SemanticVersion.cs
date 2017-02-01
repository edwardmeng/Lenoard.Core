using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lenoard.Core
{
    /// <summary>Reprensents a version object, compliant with the Semantic Version standard 2.0 (http://semver.org)</summary>
#if NetCore
    public sealed class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IComparable
#else
    [Serializable]
    public sealed class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IComparable, System.Runtime.Serialization.ISerializable
#endif
    {
        #region SemanticVersionFormatter
        private class SemanticVersionFormatter : IFormatProvider, ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg == null) throw new ArgumentNullException(nameof(arg));
                if (string.IsNullOrEmpty(format)) return string.Empty;
                var version = arg as SemanticVersion;
                if (version == null) return null;
                // single char identifiers
                return Format(format[0], version);
            }

            public object GetFormat(Type formatType)
            {
                return formatType == typeof(ICustomFormatter) || formatType == typeof(SemanticVersion) ? this : null;
            }

            private static string GetNormalizedString(SemanticVersion version)
            {
                var sb = new StringBuilder();

                sb.Append(Format('V', version));

                if (!string.IsNullOrWhiteSpace(version.Prerelease))
                {
                    sb.Append('-');
                    sb.Append(version.Prerelease);
                }

                if (!string.IsNullOrWhiteSpace(version.Build))
                {
                    sb.Append('+');
                    sb.Append(version.Build);
                }

                return sb.ToString();
            }

            private static string Format(char c, SemanticVersion version)
            {
                switch (c)
                {
                    case 'N':
                        return GetNormalizedString(version);
                    case 'V':
                        return $"{version.Major.ToString(CultureInfo.InvariantCulture)}.{version.Minor.ToString(CultureInfo.InvariantCulture)}.{version.Patch.ToString(CultureInfo.InvariantCulture)}";
                }
                return c.ToString();
            }
        }
        #endregion

        #region SemanticVersionComparer
        private class SemanticVersionComparer : IEqualityComparer<SemanticVersion>, IComparer<SemanticVersion>
        {
            bool IEqualityComparer<SemanticVersion>.Equals(SemanticVersion x, SemanticVersion y)
            {
                return Compare(x, y) == 0;
            }

            int IEqualityComparer<SemanticVersion>.GetHashCode(SemanticVersion version)
            {
                if (ReferenceEquals(version, null)) return 0;
                var hashCode = version.Major.GetHashCode();
                hashCode = (hashCode * 397) ^ version.Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ version.Patch.GetHashCode();
                if (!string.IsNullOrWhiteSpace(version.Prerelease))
                {
                    hashCode = (hashCode * 397) ^ version.Prerelease.GetHashCode();
                }
                if (!string.IsNullOrWhiteSpace(version.Build))
                {
                    hashCode = (hashCode * 397) ^ version.Build.GetHashCode();
                }
                return hashCode;
            }

            int IComparer<SemanticVersion>.Compare(SemanticVersion x, SemanticVersion y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(y, null)) return 1;
                if (ReferenceEquals(x, null)) return -1;
                // compare version
                var result = x.Major.CompareTo(y.Major);
                if (result != 0) return result;

                result = x.Minor.CompareTo(y.Minor);
                if (result != 0) return result;

                result = x.Patch.CompareTo(y.Patch);
                if (result != 0) return result;

                // compare release labels
                bool xIsPrerelease = !string.IsNullOrWhiteSpace(x.Prerelease),
                    yIsPrerelease = !string.IsNullOrWhiteSpace(y.Prerelease);
                if (xIsPrerelease && !yIsPrerelease) return -1;
                if (!xIsPrerelease && yIsPrerelease) return 1;

                return xIsPrerelease ? ComparePreRelease(x.Prerelease, y.Prerelease) : 0;
            }
        }
        #endregion

        #region Fields

        private static readonly Regex VersionExpression =
            new Regex(@"^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
                @"(\+(?<build>[0-9A-Za-z\-\.]+))?$",
#if NetCore
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);
#else
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
#endif
        private static readonly SemanticVersionComparer VersionComparer = new SemanticVersionComparer();
        private static readonly SemanticVersionFormatter VersionFormatter = new SemanticVersionFormatter();

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a SemanticVersion using SemanticVersion.Parse(string)
        /// </summary>
        /// <param name="version">Version string</param>
        public SemanticVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException(nameof(version), Strings.CannotNullOrEmpty);
            int major, minor, patch;
            string pre, build;
            if (TryParse(version, out major, out minor, out patch, out pre, out build))
            {
                Major = major;
                Minor = minor;
                Patch = patch;
                Prerelease = pre ?? string.Empty;
                Build = build ?? string.Empty;
            }
            else
            {
                throw new ArgumentException(Strings.InvalidVersion, nameof(version));
            }
        }

        /// <summary>
        ///     Creates a SemanticVersion from an existing SemanticVersion
        /// </summary>
        public SemanticVersion(SemanticVersion version)
            : this(version.Major, version.Minor, version.Patch, version.Prerelease, version.Build)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="version">The <see cref="System.Version"/> that is used to initialize 
        /// the Major, Minor, Patch and Build properties.</param>
        public SemanticVersion(Version version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            Major = version.Major;
            Minor = version.Minor;
            Patch = version.Build >= 0 ? version.Build : 0;
            Prerelease = string.Empty;
            Build = version.Revision >= 0 ? version.Revision.ToString() : string.Empty;
        }

        /// <summary>Initializese a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version component.</param>
        public SemanticVersion(int major)
            : this(major, 0)
        {
        }

        /// <summary>Initializese a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        public SemanticVersion(int major, int minor)
            : this(major, minor, 0)
        {
        }

        /// <summary>Initializese a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        public SemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, null)
        {
        }

        /// <summary>Initializese a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version component.</param>
        /// <param name="minor">The minor version component.</param>
        /// <param name="patch">The patch version component.</param>
        /// <param name="prerelease">The pre-release version component.</param>
        /// <param name="build">The build version component.</param>
        public SemanticVersion(int major, int minor, int patch, string prerelease = null, string build = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Prerelease = prerelease ?? string.Empty;
            Build = build ?? string.Empty;
        }

        #endregion

        #region Properties

        /// <summary>Gets the major version component.</summary>
        public int Major { get; }

        /// <summary>Gets the minor version component.</summary>
        public int Minor { get; }

        /// <summary>Gets the patch version component.</summary>
        public int Patch { get; }

        /// <summary>Gets the pre-release version component.</summary>
        public string Prerelease { get; }

        /// <summary>Gets the build version component.</summary>
        public string Build { get; }

        #endregion

        #region Serialization

#if !NetCore
        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion" /> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private SemanticVersion(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            Major = info.GetInt32("Major");
            Minor = info.GetInt32("Minor");
            Patch = info.GetInt32("Patch");
            Prerelease = info.GetString("Prerelease");
            Build = info.GetString("Build");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue("Major", Major);
            info.AddValue("Minor", Minor);
            info.AddValue("Patch", Patch);
            info.AddValue("Prerelease", Prerelease);
            info.AddValue("Build", Build);
        }
#endif

        #endregion

        #region Format

        /// <summary>
        /// Gives a normalized representation of the semantic version.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this semantic version.
        /// </returns>
        public override string ToString()
        {
            return ToString("N");
        }

        /// <summary>Formats the semantic version using the specified format.</summary>
        /// <param name="format">
        /// The format to use.
        /// -or- 
        /// A null reference to use the default format. 
        /// </param>
        /// <returns>The semantic version in the specified format.</returns>
        /// <remarks>
        /// <list type="bullet">
        ///     <listheader>The format value should be following:</listheader>
        ///     <item>
        ///         <term>N: </term>
        ///         <description>The normalized version string. The default value.</description>
        ///     </item>
        ///     <item>
        ///         <term>V: </term>
        ///         <description>Include only the version numbers, except the prerelease and build informations.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public string ToString(string format)
        {
            return ToString(format, VersionFormatter);
        }

        private string ToString(string format, IFormatProvider formatProvider)
        {
            string formattedString;

            if (formatProvider == null
                || !TryFormatter(format, formatProvider, out formattedString))
            {
                formattedString = ToString();
            }

            return formattedString;
        }

        private bool TryFormatter(string format, IFormatProvider formatProvider, out string formattedString)
        {
            var formatted = false;
            formattedString = null;

            var formatter = formatProvider?.GetFormat(typeof(SemanticVersion)) as ICustomFormatter;
            if (formatter != null)
            {
                formatted = true;
                formattedString = formatter.Format(format, this, formatProvider);
            }

            return formatted;
        }

        #endregion

        #region Compare

        internal static int ComparePreRelease(string version1, string version2)
        {
            string[] xReleaseParts = version1.Split('.'), yReleaseParts = version2.Split('.');
            for (int i = 0, length = Math.Max(xReleaseParts.Length, yReleaseParts.Length); i < length; i++)
            {
                if (i >= xReleaseParts.Length) return -1;
                if (i >= yReleaseParts.Length) return 1;
                var result = ComparePreReleasePart(xReleaseParts[i], yReleaseParts[i]);
                if (result != 0) return result;
            }
            return 0;
        }

        private static int ComparePreReleasePart(string version1, string version2)
        {
            int version1Num, version2Num;

            // check if the identifiers are numeric
            var v1IsNumeric = int.TryParse(version1, out version1Num);
            var v2IsNumeric = int.TryParse(version2, out version2Num);

            // if both are numeric compare them as numbers
            if (v1IsNumeric && v2IsNumeric)
            {
                return version1Num.CompareTo(version2Num);
            }
            if (v1IsNumeric || v2IsNumeric)
            {
                // numeric labels come before alpha labels
                return v1IsNumeric ? -1 : 1;
            }
            // Ignoring 2.0.0 case sensitive compare. Everything will be compared case insensitively as 2.0.1 specifies.
            return StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
        }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            return CompareTo(obj as SemanticVersion);
        }

        /// <inheritdoc />
        public int CompareTo(SemanticVersion other)
        {
            return Compare(this, other);
        }

        /// <summary>
        /// Compares the specified versions.
        /// </summary>
        /// <param name="version1">The version to compare to.</param>
        /// <param name="version2">The version to compare against.</param>
        /// <returns>If versionA &lt; versionB <c>&lt; 0</c>, if versionA &gt; versionB <c>&gt; 0</c>,
        /// if versionA is equal to versionB <c>0</c>.</returns>
        public static int Compare(SemanticVersion version1, SemanticVersion version2)
        {
            if (ReferenceEquals(version1, null))
                return ReferenceEquals(version2, null) ? 0 : -1;
            return ((IComparer<SemanticVersion>)VersionComparer).Compare(version1, version2);
        }

        /// <summary>
        /// Tests the specified versions for equality.
        /// </summary>
        /// <param name="versionA">The first version.</param>
        /// <param name="versionB">The second version.</param>
        /// <returns>If versionA is equal to versionB <c>true</c>, else <c>false</c>.</returns>
        public static bool Equals(SemanticVersion versionA, SemanticVersion versionB)
        {
            if (ReferenceEquals(versionA, null))
                return ReferenceEquals(versionB, null);
            return versionA.Equals(versionB);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as SemanticVersion);
        }

        /// <inheritdoc />
        public bool Equals(SemanticVersion other)
        {
            return CompareTo(other) == 0 && Equals(Build, other?.Build);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ((IEqualityComparer<SemanticVersion>)VersionComparer).GetHashCode(this);
        }

        #endregion

        #region Operators

        /// <summary>
        /// The override of the equals operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            return Compare(left, right) == 0;
        }

        /// <summary>
        /// The override of the un-equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is not equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return Compare(left, right) != 0;
        }

        /// <summary>
        /// The override of the less operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// The override of the greater operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// The override of the less than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return left == right || left < right;
        }

        /// <summary>
        /// The override of the greater than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return left == right || left > right;
        }

        #endregion

        #region Conversions

        /// <summary>Checks if a given string can be considered a valid <see cref="SemanticVersion" />.</summary>
        /// <param name="inputString">The string to check for validity.</param>
        /// <returns>True, if the passed string is a valid <see cref="SemanticVersion" />, otherwise false.</returns>
        public static bool IsVersion(string inputString) => VersionExpression.IsMatch(inputString);

        /// <summary>Implicitly converts a string into a <see cref="SemanticVersion" />.</summary>
        /// <param name="versionString">The string to convert.</param>
        /// <returns>The <see cref="SemanticVersion" /> object.</returns>
        public static implicit operator SemanticVersion(string versionString)
        {
            // ReSharper disable once ArrangeStaticMemberQualifier
            return Parse(versionString);
        }

        /// <summary>Explicitly converts a <see cref="System.Version" /> onject into a <see cref="SemanticVersion" />.</summary>
        /// <param name="version">The version to convert.</param>
        /// <remarks>
        ///     <para>
        ///         This operator conversts a C# <see cref="System.Version" /> object into the corresponding
        ///         <see cref="SemanticVersion" /> object.
        ///     </para>
        ///     <para>
        ///         Note, that with a C# version the <see cref="System.Version.Build" /> property is identical to the
        ///         <see cref="Patch" /> propertry on a semantic version compliant object.
        ///         Whereas the <see cref="System.Version.Revision" /> property is equivalent to the <see cref="Build" /> property
        ///         on a semantic version.
        ///         The <see cref="Prerelease" /> property is never set, since the C# version object does not use such a notation.
        ///     </para>
        /// </remarks>
        public static explicit operator SemanticVersion(Version version)
        {
            return new SemanticVersion(version);
        }

        /// <summary>Explicitly converts a <see cref="SemanticVersion" /> onject into a <see cref="System.Version" />.</summary>
        /// <param name="version">The version to convert.</param>
        /// <remarks>
        ///     <para>
        ///         This operator conversts a <see cref="SemanticVersion" /> object into the corresponding
        ///          C# <see cref="System.Version" /> object.
        ///     </para>
        ///     <para>
        ///         Note, that with a C# version the <see cref="System.Version.Build" /> property is identical from the
        ///         <see cref="Patch" /> propertry on a semantic version compliant object.
        ///         Whereas the <see cref="System.Version.Revision" /> property is equivalent to the <see cref="Build" /> property
        ///         on a semantic version.
        ///         The <see cref="Prerelease" /> property will be ignored, since the C# version object does not use such a notation.
        ///     </para>
        /// </remarks>
        public static explicit operator Version(SemanticVersion version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));
            int revision;
            if (string.IsNullOrWhiteSpace(version.Build) || !int.TryParse(version.Build, out revision))
            {
                revision = 0;
            }
            return new Version(version.Major, version.Minor, version.Patch, revision);
        }

        /// <summary>Parses the specified string to a semantic version.</summary>
        /// <param name="versionString">The version string.</param>
        /// <returns>A new <see cref="SemanticVersion" /> object that has the specified values.</returns>
        /// <exception cref="ArgumentNullException">Raised when the input string is null.</exception>
        /// <exception cref="ArgumentException">Raised when the the input string is in an invalid format.</exception>
        public static SemanticVersion Parse(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
                throw new ArgumentNullException(nameof(versionString), Strings.CannotNullOrEmpty);

            SemanticVersion version;

            if (!TryParse(versionString, out version))
                throw new ArgumentException(Strings.InvalidVersion, nameof(versionString));

            return version;
        }

        /// <summary>Tries to parse the specified string into a semantic version.</summary>
        /// <param name="versionString">The version string.</param>
        /// <param name="version">
        ///     When the method returns, contains a <see cref="SemanticVersion"/> instance equivalent
        ///     to the version string passed in, if the version string was valid, or <c>null</c> if the
        ///     version string was not valid.
        /// </param>
        /// <returns><c>False</c> when a invalid version string is passed, otherwise <c>true</c>.</returns>
        public static bool TryParse(string versionString, out SemanticVersion version)
        {
            int major, minor, patch;
            string pre, build;
            if (TryParse(versionString, out major, out minor, out patch, out pre, out build))
            {
                version = new SemanticVersion(major, minor, patch, pre, build);
                return true;
            }
            version = null;
            return false;
        }

        private static bool TryParse(string versionString, out int major, out int minor, out int patch, out string pre, out string build)
        {
            major = minor = patch = 0;
            pre = build = null;

            if (versionString == null) return false;
            versionString = versionString.Trim();
            if (versionString.Length == 0) return false;
            if (versionString[0] == 'v' || versionString[0] == 'V' || versionString[0] == '=') versionString = versionString.Substring(1);
            if (versionString.Length == 0) return false;
            var versionMatch = VersionExpression.Match(versionString);

            if (!versionMatch.Success)
                return false;
            var majorMatch = versionMatch.Groups["major"].Value;
            var minorMatch = versionMatch.Groups["minor"].Value;
            var patchMatch = versionMatch.Groups["patch"].Value;
            if (!int.TryParse(majorMatch, NumberStyles.Integer, CultureInfo.InvariantCulture, out major))
            {
                return false;
            }
            if (string.IsNullOrEmpty(minorMatch))
            {
                minor = 0;
            }
            else if (!int.TryParse(minorMatch, NumberStyles.Integer, CultureInfo.InvariantCulture, out minor))
            {
                return false;
            }
            if (string.IsNullOrEmpty(patchMatch))
            {
                patch = 0;
            }
            else if (!int.TryParse(patchMatch, NumberStyles.Integer, CultureInfo.InvariantCulture, out patch))
            {
                return false;
            }
            pre = versionMatch.Groups["pre"].Value;
            build = versionMatch.Groups["build"].Value;
            return true;
        }

        #endregion
    }
}