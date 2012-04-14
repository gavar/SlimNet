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

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CommandLine;

namespace SlimNet.ConsoleHost
{
    public class Program
    {
        static void printAsciiHeader()
        {
            Console.WriteLine(
@"    _________.__  .__          _______          __   
   /   _____/|  | |__| _____   \      \   _____/  |_ 
   \_____  \ |  | |  |/     \  /   |   \_/ __ \   __\
   /        \|  |_|  |  Y Y  \/    |    \  ___/|  |  
  /_______  /|____/__|__|_|  /\____|__  /\___  >__|  
          \/               \/         \/     \/    ");
            Console.WriteLine();
        }

        static void printVersionHeader()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.WriteLine("  Version " + fvi.ProductVersion);
            Console.WriteLine("  Copyright (C) 2011, 2012 Fredrik Holmstrom");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
#if WIN
            try
            {
                // On win32 we can try to set the 
                // size of the console to give us
                // a better overview
                Console.WindowHeight = 50;
                Console.WindowWidth = 120;
                Console.BufferHeight = 6000;
                Console.BufferWidth = 120;
            }
            catch
            {
                // It might fail though, so catch it and ignore it
            }
#endif
            printAsciiHeader();
            printVersionHeader();

            // Use the default console adapter
            Log.SetAdapter(new LogConsoleAdapter());
            Log log = Log.GetLogger(typeof(Program));

            CommandLineOptions options = new CommandLineOptions();
            ICommandLineParser parser = new CommandLineParser();

            // Parse the command line arguments
            if (parser.ParseArguments(args, options))
            {
                // Switch directory to the assembly path
                Directory.SetCurrentDirectory(options.AssemblyPath);

                // Our configuration
                SlimNet.ServerConfiguration serverConfig = null;

                try
                {
                    // Try to load configuration from command line params
                    XmlSerializer serializer = new XmlSerializer(typeof(ServerConfiguration));
                    serverConfig = (ServerConfiguration)serializer.Deserialize(new StringReader(File.ReadAllText(options.ConfigurationFile)));
                }
                catch (Exception exn)
                {
                    log.Error(exn);
                    log.Warn("Error while loading configuration file, using default configuration instead");

                    // Fallback to default configuration
                    serverConfig = new ServerConfiguration();
                }

                // Set port from command line
                serverConfig.Port = options.Port;

                while (true)
                {
                    try
                    {
                        // Create server
                        SlimNet.Server server = SlimNet.StandaloneServer.Create(serverConfig);
#if WIN
                        // If on windows we have the option of attaching the debugger
                        if (options.AttachDebugger)
                        {
                            if (!System.Diagnostics.Debugger.IsAttached)
                            {
                                System.Diagnostics.Debugger.Launch();
                            }
                        }

                        if (options.ErrorAttachDebugger)
                        {
                            Log.AttachDebuggerOnError = true;
                        }
#endif
                        // Start server
                        server.Start();
                    }
                    catch (SlimNet.RecycleException)
                    {
                        //TODO: Actually re-cycle the process
                        return;
                    }
                    catch (Exception exn)
                    {
                        log.Error(exn.GetBaseException());
                        return;
                    }
                }
            }
            else
            {
                // If parsing failed, print usage and exit
                Console.Write(options.GetUsage());
            }
        }
    }
}
