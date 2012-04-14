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
using System.Diagnostics;

namespace SlimNet
{
    public class TimeManager
    {
        static readonly Log log = Log.GetLogger(typeof(TimeManager));

        public const long MIN_STEP_SIZE = 15L;
        public const long MAX_STEP_SIZE = 1000L;

        float offset = 0f;
        float minRemoteTime = 0f;
        float previousStepTime = 0f;
        long fixedSimulationTime = 0L;

        readonly Peer peer;
        readonly object syncObject = new object();
        readonly Stopwatch timer = new Stopwatch();

        public float GameTime { get; private set; }
        public float LocalTime { get; private set; }
        public TimeManagerState State { get; private set; }
        public long SimulationStepSize { get; private set; }

        public float DeltaTimeFixed { get; private set; }
        public float DeltaTimeVariable { get; private set; }

        public long ElapsedMilliseconds { get { return timer.ElapsedMilliseconds; } }
        public float RealTime { get { return (float)((double)ElapsedMilliseconds / 1000.0); } }
        public bool ShouldStep { get { return (ElapsedMilliseconds - fixedSimulationTime) >= SimulationStepSize; } }

        internal protected TimeManager(Peer peer)
        {
            Assert.NotNull(peer, "peer");

            this.peer = peer;

            SimulationStepSize = peer.Configuration.SimulationAccuracy;
            SimulationStepSize = Math.Min(MAX_STEP_SIZE, SimulationStepSize);
            SimulationStepSize = Math.Max(MIN_STEP_SIZE, SimulationStepSize);

            DeltaTimeFixed = (float)SimulationStepSize / 1000.0f;

            State = TimeManagerState.Stopped;
        }

        public void Update()
        {
            LocalTime = RealTime;
            GameTime = LocalTime + offset;
        }

        public void Stop()
        {
            lock (syncObject)
            {
                if (State != TimeManagerState.Stopped)
                {
                    timer.Stop();
                    timer.Reset();

                    State = TimeManagerState.Stopped;

                    Update();

                    log.Info("Stopped");
                }
            }
        }

        public void Pause()
        {
            lock (syncObject)
            {
                if (State == TimeManagerState.Running)
                {
                    timer.Stop();

                    Update();

                    State = TimeManagerState.Paused;
                    log.Info("Paused");
                }
            }
        }

        public void Start()
        {
            lock (syncObject)
            {
                if(State == TimeManagerState.Stopped)
                {
                    timer.Reset();
                    timer.Start();

                    Update();

                    State = TimeManagerState.Running;
                    log.Info("Started with {0}ms tick accuracy", SimulationStepSize);
                }
                else if(State == TimeManagerState.Paused)
                {
                    timer.Start();

                    Update();

                    State = TimeManagerState.Running;
                    log.Info("Resumed");
                }
            }
        }

        internal void Step()
        {
            fixedSimulationTime += SimulationStepSize;
        }

        internal void UpdateOffset(float remoteTime)
        {
            if (remoteTime >= minRemoteTime)
            {
                minRemoteTime = remoteTime;

                float newOffset = remoteTime - LocalTime;

                if (offset == 0.0f)
                {
                    offset = newOffset;
                }
                else
                {
                    offset = (offset * 0.95f) + (newOffset * 0.05f);
                }

                Update();
            }
        }

        internal void UpdateDelta()
        {
            if (previousStepTime > 0)
            {
                DeltaTimeVariable = LocalTime - previousStepTime;
            }

            previousStepTime = LocalTime;
        }
    }
}
