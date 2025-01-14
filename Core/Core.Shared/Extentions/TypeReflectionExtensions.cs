using System.Reflection;

namespace Core.Shared.Extentions
{
    public static class TypeReflectionExtensions
    {
        /// <summary>
        /// Get all values of Fields of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllFieldsValues(this Type type)
        {

            var allFields = type.GetAllFields().ToList().ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
            return allFields;
        }
        /// <summary>
        /// Get all values of Properties of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllPropertiesValues(this Type type)
        {

            var allProperties = type.GetAllProperties().ToList().ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
            return allProperties;
        }
        /// <summary>
        /// Get all Fields of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetFields(flags).Union(GetAllFields(type.BaseType));
        }

        /// <summary>
        /// Get all properties of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetProperties(flags).Union(GetAllProperties(type.BaseType));
        }

        /// <summary>
        /// Get all constructors of a class
        /// </summary>
        /// <param name="type">Type object of that class</param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<ConstructorInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetConstructors(flags);
        }

        /// <summary>
        /// Get all methods of a class
        /// </summary>
        /// <param name="type">Type object for that class</param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetMethods(flags).Union(GetAllMethods(type.BaseType));
        }
    }
}
