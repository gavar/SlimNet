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

using CommandLine;
using CommandLine.Text;

namespace SlimNet.ConsoleHost
{
    public class CommandLineOptions
    {
        [Option("p", "port", Required = false, HelpText = "The port the server will listen on (default: 14000)")]
        public int Port = 14000;

        [Option("a", "assemblies", Required = true, HelpText = "Directory to load user assemblies and configuration from")]
        public string AssemblyPath;

        [Option("d", "debug", Required = false, HelpText = "Automatically attaches the visual studio debugger (Windows/.NET Only)")]
        public bool AttachDebugger = false;

        [Option("r", "errorDebug", Required = false, HelpText = "On error automatically attach the visual studio debugger (Windows/.NET Only)")]
        public bool ErrorAttachDebugger = false;

        [Option("c", "cfg", Required = false, HelpText = "The configuration file to load (default: SlimNet-Config.xml)")]
        public string ConfigurationFile = Constants.ServerConfigNameXml;

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            HelpText text = new HelpText(" ");

            text.AddPreOptionsLine("  Usage: SlimNet.ConsoleHost.exe -p14000 -a\"C:\\path\\to\\assemblies\\\"");
            text.AddOptions(this);

            return text.ToString();
        }
    }
}
