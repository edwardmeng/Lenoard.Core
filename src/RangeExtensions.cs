using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lenoard.Core
{
    /// <summary>
    /// Provides a set of <see langword="static"/> extension methods for the <see cref="IQueryable{TSource}"/> by using the <see cref="T:Range{T}"/> as parameter.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IQueryable<TSource> Range<TSource, TMember>(this IQueryable<TSource> source, Expression<Func<TSource, TMember>> member, Range<TMember> range)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (range == null)
            {
                return source;
            }
            var memberExpr = member.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException();
            }
#if NetCore
            bool isNullable = Nullable.GetUnderlyingType(typeof(TMember)) != null || !typeof(TMember).GetTypeInfo().IsValueType;
#else
            bool isNullable = Nullable.GetUnderlyingType(typeof(TMember)) != null || !typeof(TMember).IsValueType;
#endif
            Expression lowerExpr = null, upperExpr = null;
            if (!isNullable || !Equals(range.LowerBound, default(TMember)))
            {
                var lowerBoundExpr = Expression.Constant(range.LowerBound, typeof(TMember));
                lowerExpr = range.IncludeLowerBound ?
                    Expression.GreaterThanOrEqual(memberExpr, lowerBoundExpr) :
                    Expression.GreaterThan(memberExpr, lowerBoundExpr);
            }
            if (!isNullable || !Equals(range.UpperBound, default(TMember)))
            {
                var upperBoundExpr = Expression.Constant(range.UpperBound, typeof(TMember));
                upperExpr = range.IncludeUpperBound
                    ? Expression.LessThanOrEqual(memberExpr, upperBoundExpr)
                    : Expression.LessThan(memberExpr, upperBoundExpr);
            }
            Expression filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = Expression.AndAlso(lowerExpr, upperExpr);
            }
            if (filterExpr != null)
            {
                return source.Where(Expression.Lambda<Func<TSource, bool>>(filterExpr, member.Parameters));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the range limitation.</returns>
        public static IQueryable<TSource> Range<TSource>(this IQueryable<TSource> source, Range<TSource> range)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (range == null)
            {
                return source;
            }
#if NetCore
            bool isNullable = Nullable.GetUnderlyingType(typeof(TSource)) != null || !typeof(TSource).GetTypeInfo().IsValueType;
#else
            bool isNullable = Nullable.GetUnderlyingType(typeof(TSource)) != null || !typeof(TSource).IsValueType;
#endif
            var parameter = Expression.Parameter(typeof(TSource));
            Expression lowerExpr = null, upperExpr = null;
            if (!isNullable || !Equals(range.LowerBound, default(TSource)))
            {
                var lowerBoundExpr = Expression.Constant(range.LowerBound, typeof(TSource));
                lowerExpr = range.IncludeLowerBound ?
                    Expression.GreaterThanOrEqual(parameter, lowerBoundExpr) :
                    Expression.GreaterThan(parameter, lowerBoundExpr);
            }
            if (!isNullable || !Equals(range.UpperBound, default(TSource)))
            {
                var upperBoundExpr = Expression.Constant(range.UpperBound, typeof(TSource));
                upperExpr = range.IncludeUpperBound
                    ? Expression.LessThanOrEqual(parameter, upperBoundExpr)
                    : Expression.LessThan(parameter, upperBoundExpr);
            }
            Expression filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = Expression.AndAlso(lowerExpr, upperExpr);
            }
            if (filterExpr != null)
            {
                return source.Where(Expression.Lambda<Func<TSource, bool>>(filterExpr, parameter));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IQueryable<TSource> Range<TSource, TMember>(this IQueryable<TSource> source, Expression<Func<TSource, TMember>> member, Range<TMember?> range)
            where TMember : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (range == null)
            {
                return source;
            }
            var memberExpr = member.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException();
            }
            Expression lowerExpr = null, upperExpr = null;
            if (range.LowerBound.HasValue)
            {
                var lowerBoundExpr = Expression.Constant(range.LowerBound.Value, typeof(TMember));
                lowerExpr = range.IncludeLowerBound ?
                    Expression.GreaterThanOrEqual(memberExpr, lowerBoundExpr) :
                    Expression.GreaterThan(memberExpr, lowerBoundExpr);
            }
            if (range.UpperBound.HasValue)
            {
                var upperBoundExpr = Expression.Constant(range.UpperBound.Value, typeof(TMember));
                upperExpr = range.IncludeUpperBound
                    ? Expression.LessThanOrEqual(memberExpr, upperBoundExpr)
                    : Expression.LessThan(memberExpr, upperBoundExpr);
            }
            Expression filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = Expression.AndAlso(lowerExpr, upperExpr);
            }
            if (filterExpr != null)
            {
                return source.Where(Expression.Lambda<Func<TSource, bool>>(filterExpr, member.Parameters));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IQueryable<TSource> Range<TSource, TMember>(this IQueryable<TSource> source, Expression<Func<TSource, TMember?>> member, Range<TMember> range)
             where TMember : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (range == null)
            {
                return source;
            }
            var memberExpr = member.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException();
            }
            var lowerBoundExpr = Expression.Constant(range.LowerBound, typeof(TMember));
            var upperBoundExpr = Expression.Constant(range.UpperBound, typeof(TMember));
            Expression lowerExpr = range.IncludeLowerBound ?
                Expression.GreaterThanOrEqual(memberExpr, lowerBoundExpr) :
                Expression.GreaterThan(memberExpr, lowerBoundExpr);
            Expression upperExpr = range.IncludeUpperBound
                ? Expression.LessThanOrEqual(memberExpr, upperBoundExpr)
                : Expression.LessThan(memberExpr, upperBoundExpr);
            return source.Where(Expression.Lambda<Func<TSource, bool>>(Expression.AndAlso(lowerExpr, upperExpr), member.Parameters));
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IEnumerable<TSource> Range<TSource, TMember>(this IEnumerable<TSource> source, Func<TSource, TMember> member, Range<TMember> range)
        {
#if NetCore
            bool isNullable = Nullable.GetUnderlyingType(typeof(TMember)) != null || !typeof(TMember).GetTypeInfo().IsValueType;
#else
            bool isNullable = Nullable.GetUnderlyingType(typeof(TMember)) != null || !typeof(TMember).IsValueType;
#endif
            Func<TMember, bool> lowerExpr = null, upperExpr = null;
            if (!isNullable || !Equals(range.LowerBound, default(TMember)))
            {
                lowerExpr = range.IncludeLowerBound
                    ? new Func<TMember, bool>(value => Comparer<TMember>.Default.Compare(value, range.LowerBound) >= 0)
                    : value => Comparer<TMember>.Default.Compare(value, range.LowerBound) > 0;
            }
            if (!isNullable || !Equals(range.UpperBound, default(TMember)))
            {
                upperExpr = range.IncludeUpperBound
                    ? new Func<TMember, bool>(value => Comparer<TMember>.Default.Compare(value, range.UpperBound) <= 0) :
                    value => Comparer<TMember>.Default.Compare(value, range.UpperBound) < 0;
            }
            Func<TMember, bool> filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = value => lowerExpr(value) && upperExpr(value);
            }
            if (filterExpr != null)
            {
                return source.Where(item => filterExpr(member(item)));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IEnumerable<TSource> Range<TSource>(this IEnumerable<TSource> source, Range<TSource> range)
        {
#if NetCore
            bool isNullable = Nullable.GetUnderlyingType(typeof(TSource)) != null || !typeof(TSource).GetTypeInfo().IsValueType;
#else
            bool isNullable = Nullable.GetUnderlyingType(typeof(TSource)) != null || !typeof(TSource).IsValueType;
#endif
            Func<TSource, bool> lowerExpr = null, upperExpr = null;
            if (!isNullable || !Equals(range.LowerBound, default(TSource)))
            {
                lowerExpr = range.IncludeLowerBound
                    ? new Func<TSource, bool>(value => Comparer<TSource>.Default.Compare(value, range.LowerBound) >= 0)
                    : value => Comparer<TSource>.Default.Compare(value, range.LowerBound) > 0;
            }
            if (!isNullable || !Equals(range.UpperBound, default(TSource)))
            {
                upperExpr = range.IncludeUpperBound
                    ? new Func<TSource, bool>(value => Comparer<TSource>.Default.Compare(value, range.UpperBound) <= 0) :
                    value => Comparer<TSource>.Default.Compare(value, range.UpperBound) < 0;
            }
            Func<TSource, bool> filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = value => lowerExpr(value) && upperExpr(value);
            }
            if (filterExpr != null)
            {
                return source.Where(item => filterExpr(item));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IEnumerable<TSource> Range<TSource, TMember>(this IEnumerable<TSource> source, Func<TSource, TMember> member, Range<TMember?> range)
            where TMember : struct
        {
            Func<TMember, bool> lowerExpr = null, upperExpr = null;
            if (range.LowerBound.HasValue)
            {
                lowerExpr = range.IncludeLowerBound
                    ? new Func<TMember, bool>(value => Comparer<TMember>.Default.Compare(value, range.LowerBound.Value) >= 0)
                    : value => Comparer<TMember>.Default.Compare(value, range.LowerBound.Value) > 0;
            }
            if (range.UpperBound.HasValue)
            {
                upperExpr = range.IncludeUpperBound
                    ? new Func<TMember, bool>(value => Comparer<TMember>.Default.Compare(value, range.UpperBound.Value) <= 0) :
                    value => Comparer<TMember>.Default.Compare(value, range.UpperBound.Value) < 0;
            }
            Func<TMember, bool> filterExpr;
            if (lowerExpr == null)
            {
                filterExpr = upperExpr;
            }
            else if (upperExpr == null)
            {
                filterExpr = lowerExpr;
            }
            else
            {
                filterExpr = value => lowerExpr(value) && upperExpr(value);
            }
            if (filterExpr != null)
            {
                return source.Where(item => filterExpr(member(item)));
            }
            return source;
        }

        /// <summary>
        /// Filters a sequence of values based on a <see cref="T:Range{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TMember">The type of the filtering element member.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="member">The lambda expression to indicate the element member to filter.</param>
        /// <param name="range">The <see cref="T:Range{T}"/> used to filter the sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that element members satisfy the range limitation.</returns>
        public static IEnumerable<TSource> Range<TSource, TMember>(this IEnumerable<TSource> source, Func<TSource, TMember?> member, Range<TMember> range)
            where TMember : struct
        {
            var lowerExpr = range.IncludeLowerBound
                ? new Func<TMember?, bool>(value => value.HasValue && Comparer<TMember>.Default.Compare(value.Value, range.LowerBound) >= 0)
                : (value => value.HasValue && Comparer<TMember>.Default.Compare(value.Value, range.LowerBound) > 0);
            var upperExpr = range.IncludeUpperBound
                ? new Func<TMember?, bool>(value => value.HasValue && Comparer<TMember>.Default.Compare(value.Value, range.UpperBound) <= 0) :
                value => value.HasValue && Comparer<TMember>.Default.Compare(value.Value, range.UpperBound) < 0;
            Func<TMember?, bool> filterExpr = value => lowerExpr(value) && upperExpr(value);
            return source.Where(item => filterExpr(member(item)));
        }
    }
}
