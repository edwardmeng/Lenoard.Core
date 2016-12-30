using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lenoard.Core
{
    public static class RangeExtensions
    {
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
