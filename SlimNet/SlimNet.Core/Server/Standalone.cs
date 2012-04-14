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
using System.IO;
using System.Reflection;
using System.Threading;

#if WIN && !UNITY_WEBPLAYER
using SlimNet.Utils;
#endif

namespace SlimNet
{
    public class StandaloneServer : Server
    {
        static Log log = Log.GetLogger(typeof(StandaloneServer));

        AutoResetEvent timerEvent;

        public static StandaloneServer Create()
        {
            return Create(new ServerConfiguration());
        }

        public static StandaloneServer Create(ServerConfiguration configuration)
        {
            return new StandaloneServer(configuration);
        }

        StandaloneServer(ServerConfiguration configuration)
            : base(configuration)
        {
            timerEvent = new AutoResetEvent(false);
        }

        protected override void LoadAssemblies()
        {
            try
            {
                log.Info("Loading user assemblies from {0}{1}", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);

                if (File.Exists(ServerConfiguration.CombinedAssemblyPath))
                {
                    Assembly.LoadFrom(ServerConfiguration.CombinedAssemblyPath);
                }
                else
                {
                    Assembly.LoadFrom(ServerConfiguration.SharedAssemblyPath);
                    Assembly.LoadFrom(ServerConfiguration.ServerAssemblyPath);
                }
            }
            catch (Exception exn)
            {
                log.Error(exn);

                // We have to throw an exception here, since we can't continue
                throw new RuntimeException("Can't initialize server, user assemblies failed to load.");
            }
        }

        public override void Start()
        {
            log.Info("Starting server on *:{0}", ServerConfiguration.Port);

            // Start server listener
            NetworkServer.Listen();

            // Start time manager
            Context.Time.Start();

            // Start the event timer
            startTimer();

            // Setup event array
            AutoResetEvent[] resetEvents = new AutoResetEvent[2];
            resetEvents[0] = timerEvent;
            resetEvents[1] = NetworkPeer.MessageReceivedEvent;

            // Send buffer timer
            long lastSendTime = 0;
            long sendBuffering = Math.Min(ServerConfiguration.SendBuffering, ServerConfiguration.SimulationAccuracy / 2L);

            while (true)
            {
                // Wait for any event
                switch (WaitHandle.WaitAny(resetEvents))
                {
                    case 0:
                        
                        Context.Time.Update();
                        Context.Scheduler.RunExpired();

                        goto case 1;

                    case 1:

                        // Reset send time
                        lastSendTime = Context.Time.ElapsedMilliseconds;

                        // Try fixed update
                        Context.Simulate();

                        // Receive client input
                        while (NetworkServer.ReceiveOne())
                        {
                            // Try fixed update
                            Context.Simulate();

                            // Check if we should send 
                            if (Context.Time.ElapsedMilliseconds - lastSendTime > sendBuffering)
                            {
                                if (Context.Send())
                                {
                                    lastSendTime = Context.Time.ElapsedMilliseconds;
                                }
                            }
                        }

                        // Send data
                        Context.Send();
                        break;
                }
            }
        }
#if WIN && !UNITY_WEBPLAYER
        void startTimer()
        {
            TimerQueueTimer.Create(0, (uint)ServerConfiguration.TimerAccuracy, new TimerQueueTimer.WaitOrTimerDelegate(eventSignaled));
        }

        void eventSignaled(IntPtr p, bool s)
        {
            timerEvent.Set();
        }
#else
        Timer threadTimer;

        void startTimer()
        {
            threadTimer = new Timer(new TimerCallback(timerSignaled), null, 0, ServerConfiguration.TimerAccuracy);
        }

        void timerSignaled(object _)
        {
            timerEvent.Set();
        }
#endif
    }
}
