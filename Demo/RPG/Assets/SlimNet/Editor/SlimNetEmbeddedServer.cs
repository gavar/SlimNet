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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using SlimNet.Unity;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SlimNetEmbeddedServer : EditorWindow
{
    static Process process = null;
    static SlimNetEmbeddedServer window = null;
    static string lastOutput = "";

    static bool isPlaying = false;
    static bool embeddedServerRunning = false;
    static StringBuilder sb = new StringBuilder(1024 * 1024);

    static SlimNetEmbeddedServer()
    {
        EditorApplication.playmodeStateChanged += playModeChanged;
    }

    static void playModeChanged()
    {
        if (EditorApplication.isPlaying && !isPlaying)
        {
            isPlaying = true;

            if (SlimNetEditorSettings.Instance.StartEmbeddedServerOnPlay && SlimNetConfigurationTab.ServerConfiguratin.ServerMode == SlimNet.ServerMode.Standalone)
            {
                onStart();
            }
        }
        else if (isPlaying)
        {
            isPlaying = false;

            if (SlimNetConfigurationTab.ServerConfiguratin.ServerMode == SlimNet.ServerMode.Standalone)
            {
                onStop();
            }
        }
    }

    static void onStop()
    {
        if (process != null)
        {
            UnityEngine.Debug.Log("[SlimNet] Stopping Embedded Server");

            try
            {
                process.Kill();
                process = null;
            }
            catch
            {

            }
        }
    }

    [MenuItem("SlimNet/Embedded Server")]
    static void Init()
    {
        window = (SlimNetEmbeddedServer)EditorWindow.GetWindow(typeof(SlimNetEmbeddedServer));
        window.title = "Server";
    }

    static IEnumerable<string> getFiles(string dir, string extension)
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

    static string compileFileList
    {
        get
        {
            string sharedPath = String.Format("{0}{1}Assets{1}SlimNet{1}Shared", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
            string serverPath = String.Format("{0}{1}Server{1}Source", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
            return String.Join(" ", getFiles(sharedPath, ".cs").Concat(getFiles(serverPath, ".cs")).Select(x => '"' + x + '"').ToArray());
        }
    }

    static string serverTargetDirectory
    {
        get
        {
            return String.Format("{0}{1}Server", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
        }
    }

    static string serverTargetAssemblyDirectory
    {
        get
        {
            return String.Format("{0}{1}Assemblies", serverTargetDirectory, Path.DirectorySeparatorChar);
        }
    }

    static string cscPath
    {
        get
        {
            // Use 64 bit compiler if available
            if (File.Exists(@"C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe"))
            {
                return @"C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe";
            }

            return @"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe";
        }
    }

    static string serverAssemblyPath
    {
        get
        {
            return String.Format("{0}{1}{2}", serverTargetDirectory, Path.DirectorySeparatorChar, SlimNet.Constants.CombinedAssemblyName);
        }
    }

    static string serverSlimNetPath
    {
        get
        {
            return String.Format("{0}{1}SlimNet.dll", serverTargetDirectory, Path.DirectorySeparatorChar);
        }
    }

    static string serverSlimNetPdbPath
    {
        get
        {
            return serverSlimNetPath.Replace(".dll", ".pdb");
        }
    }

    static string serverMdbPath
    {
        get
        {
            return serverAssemblyPath + ".mdb";
        }
    }
     
    static string serverPdbPath
    {
        get
        {
            return serverAssemblyPath.Replace(".dll", ".pdb");
        }
    }

    static bool isOSX
    {
        get
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }

    static bool isWIN
    {
        get
        {
            return !isOSX;
        }
    }

    static string monoPath
    {
        get
        {
            if (isOSX)
            {
                return "mono";
            }
            else
            {
                return @"C:\Program Files (x86)\Unity\Editor\Data\Mono\bin\mono.exe";
            }
        }
    }

    static string osPreProcessorDefine
    {
        get
        {
            if (isOSX)
            {
                return "SLIMNET_OSX";
            }
            else
            {
                return "SLIMNET_WIN";
            }
        }
    }

    static string gmcsPath
    {
        get
        {
            if (isOSX)
            {
                return "/Applications/Unity/Unity.app/Contents/Frameworks/MonoBleedingEdge/lib/mono/2.0/gmcs.exe";
            }
            else
            {
                return '"' + @"C:\Program Files (x86)\Unity\Editor\Data\Mono\lib\mono\2.0\gmcs.exe" + '"';
            }
        }
    }

    static string slimNetAssemblyPath
    {
        get
        {
            return String.Format("{0}{1}Assets{1}SlimNet{1}Assemblies{1}SlimNet.dll", Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar);
        }
    }

    static string slimNetPdbPath
    {
        get
        {
            return slimNetAssemblyPath.Replace(".dll", ".pdb");
        }
    }

    static string slimNetMdbPath
    {
        get
        {
            return slimNetAssemblyPath.Replace(".dll", ".mdb");
        }
    }

    static string slimNetConsoleHostPath
    {
        get
        {
            /*
            if (isOSX)
            {

                return "$PATH$/SlimNet.ConsoleHost.exe";
            }
            else
            {*/
			
            return String.Format("{0}{1}SlimNet.ConsoleHost.exe", SlimNetEditorSettings.Instance.BinaryPath, Path.DirectorySeparatorChar);

            //}
        }
    }

    static string serverDebuggerCommand
    {
        get
        {
            if (isOSX || !SlimNetEditorSettings.Instance.UseVisualStudioCompiler)
            {
                return "";
            }

            return (SlimNetEditorSettings.Instance.AttachDebuggerToEmbeddedServer ? "-d" : "") + (SlimNetEditorSettings.Instance.AttachDebuggerOnError ? " -r" : "");
        }
    }

    static string references
    {
        get
        {
            string cmdOp = SlimNetEditorSettings.Instance.UseVisualStudioCompiler ? "/" : "-";
            string[] files =
                getFiles("Server" + Path.DirectorySeparatorChar + "Assemblies", ".dll")
                .Select(x => cmdOp + "reference:" + x)
                .ToArray();

            return String.Join(" ", files);
        }
    }

    static void onStart()
    {
        if (process == null)
        {
            sb.Remove(0, sb.Length);

            UnityEngine.Debug.Log("[SlimNet] Compiling Server Code");

            if (!Directory.Exists(serverTargetDirectory))
            {
                try
                {
                    Directory.CreateDirectory(serverTargetDirectory);
                }
                catch (Exception exn)
                {
                    UnityEngine.Debug.Log("Failed creating directory " + serverTargetDirectory + ": '" + exn.Message + "'");
                    return;
                }
            }

            if (!Directory.Exists(serverTargetAssemblyDirectory))
            {
                try
                {
                    Directory.CreateDirectory(serverTargetAssemblyDirectory);
                }
                catch (Exception exn)
                {
                    UnityEngine.Debug.Log("Failed creating directory " + serverTargetAssemblyDirectory + ": '" + exn.Message + "'");
                    return;
                }
            }

            // Make sure we delete the current file
            File.Delete(serverMdbPath);
            File.Delete(serverPdbPath);
            File.Delete(serverAssemblyPath);

            // Arguments to the compiler
            string compilerPath = SlimNetEditorSettings.Instance.UseVisualStudioCompiler ? cscPath : monoPath;
            string arguments = 
                String.Format(
                    SlimNetEditorSettings.Instance.UseVisualStudioCompiler ? 
                        "/out:{1} /reference:{2} {5} /define:{3} /debug+ /target:library /platform:anycpu {4}" :
                        "{0} -out:{1} -reference:{2} {5} -define:{3} -debug+ -platform:anycpu -target:library {4}", 
                    gmcsPath, 
                    serverAssemblyPath, 
                    slimNetAssemblyPath, 
                    osPreProcessorDefine,
                    compileFileList,
                    references
                );

            // Print compiler command line arguments
            sb.AppendLine(compilerPath + " " + arguments + Environment.NewLine);

            // Start process
            startProcess(compilerPath, arguments, true);
        }
    }

    static Process startProcess(string path, string args, bool startServerAfter)
    {
        Process p = new Process();

        p.StartInfo.FileName = path;
        p.StartInfo.Arguments = args;

        p.EnableRaisingEvents = true;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.RedirectStandardOutput = true;

        p.ErrorDataReceived += process_OutputDataReceived;
        p.OutputDataReceived += process_OutputDataReceived;

        p.Start();
        p.BeginErrorReadLine();
        p.BeginOutputReadLine();

        if (startServerAfter)
        {
            p.Exited += new EventHandler(compilerDone);
        }
        else
        {
            p.Exited += new EventHandler(serverDone);
        }

        return p;
    }

    static void serverDone(object sender, EventArgs e)
    {
        embeddedServerRunning = false;
    }

    static void compilerDone(object sender, EventArgs e)
    {
        if (File.Exists(serverAssemblyPath))
        {
            File.Delete(serverSlimNetPath);
            File.Delete(serverSlimNetPdbPath);

            File.Copy(slimNetAssemblyPath, serverSlimNetPath);
            File.Copy(slimNetPdbPath, serverSlimNetPdbPath);

            UnityEngine.Debug.Log("[SlimNet] Starting Embedded Server");

            // Arguments for server
            string path = "";
            string arguments = String.Format("-p{0} -a\"{1}\" {2}", SlimNetEditorSettings.Instance.EmbeddedServerPort, serverTargetDirectory, serverDebuggerCommand).Trim();

            if (isOSX)
            {
                path = monoPath;
                arguments = slimNetConsoleHostPath + " " + arguments;
            }
            else
            {
                path = slimNetConsoleHostPath;
            }

            // Log server command line
            sb.AppendLine(path + " " + arguments + Environment.NewLine);

            // Start server process
            process = startProcess(path, arguments, false);
            embeddedServerRunning = true;
        }
        else
        {
            exit = true;
        }
    }

    static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        lock (sb)
        {
            string v = e.Data.Trim();

            if (!string.IsNullOrEmpty(v))
            {
                lastOutput = v;
            }

            if (sb.Length > 8192)
            {
                sb.Remove(0, 2048);
            }

            sb.AppendLine(e.Data);
        }
    }

    static bool exit = false;
    int previousLength = 0;
    GUIStyle style = null;
    Vector2 scrollPosition = Vector2.zero;

    void Update()
    {
        if (exit)
        {
            exit = false;
            lastOutput = "";
            EditorApplication.isPlaying = false;
            UnityEngine.Debug.LogError(lastOutput ?? "");
        }

        Repaint();
    }

    void OnGUI()
    {
        if (SlimNetConfigurationTab.ServerConfiguratin.ServerMode == SlimNet.ServerMode.Standalone)
        {
            if (style == null)
            {
                style = new GUIStyle("TextArea");
                style.font = Resources.Load("CONSOLA") as Font;
                style.fontSize = 11;
                style.stretchHeight = true;
            }

            using (new SlimNetGUILayout.Horizontal())
            {
                GUILayout.Label("SlimNet Embedded Server", EditorStyles.boldLabel);

                if (embeddedServerRunning)
                {
                    if (GUILayout.Button("Stop"))
                    {
                        onStop();
                    }
                }
                else
                {
                    if (!SlimNetEditorSettings.Instance.StartEmbeddedServerOnPlay && Application.isPlaying)
                    {
                        if (GUILayout.Button("Start"))
                        {
                            onStart();
                        }
                    }
                }
            }

            using (new SlimNetGUILayout.Horizontal(GUILayout.Width(100)))
            {
                GUILayout.Label("Port", GUILayout.Width(35));

                SlimNetEditorSettings.Instance.EmbeddedServerPort =
                    EditorGUILayout.IntField(SlimNetEditorSettings.Instance.EmbeddedServerPort);
            }

            using (new SlimNetGUILayout.Horizontal(GUILayout.Width(300)))
            {
                GUILayout.Label("Build Path", GUILayout.Width(70));

                SlimNetEditorSettings.Instance.BinaryPath
                    = GUILayout.TextField(SlimNetEditorSettings.Instance.BinaryPath);
            }

            SlimNetEditorSettings.Instance.StartEmbeddedServerOnPlay =
                GUILayout.Toggle(SlimNetEditorSettings.Instance.StartEmbeddedServerOnPlay, "Autostart on play");

            if (isWIN)
            {
                SlimNetEditorSettings.Instance.UseVisualStudioCompiler =
                    GUILayout.Toggle(SlimNetEditorSettings.Instance.UseVisualStudioCompiler, "Use Visual Studio Compiler (Windows Only)");

                if (SlimNetEditorSettings.Instance.UseVisualStudioCompiler)
                {
                    SlimNetEditorSettings.Instance.AttachDebuggerToEmbeddedServer
                        = GUILayout.Toggle(SlimNetEditorSettings.Instance.AttachDebuggerToEmbeddedServer, "Attach Debugger On Start");

                    SlimNetEditorSettings.Instance.AttachDebuggerOnError
                        = GUILayout.Toggle(SlimNetEditorSettings.Instance.AttachDebuggerOnError, "Attach Debugger On Error");
                }
            }
            else
            {
                SlimNetEditorSettings.Instance.UseVisualStudioCompiler = false;
                SlimNetEditorSettings.Instance.AttachDebuggerToEmbeddedServer = false;
            }

            if (sb.Length != previousLength)
            {
                scrollPosition = new Vector2(0, float.MaxValue);
            }

            previousLength = sb.Length;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.TextArea(sb.ToString(), style);
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Space(5);
            GUILayout.Label("The embedded server is only available in standalone mode", SlimNetGUIStyles.GrayText);
        }
    }
}