using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SynchronizationSubscriberService.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Получает тип метода(из текущей dll) по имени
        /// </summary>
        /// <param name="methodName">Название метода</param>
        /// <returns>тип</returns>
        public static Type GetMethodByName(string methodName)
        {
            var method = Assembly.GetEntryAssembly().GetTypes()
                .FirstOrDefault(t => t.Name == methodName);
            if (method == null)
            {
                throw new ArgumentNullException($"{nameof(GetMethodByName)} methodName: {methodName}");
            }
            return method;
        }

        public static Type GenericType(Type method, Type genericType)
        {
            Type[] typeArgs = { method };
            return genericType.MakeGenericType(typeArgs);
        }
    }
}
