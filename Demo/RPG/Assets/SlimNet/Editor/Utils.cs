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

using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;

namespace SlimNet.Unity.Editor
{
    public class Utils
    {
        public static void WriteTextAsset(string path, string data)
        {
            File.WriteAllText(path, data);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static string Serialize<T>(T v)
        {
            StringBuilder o = new StringBuilder();
            StringWriter w = new StringWriter(o);
            XmlSerializer s = new XmlSerializer(typeof(T));

            s.Serialize(w, v);
            w.Flush();

            return o.ToString();
        }
    }
}
