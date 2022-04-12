using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Ultimate.ORM
{
    public interface IObjectMapper
    {
        Task<T> ToSingleObject<T>(DbCommand command, CancellationToken ct = default) where T : new();
        Task<List<T>> ToMultipleObjects<T>(DbCommand command, CancellationToken ct = default) where T : new();
    }
}