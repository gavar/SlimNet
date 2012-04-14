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
public static class SlimNetProjectSynchronizer
{
    static string path;
    static string sourcePath;
    static string assemblyPath;
    static string sharedPath;
    static string projectPath;
    static string[] files;
    static Regex guidRegex;
    static Regex solutionGuidRegex;
    static Regex fileRegex;
    static Regex referenceRegex;
    static DateTime[] mtimes;
    static DateTime checkTime = DateTime.MinValue;

    static SlimNetProjectSynchronizer()
    {
        files =
            new string[] { 
                "Assembly-CSharp.csproj", 
                "Assembly-CSharp-vs.csproj", 
                Path.GetFileName(Directory.GetCurrentDirectory()) + ".sln",
                Path.GetFileName(Directory.GetCurrentDirectory()) + "-csharp.sln",
            };

        mtimes = new DateTime[files.Length];
        checkTime = DateTime.Now;
        guidRegex = new Regex("<ProjectGuid>{(.*)}</ProjectGuid>");
        solutionGuidRegex = new Regex("Project\\(\"(.*)\"\\) =");
        fileRegex = new Regex("<Compile Include=\"(.+)\"");
        referenceRegex = new Regex("<Reference Include=\"(.+)\"");

        EditorApplication.update += onUpdate;
    }

    static IEnumerable<string> getFiles(string dir, string extension)
    {
        if (Directory.Exists(dir))
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(extension))
                {
                    yield return file.Replace(path + Path.DirectorySeparatorChar, "");
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

    static string makeAssemblyReference(string f)
    {
        return String.Format(
            "<Reference Include=\"{1}\"><HintPath>Server{0}Assemblies{0}{1}</HintPath></Reference>",
            Path.DirectorySeparatorChar,
            Path.GetFileName(f)
        );
    }

    static string makeCompilerInclude(string f)
    {
        return String.Format("<Compile Include=\"{0}\" />", f);
    }

    static void updateSolutionFile(string f, string guid)
    {
        if (File.Exists(f))
        {
            string contents = File.ReadAllText(f);

            if (!contents.Contains(guid))
            {
                string projectName = Path.GetFileName(path);
                string solutionGuid = solutionGuidRegex.Match(contents).Groups[1].Value;
                string projectRow = String.Format("Project(\"{0}\") = \"{1}\", \"SlimNet-Server.csproj\", \"{2}\"\nEndProject", solutionGuid, projectName, "{" + guid + "}");

                contents = contents.Replace("# Visual Studio 2008" + Environment.NewLine, "# Visual Studio 2008" + Environment.NewLine + Environment.NewLine + projectRow);

                string configRow = "	GlobalSection(ProjectConfigurationPlatforms) = postSolution\n		{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n		{0}.Debug|Any CPU.Build.0 = Debug|Any CPU\n		{0}.Release|Any CPU.ActiveCfg = Release|Any CPU\n		{0}.Release|Any CPU.Build.0 = Release|Any CPU";
                configRow = String.Format(configRow, "{" + guid + "}");

                contents = contents.Replace("	GlobalSection(ProjectConfigurationPlatforms) = postSolution", configRow);

                File.WriteAllText(f, contents);
            }
        }
    }

    static string projectFiles
    {
        get
        {
            string[] sourceFiles =
                getFiles(sourcePath, ".cs")
                    .Concat(getFiles(sharedPath, ".cs"))
                    .Select<string, string>(makeCompilerInclude)
                    .ToArray();

            return "<ItemGroup>\n" + String.Join("\n", sourceFiles) + "\n</ItemGroup>";
        }
    }

    static string assemblyReferences
    {
        get
        {
            string[] files = 
                getFiles(assemblyPath, ".dll")
                .Select<string, string>(makeAssemblyReference)
                .ToArray();

            return String.Join("\n", files);
        }
    }

    static bool compareCurrentToNew(string newProjectFile)
    {
        if (!File.Exists(projectPath))
        {
            return false;
        }

        try
        {
            string oldProjectFile = File.ReadAllText(projectPath);

            HashSet<string> oldContents = new HashSet<string>();
            HashSet<string> newContents = new HashSet<string>();

            oldContents.UnionWith(fileRegex.Matches(oldProjectFile).Cast<Match>().Select(x => x.Groups[1].Value));
            newContents.UnionWith(fileRegex.Matches(newProjectFile).Cast<Match>().Select(x => x.Groups[1].Value));

            if (oldContents.SetEquals(newContents))
            {
                oldContents.Clear();
                newContents.Clear();

                oldContents.UnionWith(referenceRegex.Matches(oldProjectFile).Cast<Match>().Select(x => x.Groups[1].Value));
                newContents.UnionWith(referenceRegex.Matches(newProjectFile).Cast<Match>().Select(x => x.Groups[1].Value));

                return oldContents.SetEquals(newContents);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    static void onUpdate()
    {
        // Once a second only
        if (checkTime.AddSeconds(1) < DateTime.Now)
        {
            path = Path.GetDirectoryName(Application.dataPath).TrimEnd('/').Replace('/', Path.DirectorySeparatorChar);
            sourcePath = String.Format("{1}{0}Server", Path.DirectorySeparatorChar, path);
            assemblyPath = String.Format("{1}{0}Server{0}Assemblies", Path.DirectorySeparatorChar, path);
            sharedPath = String.Format("{1}{0}Assets{0}SlimNet{0}Shared", Path.DirectorySeparatorChar, path);
            projectPath = String.Format("{1}{0}SlimNet-Server.csproj", Path.DirectorySeparatorChar, path);

            try
            {
                bool updated = false;

                for (int i = 0; i < files.Length; ++i)
                {
                    string filePath = path + Path.DirectorySeparatorChar + files[i];

                    if (File.Exists(filePath))
                    {
                        if (!updated && File.GetLastWriteTime(filePath) > mtimes[i])
                        {
                            TextAsset projectFile = AssetDatabase.LoadAssetAtPath("Assets/SlimNet/Resources/ServerProjectFile.txt", typeof(TextAsset)) as TextAsset;

                            if (projectFile == null)
                            {
                                Debug.Log("[SlimNet] Could not find 'Assets/SlimNet/Resources/ServerProjectFile.txt'");
                                break;
                            }

                            string finalProjectFile = 
                                projectFile.text
                                    .Replace("<!--SOURCE_FILES-->", projectFiles)
                                    .Replace("<!--ASSEMBLY_REFERENCES-->", assemblyReferences);

                            if (!compareCurrentToNew(finalProjectFile))
                            {
                                File.WriteAllText(projectPath, finalProjectFile);
                            }

                            string guid = guidRegex.Match(finalProjectFile).Groups[1].Value;

                            updateSolutionFile(files[2], guid);
                            updateSolutionFile(files[3], guid);

                            updated = true;
                        }

                        mtimes[i] = File.GetLastWriteTime(filePath);
                        break;
                    }
                }
            }
            finally
            {
                checkTime = DateTime.Now;
            }
        }
    }
}