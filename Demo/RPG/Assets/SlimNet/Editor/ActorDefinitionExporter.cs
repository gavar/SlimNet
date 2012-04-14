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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SlimNetActorDefinitionExporter
{
    static DateTime lastCheck;
    static Regex autoGenRegex;

    static SlimNetActorDefinitionExporter()
    {
        EditorApplication.update += onUpdate;

        lastCheck = DateTime.Now;
        autoGenRegex = new Regex("//\\{AutoGen\\}(.*)//\\{AutoGen\\}", RegexOptions.Singleline);
    }


    static IEnumerable<string> getFiles(string dir, string extension)
    {
        if (Directory.Exists(dir))
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(extension))
                {
                    yield return file;
                }
            }

            foreach (string subDir in Directory.GetDirectories(dir))
            {
                foreach (string file in getFiles(subDir, extension))
                {
                    yield return file;
                }
            }
        }
    }

    static void onUpdate()
    {
        if (lastCheck.AddSeconds(5) < DateTime.Now)
        {
            string[] files = getFiles("Assets/SlimNet/Shared", ".cs").Where(x => x.EndsWith("ActorDefinition.Gen.cs")).ToArray();
            string[] resources = files.Select(x => Path.GetFileName(x.Replace("ActorDefinition.Gen.cs", ""))).ToArray();
            string[] contents = files.Select<string, string>(File.ReadAllText).ToArray();

            for (int i = 0; i < resources.Length; ++i)
            {
                UnityEngine.Object prefab = Resources.Load(resources[i], typeof(GameObject));

                if (prefab != null)
                {
                    GameObject instance = GameObject.Instantiate(prefab) as GameObject;

                    string collider = exportCollider(instance);
                    string content = autoGenRegex.Replace(contents[i], "//{AutoGen}\r\n    " + collider + "\r\n    //{AutoGen}");

                    if(content != contents[i])
                    {
                        File.WriteAllText(files[i], content);
                    }

                    GameObject.DestroyImmediate(instance);
                }
            }

            lastCheck = DateTime.Now;
        }
    }

    static string exportCollider(GameObject resource)
    {
        UnityEngine.Collider c = resource.GetComponent<UnityEngine.Collider>();
        UnityEngine.SphereCollider sc = resource.GetComponent<UnityEngine.SphereCollider>();

        if (sc != null)
        {
            return String.Format(
                "public override Collider Collider {{ get {{ return new SphereCollider(Vector3.Zero, new Vector3({0}f, {1}f, {2}f), {3}f); }} }}",
                    c.bounds.center.x, c.bounds.center.y, c.bounds.center.z, sc.radius
                );
        }
        else if(c != null)
        {
            return String.Format(
                "public override Collider Collider {{ get {{ return new BoxCollider(Vector3.Zero, new Vector3({0}f, {1}f, {2}f), new Vector3({3}f, {4}f, {5}f)); }} }}",
                c.bounds.extents.x, c.bounds.extents.y, c.bounds.extents.z, c.bounds.center.x, c.bounds.center.y, c.bounds.center.z
            );
        }

        return "";
    }
}