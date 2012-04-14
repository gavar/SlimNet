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
using SlimMath;
using SlimNet.Utils;

namespace SlimNet 
{
    using Reader = Func<Network.ByteInStream, Type, object>;
    using Writer = Action<Network.ByteOutStream, Type, object>;

    public abstract class RPCResult
    {
        internal int ReturnIndex { get; set; }
        internal object BoxedValue { get; set; }

        public Type Type { get; private set; }
        public string Name { get; internal set; }
        public bool IsDone { get; internal set; }

        public RPCResult(Type type)
        {
            Type = type;
        }

        internal abstract void Done();
    }

    public class RPCResult<T> : RPCResult
    {
        public event Action<T> OnComplete;
        public T Value { get { return BoxedValue != null ? (T)BoxedValue : default(T); } }

        public RPCResult()
            : base(typeof(T))
        {

        }

        internal override void Done()
        {
            if (OnComplete != null)
            {
                OnComplete(Value);
            }
        }
    }

    public class RPCInfo
    {
        public Player Caller { get; internal set; }
        public Context Context { get; internal set; }
        public float RemoteGameTime { get; internal set; }
    }

    public partial class RPCDispatcher : IPacketHandler
    {
        class Receiver
        {
            public string Name;
            public Type[] Parameters;
            public object Instance;
            public MethodInfo Method;
            public bool RequiresInfoArgument;
            public bool ReturnsValue;
        }

        int resultReturnIndexCounter = 0;
        Log log = Log.GetLogger(typeof(RPCDispatcher));

        Dictionary<int, RPCResult> results = new Dictionary<int, RPCResult>();
        Dictionary<string, Receiver> receivers = new Dictionary<string, Receiver>();

        Dictionary<Type, Reader> readers = new Dictionary<Type, Reader>();
        Dictionary<Type, Writer> writers = new Dictionary<Type, Writer>();

        public Context Context
        {
            get;
            private set;
        }
        
        internal RPCDispatcher(Context context)
        {
            Context = context;

            writers.Add(typeof(bool), (s, t, o) => s.WriteBool((bool)o));
            writers.Add(typeof(bool[]), (s, t, o) => s.WriteBoolArray((bool[])o));
            writers.Add(typeof(byte), (s, t, o) => s.WriteByte((byte)o));
            writers.Add(typeof(byte[]), (s, t, o) => s.WriteByteArray((byte[])o));
            writers.Add(typeof(sbyte), (s, t, o) => s.WriteSByte((sbyte)o));
            writers.Add(typeof(short), (s, t, o) => s.WriteShort((short)o));
            writers.Add(typeof(short[]), (s, t, o) => s.WriteShortArray((short[])o));
            writers.Add(typeof(ushort), (s, t, o) => s.WriteUShort((ushort)o));
            writers.Add(typeof(ushort[]), (s, t, o) => s.WriteUShortArray((ushort[])o));
            writers.Add(typeof(int), (s, t, o) => s.WriteInt((int)o));
            writers.Add(typeof(int[]), (s, t, o) => s.WriteIntArray((int[])o));
            writers.Add(typeof(long), (s, t, o) => s.WriteLong((long)o));
            writers.Add(typeof(long[]), (s, t, o) => s.WriteLongArray((long[])o));
            writers.Add(typeof(float), (s, t, o) => s.WriteSingle((float)o));
            writers.Add(typeof(float[]), (s, t, o) => s.WriteSingleArray((float[])o));
            writers.Add(typeof(double), (s, t, o) => s.WriteDouble((double)o));
            writers.Add(typeof(double[]), (s, t, o) => s.WriteDoubleArray((double[])o));
            writers.Add(typeof(string), (s, t, o) => s.WriteString((string)o));
            writers.Add(typeof(string[]), (s, t, o) => s.WriteStringArray((string[])o));
            writers.Add(typeof(Vector3), (s, t, o) => s.WriteVector3((Vector3)o));
            writers.Add(typeof(Vector3[]), (s, t, o) => s.WriteVector3Array((Vector3[])o));
            writers.Add(typeof(Quaternion), (s, t, o) => s.WriteQuaternion((Quaternion)o));
            writers.Add(typeof(Quaternion[]), (s, t, o) => s.WriteQuaternionArray((Quaternion[])o));
            writers.Add(typeof(Actor), (s, t, o) => s.WriteUShort(((Actor)o).Id));
            writers.Add(typeof(Player), (s, t, o) => s.WriteUShort(((Player)o).Id));
            writers.Add(typeof(Pair<,>), writePair);

            readers.Add(typeof(bool), (s, t) => s.ReadBool());
            readers.Add(typeof(bool[]), (s, t) => s.ReadBoolArray());
            readers.Add(typeof(byte), (s, t) => s.ReadByte());
            readers.Add(typeof(byte[]), (s, t) => s.ReadByteArray());
            readers.Add(typeof(sbyte), (s, t) => s.ReadSByte());
            readers.Add(typeof(short), (s, t) => s.ReadShort());
            readers.Add(typeof(short[]), (s, t) => s.ReadShortArray());
            readers.Add(typeof(ushort), (s, t) => s.ReadUShort());
            readers.Add(typeof(ushort[]), (s, t) => s.ReadUShortArray());
            readers.Add(typeof(int), (s, t) => s.ReadInt());
            readers.Add(typeof(int[]), (s, t) => s.ReadIntArray());
            readers.Add(typeof(long), (s, t) => s.ReadLong());
            readers.Add(typeof(long[]), (s, t) => s.ReadLongArray());
            readers.Add(typeof(float), (s, t) => s.ReadSingle());
            readers.Add(typeof(float[]), (s, t) => s.ReadSingleArray());
            readers.Add(typeof(double), (s, t) => s.ReadDouble());
            readers.Add(typeof(double[]), (s, t) => s.ReadDoubleArray());
            readers.Add(typeof(string), (s, t) => s.ReadString());
            readers.Add(typeof(string[]), (s, t) => s.ReadStringArray());
            readers.Add(typeof(Vector3), (s, t) => s.ReadVector3());
            readers.Add(typeof(Vector3[]), (s, t) => s.ReadVector3Array());
            readers.Add(typeof(Quaternion), (s, t) => s.ReadQuaternion());
            readers.Add(typeof(Quaternion[]), (s, t) => s.ReadQuaternionArray());
            readers.Add(typeof(Actor), readActor);
            readers.Add(typeof(Player), readPlayer);
            readers.Add(typeof(Pair<,>), readPair);

            initDispatchers();
            initReceivers();
        }

        public bool OnPacket(byte _, Context context, Network.ByteInStream stream)
        {
            if (stream.ReadBool())
            {
                return onCall(stream);
            }
            else
            {
                return onReturn(stream);
            }
        }

        internal RPCResult<T> CreateResult<T>()
        {
            RPCResult<T> result = new RPCResult<T>();
            result.ReturnIndex = ++resultReturnIndexCounter;
            return result;
        }

        internal void Invoke(IEnumerable<Player> players, RPCResult result, string name, params object[] args)
        {
            Network.ByteOutStream stream = new Network.ByteOutStream(256);

            stream.WriteByte(HeaderBytes.RemoteProcedureCall);
            stream.WriteBool(true);
            stream.WriteString(name);

            if (result != null)
            {
                stream.WriteInt(result.ReturnIndex);
                results.Add(result.ReturnIndex, result);
                result.Name = name;
            }

            foreach (object arg in args)
            {
                Writer writer;

                if (!writers.TryGetValue(arg.GetType(), out writer))
                {
                    log.Error("No RPC argument writer for type '{0}', can't send RPC '{1}'", arg.GetTypeName(), name);
                    return;
                }

                writer(stream, arg.GetType(), arg);
            }

            log.Info("Sending {0}", name);

            foreach (Player player in players)
            {
                player.Connection.Queue(stream, true);
            }
        }

        void writePair(Network.ByteOutStream s, Type t, object v)
        {
            object first = t.GetProperty("First").GetValue(v, null);
            object second = t.GetProperty("Second").GetValue(v, null);

            Writer firstW;
            Writer secondW;

            if (!writers.TryGetTypeValue(first.GetType(), out firstW) || !writers.TryGetTypeValue(second.GetType(), out secondW))
            {
                return;
            }

            firstW(s, first.GetType(), first);
            secondW(s, second.GetType(), second);
        }

        object readPair(Network.ByteInStream s, Type t)
        {
            Type[] tArgs = t.GetGenericArgs();

            Reader r;

            if (!readers.TryGetTypeValue(tArgs[0], out r))
            {
                return null;
            }

            object first = r(s, tArgs[0]);

            if (!readers.TryGetTypeValue(tArgs[1], out r))
            {
                return null;
            }

            object second = r(s, tArgs[1]);
            return Activator.CreateInstance(t, first, second);
        }

        object readPlayer(Network.ByteInStream s, Type t)
        {
            Player p;
            ushort id = s.ReadUShort();

            if (Context.GetPlayer(id, out p))
            {
                return p;
            }

            log.Warn("No player with id #{0} found", id);
            return null;
        }

        object readActor(Network.ByteInStream s, Type t)
        {
            Actor a;
            ushort id = s.ReadUShort();

            if (Context.GetActor(id, out a))
            {
                return a;
            }

            log.Warn("No actor with id #{0} found", id);
            return null;
        }

        void initDispatchers()
        {
            int createdDispatchers = 0;

            foreach (FieldInfo field in GetDispatcherFields())
            {
                Type type = field.FieldType;
                string id = field.GetPrettyName();

                field.SetValue(null, Activator.CreateInstance(type, this, id));

                ++createdDispatchers;
            }

            log.Info("Found {0} RPC methods", createdDispatchers);
        }

        void initReceivers()
        {
            List<MethodInfo> methods = TypeUtils2.GetMethodsWithAttributes(typeof(RPCAttribute));

            foreach (MethodInfo method in methods)
            {
                if (!method.IsStatic)
                {
                    log.Error("The method '{0}' is not static, and can't be used as an RPC", method.GetPrettyName());
                    continue;
                }

                initReceiver(null, method);
            }

            log.Info("Found {0} RPC receivers", receivers.Count);
        }

        void initReceiver(object instance, MethodInfo method)
        {
            Receiver receiver = new Receiver();

            RPCAttribute attribute = method.GetAttribute<RPCAttribute>();
            Assert.NotNull(attribute, "attribute");

            FieldInfo field = attribute.Type.GetField(method.Name, BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(field, "field");

            bool isFunc = field.FieldType.Name.StartsWith("RPCFunc");

            Type[] generics = field.FieldType.GetGenericArgs();
            Type[] parameters = method.GetParameters().Select(x => x.ParameterType).ToArray();

            if (isFunc)
            {
                generics = generics.Take(generics.Length - 1).ToArray();
            }

            if (generics.Length > parameters.Length || generics.Length < parameters.Length)
            {
                int lastParam = parameters.Length - 1;
                if (lastParam >= 0 && generics.Length == lastParam && ReferenceEquals(parameters[lastParam], typeof(RPCInfo)))
                {
                    receiver.RequiresInfoArgument = true;
                    parameters = parameters.Take(generics.Length).ToArray();
                }
                else
                {
                    log.Error("The parameter count did not match between the field '{0}' and the method '{0}'", field.GetPrettyName(), method.GetPrettyName());
                    return;
                }
            }

            for (int i = 0; i < generics.Length; ++i)
            {
                if (!ReferenceEquals(generics[i], parameters[i]))
                {
                    log.Error("The type of argument #{0} does not match between the RPC '{1}' and the Method '{2}'", i + 1, field.GetPrettyName(), method.GetPrettyName());
                    return;
                }
            }

            receiver.Name = field.GetPrettyName();
            receiver.Parameters = parameters;
            receiver.Instance = instance;
            receiver.Method = method;
            receiver.ReturnsValue = isFunc;

            receivers.Add(receiver.Name, receiver);
        }

        bool onCall(Network.ByteInStream stream)
        {
            string name = stream.ReadString();

            Receiver receiver;

            if (!receivers.TryGetValue(name, out receiver))
            {
                log.Error("No receiver for '{0}' found", name);
                return false;
            }

            int returnIndex = 0;
            int length = receiver.Parameters.Length;

            if (receiver.ReturnsValue)
            {
                returnIndex = stream.ReadInt();
            }

            if (receiver.RequiresInfoArgument)
            {
                length += 1;
            }

            object[] args = new object[length];

            for (var i = 0; i < receiver.Parameters.Length; ++i)
            {
                Type t = receiver.Parameters[i];
                Reader reader;

                if (!readers.TryGetTypeValue(t, out reader))
                {
                    return false;
                }

                args[i] = reader(stream, t);

                if (args[i] is Player)
                {
                    if (!ReferenceEquals(args[i], stream.Player))
                    {
                        log.Error("RPC from {0} tried to use object for {1}", stream.Player, args[i]);
                        return false;
                    }
                }
            }

            if (receiver.RequiresInfoArgument)
            {
                RPCInfo info = new RPCInfo();

                info.Caller = stream.Player;
                info.Context = Context;
                info.RemoteGameTime = stream.RemoteGameTime;

                args[args.Length - 1] = info;
            }

            log.Info("Received {0}", receiver.Name);

            if (receiver.ReturnsValue)
            {
                object result = receiver.Method.Invoke(receiver.Instance, args);
                Network.ByteOutStream resultStream = new Network.ByteOutStream(128);

                resultStream.WriteByte(HeaderBytes.RemoteProcedureCall);
                resultStream.WriteBool(false);
                resultStream.WriteInt(returnIndex);

                if (result == null)
                {
                    resultStream.WriteBool(false);
                }
                else
                {
                    Writer writer;
                    Type t = result.GetType();

                    resultStream.WriteBool(true);

                    if (writers.TryGetTypeValue(t, out writer))
                    {
                        writer(resultStream, t, result);
                    }
                }

                stream.Player.Connection.Queue(resultStream, true);
            }
            else
            {
                receiver.Method.Invoke(receiver.Instance, args);
            }

            return true;
        }

        bool onReturn(Network.ByteInStream stream)
        {
            int returnId = stream.ReadInt();

            RPCResult result;

            if (!results.TryGetValue(returnId, out result))
            {
                log.Error("No RPC result with the return index #{0} pending", returnId);
                return false;
            }

            if (stream.ReadBool())
            {
                Reader reader;

                if (!readers.TryGetTypeValue(result.Type, out reader))
                {
                    return false;
                }

                result.BoxedValue = reader(stream, result.Type);
            }

            log.Info("Received result for {0}", result.Name);

            result.IsDone = true;
            result.Done();

            results.Remove(result.ReturnIndex);

            return true;
        }
    }
}
