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
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

namespace SlimNet.Unity
{
    public class SlimNetEditorSettings
    {
        static bool initializing = true;
        static SlimNetEditorSettings instance;

        public static SlimNetEditorSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    init();
                }

                return instance;
            }
        }

        static void init()
        {
            try
            {
                TextAsset settingsXml = Resources.Load("SlimNet-EditorSettings", typeof(TextAsset)) as TextAsset;

                if (settingsXml != null)
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SlimNetEditorSettings));
                        instance = (SlimNetEditorSettings)serializer.Deserialize(new StringReader(settingsXml.text));
                    }
                    catch
                    {

                    }
                }
            }
            finally
            {
                if (instance == null)
                {
                    instance = new SlimNetEditorSettings();
                }

                initializing = false;
            }
        }

        static void save()
        {
            if (!initializing && instance != null)
            {
                StringBuilder builder = new StringBuilder();
                StringWriter writer = new StringWriter(builder);

                XmlSerializer serializer = new XmlSerializer(typeof(SlimNetEditorSettings));
                serializer.Serialize(writer, instance);

                writer.Flush();

                string path = String.Format("Assets{0}SlimNet{0}Resources{0}SlimNet-EditorSettings.xml", instance.Separator);

                File.WriteAllText(path, builder.ToString());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        string binaryPath = @"C:\SlimNet\Build";
        int embeddedServerPort = 14000;
        bool useVisualStudioCompiler;
        bool startEmbeddedServerOnPlay;
        bool attachDebuggerToEmbeddedServer;
        bool attachDebuggerOnError;

        SlimNetEditorSettings()
        {

        }

        public string Separator
        {
            get { return Path.DirectorySeparatorChar.ToString(); }
        }

        public string ServerFolder
        {
            get
            {
                return Directory.GetCurrentDirectory() + Separator + "Server";
            }
        }

        public int EmbeddedServerPort
        {
            get
            {
                return embeddedServerPort;
            }
            set
            {
                if (value > 1024 && value < UInt16.MaxValue)
                {
                    if (embeddedServerPort != value)
                    {
                        embeddedServerPort = value;
                        save();
                    }

                    embeddedServerPort = value;
                }
            }
        }

        public bool StartEmbeddedServerOnPlay
        {
            get
            {
                return startEmbeddedServerOnPlay;
            }
            set
            {
                if (startEmbeddedServerOnPlay != value)
                {
                    startEmbeddedServerOnPlay = value;
                    save();
                }

                startEmbeddedServerOnPlay = value;
            }
        }

        public bool UseVisualStudioCompiler
        {
            get
            {
                return useVisualStudioCompiler;
            }
            set
            {
                if (useVisualStudioCompiler != value)
                {
                    useVisualStudioCompiler = value;
                    save();
                }

                useVisualStudioCompiler = value;
            }
        }

        public bool AttachDebuggerToEmbeddedServer
        {
            get
            {
                return attachDebuggerToEmbeddedServer;
            }
            set
            {
                if (attachDebuggerToEmbeddedServer != value)
                {
                    attachDebuggerToEmbeddedServer = value;
                    save();
                }

                attachDebuggerToEmbeddedServer = value;
            }
        }

        public bool AttachDebuggerOnError
        {
            get
            {
                return attachDebuggerOnError;
            }
            set
            {
                if (attachDebuggerOnError != value)
                {
                    attachDebuggerOnError = value;
                    save();
                }

                attachDebuggerOnError = value;
            }
        }

        public string BinaryPath
        {
            get
            {
                return binaryPath ?? "";
            }
            set
            {
                value = value ?? "";

                if (binaryPath != value)
                {
                    binaryPath = value;
                    save();
                }

                binaryPath = value;
            }
        }


        public void VerifyServerFolderExists()
        {
            if (!Directory.Exists(ServerFolder))
            {
                Directory.CreateDirectory(ServerFolder);
            }
        }
    }
}
