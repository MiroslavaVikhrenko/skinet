using System.Linq.Expressions;

namespace Core.Interfaces;

public interface ISpecification<T>
{
    // T as parameter and return bool
    Expression<Func<T, bool>>? Criteria { get; }
    // object as it maight be string (name) or decimal (price) for ordering
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    bool IsDistinct { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Select { get; }
}