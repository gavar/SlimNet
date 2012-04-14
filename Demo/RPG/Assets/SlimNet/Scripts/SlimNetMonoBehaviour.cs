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
using System.Linq;
using System.Reflection;
using SlimNet;
using SlimNet.Unity;
using UnityEngine;

public class SlimNetMonoBehaviour : MonoBehaviour
{
    static Dictionary<Type, Pair<MethodInfo, Type>[]> methodTypes = new Dictionary<Type, Pair<MethodInfo, Type>[]>();
    static MethodInfo registerReceiverMethod = null;
    static object[] args = new object[1];

    static void registerMethods(SlimNetMonoBehaviour o)
    {
        if (o == null || o.networkActor == null)
        {
            return;
        }

        if (registerReceiverMethod == null)
        {
            registerReceiverMethod = typeof(Actor).GetMethod("RegisterEventReceiver");
        }

        Type action = typeof(Action<>);
        Type type = o.GetType();
        Pair<MethodInfo, Type>[] methods = null;

        if (!methodTypes.TryGetValue(type, out methods))
        {
            methodTypes[type] = methods =
                type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.ReturnType == typeof(void))
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters()[0].ParameterType.IsSubclassOf(typeof(Event<Actor>)))
                .Select(x => Tuple.Create(x, x.GetParameters()[0].ParameterType))
                .ToArray();
        }

        foreach (Pair<MethodInfo, Type> p in methods)
        {
            Type t = action.MakeGenericType(p.Second);
            args[0] = Delegate.CreateDelegate(t, o, p.First);
            registerReceiverMethod.MakeGenericMethod(p.Second).Invoke(o.networkActor, args);
        }
    }

    public bool networkActorIsMine { get { return networkActor != null ? networkActor.IsMine : false; } }
    public Actor networkActor { get { return this.GetSlimNetActor(); } }
    public Context networkContext { get { return this.GetSlimNetContext(); } }

    protected void RegisterEventReceivers()
    {
        registerMethods(this);
    }

    protected void Start()
    {
        RegisterEventReceivers();
    }
}