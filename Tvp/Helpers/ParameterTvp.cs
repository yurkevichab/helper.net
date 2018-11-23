using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using Dapper;
using Microsoft.SqlServer.Server;
using SynchronizationSubscriberService.Attribute;

namespace SynchronizationSubscriberService.Helpers
{
    public class ParameterTvp<T> : SqlMapper.IDynamicParameters
    {
        private readonly IEnumerable<T> _parameters;

        public ParameterTvp(IEnumerable<T> parameters)
        {
            _parameters = parameters;
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var sqlCommand = (SqlCommand) command;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            var items = new List<SqlDataRecord>();
            var tvpSettings = typeof(T).GetTypeInfo().GetCustomAttribute<TvpTypeSettingsAttribute>();

            if (tvpSettings == null)
            {
                throw new Exception($"У class'a  {typeof(T)} не заданны атрибут SQL");
            }

            foreach (var param in _parameters)
            {
                var properties = param.GetType().GetTypeInfo().GetProperties()
                    .Where(prop => prop.IsDefined(typeof(TvpColumnsSettingsAttribute))).ToList();

                var sqlmeta = new List<SqlMetaData>();
                var tvpColumnValues = new Dictionary<string, TvpColumnsSettingsAttribute>();
                foreach (var property in properties)
                {
                    var columnAtt = property.GetCustomAttribute<TvpColumnsSettingsAttribute>();
                    tvpColumnValues.Add(property.Name, columnAtt);
                    sqlmeta.Add(new SqlMetaData(columnAtt.SqlColumnName, columnAtt.SqlColumnType));
                }

                var record = new SqlDataRecord(sqlmeta.ToArray());
                foreach (var property in properties)
                {
                    fillSqlDataRecord(param, tvpColumnValues[property.Name], property.Name, ref record);
                }
                items.Add(record);
            }

            var p = sqlCommand.Parameters.Add(tvpSettings.SqlParameterName, SqlDbType.Structured);
            p.Direction = ParameterDirection.Input;
            p.TypeName = tvpSettings.SqlTypeName;
            p.Value = items;
        }

        /// <summary>
        /// Метод для заполниния SqlDataRecord значениями в соответстии с типом
        /// </summary>
        /// <param name="data">значение поля</param>
        /// <param name="propAttr"> тип колонки</param>
        /// <param name="record">Sql запись</param>
        private void fillSqlDataRecord(dynamic data, TvpColumnsSettingsAttribute propAttr,string propertyName,
            ref SqlDataRecord record)
        {
            var correctdata = typeof(T).GetTypeInfo().GetProperty(propertyName).GetValue(data, null);
            switch (propAttr.SqlColumnType)
            {
                case SqlDbType.Int:
                    if (propAttr.IsNull)
                    {
                        record.SetSqlInt32(propAttr.Ordinal, correctdata!=null ? correctdata : SqlInt32.Null);
                        break;
                    }
                    record.SetInt32(propAttr.Ordinal, correctdata);
                    break;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    if (propAttr.IsNull)
                    {
                        record.SetSqlDateTime(propAttr.Ordinal,
                            !propAttr.IsNull && correctdata.HasValue ? correctdata.GetValueOrDefault() : SqlInt32.Null);
                        break;
                    }
                    record.SetDateTime(propAttr.Ordinal, (DateTime)correctdata);
                    break;
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    record.SetString(propAttr.Ordinal, correctdata);
                    break;
                case SqlDbType.Bit:
                    record.SetBoolean(propAttr.Ordinal, correctdata);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("SqlType not found");

            }
        }
    }
}
