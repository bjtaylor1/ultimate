using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Ultimate.ORM
{
    public interface IObjectMapper
    {
        Task<List<T>> ToMultipleObjects<T>(DbCommand command) where T : new();
        Task<T> ToSingleObject<T>(DbCommand command) where T : new();
    }
}