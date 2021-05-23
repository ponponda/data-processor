using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Utility {
    static class TypeUtility {
        public static bool IfNullable(Type type) => !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        public static object GetDefaultValue(Type type) => type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }
}
