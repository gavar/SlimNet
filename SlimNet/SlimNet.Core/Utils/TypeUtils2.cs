/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SlimNet.Utils
{
    public static class TypeUtils2
    {
        public static readonly object[] EmptyObjects = new object[0];
        public static readonly BindingFlags AnyInstanceFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public static readonly BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
        public static readonly BindingFlags AnyFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        readonly static Log log = Log.GetLogger(typeof(TypeUtils2));
        readonly static HashSet<string> assemblyIgnoreList = new HashSet<string>();

        static TypeUtils2()
        {
            assemblyIgnoreList.Add("Mono");
            assemblyIgnoreList.Add("UnityScript");
            assemblyIgnoreList.Add("Boo");
            assemblyIgnoreList.Add("System");
            assemblyIgnoreList.Add("I18N");
            assemblyIgnoreList.Add("UnityEngine");
            assemblyIgnoreList.Add("UnityEditor");
            assemblyIgnoreList.Add("mscorlib");
        }

        public static bool AssemblyIsIgnored(Assembly assembly)
        {
            foreach (var skipName in assemblyIgnoreList)
            {
                if (assembly.FullName.StartsWith(skipName))
                {
                    return true;
                }
            }

            return false;
        }

        public static Type[] GetEnumTypes()
        {
            List<Type> types = new List<Type>();

            foreach (Assembly asm in GetAssemblies())
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (ReferenceEquals(type.BaseType, typeof(Enum)))
                    {
                        types.Add(type);
                    }
                }
            }

            return types.ToArray();
        }

        public static List<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (AssemblyIsIgnored(asm))
                    continue;

                assemblies.Add(asm);
            }

            return assemblies;
        }

        public static List<Type> GetSubTypes(Type type)
        {
            List<Type> types = new List<Type>(8);

            foreach (var asm in GetAssemblies())
            {
                foreach (Type subType in asm.GetTypes())
                {
                    if (!subType.IsClass)
                        continue;

                    if (subType.IsAbstract)
                        continue;

                    if (subType.IsGenericTypeDefinition)
                        continue;

                    if (!subType.IsPublic)
                        continue;

                    if (type.IsClass && !subType.IsSubclassOf(type))
                        continue;

                    if (type.IsInterface && !type.IsAssignableFrom(subType))
                        continue;

                    types.Add(subType);
                }

            }

            return types;
        }

        public static List<Type> GetTypesWithAttributes(List<Type> types, bool inherited, params Type[] attributeTypes)
        {
            // Quick for no attributes
            if (attributeTypes.Length == 0)
            {
                return new List<Type>(types);
            }

            List<Type> finalTypes = new List<Type>(types.Count);

            foreach (Type type in types)
            {
                foreach (Type attribute in attributeTypes)
                {
                    if (type.GetCustomAttributes(attribute, inherited).Length > 0)
                    {
                        finalTypes.Add(type);
                        break;
                    }
                }
            }

            return finalTypes;
        }

        public static Type[] GetGenericArgs(this Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return type.GetGenericArguments();
            }

            return new Type[0];
        }


        public static List<Type> GetClasses(Assembly assembly)
        {
            return GetClasses(assembly, false);
        }

        public static List<Type> GetClasses(Assembly assembly, bool allowAbstract)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if(type.IsClass && (!type.IsAbstract || allowAbstract) && !type.IsGenericTypeDefinition)
                {
                    types.Add(type);
                }
            }

            return types;
        }

        public static string GetPrettyName(this Type type)
        {
            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();
                string genericNames = String.Join(",", genericArgs.Select<System.Type, System.String>(GetPrettyName).ToArray());
                return type.GetGenericTypeDefinition().FullName.Replace("`" + genericArgs.Length, "").Replace('+', '.') + "<" + genericNames + ">";
            }

            return type.FullName;
        }

        public static string GetPrettyName(this MemberInfo m)
        {
            string name = m.DeclaringType.GetPrettyName() + "." + m.Name;

            if (name.StartsWith("."))
            {
                name = "global::" + name.TrimStart('.');
            }

            return name;
        }

        public static T GetAttribute<T>(this MemberInfo m)
            where T : Attribute
        {
            object[] attrs = m.GetCustomAttributes(typeof(T), true);

            if (attrs.Length > 0)
            {
                return (T)attrs[0];
            }

            return null;
        }

        public static List<Type> GetStructs(Assembly assembly)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsValueType && !type.IsPrimitive && !type.IsEnum)
                {
                    types.Add(type);
                }
            }

            return types;
        }

        public static List<FieldInfo> GetFieldsWithTypes(BindingFlags flags, params Type[] types)
        {
            List<FieldInfo> result = new List<FieldInfo>();

            if (types.Length > 0)
            {
                foreach (Assembly asm in GetAssemblies())
                {
                    foreach (Type type in GetClasses(asm, true))
                    {
                        foreach (FieldInfo field in type.GetFields(flags))
                        {
                            foreach (Type t in types)
                            {
                                if (Object.ReferenceEquals(field.FieldType, t))
                                {
                                    result.Add(field);
                                    break;
                                }

                                if (field.FieldType.IsGenericType && t.IsGenericTypeDefinition && Object.ReferenceEquals(field.FieldType.GetGenericTypeDefinition(), t))
                                {
                                    result.Add(field);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static List<MethodInfo> GetMethodsWithAttributes(BindingFlags flags, params Type[] attributes)
        {
            List<MethodInfo> result = new List<MethodInfo>();

            if (attributes.Length > 0)
            {
                foreach (Assembly asm in GetAssemblies())
                {
                    foreach (Type type in GetClasses(asm))
                    {
                        foreach (MethodInfo method in type.GetMethods(flags))
                        {
                            foreach (Type attribute in attributes)
                            {
                                if (Attribute.IsDefined(method, attribute))
                                {
                                    result.Add(method);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static List<MethodInfo> GetMethodsWithAttributes(params Type[] attributes)
        {
            return GetMethodsWithAttributes(AnyFlags, attributes);
        }

        public static object CreateInstance(this Type type)
        {
            Assert.NotNull(type, "type");

            if (type.IsAbstract)
            {
                log.Error("Can't create instance of abstract type {0}", type.FullName);
                return null;
            }

            if (type.IsGenericTypeDefinition)
            {
                log.Error("Can't create instance of generic type definition {0}", type.FullName);
                return null;
            }

            if (type.IsPrimitive)
            {
                log.Error("Can't create instance of primitive type {0}", type.FullName);
                return null;
            }

            // For value type
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            ConstructorInfo ctor = GetDefaultConstructor(type);

            if (ctor == null)
            {
                log.Error("Can't create instance of type {0} as it has no default constructor", type.FullName);
                return null;
            }

            return ctor.Invoke(EmptyObjects);
        }

        public static List<Type> GetSubTypesWithAttribute(Type baseType, bool inherit, params Type[] attributeTypes)
        {
            return GetTypesWithAttributes(GetSubTypes(baseType), inherit, attributeTypes);
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructor(AnyInstanceFlag, null, Type.EmptyTypes, null);
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return GetDefaultConstructor(type) != null;
        }
    }
}
