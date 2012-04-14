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

namespace SlimNet
{
    public static class TypeUtils
    {
        public static readonly object[] EmptyObjects = new object[0];
        public static readonly BindingFlags AnyInstanceFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public static readonly BindingFlags PublicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;

        readonly static Log log = Log.GetLogger(typeof(TypeUtils));
        readonly static HashSet<string> assemblyIgnoreList = new HashSet<string>();

        static TypeUtils()
        {
            assemblyIgnoreList.Add("Mono");
            assemblyIgnoreList.Add("UnityScript");
            assemblyIgnoreList.Add("Boo");
            assemblyIgnoreList.Add("System");
            assemblyIgnoreList.Add("I18N");
            assemblyIgnoreList.Add("SlimMath");
            assemblyIgnoreList.Add("Lidgren");
            assemblyIgnoreList.Add("UnityEngine");
            assemblyIgnoreList.Add("UnityEditor");
            assemblyIgnoreList.Add("mscorlib");
            assemblyIgnoreList.Add("SlimIOCP");
        }

        static bool shouldIgnore(Assembly assembly)
        {
            foreach (var skipName in assemblyIgnoreList)
            {
                if (assembly.FullName.StartsWith(skipName))
                    return true;
            }

            return false;
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            return type.GetProperties(PublicInstanceFlags);
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            return type.GetConstructor(AnyInstanceFlag, null, Type.EmptyTypes, null);
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return GetDefaultConstructor(type) != null;
        }

        public static bool HasConstructor<T>(this Type type)
        {
            return type.GetConstructor(PublicInstanceFlags, null, new[] { typeof(T) }, null) != null;
        }

        public static object CreateInstance(string type)
        {
            Assert.NotNullOrEmpty(type, "type");
            return CreateInstance(Type.GetType(type));
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

        public static List<Type> GetSubTypes(this Type type)
        {
            return GetSubTypes(type, AppDomain.CurrentDomain);
        }

        public static List<Type> GetSubTypes(this Type type, AppDomain domain)
        {
            return GetSubTypes(type, domain.GetAssemblies());
        }

        public static List<Type> GetSubTypes(this Type type, Assembly[] assemblies)
        {
            return GetSubTypes(type, assemblies, null);
        }

        public static Type[] GetEnumTypes()
        {
            List<Type> enumTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly asm in assemblies)
            {
                foreach (Type type in asm.GetTypes())
                {
                    if(ReferenceEquals(type.BaseType, typeof(Enum)))
                    {
                        enumTypes.Add(type);
                    }
                }
            }

            return enumTypes.ToArray();
        }

        public static List<Type> GetSubTypes(this Type type, Assembly[] assemblies, string name)
        {
            Assembly asm = null;

            var types = new List<Type>();

            foreach (var assembly in assemblies)
            {
                asm = assembly;

                if (shouldIgnore(assembly))
                    continue;

                if (name != null && assembly.FullName != name)
                    continue;

                foreach (Type subType in assembly.GetTypes())
                {
                    if (!subType.IsClass)
                        continue;

                    if (subType.IsAbstract)
                        continue;

                    if (!subType.IsPublic)
                        continue;

                    if (!subType.IsSubclassOf(type))
                        continue;

                    types.Add(subType);
                }

            }

            return types;
        }

        public static string GetTypeName(this object obj)
        {
            if (obj == null)
            {
                log.Warn("Tried to get type name of null reference");
                return "Null";
            }

            return obj.GetType().FullName;
        }
    }
}