using System;

namespace SynchronizationSubscriberService.Attribute
{
    /// <summary>
    /// Атрибут задающий настройки для TVP параметра
    /// </summary>
    /// <value name="sqlTypeName">Тип Tvp параметра в SQL</value>
    /// <value name="sqlParameterName">Название параметра</value>
    [AttributeUsage(AttributeTargets.Class)]
    public class TvpTypeSettingsAttribute : System.Attribute
    {
        public TvpTypeSettingsAttribute(string sqlTypeName, string sqlParameterName)
        {
            SqlTypeName = sqlTypeName;
            SqlParameterName = sqlParameterName;
        }

        public string SqlTypeName { get; set; }
        public string SqlParameterName { get; set; }
    }
}
