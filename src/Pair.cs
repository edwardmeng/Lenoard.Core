using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lenoard.Core
{
    /// <summary>
    ///     Stores a pair of objects within a single struct. This struct is useful to use as the
    ///     T of a collection, or as the TKey or TValue of a dictionary.
    /// </summary>
#if !NetCore
    [Serializable]
#endif
    public struct Pair<TFirst, TSecond> : IComparable,
        IComparable<Pair<TFirst, TSecond>>
    {
        /// <summary>
        ///     Comparers for the first and second type that are used to compare
        ///     values.
        /// </summary>
        private static readonly IComparer<TFirst> _firstComparer = Comparer<TFirst>.Default;

        private static readonly IEqualityComparer<TFirst> _firstEqualityComparer = EqualityComparer<TFirst>.Default;
        private static readonly IComparer<TSecond> _secondComparer = Comparer<TSecond>.Default;
        private static readonly IEqualityComparer<TSecond> _secondEqualityComparer = EqualityComparer<TSecond>.Default;

        /// <summary>
        ///     The first element of the pair.
        /// </summary>
        public TFirst First { get; }

        /// <summary>
        ///     The second element of the pair.
        /// </summary>
        public TSecond Second { get; }

        /// <summary>
        ///     Creates a new pair with given first and second elements.
        /// </summary>
        /// <param name="first">The first element of the pair.</param>
        /// <param name="second">The second element of the pair.</param>
        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        ///     Creates a new pair using elements from a KeyValuePair structure. The
        ///     First element gets the Key, and the Second elements gets the Value.
        /// </summary>
        /// <param name="keyAndValue">The KeyValuePair to initialize the Pair with .</param>
        public Pair(KeyValuePair<TFirst, TSecond> keyAndValue)
        {
            First = keyAndValue.Key;
            Second = keyAndValue.Value;
        }

        /// <summary>
        ///     <para>
        ///         Compares this pair to another pair of the some type. The pairs are compared by using
        ///         the IComparable&lt;T&gt; or IComparable interface on TFirst and TSecond. The pairs
        ///         are compared by their first elements first, if their first elements are equal, then they
        ///         are compared by their second elements.
        ///     </para>
        ///     <para>
        ///         If either TFirst or TSecond does not implement IComparable&lt;T&gt; or IComparable, then
        ///         an NotSupportedException is thrown, because the pairs cannot be compared.
        ///     </para>
        /// </summary>
        /// <param name="obj">The pair to compare to.</param>
        /// <returns>
        ///     An integer indicating how this pair compares to <paramref name="obj" />. Less
        ///     than zero indicates this pair is less than <paramref name="obj" />. Zero indicate this pair is
        ///     equals to <paramref name="obj" />. Greater than zero indicates this pair is greater than
        ///     <paramref name="obj" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj" /> is not of the correct type.</exception>
        /// <exception cref="NotSupportedException">
        ///     Either FirstSecond or TSecond is not comparable
        ///     via the IComparable&lt;T&gt; or IComparable interfaces.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {

            if (obj is Pair<TFirst, TSecond>)
                return CompareTo((Pair<TFirst, TSecond>)obj);
            throw new ArgumentException(Strings.BadComparandType, nameof(obj));
        }

        /// <summary>
        ///     <para>
        ///         Compares this pair to another pair of the some type. The pairs are compared by using
        ///         the IComparable&lt;T&gt; or IComparable interface on TFirst and TSecond. The pairs
        ///         are compared by their first elements first, if their first elements are equal, then they
        ///         are compared by their second elements.
        ///     </para>
        ///     <para>
        ///         If either TFirst or TSecond does not implement IComparable&lt;T&gt; or IComparable, then
        ///         an NotSupportedException is thrown, because the pairs cannot be compared.
        ///     </para>
        /// </summary>
        /// <param name="other">The pair to compare to.</param>
        /// <returns>
        ///     An integer indicating how this pair compares to <paramref name="other" />. Less
        ///     than zero indicates this pair is less than <paramref name="other" />. Zero indicate this pair is
        ///     equals to <paramref name="other" />. Greater than zero indicates this pair is greater than
        ///     <paramref name="other" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///     Either FirstSecond or TSecond is not comparable
        ///     via the IComparable&lt;T&gt; or IComparable interfaces.
        /// </exception>
        public int CompareTo(Pair<TFirst, TSecond> other)
        {
            try
            {
                var firstCompare = _firstComparer.Compare(First, other.First);
                if (firstCompare != 0)
                    return firstCompare;
                return _secondComparer.Compare(Second, other.Second);
            }
            catch (ArgumentException)
            {
                // Determine which type caused the problem for a better error message.
#if NetCore
                if (!typeof(IComparable<TFirst>).GetTypeInfo().IsAssignableFrom(typeof(TFirst).GetTypeInfo()) &&
                    !typeof(IComparable).GetTypeInfo().IsAssignableFrom(typeof(TFirst).GetTypeInfo()))
                    throw new NotSupportedException(string.Format(Strings.BadComparandType, typeof(TFirst).FullName));
                if (!typeof(IComparable<TSecond>).GetTypeInfo().IsAssignableFrom(typeof(TSecond).GetTypeInfo()) &&
                    !typeof(IComparable).GetTypeInfo().IsAssignableFrom(typeof(TSecond).GetTypeInfo()))
                    throw new NotSupportedException(string.Format(Strings.BadComparandType, typeof(TSecond).FullName));
#else
                if (!typeof(IComparable<TFirst>).IsAssignableFrom(typeof(TFirst)) &&
                    !typeof(IComparable).IsAssignableFrom(typeof(TFirst)))
                    throw new NotSupportedException(string.Format(Strings.BadComparandType, typeof(TFirst).FullName));
                if (!typeof(IComparable<TSecond>).IsAssignableFrom(typeof(TSecond)) &&
                    !typeof(IComparable).IsAssignableFrom(typeof(TSecond)))
                    throw new NotSupportedException(string.Format(Strings.BadComparandType, typeof(TSecond).FullName));
#endif
                throw; // Hmmm. Unclear why we got the ArgumentException. 
            }
        }

        /// <summary>
        ///     Determines if this pair is equal to another object. The pair is equal to another object
        ///     if that object is a <see cref="Pair{TFirst,TSecond}"/>, both element types are the same, and the first and second elements
        ///     both compare equal using <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="obj">Object to compare for equality.</param>
        /// <returns><c>true</c> if the objects are equal. <c>false</c> if the objects are not equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Pair<TFirst, TSecond>)
            {
                var other = (Pair<TFirst, TSecond>)obj;
                return Equals(other);
            }
            return false;
        }

        /// <summary>
        ///     Determines if this pair is equal to another pair. The pair is equal if  the first and second elements
        ///     both compare equal using <see cref="IEqualityComparer{T}.Equals(T,T)"/> or <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="other"><see cref="Pair{TFirst,TSecond}"/> to compare with for equality.</param>
        /// <returns><c>true</c> if the pairs are equal. <c>false</c> if the pairs are not equal.</returns>
        public bool Equals(Pair<TFirst, TSecond> other)
        {
            return _firstEqualityComparer.Equals(First, other.First) && _secondEqualityComparer.Equals(Second, other.Second);
        }

        /// <summary>
        ///     Returns a hash code for the pair, suitable for use in a hash-table or other hashed collection.
        ///     Two pairs that compare equal (using Equals) will have the same hash code. The hash code for
        ///     the pair is derived by combining the hash codes for each of the two elements of the pair.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            // Build the hash code from the hash codes of First and Second. 
            var hashFirst = First == null ? 0x61E04917 : First.GetHashCode();
            var hashSecond = Second == null ? 0x198ED6A3 : Second.GetHashCode();
            return hashFirst ^ hashSecond;
        }

        /// <summary>
        ///     Returns a string representation of the pair. The string representation of the pair is
        ///     of the form:
        ///     <c>First: {0}, Second: {1}</c>
        ///     where {0} is the result of First.ToString(), and {1} is the result of Second.ToString() (or
        ///     "null" if they are null.)
        /// </summary>
        /// <returns> The string representation of the pair.</returns>
        public override string ToString()
        {
            return $"First: {(First == null ? "null" : First.ToString())}, Second: {(Second == null ? "null" : Second.ToString())}";
        }

        /// <summary>
        ///     Converts this Pair to a KeyValuePair. The Key part of the KeyValuePair gets
        ///     the First element, and the Value part of the KeyValuePair gets the Second
        ///     elements.
        /// </summary>
        /// <returns>The KeyValuePair created from this Pair.</returns>
        public KeyValuePair<TFirst, TSecond> ToKeyValuePair()
        {
            return new KeyValuePair<TFirst, TSecond>(First, Second);
        }

        /// <summary>
        ///     Determines if two pairs are equal. Two pairs are equal if  the first and second elements
        ///     both compare equal using <see cref="IEqualityComparer{T}.Equals(T,T)"/> or <see cref="object.Equals(object)"/>.
        /// </summary>
        /// <param name="pair1">First pair to compare.</param>
        /// <param name="pair2">Second pair to compare.</param>
        /// <returns><c>true</c> if the pairs are equal. <c>false</c> if the pairs are not equal.</returns>
        public static bool operator ==(Pair<TFirst, TSecond> pair1, Pair<TFirst, TSecond> pair2)
        {
            return _firstEqualityComparer.Equals(pair1.First, pair2.First) && _secondEqualityComparer.Equals(pair1.Second, pair2.Second);
        }

        /// <summary>
        ///     Determines if two pairs are not equal. Two pairs are equal if  the first and second elements
        ///     both compare equal using IComparable&lt;T&gt;.Equals or object.Equals.
        /// </summary>
        /// <param name="pair1">First pair to compare.</param>
        /// <param name="pair2">Second pair to compare.</param>
        /// <returns>True if the pairs are not equal. False if the pairs are equal.</returns>
        public static bool operator !=(Pair<TFirst, TSecond> pair1, Pair<TFirst, TSecond> pair2)
        {
            return !(pair1 == pair2);
        }

        /// <summary>
        ///     Converts a <see cref="Pair{TFirst,TSecond}"/> to a <see cref="KeyValuePair{TKey,TValue}"/>. 
        ///     The <see cref="KeyValuePair{TKey,TValue}.Key"/> part of the <see cref="KeyValuePair{TKey,TValue}"/> gets
        ///     the <see cref="First"/> element, and the <see cref="KeyValuePair{TKey,TValue}.Value"/> part of the <see cref="KeyValuePair{TKey,TValue}"/> gets the <see cref="Second"/>
        ///     elements.
        /// </summary>
        /// <param name="pair"><see cref="Pair{TFirst,TSecond}"/> to convert.</param>
        /// <returns>The <see cref="KeyValuePair{TKey,TValue}"/> created from <paramref name="pair" />.</returns>
        public static explicit operator KeyValuePair<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
        {
            return new KeyValuePair<TFirst, TSecond>(pair.First, pair.Second);
        }

        /// <summary>
        ///     Converts a <see cref="KeyValuePair{TKey,TValue}"/> structure into a <see cref="Pair{TFirst,TSecond}"/>. 
        ///     The <see cref="First"/> element gets the <see cref="KeyValuePair{TKey,TValue}.Key"/>, 
        ///     and the <see cref="Second"/> element gets the <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </summary>
        /// <param name="keyAndValue">The <see cref="KeyValuePair{TKey,TValue}"/> to convert.</param>
        /// <returns>The <see cref="Pair{TFirst,TSecond}"/> converted from the <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        public static explicit operator Pair<TFirst, TSecond>(KeyValuePair<TFirst, TSecond> keyAndValue)
        {
            return new Pair<TFirst, TSecond>(keyAndValue);
        }

        /// <summary>
        ///     Converts a <see cref="Pair{TFirst,TSecond}"/> to a <see cref="Tuple{T1,T2}"/>. 
        ///     The <see cref="Tuple{T1,T2}.Item1"/> part of the <see cref="Tuple{T1,T2}"/> gets
        ///     the <see cref="First"/> element, and the <see cref="Tuple{T1,T2}.Item2"/> part of the <see cref="Tuple{T1,T2}"/> gets the <see cref="Second"/>
        ///     elements.
        /// </summary>
        /// <param name="pair"><see cref="Pair{TFirst,TSecond}"/> to convert.</param>
        /// <returns>The <see cref="Tuple{T1,T2}"/> created from <paramref name="pair" />.</returns>
        public static explicit operator Tuple<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
        {
            return new Tuple<TFirst, TSecond>(pair.First, pair.Second);
        }

        /// <summary>
        ///     Converts a <see cref="Tuple{T1,T2}"/> into a <see cref="Pair{TFirst,TSecond}"/>. 
        ///     The <see cref="First"/> element gets the <see cref="Tuple{T1,T2}.Item1"/>, 
        ///     and the <see cref="Second"/> element gets the <see cref="Tuple{T1,T2}.Item2"/>.
        /// </summary>
        /// <param name="tuple">The <see cref="Tuple{T1,T2}"/> to convert.</param>
        /// <returns>The <see cref="Pair{TFirst,TSecond}"/> converted from the <see cref="Tuple{T1,T2}"/>.</returns>
        public static explicit operator Pair<TFirst, TSecond>(Tuple<TFirst, TSecond> tuple)
        {
            if (tuple == null) throw new ArgumentNullException(nameof(tuple));
            return new Pair<TFirst, TSecond>(tuple.Item1, tuple.Item2);
        }
    }

    /// <summary>
    /// Provides static method for creating pair objects.
    /// </summary>
    public static class Pair
    {
        /// <summary>
        /// Creates a new pair.
        /// </summary>
        /// <typeparam name="TFirst">The type of the first component of the pair.</typeparam>
        /// <typeparam name="TSecond">The type of the second component of the pair.</typeparam>
        /// <param name="first">The value of the first component of the pair.</param>
        /// <param name="second">The value of the second component of the pair.</param>
        /// <returns>A pair whose value is (first, second).</returns>
        public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new Pair<TFirst, TSecond>(first, second);
        }
    }
}