using System;
using System.Collections.Concurrent;
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
        public ObjectMapper() : this(new CommandExecutor())
        {

        }
        public ObjectMapper(CommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        private ConcurrentDictionary<Type, MethodInfo> tryParseMethods = new ConcurrentDictionary<Type, MethodInfo>();
        private readonly CommandExecutor commandExecutor;

        public async Task<T> ToSingleObject<T>(DbCommand command, CancellationToken ct = default) where T : new()
        {
            using var reader = await commandExecutor.ExecuteReaderAsync(command, System.Data.CommandBehavior.SingleResult, ct);
            return await ToSingleObject<T>(reader, ct);
        }

        public async Task<T> ToSingleObject<T>(DbDataReader reader, CancellationToken ct) where T : new()
        {
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
            await using var reader = await commandExecutor.ExecuteReaderAsync(command, ct);
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

        private T ObjectFromReader<T>(DbDataReader reader, PropertyInfo[] properties, Dictionary<PropertyInfo, int> ordinalMap) where T : new()
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

        public object ConvertValue(object rawVal, Type propertyType)
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
                
                returnValue = TryParse(rawVal, typeToConvertTo, out var parsedObject)
                    ? parsedObject : Convert.ChangeType(rawVal, typeToConvertTo);
            }

            return returnValue;
        }

        private bool TryParse(object rawVal, Type type, out object parsedObject)
        {
            parsedObject = null;
            if(rawVal == null)
            {
                return false;
            }
            
            // has it got a tryParse method?
            var tryParseMethod = tryParseMethods.GetOrAdd(type, t => t.GetMethod("TryParse", 0, new [] {rawVal.GetType(), type.MakeByRefType()} ));
            if(tryParseMethod != null)
            {
                var methodParams = new object [] {rawVal, parsedObject};
                var retVal = (bool)tryParseMethod.Invoke(null, methodParams);
                parsedObject = methodParams[1];
                return retVal;
            }
            else
            {
                return false;
            }
        }
    }
}
