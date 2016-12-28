using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lenoard.Core
{
    /// <summary>
    /// The data URI scheme is a uniform resource identifier (URI) scheme that provides a way to include data in-line in web pages as if they were external resources.
    /// </summary>
    /// <seealso cref="https://en.wikipedia.org/wiki/Data_URI_scheme"/>
    /// <seealso cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/data_URIs"/>
    /// <seealso cref="http://tools.ietf.org/html/rfc2397"/>
    [Serializable]
    public abstract class DataUri
    {
        #region Nested Types

        private class ParseResult
        {
            internal string MediaType;
            internal readonly NameValueCollection Parameters = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            internal string Encoding;
            internal string Data;
            private readonly bool _canThrow;
            private ParseFailureKind _failure;
            private string _failureArgumentName;
            private string _failureMessage;
            private object _failureMessageFormatArgument;
            private Exception _innerException;

            public ParseResult(bool canThrow)
            {
                _canThrow = canThrow;
                _failure = ParseFailureKind.None;
                _failureMessage = null;
                _failureMessageFormatArgument = null;
                _failureArgumentName = null;
                _innerException = null;
            }

            public void SetFailure(ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument = null,
                string failureArgumentName = null, Exception innerException = null)
            {
                _failure = failure;
                _failureMessage = failureMessageID;
                _failureMessageFormatArgument = failureMessageFormatArgument;
                _failureArgumentName = failureArgumentName;
                _innerException = innerException;
                if (_canThrow)
                {
                    throw GetParseException();
                }
            }

            public Exception GetParseException()
            {
                switch (_failure)
                {
                    case ParseFailureKind.ArgumentNull:
                        return new ArgumentNullException(_failureArgumentName, _failureMessage);

                    case ParseFailureKind.Format:
                        return new FormatException(_failureMessage);

                    case ParseFailureKind.FormatWithParameter:
                        return new FormatException(string.Format(_failureMessage, _failureMessageFormatArgument));

                    case ParseFailureKind.NativeException:
                        return _innerException;

                    case ParseFailureKind.FormatWithInnerException:
                        return new FormatException(_failureMessage, _innerException);
                }
                return new FormatException("Unrecognized data uri format.");
            }
        }

        private enum ParseFailureKind
        {
            None,
            ArgumentNull,
            Format,
            FormatWithParameter,
            NativeException,
            FormatWithInnerException
        }

        private class ReadOnlyNameValueCollection : NameValueCollection
        {
            internal ReadOnlyNameValueCollection(IEqualityComparer equalityComparer) : base(equalityComparer)
            {
            }

            internal ReadOnlyNameValueCollection(NameValueCollection value) : base(value)
            {
            }

            internal void SetReadOnly()
            {
                IsReadOnly = true;
            }
        }

        #endregion

        #region Fields

        private const string DefaultMediaType = "text/plain";
        private readonly string _mediaType;
        private readonly ReadOnlyNameValueCollection _parameters;
        private const string UnrecognizedFormat = "Unrecognized data uri format.";

        #endregion

        #region Constructors

        protected DataUri(string mediaType, NameValueCollection parameters)
        {
            _mediaType = string.IsNullOrEmpty(mediaType) ? DefaultMediaType : mediaType;
            _parameters = parameters == null ? new ReadOnlyNameValueCollection(StringComparer.InvariantCultureIgnoreCase) : new ReadOnlyNameValueCollection(parameters);
            _parameters.SetReadOnly();
        }

        protected DataUri(string mediaType)
            : this(mediaType, null)
        {
        }

        protected DataUri()
            : this(null)
        {
        }

        #endregion

        #region Properties

        [DefaultValue(DefaultMediaType)]
        public string MediaType
        {
            get { return _mediaType; }
        }

        public NameValueCollection Parameters
        {
            get { return _parameters; }
        }

        public abstract string Encoding { get; }

        #endregion

        #region Methods

        public override string ToString()
        {
            var result = new StringBuilder("data:");
            if (!string.Equals(_mediaType, DefaultMediaType, StringComparison.InvariantCultureIgnoreCase))
            {
                result.Append(_mediaType).Append(";");
            }
            foreach (string parameterName in Parameters)
            {
                result.Append(parameterName).Append("=").Append(Parameters[parameterName]).Append(";");
            }
            return result.ToString();
        }

        protected virtual bool Equals(DataUri other)
        {
            if (!string.Equals(other.MediaType, MediaType, StringComparison.InvariantCultureIgnoreCase) || Parameters.Count != other.Parameters.Count)
            {
                return false;
            }
            foreach (string parameterName in Parameters)
            {
                if (Parameters[parameterName] != other.Parameters[parameterName])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as DataUri;
            if (other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _mediaType != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_mediaType) : 0;
                foreach (string parameterName in Parameters)
                {
                    hashCode = (hashCode * 397) ^ (parameterName != null ? parameterName.GetHashCode() : 0);
                    var parameterValue = Parameters[parameterName];
                    hashCode = (hashCode * 397) ^ (parameterValue != null ? parameterValue.GetHashCode() : 0);
                }
                return hashCode;
            }
        }

        #endregion

        #region Parse

        public static bool TryParse(string uriString, out DataUri uri)
        {
            ParseResult result = new ParseResult(false);
            if (TryParse(uriString, result) && CreateDataUri(result, out uri)) return true;
            uri = null;
            return false;
        }

        public static DataUri Parse(string uriString)
        {
            if (uriString == null)
            {
                throw new ArgumentNullException("uriString");
            }
            var result = new ParseResult(true);
            DataUri uri;
            if (!TryParse(uriString, result) || !CreateDataUri(result, out uri))
            {
                throw result.GetParseException();
            }
            return uri;
        }

        private static bool CreateDataUri(ParseResult result, out DataUri uri)
        {
            uri = null;
            if (string.IsNullOrEmpty(result.Data))
            {
                result.SetFailure(ParseFailureKind.Format, UnrecognizedFormat, failureArgumentName: "uriString");
                return false;
            }
            if (string.IsNullOrEmpty(result.Encoding))
            {
                uri = new TextDataUri(result.MediaType, result.Parameters, result.Data);
                return true;
            }
            if (string.Equals(result.Encoding, "base64", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    uri = new Base64DataUri(result.MediaType, result.Parameters, Convert.FromBase64String(result.Data));
                    return true;
                }
                catch (FormatException ex)
                {
                    result.SetFailure(ParseFailureKind.FormatWithInnerException, UnrecognizedFormat, failureArgumentName: "uriString", innerException: ex);
                    return false;
                }
                catch (Exception ex)
                {
                    result.SetFailure(ParseFailureKind.NativeException, null, innerException: ex);
                    return false;
                }
            }
            result.SetFailure(ParseFailureKind.Format, string.Format("Unrecognized data uri for unknown data encoding: {0}.", result.Encoding), failureArgumentName: "uriString");
            return false;
        }

        private static bool TryParse(string uriString, ParseResult result)
        {
            // data:[<media type>][;charset=<character set>][;base64],<data>
            if (uriString == null)
            {
                result.SetFailure(ParseFailureKind.ArgumentNull, "The data uri cannot be null.",
                    failureArgumentName: "uriString");
                return false;
            }
            uriString = uriString.Trim();
            if (uriString.Length == 0)
            {
                result.SetFailure(ParseFailureKind.Format, "The data uri cannot be empty string.",
                    failureArgumentName: "uriString");
                return false;
            }
            // The data uri must be starts with 'data:'
            const string dataPrefix = "data:";
            if (!uriString.StartsWith(dataPrefix))
            {
                result.SetFailure(ParseFailureKind.Format, UnrecognizedFormat, failureArgumentName: "uriString");
                return false;
            }
            uriString = uriString.Substring(dataPrefix.Length).Trim();
            Func<Predicate<string>, string> getUriSection = predicate =>
            {
                var semicolon = uriString.IndexOf(';');
                var comma = uriString.IndexOf(',');
                int endIndex = -1;
                if (semicolon < 0)
                {
                    endIndex = comma;
                }
                else if (comma < 0)
                {
                    endIndex = semicolon;
                }
                else
                {
                    endIndex = Math.Min(semicolon, comma);
                }
                if (endIndex >= 0)
                {
                    string section = uriString.Substring(0, endIndex);
                    if (predicate == null || predicate(section))
                    {
                        uriString = endIndex == semicolon
                            ? uriString.Substring(semicolon + 1).Trim()
                            : uriString.Substring(comma).Trim();
                        return section;
                    }
                }
                return string.Empty;
            };
            // parse optional mimetype
            var mediaType = getUriSection(null);
            if (!string.IsNullOrEmpty(mediaType) && !Regex.IsMatch(mediaType, "^\\w+\\/\\w+$"))
            {
                result.SetFailure(ParseFailureKind.Format, string.Format("Unrecognized data uri for invalid media type: {0}.", mediaType), failureArgumentName: "uriString");
                return false;
            }
            result.MediaType = mediaType;

            string sectionString;
            while (!string.IsNullOrEmpty(sectionString = getUriSection(section => section.IndexOf('=') > 0)))
            {
                var equalIndex = sectionString.IndexOf('=');
                var parameterName = sectionString.Substring(0, equalIndex).Trim();
                if (string.IsNullOrEmpty(parameterName))
                {
                    result.SetFailure(ParseFailureKind.Format, UnrecognizedFormat, failureArgumentName: "uriString");
                    return false;
                }
                result.Parameters.Add(parameterName, sectionString.Substring(equalIndex + 1).Trim());
            }
            // parse data section
            if (string.IsNullOrEmpty(uriString))
            {
                result.SetFailure(ParseFailureKind.Format, UnrecognizedFormat, failureArgumentName: "uriString");
                return false;
            }
            var commaIndex = uriString.IndexOf(',');
            if (commaIndex < 0)
            {
                result.SetFailure(ParseFailureKind.Format, UnrecognizedFormat, failureArgumentName: "uriString");
                return false;
            }
            result.Encoding = uriString.Substring(0, commaIndex).Trim();
            result.Data = uriString.Substring(commaIndex + 1).Trim();
            return true;
        }

        #endregion

        #region Operators

        public static bool operator ==(DataUri uri1, DataUri uri2)
        {
            return ReferenceEquals(uri1, uri2) || (uri1 != null && uri2 != null && uri1.Equals(uri2));
        }

        public static bool operator !=(DataUri uri1, DataUri uri2)
        {
            return !(uri1 == uri2);
        }

        #endregion
    }

    public sealed class Base64DataUri : DataUri
    {
        private readonly byte[] _content;

        #region Constructors

        public Base64DataUri(string mediaType, NameValueCollection parameters, byte[] content) : base(mediaType, parameters)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        public Base64DataUri(string mediaType, byte[] content) : base(mediaType)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        public Base64DataUri(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        #endregion

        #region Properties

        public byte[] Content
        {
            get { return _content; }
        }

        public override string Encoding
        {
            get { return "base64"; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return base.ToString() + "base64," + Convert.ToBase64String(_content);
        }

        protected override bool Equals(DataUri other)
        {
            if (!base.Equals(other)) return false;
            var uri = other as Base64DataUri;
            if (uri == null) return false;
            return Content.SequenceEqual(uri.Content);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _content.Aggregate(base.GetHashCode(), (hashCode, b) => (hashCode * 397) ^ b.GetHashCode());
            }
        }

        #endregion
    }

    public sealed class TextDataUri : DataUri
    {
        private readonly string _content;

        #region Constructors

        public TextDataUri(string mediaType, NameValueCollection parameters, string content) : base(mediaType, parameters)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        public TextDataUri(string mediaType, string content) : base(mediaType)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        public TextDataUri(string content)
        {
            if (content == null) throw new ArgumentNullException("content");
            _content = content;
        }

        #endregion

        #region Properties

        public string Content
        {
            get { return _content; }
        }

        public override string Encoding
        {
            get { return string.Empty; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return base.ToString() + "," + _content;
        }

        protected override bool Equals(DataUri other)
        {
            if (!base.Equals(other)) return false;
            var uri = other as TextDataUri;
            if (uri == null) return false;
            return Content == uri.Content;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Content != null ? Content.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
