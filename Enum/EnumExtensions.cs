using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Portal.Application.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        ///    Возвращает атрибут примененный к Enum
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : System.Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        ///    Возвращает значения атрибута всех полей Enum
        /// </summary>
        public static IEnumerable<TAttribute> GetAttributesFileds<TEnumType, TAttribute>()
              where TAttribute : System.Attribute
        {
            return Enum.GetValues(typeof(TEnumType)).Cast<Enum>().Select(x => GetAttribute<TAttribute>(x)).ToList();
        }
    }
}