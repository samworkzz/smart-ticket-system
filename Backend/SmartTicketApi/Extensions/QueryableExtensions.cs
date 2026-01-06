using System.Linq.Expressions;
using SmartTicketApi.Models.DTOs.Shared;

namespace SmartTicketApi.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string? sortBy, bool descending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return source;

            var param = Expression.Parameter(typeof(T), "x");
            
            // Handle nested properties (e.g., "TicketStatus.StatusName")
            Expression property = param;
            try 
            {
                foreach (var member in sortBy.Split('.'))
                {
                    property = Expression.PropertyOrField(property, member);
                }
            }
            catch 
            {
                // If property not found, return original source or handle error
                // For now, ignore invalid sort columns to prevent crash
                return source; 
            }

            var lambda = Expression.Lambda(property, param);

            string methodName = descending ? "OrderByDescending" : "OrderBy";
            
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(lambda)
            );

            return source.Provider.CreateQuery<T>(resultExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
