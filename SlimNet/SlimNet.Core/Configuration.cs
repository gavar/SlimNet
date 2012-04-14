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

namespace SlimNet
{
    public partial class Configuration
    {
        public string GameName = "MyGame";
        public int SimulationAccuracy = 100;
        public LogLevel LogLevel = LogLevel.All;
        public float LidgrenSimulatedLoss = 0.0f;
        public float LidgrenSimulatedLatency = 0.0f;
        public float LidgrenSimulatedRandomLatency = 0.0f;
    }
}
