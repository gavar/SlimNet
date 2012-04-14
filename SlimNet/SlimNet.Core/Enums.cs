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

namespace SlimNet
{
    [Flags]
    public enum EventTargets : byte
    {
        None = 0,
        Server = 1,
        Owner = 2,
        Remotes = 4,
        Everyone = 7
    }

    [Flags]
    public enum EventSources : byte
    {
        None = 0,
        Remotes = 1,
        Owner = 2,
        Everyone = 3
    }

    public enum ProximityLevel : byte
    {
        None = 0,
        World = 1,
        Zone = 2,
        Horizon = 3,
        Far = 4,
        Medium = 5,
        Near = 6,
        Melee = 7,
    }

    public enum TransformSource : byte
    {
        SlimNet,
        Engine
    }

    public enum ServerMode : byte
    {
        Standalone,
        Integrated
    }

    public enum ActorRole : byte
    {
        Authority, // Every actor on the server
        Autonom, // Actors when on their owning client
        Simulated // Actors when simulated on a remote client
    }

    [Flags]
    public enum LogLevel : byte
    {
        None = 0,
        Trace = 1,
        Info = 2,
        Debug = 4,
        Warn = 8,
        Error = 16,
        All = 31
    }

    public enum TimeManagerState : byte
    {
        Running,
        Paused,
        Stopped
    }
}
