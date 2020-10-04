using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes
{
    internal abstract class UserDefinedTableType : SqlDataRecord
    {
        protected UserDefinedTableType(params SqlMetaData[] metaData)
            : base(metaData)
        {
        }

        protected void SetValue<T>(int index, T value)
        {
            var valType = typeof(T);

            // We need to handle both enumeration and non-enumeration types. These should be done differently.
            if (valType.IsEnum)
            { base.SetValue(index, Convert.ChangeType(value, valType.GetEnumUnderlyingType())); }
            else
            {
                // If the type is a nullable type, peel it back to get to the real type. Nullability is handled elsewhere.
                if (valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    valType = valType.GenericTypeArguments[0];
                }

                if (value == null)
                {
                    base.SetValue(index, null);
                }
                else
                {
                    base.SetValue(index, Convert.ChangeType(value, valType));
                }
            }
        }

        public static SqlMetaData GetColumnMetadata<T>(string columnName)
        {
            var dbType = getColumnType<T>();

            return dbType == SqlDbType.NVarChar
                ? new SqlMetaData(columnName, dbType, SqlMetaData.Max)
                : new SqlMetaData(columnName, dbType);
        }

        private static SqlDbType getColumnType<T>()
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
                type = type.GetEnumUnderlyingType();
            }

            // byte, short, int, long are easy - they're just mapped to tinyint, smallint, int, bigint.
            // sbyte could be negative, which tinyint doesn't allow. Map it to a smallint.
            // ushort could be twice the maximum size of a smallint, so map it to an int.
            // uint could be twice the maximum size of an int, so map it to a long.
            // ulong could be twice the size of a bigint, so map it to a decimal.
            if (type == typeof(byte))
                return SqlDbType.TinyInt;
            if (type == typeof(sbyte) || type == typeof(short))
                return SqlDbType.SmallInt;
            if (type == typeof(ushort) || type == typeof(int))
                return SqlDbType.Int;
            if (type == typeof(uint) || type == typeof(long))
                return SqlDbType.BigInt;
            if (type == typeof(byte))
                return SqlDbType.Decimal;
            if (type == typeof(Guid))
                return SqlDbType.UniqueIdentifier;
            if (type == typeof(string))
                return SqlDbType.NVarChar;
            if (type == typeof(object))
                return SqlDbType.Variant;
            if (type == typeof(DateTimeOffset))
                return SqlDbType.DateTimeOffset;
            if (type == typeof(DateTime))
                return SqlDbType.DateTime2;

            throw new InvalidOperationException("Non-enumeration type must be one of Guid or string.");
        }
    }
}
