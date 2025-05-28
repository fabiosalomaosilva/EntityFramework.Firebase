using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EfCore.FirestoreProvider.Query
{
    public class FirestoreQueryCompilationContext : QueryCompilationContext
    {
        public FirestoreQueryCompilationContext(QueryCompilationContextDependencies dependencies, bool async)
            : base(dependencies, async)
        {
        }

        // Aqui você pode adicionar métodos e propriedades adicionais
        // que são necessários para compilar consultas Firestore.
    }
}
