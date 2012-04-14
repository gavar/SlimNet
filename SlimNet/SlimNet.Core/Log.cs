/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This software is provided 'as-is', without any express or implied
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
 * You are free to share, copy and distribute the software in it's original, 
 * unmodified form. You are not allowed to distribute or make publicly 
 * available the software itself or it's sources in any modified manner. 
 * This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Diagnostics;

namespace SlimNet
{
    public class LogConsoleAdapter : ILogAdapter
    {
        public void Log(Log.LogEvent @event)
        {
#if WIN
            ConsoleColor color = Console.ForegroundColor;

            switch (@event.Level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
#endif
            Console.WriteLine(
                String.Format("{0} {1} {2} - {3}", 
                (long)@event.Time.TotalMilliseconds, 
                @event.Level.ToString().ToUpper().PadRight(5), 
                @event.Log.Name, 
                @event.Message)
            );
#if WIN
            Console.ForegroundColor = color;
#endif
        }
    }

    public class LogNullAdapter : ILogAdapter
    {
        public void Log(Log.LogEvent @event)
        {

        }
    }

    public class Log
    {
        public class LogEvent
        {
            public Log Log;
            public string Message;
            public LogLevel Level;
            public TimeSpan Time;
        }

        #region Static

        static Stopwatch timer = new Stopwatch();
        static ILogAdapter adapter;
        static LogLevel level = LogLevel.All;
        static readonly object syncRoot = new object();

        public static event Action NoAdapterSet;
        public static bool AttachDebuggerOnError = false;

        public static Log GetLogger(string name)
        {
            if (!timer.IsRunning)
            {
                timer.Start();
            }

            return new Log(name);
        }

        public static Log GetLogger(Type type)
        {
            return GetLogger(Utils.TypeUtils2.GetPrettyName(type));
        }

        public static void SetAdapter(ILogAdapter adapter)
        {
            Assert.NotNull(adapter, "adapter");
            Log.adapter = adapter;
        }

        public static void SetLevel(LogLevel level)
        {
            Log.level = level;
            GetLogger(typeof(Log)).log(LogLevel.None, "Logging level set to {0}", level);
        }

        #endregion

        public string Name { get; private set; }

        Log(string name)
        {
            Name = name;
        }

        public void Trace(object message)
        {
            message = message ?? "<Null>";
            Trace(message.ToString());
        }

        public void Trace(string message, params object[] args)
        {
            log(LogLevel.Trace, message, args);
        }

        public void Info(object message)
        {
            message = message ?? "<Null>";
            Info(message.ToString());
        }

        public void Info(string message, params object[] args)
        {
            log(LogLevel.Info, message, args);
        }

        public void Debug(object message)
        {
            message = message ?? "<Null>";
            Debug(message.ToString());
        }

        public void Debug(string message, params object[] args)
        {
            log(LogLevel.Debug, message, args);
        }

        public void Warn(object message)
        {
            message = message ?? "<Null>";
            Warn(message.ToString());
        }

        public void Warn(string message, params object[] args)
        {
            log(LogLevel.Warn, message, args);
        }

        public void Error(object message)
        {
            message = message ?? "<Null>";
            Error(message.ToString());
        }

        public void Error(string message, params object[] args)
        {
#if DEBUG
            message = message + " \n" + Environment.StackTrace;
#endif

            log(LogLevel.Error, message, args);

#if WIN && DEBUG
            if (AttachDebuggerOnError && !Debugger.IsAttached)
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                        Debugger.Launch();
                        break;
                }
            }
#endif
        }

        public void Error(Exception exn)
        {
            if (exn != null)
            {
                exn = exn.GetBaseException();
                Error("{0}: {1}\r\n{2}", exn.GetType().Name, exn.Message, exn.StackTrace);
            }
        }

        public void Message(LogLevel level, string message, params object[] args)
        {
            log(level, message, args);
        }

        void log(LogLevel level, string message, params object[] args)
        {
            if (adapter != null)
            {
                if ((Log.level & level) == level)
                {
                    lock (syncRoot)
                    {
                        adapter.Log(
                            new LogEvent
                            {
                                Level = level,
                                Log = this,
                                Message = String.Format(message, args),
                                Time = timer.Elapsed
                            }
                        );
                    }
                }
            }
            else
            {
                if (NoAdapterSet != null)
                {
                    lock (syncRoot)
                    {
                        if (adapter != null)
                        {
                            NoAdapterSet.Invoke();
                        }
                    }
                }

                if (adapter != null)
                {
                    log(level, message, args);
                    return;
                }

                throw new RuntimeException("No logging adapter set, you need to call Log.SetAdapter");
            }
        }
    }
}
