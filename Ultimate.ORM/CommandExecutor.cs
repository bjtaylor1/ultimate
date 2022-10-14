using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Ultimate.ORM
{
    public class CommandExecutor
    {
        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, System.Data.CommandBehavior commandBehaviour, CancellationToken ct) => command.ExecuteReaderAsync(commandBehaviour, ct);
        public virtual Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken ct) => command.ExecuteReaderAsync(ct);
    }
}
