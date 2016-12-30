using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Lenoard.Core
{
    /// <summary>
    /// Represents a method that parses a string value to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to parse the string value to.</typeparam>
    /// <param name="input">The string value to parse.</param>
    /// <param name="result">The parsed value. </param>
    /// <returns><c>true</c> if the parse operation was successful; otherwise, <c>false</c>.</returns>
    public delegate bool BoundaryParser<T>(string input, out T result);

    /// <summary>
    /// Represents a range.
    /// </summary>
    /// <typeparam name="T">The type of range</typeparam>
#if !NetCore
    [Serializable]
#endif
    public class Range<T> : IEquatable<Range<T>>
    {
        private static readonly Func<string, T> _parser;
        private static readonly BoundaryParser<T> _tryParser;

        #region Constructors

        static Range()
        {
            _parser = CreateParser();
            _tryParser = CreateTryParser();
        }

        /// <summary>
        /// Initializes a new Range
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range</param>
        /// <param name="upperBound">The upper bound of the range</param>
        /// <param name="includeLowerBound">If the lower bound should be included</param>
        /// <param name="includeUpperBound">If the upper bound should be included</param>
        public Range(T lowerBound, T upperBound, bool includeLowerBound = true, bool includeUpperBound = true)
            : this(lowerBound, upperBound, includeLowerBound, includeUpperBound, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new Range
        /// </summary>
        /// <param name="lowerBound">The lower bound of the range</param>
        /// <param name="upperBound">The upper bound of the range</param>
        /// <param name="includeLowerBound">If the lower bound should be included</param>
        /// <param name="includeUpperBound">If the upper bound should be included</param>
        /// <param name="comparer">The comparison to use for the range elements</param>
        public Range(T lowerBound, T upperBound, bool includeLowerBound, bool includeUpperBound, IComparer<T> comparer)
        {
            Comparer = comparer;
            LowerBound = lowerBound;
            UpperBound = upperBound;
            IncludeLowerBound = includeLowerBound;
            IncludeUpperBound = includeUpperBound;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The lower bound of the range
        /// </summary>
        public T LowerBound { get; }

        /// <summary>
        /// The upper bound of the range
        /// </summary>
        public T UpperBound { get; }

        /// <summary>
        /// The comparison used for the elements in the range
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// If the lower bound is included in the range
        /// </summary>
        public bool IncludeLowerBound { get; }

        /// <summary>
        /// If the upper bound is included in the range
        /// </summary>
        public bool IncludeUpperBound { get; }

        #endregion

        /// <summary>
        /// Determines if the value specified is contained within the range
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>Returns true if the value is contained within the range, otherwise false</returns>
        public bool Contains(T value)
        {
            int left = Comparer.Compare(value, LowerBound);
            if (Comparer.Compare(value, LowerBound) < 0 || (left == 0 && !IncludeLowerBound))
                return false;

            int right = Comparer.Compare(value, UpperBound);
            return right < 0 || (right == 0 && IncludeUpperBound);
        }

        #region Parse

        private static bool IsNullable(out Type underlyingType)
        {
#if NetCore
            var type = typeof(T).GetTypeInfo();
            bool isNullable = type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            underlyingType = isNullable ? type.GenericTypeArguments[0] : typeof(T);
#else
            var type = typeof(T);
            bool isNullable = type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            underlyingType = isNullable ? type.GetGenericArguments()[0] : type;
#endif
            return isNullable;
        }

        private static MethodInfo FindParseMethod(Type type)
        {
#if NetCore
            return type.GetTypeInfo().GetDeclaredMethods("Parse")
                .SingleOrDefault(method =>
                    {
                        if (!method.IsPublic || !method.IsStatic) return false;
                        var parameters = method.GetParameters();
                        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
                    });
#else
            return type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
#endif
        }

        private static MethodInfo FindTryParseMethod(Type type)
        {
#if NetCore
            return type.GetTypeInfo().GetDeclaredMethods("TryParse")
                .SingleOrDefault(method =>
                {
                    if (!method.IsPublic || !method.IsStatic) return false;
                    var parameters = method.GetParameters();
                    return parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == type.MakeByRefType();
                });
#else
            return type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), type.MakeByRefType() }, null);
#endif
        }

        private static Func<string, T> CreateParser()
        {
            if (typeof(T) == typeof(string))
            {
                return input => (T)(object)input;
            }
            Type underlyingType;
            var nullable = IsNullable(out underlyingType);
            var parseMethod = FindParseMethod(underlyingType);
            if (parseMethod != null && parseMethod.ReturnType == underlyingType)
            {
                var parameter = Expression.Parameter(typeof(string));
                var parsedValue = Expression.Call(parseMethod, parameter);
                Func<Func<string, T>> defaultCreator = () => Expression.Lambda<Func<string, T>>(parsedValue, parameter).Compile();
                if (nullable ||
#if NetCore
                    !typeof(T).GetTypeInfo().IsValueType
#else
                    !typeof(T).IsValueType
#endif
                    )
                {
                    Func<string, T> lambda;
                    if (nullable)
                    {
                        var constructor = typeof(T).GetConstructor(new[] { underlyingType });
                        lambda = Expression.Lambda<Func<string, T>>(Expression.New(constructor, parsedValue), parameter).Compile();
                    }
                    else
                    {
                        lambda = defaultCreator();
                    }
                    return input => string.IsNullOrWhiteSpace(input) ? default(T) : lambda(input.Trim());
                }
                return defaultCreator();
            }
            return null;
        }

        private static BoundaryParser<T> CreateTryParser()
        {
            if (typeof(T) == typeof(string))
            {
                return (string input, out T result) =>
                {
                    result = (T) (object) input;
                    return true;
                };
            }
            Type underlyingType;
            var nullable = IsNullable(out underlyingType);
            var tryParseMethod = FindTryParseMethod(underlyingType);
            if (tryParseMethod != null && tryParseMethod.ReturnType == typeof(bool))
            {
                var parameterInput = Expression.Parameter(typeof(string));
                var parameterResult = Expression.Parameter(typeof(T).MakeByRefType());
                Func<BoundaryParser<T>> defaultCreator =
                    () => Expression.Lambda<BoundaryParser<T>>(Expression.Call(tryParseMethod, parameterInput, parameterResult), parameterInput, parameterResult).Compile();

                if (nullable ||
#if NetCore
                    !typeof(T).GetTypeInfo().IsValueType
#else
                    !typeof(T).IsValueType
#endif
                )
                {
                    BoundaryParser<T> lambda;
                    if (nullable)
                    {
                        var parsedValue = Expression.Variable(underlyingType);
                        var result = Expression.Variable(typeof(bool));
                        var assign = Expression.Assign(result, Expression.Call(tryParseMethod, parameterInput, parsedValue));
                        var constructor = typeof(T).GetConstructor(new[] { underlyingType });
                        var assignValue = Expression.Assign(parameterResult, Expression.New(constructor, parsedValue));
                        lambda = Expression.Lambda<BoundaryParser<T>>(
                            Expression.Block(new[] { parsedValue, result }, assign, assignValue, result),
                            parameterInput, parameterResult).Compile();
                    }
                    else
                    {
                        lambda = defaultCreator();
                    }
                    return (string input, out T result) =>
                    {
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            result = default(T);
                            return true;
                        }
                        return lambda(input, out result);
                    };
                }
                return defaultCreator();
            }
            return null;
        }

        /// <summary>
        /// Converts the string representation of a range to the equivalent range. 
        /// </summary>
        /// <param name="input">The range string to parse.</param>
        /// <param name="result">
        /// The range that will contain the parsed value. 
        /// If the method returns <c>true</c>, result contains a valid range. 
        /// If the method returns <c>false</c>, result will be <see langword="null"/>. </param>
        /// <returns><c>true</c> if the parse operation was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string input, out Range<T> result)
        {
            if (_tryParser == null)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotFindTryParse, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)));
            }
            return TryParse(input, _tryParser, out result);
        }

        /// <summary>
        /// Converts the string representation of a range to the equivalent range. 
        /// </summary>
        /// <param name="input">The range string to parse.</param>
        /// <param name="boundaryParser">The parser to parse the boundary value.</param>
        /// <param name="result">
        /// The range that will contain the parsed value. 
        /// If the method returns <c>true</c>, result contains a valid range. 
        /// If the method returns <c>false</c>, result will be <see langword="null"/>. </param>
        /// <returns><c>true</c> if the parse operation was successful; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string input, BoundaryParser<T> boundaryParser, out Range<T> result)
        {
            bool includeLowerBound, includeUpperBound;
            string lowerBoundText, upperBoundText;
            result = null;
            ParseRange(input, out includeLowerBound, out includeUpperBound, out lowerBoundText, out upperBoundText);
            T lowerBound, upperBound;
            if (!boundaryParser(lowerBoundText, out lowerBound) || !boundaryParser(upperBoundText, out upperBound)) return false;
            result = new Range<T>(lowerBound, upperBound, includeLowerBound, includeUpperBound);
            return true;
        }

        /// <summary>
        /// Converts the string representation of a range to the equivalent range.
        /// </summary>
        /// <param name="input">The range string to parse.</param>
        /// <param name="boundaryParser">The parser to parse the boundary value.</param>
        /// <returns>A range that contains the value that was parsed.</returns>
        public static Range<T> Parse(string input, Func<string, T> boundaryParser)
        {
            bool includeLowerBound, includeUpperBound;
            string lowerBoundText, upperBoundText;
            ParseRange(input, out includeLowerBound, out includeUpperBound, out lowerBoundText, out upperBoundText);
            T lowerBound, upperBound;
            try
            {
                lowerBound = boundaryParser(lowerBoundText);
            }
            catch (Exception ex)
            {
                throw new FormatException(Strings.UnrecognizedRange, ex);
            }
            try
            {
                upperBound = boundaryParser(upperBoundText);
            }
            catch (Exception ex)
            {
                throw new FormatException(Strings.UnrecognizedRange, ex);
            }
            return new Range<T>(lowerBound, upperBound, includeLowerBound, includeUpperBound);
        }

        /// <summary>
        /// Converts the string representation of a range to the equivalent range.
        /// </summary>
        /// <param name="input">The range string to parse.</param>
        /// <returns>A range that contains the value that was parsed.</returns>
        public static Range<T> Parse(string input)
        {
            if (_parser == null)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotFindParseMethod, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)));
            }
            return Parse(input, _parser);
        }

        private static void ParseRange(string input, out bool includeLowerBound, out bool includeUpperBound, out string lowerBound, out string upperBound)
        {
            includeLowerBound = false;
            includeUpperBound = false;
            lowerBound = null;
            upperBound = null;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            input = input.Trim();
            if (input.Length == 0)
            {
                return;
            }
            if (ParseRangeWithParenthesis(input, out includeLowerBound, out includeUpperBound, out lowerBound, out upperBound))
            {
                return;
            }
            if (ParseRangeWithHyphen(input, out lowerBound, out upperBound))
            {
                includeLowerBound = true;
                includeUpperBound = true;
                return;
            }
            if (ParseRangeWithComparer(input, out includeLowerBound, out includeUpperBound, out lowerBound, out upperBound))
            {
                return;
            }
            includeLowerBound = true;
            includeUpperBound = true;
            lowerBound = upperBound = input;
        }

        private static bool ParseRangeWithParenthesis(string input, out bool includeLowerBound, out bool includeUpperBound, out string lowerBound, out string upperBound)
        {
            includeLowerBound = false;
            includeUpperBound = false;
            lowerBound = null;
            upperBound = null;
            if (input.Length <= 3) return false;
            if (input[0] == '(')
            {
                includeLowerBound = false;
            }
            else if (input[0] == '[')
            {
                includeLowerBound = true;
            }
            else
            {
                return false;
            }
            var lastChar = input[input.Length - 1];
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
            input = input.Substring(1, input.Length - 2);
            var separatorIndex = input.IndexOf(',');
            if (separatorIndex == -1)
            {
                return false;
            }
            lowerBound = input.Substring(0, separatorIndex);
            upperBound = input.Substring(separatorIndex + 1);
            return true;
        }

        private static bool ParseRangeWithHyphen(string input, out string lowerBound, out string upperBound)
        {
            lowerBound = null;
            upperBound = null;
            var index = input.IndexOf('~');
            if (index == -1)
            {
                int startIndex = 0, hyphenIndex;
                while ((hyphenIndex = input.IndexOf('-', startIndex)) != -1)
                {
                    if (hyphenIndex >= input.Length - 1 || !char.IsNumber(input[hyphenIndex + 1]))
                    {
                        break;
                    }
                    startIndex = hyphenIndex + 1;
                }
                if (hyphenIndex != -1)
                {
                    index = hyphenIndex;
                }
            }
            if (index == -1) return false;
            lowerBound = input.Substring(0, index);
            upperBound = input.Substring(index + 1);
            return true;
        }

        private static bool ParseRangeWithComparer(string input, out bool includeLowerBound, out bool includeUpperBound, out string lowerBound, out string upperBound)
        {
            includeLowerBound = false;
            includeUpperBound = false;
            lowerBound = null;
            upperBound = null;
            if (input.StartsWith(">="))
            {
                includeLowerBound = true;
                lowerBound = input.Substring(2);
                return true;
            }
            if (input.StartsWith("<="))
            {
                includeUpperBound = true;
                upperBound = input.Substring(2);
                return true;
            }
            if (input.StartsWith(">"))
            {
                lowerBound = input.Substring(1);
                return true;
            }
            if (input.StartsWith("<"))
            {
                upperBound = input.Substring(1);
                return true;
            }
            if (input.StartsWith("=="))
            {
                includeLowerBound = true;
                includeUpperBound = true;
                lowerBound = upperBound = input.Substring(2);
                return true;
            }
            if (input.StartsWith("="))
            {
                includeLowerBound = true;
                includeUpperBound = true;
                lowerBound = upperBound = input.Substring(1);
                return true;
            }
            return false;
        }

        #endregion

        #region Equality Stuff

        /// <summary>
        ///     Determines if this range is equal to another range. 
        /// </summary>
        /// <param name="other"><see cref="Range{T}"/> to compare with for equality.</param>
        /// <returns><c>true</c> if the range are equal. <c>false</c> if the range are not equal.</returns>
        public bool Equals(Range<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.IncludeLowerBound.Equals(IncludeLowerBound) &&
                other.IncludeUpperBound.Equals(IncludeUpperBound) &&
                Equals(other.LowerBound, LowerBound) &&
                Equals(other.UpperBound, UpperBound);
        }

        /// <summary>
        ///     Determines if this range is equal to another object.
        /// </summary>
        /// <param name="obj">Object to compare for equality.</param>
        /// <returns><c>true</c> if the objects are equal. <c>false</c> if the objects are not equal.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Range<T>)) return false;
            return Equals((Range<T>)obj);
        }

        /// <summary>
        ///     Returns a hash code for the pair, suitable for use in a hash-table or other hashed collection.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = IncludeLowerBound.GetHashCode();
                result = (result * 397) ^ IncludeUpperBound.GetHashCode();
                result = (result * 397) ^ LowerBound.GetHashCode();
                result = (result * 397) ^ UpperBound.GetHashCode();
                return result;
            }
        }

        /// <summary>
        ///     Determines if two ranges are equal.
        /// </summary>
        /// <param name="left">First range to compare.</param>
        /// <param name="right">Second range to compare.</param>
        /// <returns><c>true</c> if the ranges are equal. <c>false</c> if the ranges are not equal.</returns>
        public static bool operator ==(Range<T> left, Range<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Determines if two ranges are not equal.
        /// </summary>
        /// <param name="left">First range to compare.</param>
        /// <param name="right">Second range to compare.</param>
        /// <returns><c>true</c> if the ranges are not equal. <c>false</c> if the ranges are equal.</returns>
        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
