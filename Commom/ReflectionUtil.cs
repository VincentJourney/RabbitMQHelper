using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RabbitMQHelper.Commom
{
    public class ReflectionUtil
    {
        public static bool ObjectIsNull<T>(T Model) where T : class
        {
            if (Model == null) return false;
            PropertyInfo[] properties = Model.GetType().GetProperties();
            foreach (var item in properties)
            {
                if (string.IsNullOrWhiteSpace(item.GetValue(Model).ToString()))
                    return false;
            }
            return true;
        }
    }
}
