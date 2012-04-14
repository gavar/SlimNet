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