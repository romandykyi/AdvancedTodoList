using AdvancedTodoList.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTodoList.Infrastructure.Extensions;

public static class QueryableSpecificationExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> queryable, ISpecification<T> specification)
        where T : class
    {
        // Include all expression-based includes
        var queryableResultWithIncludes = specification.Includes
            .Aggregate(queryable, (current, include) => current.Include(include));

        // Include string-based include statements
        var secondaryResult = specification.IncludeStrings
            .Aggregate(queryableResultWithIncludes,
                (current, include) => current.Include(include));

        // Apply criteria and return the result
        return secondaryResult.Where(specification.Criteria);
    }
}
