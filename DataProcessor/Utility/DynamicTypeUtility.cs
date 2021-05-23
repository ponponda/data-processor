using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Utility {
    static class DynamicTypeUtility {
        // any custom name
        private static AssemblyName assemblyName = new AssemblyName() { Name = "DynamicLinqTypes" };
        // To create transient dynamic module
        private static ModuleBuilder moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule(assemblyName.Name);
        private static Dictionary<string, Type> builtTypeNames = new Dictionary<string, Type>();

        private static string GetTypeName(Dictionary<string, Type> fields) {
            //TODO: optimize the type caching -- if fields are simply reordered, that doesn't mean that they're actually different types, so this needs to be smarter
            string key = string.Empty;
            foreach(var field in fields.OrderBy(e => e.Key))
                key += field.Key + ";" + field.Value.Name + ";";

            return key;
        }

        public static Type Parse(Dictionary<string, Type> fields) {
            string typeName = GetTypeName(fields);
            if(builtTypeNames.ContainsKey(typeName)) return builtTypeNames[typeName];

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

            foreach(var field in fields) 
                typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
            
            builtTypeNames[typeName] = typeBuilder.CreateType();
            return builtTypeNames[typeName];
        }
    }
}
