using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ultimate.ORM
{
    public class ObjectMapper : IObjectMapper
    {
        public async Task<T> ToSingleObject<T>(DbCommand command, CancellationToken ct = default) where T : new()
        {
            using var reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleResult, ct);
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var ordinalMap = GetOrdinalMap(properties, reader);
            if (await reader.ReadAsync(ct))
            {
                T t = ObjectFromReader<T>(reader, properties, ordinalMap);
                return t;
            }
            else throw new InvalidOperationException("The command did not return any records");
        }

        public async Task<List<T>> ToMultipleObjects<T>(DbCommand command, CancellationToken ct = default) where T : new()
        {
            await using var reader = await command.ExecuteReaderAsync(ct);
            return await ToMultipleObjects<T>(reader, ct);
        }

        public async Task<List<T>> ToMultipleObjects<T>(DbDataReader reader, CancellationToken ct = default) where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var ordinalMap = GetOrdinalMap(properties, reader);
            var returnValue = new List<T>();
            while (await reader.ReadAsync(ct))
            {
                T t = ObjectFromReader<T>(reader, properties, ordinalMap);
                returnValue.Add(t);
            }
            return returnValue;
        }

        private static T ObjectFromReader<T>(DbDataReader reader, PropertyInfo[] properties, Dictionary<PropertyInfo, int> ordinalMap) where T : new()
        {
            var t = new T();
            foreach (var prop in properties)
            {
                int ordinal = ordinalMap[prop];
                object valToSet;
                if (prop.PropertyType == typeof(long))
                {
                    valToSet = reader.GetInt64(ordinal);
                }
                else
                {
                    var rawVal = reader[ordinal];
                    valToSet = ConvertValue(rawVal, prop.PropertyType);
                }
                
                prop.SetValue(t, valToSet);
            }
            return t;
        }

        private Dictionary<PropertyInfo, int> GetOrdinalMap(PropertyInfo[] properties, DbDataReader reader)
        {
            var unsatisfiedProperties = new List<string>();
            var returnValue = new Dictionary<PropertyInfo, int>();
            foreach(var prop in properties)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(prop.Name);
                    if (ordinal == -1)
                    {
                        unsatisfiedProperties.Add(prop.Name);
                    }
                    else
                    {
                        returnValue.Add(prop, ordinal);
                    }
                }
                catch(IndexOutOfRangeException)
                {
                    unsatisfiedProperties.Add(prop.Name);
                }
            }
            if(unsatisfiedProperties.Any())
            {
                var propList = string.Join(Environment.NewLine, unsatisfiedProperties);
                throw new InvalidOperationException($"The following properties were not satisfied:{Environment.NewLine}{propList}");
            }
            return returnValue;
        }

        private static object ConvertValue(object rawVal, Type propertyType)
        {
            object returnValue;
            if(rawVal == null || DBNull.Value.Equals(rawVal))
            {
                returnValue = null;
            }
            else if(propertyType.IsEnum)
            {
                if (rawVal is string stringVal)
                {
                    returnValue = Enum.Parse(propertyType, stringVal);
                }
                else if (rawVal is int intVal)
                {
                    returnValue = Enum.ToObject(propertyType, intVal);
                }
                else throw new InvalidOperationException($"Attempt to convert value of type {rawVal.GetType()} to enum {propertyType} failed. Must be a string or int.");
            }
            else
            {
                Type typeToConvertTo;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typeToConvertTo = propertyType.GetGenericArguments().Single();
                }
                else
                {
                    typeToConvertTo = propertyType;
                }
                returnValue = Convert.ChangeType(rawVal, typeToConvertTo);
            }

            return returnValue;
        }
    }
}
