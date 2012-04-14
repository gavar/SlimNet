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

namespace SlimNet
{
    public static class DictionaryExtensions
    {
        static readonly Log log = Log.GetLogger(typeof(DictionaryExtensions));

        public static bool TryGetTypeValue<T>(this Dictionary<Type, T> dict, Type type, out T editor)
        {
            string name = type.Name;

            while (type != null)
            {
                if (dict.TryGetValue(type, out editor))
                {
                    return true;
                }

                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    if (dict.TryGetValue(type.GetGenericTypeDefinition(), out editor))
                    {
                        return true;
                    }
                }

                type = type.BaseType;
            }

            log.Error("No value for type {0} found", name);
            editor = default(T);
            return false;
        }
    }
}
