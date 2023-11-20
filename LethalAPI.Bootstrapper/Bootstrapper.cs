﻿// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1649 // file should match name of type
#pragma warning disable SA1201 // enum should not follow type
#pragma warning disable SA1403 // file can only contain one namespace.
#pragma warning disable SA1402 // file may only contain one type.

#if Melonloader
using LethalAPI.Bootstrapper.MelonLoader;
using MelonLoader;

[assembly: MelonInfo(typeof(Bootstrapper), "LethalAPI-Bootstrap", "1.0.0", "LethalAPI Modding Community")]

// ReSharper disable CheckNamespace
namespace LethalAPI.Bootstrapper.MelonLoader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Core;
    using global::MelonLoader.Pastel;
    using LethalAPI.Bootstrapper;

    /// <summary>
    /// The bootstrapping class for MelonLoader.
    /// </summary>
    internal class Bootstrapper : MelonMod
    {
        /// <inheritdoc />
        public override void OnEarlyInitializeMelon()
        {
            Loader.LoadMethod = LoadMethod.MelonLoader;
            LogMessage("Loading Lethal API Bootstrapper [MelonLoader]");
            Loader.Load();
        }

        /// <summary>
        /// Logs a message via MelonLoader.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void LogMessage(string message)
        {
            StringBuilder fullstring = new();
            StringBuilder stringBuilder = new();
            ConsoleColor oldColor = System.ConsoleColor.Gray;
            bool previouslyEscaped = false;
            for (int i = 1; i < message.Length; i++)
            {
                char c = message[i];
                if (c == '\\')
                {
                    stringBuilder.Append(c);
                    previouslyEscaped = true;
                    continue;
                }

                if (c != '&' || previouslyEscaped)
                {
                    previouslyEscaped = false;
                    stringBuilder.Append(c);
                    continue;
                }

                StringBuilder searchColor = new();
                for (int j = 0; j < Log.LongestColor + 1; j++)
                {
                    searchColor.Append(message[i + j]);
                }

                if (!Log.TryGetColor(searchColor.ToString(), out ConsoleColor? color, out int removeLength) || color is null)
                {
                    stringBuilder.Append(c);
                    continue;
                }

                // Output prior text.
                // fullstring.Append(stringBuilder);
                fullstring.Append(stringBuilder.ToString().Pastel(GetColor(oldColor)));
                oldColor = color.Value;

                // Change Color and reset for next string.
                stringBuilder = new StringBuilder();

                // Skip this many characters as they are all coloring.
                i += removeLength;
            }

            if (stringBuilder.Length > 0)
                fullstring.Append(stringBuilder);

            WriteString(fullstring.ToString());
        }

        /// <summary>
        /// Loads paths for melon loader.
        /// </summary>
        internal static void LoadPathsMelonLoader()
        {
            Core.Loader.PluginLoader.PluginDirectory = global::MelonLoader.Utils.MelonEnvironment.ModsDirectory;
            Core.Loader.PluginLoader.DependencyDirectory = global::MelonLoader.Utils.MelonEnvironment.DependenciesDirectory;
            Core.Loader.PluginLoader.ConfigDirectory = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "../", "Configs");
        }

        private static ReadOnlyDictionary<ConsoleColor, Color> ColorTranslations => new(
            new Dictionary<ConsoleColor, Color>()
            {
                { System.ConsoleColor.Black, Color.Black },
                { System.ConsoleColor.Red, Color.Red },
                { System.ConsoleColor.Yellow, Color.Yellow },
                { System.ConsoleColor.Green, Color.Green },
                { System.ConsoleColor.Cyan, Color.Cyan },
                { System.ConsoleColor.Blue, Color.Blue },
                { System.ConsoleColor.Magenta, Color.Magenta },
                { System.ConsoleColor.Gray, Color.LightGray },
                { System.ConsoleColor.DarkRed, Color.DarkRed },
                { System.ConsoleColor.DarkYellow, Color.DarkGoldenrod },
                { System.ConsoleColor.DarkGreen, Color.DarkGreen },
                { System.ConsoleColor.DarkCyan, Color.DarkCyan },
                { System.ConsoleColor.DarkBlue, Color.DarkBlue },
                { System.ConsoleColor.DarkMagenta, Color.DarkMagenta },
                { System.ConsoleColor.DarkGray, Color.DarkGray },
                { System.ConsoleColor.White, Color.White },
            });

        private static Color GetColor(ConsoleColor color) => ColorTranslations[color];

        // This is patched.
        private static void WriteString(string msg)
        {
            // MelonLogger.MsgDirect(GetColor(color ?? Log.ColorCodes["r"]), msg);
            MelonLogger.MsgDirect(Color.LightGray, msg);
        }
    }
}
#endif

#if Bepinex
namespace LethalAPI.Bootstrapper.BepInEx
{
    using System.IO;

    using global::BepInEx;
    using global::BepInEx.Logging;

    /// <summary>
    /// The bootstrapping class for BepInEx.
    /// </summary>
    [BepInPlugin("LethalAPI-Bootstrap", "LethalAPI", "1.0.0")]
    internal class Bootstrapper : BaseUnityPlugin
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static ManualLogSource logger = null!;

        /// <summary>
        /// Logs a message via BepInEx.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void LogMessage(string message)
        {
            logger.Log((LogLevel)62, message);
        }

        /// <summary>
        /// Loads paths for BepInEx.
        /// </summary>
        internal static void LoadPathsBepInEx()
        {
            Core.Loader.PluginLoader.PluginDirectory = Paths.PluginPath;
            Core.Loader.PluginLoader.DependencyDirectory = Path.GetFullPath(Path.Combine(Paths.PluginPath, "../", "Dependencies"));
            Core.Loader.PluginLoader.ConfigDirectory = Paths.ConfigPath;
        }

        // ReSharper disable once UnusedMember.Local
        private void Awake()
        {
            logger = this.Logger;
            Loader.LoadMethod = LoadMethod.BepInEx;
            LogMessage("Loading Lethal API Bootstrapper [BepInEx]");
            Loader.Load();
        }
    }
}
#endif

#if Manual
namespace Doorstop
{
    using LethalAPI.Bootstrapper;

    /// <summary>
    /// The bootstrapping class for Doorstop.
    /// </summary>
    internal static class Entrypoint
    {
        /// <summary>
        /// The doorstop start method.
        /// </summary>
        internal static void Start()
        {
            Loader.LoadMethod = LoadMethod.Doorstop;
            UnityEngine.Debug.Log("Loading Lethal API Bootstrapper [Doorstop]");
            Loader.Load();
        }

        /// <summary>
        /// Loads paths for manual installations.
        /// </summary>
        internal static void LoadPathsManual()
        {
            LethalAPI.Core.Loader.PluginLoader.PluginDirectory = string.Empty;
            LethalAPI.Core.Loader.PluginLoader.DependencyDirectory = string.Empty;
            LethalAPI.Core.Loader.PluginLoader.ConfigDirectory = string.Empty;
        }
    }
}
#endif

namespace LethalAPI.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;

    using Core.Loader;
    using HarmonyLib;

    /// <summary>
    /// The bootstrap loader class.
    /// </summary>
    public static class Loader
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly List<Assembly> Dependencies = new ();
        private static Assembly baseAssembly = null!;

        /// <summary>
        /// Gets the <see cref="LoadMethod"/> being used.
        /// </summary>
        public static LoadMethod LoadMethod { get; internal set; } = LoadMethod.Manual;

        /// <summary>
        /// Loads the framework.
        /// </summary>
        internal static void Load()
        {
            CosturaUtility.Initialize();
            Log($"[LethalAPI-Bootstrapper] Loading Bootstrapper [{LoadMethod}].");
            baseAssembly = typeof(Loader).Assembly;
            LoadPaths();
            LoadDependencies();
        }

        private static void Log(string message) => Postfix(message);

        private static void Postfix(string message)
        {
#if Melonloader
            MelonLoader.Bootstrapper.LogMessage(message);
#endif
#if Bepinex
            BepInEx.Bootstrapper.LogMessage(message);
#endif
#if Manual
            UnityEngine.Debug.Log(message);
#endif
        }

        private static Harmony Harmony => new ("com.lethalapi.bootstrapper");

        private static void LoadPaths()
        {
            #if Bepinex
                BepInEx.Bootstrapper.LoadPathsBepInEx();
            #endif
            #if Melonloader
                MelonLoader.Bootstrapper.LoadPathsMelonLoader();
            #endif
            #if Manual
                Doorstop.Entrypoint.LoadPathsManual();
            #endif
        }

        private static void LoadDependencies()
        {
            Log("[LethalAPI-Bootstrapper] Loading dependencies.");
            foreach (string resourcePath in baseAssembly.GetManifestResourceNames())
            {
                if (!LoadAssembly(resourcePath, out Assembly? resultAssembly))
                    continue;

                Log($"[LethalAPI-Bootstrapper] Loaded embedded dependency '{resultAssembly!.GetName().Name}'@v{resultAssembly.GetName().Version}.");
                try
                {
                    Dependencies.Add(resultAssembly);
                }
                catch (Exception e)
                {
                    Log($"[LethalAPI-Bootstrapper] An error has occured while loading dependency '{resourcePath}'. Exception: \n{e}");
                }
            }

            try
            {
                // Apply the patches for logging in a type load safe manner.
#if Bepinex
                PluginLoader.FixLoggingBepInEx(Harmony);
#endif

                // This is less expensive.
                Harmony.Patch(AccessTools.Method(typeof(Core.Log), nameof(Core.Log.Raw)), null, new HarmonyMethod(AccessTools.Method(typeof(Loader), nameof(Postfix))));
                Log("[LethalAPI-Bootstrapper] Log fix patched.");
                _ = new PluginLoader();
                foreach (Assembly dependency in Dependencies)
                {
                    PluginLoader.Dependencies.Add(dependency);
                }
            }
            catch (Exception e)
            {
                Log($"[LethalAPI-Bootstrapper] An error has occured while loading the LethalAPI Core Plugin. Exception: \n{e}");
            }
        }

        private static bool LoadAssembly(string resourcePath, out Assembly? assembly)
        {
            try
            {
                assembly = null;
                if (!resourcePath.EndsWith(".dll.compressed") && !resourcePath.EndsWith(".dll"))
                    return false;

                MemoryStream memStream = new();
                Stream? dataStream = baseAssembly.GetManifestResourceStream(resourcePath);
                if (dataStream is null)
                    return false;

                if (resourcePath.EndsWith(".dll.compressed"))
                {
                    DeflateStream decompressionStream = new(dataStream, CompressionMode.Decompress);
                    decompressionStream.CopyTo(memStream);
                }
                else
                {
                    dataStream.CopyTo(memStream);
                }

                assembly = AppDomain.CurrentDomain.Load(memStream.ToArray());
                return true;
            }
            catch (TypeLoadException e)
            {
                Log($"Missing dependency for plugin '{resourcePath}'. Exception: {e.Message}");
            }
            catch (Exception e)
            {
                Log($"Could not load a dependency or plugin. Exception: \n{e}");
            }

            assembly = null;
            return false;
        }
    }

    /// <summary>
    /// Represents the different load methods that can be used.
    /// </summary>
    public enum LoadMethod
    {
        /// <summary>
        /// Loaded via BepInEx.
        /// </summary>
        BepInEx = 1,

        /// <summary>
        /// Loaded via MelonLoader.
        /// </summary>
        MelonLoader = 2,

        /// <summary>
        /// Loaded via Doorstop.
        /// </summary>
        Doorstop = 4,

        /// <summary>
        /// Loaded manually.
        /// </summary>
        Manual = 8,
    }
}