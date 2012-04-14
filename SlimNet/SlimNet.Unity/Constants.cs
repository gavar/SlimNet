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

namespace SlimNet.Unity
{
    public static class Constants
    {
        public static readonly string ClientConfigAssetPath = 
            String.Format("Assets{0}SlimNet{0}Resources{0}{1}", Path.DirectorySeparatorChar, SlimNet.Constants.ClientConfigNameXml);

        public static readonly string ServerConfigAssetPath =
            String.Format("Assets{0}SlimNet{0}Resources{0}{1}", Path.DirectorySeparatorChar, SlimNet.Constants.ServerConfigNameXml);
    }
}