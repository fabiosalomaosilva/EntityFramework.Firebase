using System.Linq;
using System.Linq.Expressions;

namespace EfCore.FirestoreProvider.Query
{
    public interface IFirestoreQueryProvider : IQueryProvider
    {
        IQueryable<TElement> CreateFirestoreQuery<TElement>(Expression expression);
    }
}
