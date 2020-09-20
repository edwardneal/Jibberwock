using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.Utility
{
    internal class ValueDbList<T> : List<T>, IEnumerable<SqlDataRecord>
    {
        public string KeyColumnName { get; private set; }

        public ValueDbList(string keyColumnName, IEnumerable<T> collection)
            : base(collection)
        {
            if (string.IsNullOrEmpty(keyColumnName))
                throw new ArgumentNullException(nameof(keyColumnName));

            KeyColumnName = keyColumnName;
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var columnType = getColumnType();
            var metadata = columnType == System.Data.SqlDbType.NVarChar ? new SqlMetaData(KeyColumnName, columnType, SqlMetaData.Max) : new SqlMetaData(KeyColumnName, columnType);

            foreach (var item in this)
            {
                var sdr = new SqlDataRecord(metadata);
                var type = typeof(T);

                // We need to handle both enumeration and non-enumeration types. These should be done differently.
                if (type.IsEnum)
                { sdr.SetValue(0, Convert.ChangeType(item, typeof(T).GetEnumUnderlyingType())); }
                else
                {
                    // If the type is a nullable type, peel it back to get to the real type. Nullability is handled elsewhere.
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = type.GenericTypeArguments[0];
                    }

                    if (item == null)
                    {
                        sdr.SetValue(0, null);
                    }
                    else
                    {
                        sdr.SetValue(0, Convert.ChangeType(item, type));
                    }
                }

                yield return sdr;
            }
        }

        private System.Data.SqlDbType getColumnType()
        {
            // We need to handle both enumeration and non-enumeration types. These should be done differently.
            var type = typeof(T);

            // If the type is a nullable type, peel it back to get to the real type. Nullability is handled elsewhere.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GenericTypeArguments[0];
            }

            if (type.IsEnum)
            {
                // We need to map the base type of the enumeration into a SqlDbType which can represent it
                // Base types are byte, sbyte, short, ushort, int, uint, long, ulong
                // SqlDbTypes are tinyint, smallint, int, bigint
                type = typeof(T).GetEnumUnderlyingType();

                // byte, short, int, long are easy - they're just mapped to tinyint, smallint, int, bigint.
                // sbyte could be negative, which tinyint doesn't allow. Map it to a smallint.
                // ushort could be twice the maximum size of a smallint, so map it to an int.
                // uint could be twice the maximum size of an int, so map it to a long.
                // ulong could be twice the size of a bigint, so map it to a decimal.

                if (type == typeof(byte))
                    return System.Data.SqlDbType.TinyInt;
                if (type == typeof(sbyte) || type == typeof(short))
                    return System.Data.SqlDbType.SmallInt;
                if (type == typeof(ushort) || type == typeof(int))
                    return System.Data.SqlDbType.Int;
                if (type == typeof(uint) || type == typeof(long))
                    return System.Data.SqlDbType.BigInt;
                if (type == typeof(byte))
                    return System.Data.SqlDbType.Decimal;

                throw new InvalidOperationException("Enumeration type must be one of byte, sbyte, short, ushort, int, uint, long or ulong.");
            }
            else
            {
                if (type == typeof(Guid))
                    return System.Data.SqlDbType.UniqueIdentifier;
                if (type == typeof(string))
                    return System.Data.SqlDbType.NVarChar;

                throw new InvalidOperationException("Non-enumeration type must be one of Guid or string.");
            }
        }
    }
}
