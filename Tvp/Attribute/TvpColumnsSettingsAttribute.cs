using System;
using System.Data;

namespace SynchronizationSubscriberService.Attribute
{
    /// <summary>
    /// Атрибут для мапинга столбцов для TVP
    /// </summary>
    /// <value name="sqlColumnName">Название столбца</value>
    /// <value name="sqlColumnType">Заданный тип стобца в sql</value>
    /// <value name="ordinal">Порядковый индекс столбца в sql типе</value>
    /// <value name="isNull">Тип имеет null значения</value>
    [AttributeUsage(AttributeTargets.Property)]
    public class TvpColumnsSettingsAttribute : System.Attribute
    {
        public TvpColumnsSettingsAttribute(string sqlColumnName, SqlDbType sqlColumnType, int ordinal, bool isNull = false)
        {
            Ordinal = ordinal;
            SqlColumnType = sqlColumnType;
            IsNull = isNull;
            SqlColumnName = sqlColumnName;
        }
        public int Ordinal { get; set; }
        public string SqlColumnName{ get; set; }
        public SqlDbType SqlColumnType { get; set; }
        public bool IsNull { get; set; }
    }
}
