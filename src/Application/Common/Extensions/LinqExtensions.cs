using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Common.Extensions;

public static class LinqExtensions
{
    extension<T>(IQueryable<T> query) where T : class
    {
        public IQueryable<T> ApplySort(string? sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var allowedProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            string[] sortExpressions = [];

            foreach (var part in sortBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
                    continue;

                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? "descending"
                    : "ascending";

                _ = sortExpressions.Append($"{tokens[0]} {direction}");
            }

            return sortExpressions.Length > 0
                ? query.OrderBy(string.Join(", ", sortExpressions))
                : query;
        }

        public IQueryable<T> WhereIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }

    extension(string? query)
    {
        public string NormalizeSearchQuery()
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            var terms = query
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t + ":*");

            return string.Join(" & ", terms);
        }
    }
}