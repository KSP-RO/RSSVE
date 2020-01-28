//  ================================================================================
//  Real Solar System Visual Enhancements for Kerbal Space Program.
//
//  Copyright © 2016 - 2019, Alexander "Phineas Freak" Kampolis.
//
//  This file is part of Real Solar System Visual Enhancements.
//
//  Real Solar System Visual Enhancements is licensed under a Creative Commons
//  Attribution-NonCommercial-ShareAlike 4.0 (CC-BY-NC-SA 4.0) license.
//
//  You should have received a copy of the license along with this work. If not,
//  visit the official Creative Commons web page:
//
//      • https://www.creativecommons.org/licensies/by-nc-sa/4.0
//  ================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace RSSVE
{
    #region RSSVE Constants

    /// <summary>
    /// Class to set up the parameters required and used by RSSVE.
    /// </summary>
    /// <returns>
    /// Does not return a value.
    /// </returns>
    public static class Constants
    {
        /// <summary>
        /// Version check structure.
        /// </summary>
        public struct VersionCompatible
        {
            /// <summary>
            /// The minimum major version value.
            /// </summary>
            public static readonly int MajorMin = 1;

            /// <summary>
            /// The maximum major version value.
            /// </summary>
            public static readonly int MajorMax = 1;

            /// <summary>
            /// The minimum minor version value.
            /// </summary>
            public static readonly int MinorMin = 3;

            /// <summary>
            /// The maximum minor version value.
            /// </summary>
            public static readonly int MinorMax = 7;

            /// <summary>
            /// The minimum revision version value.
            /// </summary>
            public static readonly int RevisionMin = 0;

            /// <summary>
            /// The maximum revision version value.
            /// </summary>
            public static readonly int RevisionMax = 9;
        }

        /// <summary>
        /// The compatible Unity version.
        /// </summary>
        public static readonly string UnityVersion = "2017.1.3p1";

        /// <summary>
        /// The name of the assembly (used as a tag for the notification dialogs and the log file).
        /// </summary>
        public static readonly string AssemblyName = "RSSVE";

        /// <summary>
        /// The (relative to the "GameData" directory) path where the assembly file resides.
        /// </summary>
        public static readonly string AssemblyPath = AssemblyName + Path.AltDirectorySeparatorChar + "Plugins";

        /// <summary>
        /// The (relative to the "GameData" directory) path where the Environmental Visual Enhancements mod resides.
        /// </summary>
        public static readonly string AssemblyEVEPath =
            "environmentalvisualenhancements" + Path.AltDirectorySeparatorChar + "plugins";

        /// <summary>
        /// The (relative to the "GameData" directory) path where the Module Manager mod resides.
        /// </summary>
        public static readonly string AssemblyMMPath = string.Empty;

        /// <summary>
        /// The (relative to the "GameData" directory) path where the Real Solar System mod resides.
        /// </summary>
        public static readonly string AssemblyRSSPath = "realsolarsystem" + Path.AltDirectorySeparatorChar + "plugins";

        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        public static readonly string ConfigurationFileName = "RSSVE_Settings_Config.cfg";

        /// <summary>
        /// The (relative to the "GameData" directory) path where the configuration file resides.
        /// </summary>
        public static readonly string ConfigurationFilePath =
            "GameData" + Path.AltDirectorySeparatorChar + AssemblyName + Path.AltDirectorySeparatorChar + "Configs" +
            Path.AltDirectorySeparatorChar + "Misc_Configs";
    }

    #endregion

    #region RSSVE Notification Functions

    /// <summary>
    /// Class to create user notification dialogs and log basic information to the KSP log file.
    /// </summary>
    static class Notification
    {
        /// <summary>
        /// Method to create pop-up notification dialogs.
        /// </summary>
        /// <param name = "DialogName">The internal name of the dialog window spawned.</param>
        /// <param name = "TitleText">The title text of the dialog window (string).</param>
        /// <param name = "TitleColor">The color of the message text.</param>
        /// <param name = "ContentText">The message text of the dialog window (string).</param>
        /// <param name = "ContentColor">The color of the message text.</param>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        public static void Dialog(string DialogName, string TitleText, string TitleColor, string ContentText,
            string ContentColor)
        {
            if (!string.IsNullOrEmpty(DialogName) &&
                !string.IsNullOrEmpty(TitleText) &&
                !string.IsNullOrEmpty(TitleColor) &&
                !string.IsNullOrEmpty(ContentText) &&
                !string.IsNullOrEmpty(ContentColor))
            {
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), DialogName,
                    string.Format("<color={0}>{1}</color>", TitleColor, TitleText),
                    string.Format("<color={0}>{1}</color>", ContentColor, ContentText), "OK", false, HighLogic.UISkin,
                    true, string.Empty);
            }
        }

        /// <summary>
        /// Method to print generic text in the KSP debug log.
        /// </summary>
        /// <param name = "AssemblyTagName">Name of the assembly (to be used as a log tag) (string).</param>
        /// <param name = "LogType">The type of the log. Can be one one of the following: null (for the basic "Log" type), Warning or Error (string).</param>
        /// <param name = "Content">The message to be logged (string).</param>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        public static void Logger(string AssemblyTagName, string LogType, string Content)
        {
            if (!string.IsNullOrEmpty(AssemblyTagName) && !string.IsNullOrEmpty(Content))
            {
                switch (LogType)
                {
                    case ("Warning"):

                        UnityEngine.Debug.LogWarning(string.Format("[{0}]: {1}", AssemblyTagName, Content));

                        break;

                    case ("Error"):

                        UnityEngine.Debug.LogError(string.Format("[{0}]: {1}", AssemblyTagName, Content));

                        break;

                    default:

                        UnityEngine.Debug.Log(string.Format("[{0}]: {1}", AssemblyTagName, Content));

                        break;
                }
            }
        }
    }

    #endregion

    #region RSSVE System Functions

    /// <summary>
    /// Class to setup basic system and utility functions.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Method to check whether a value is inside the specified range.
        /// </summary>
        /// <param name = "nNumber">The value to check (integer).</param>
        /// <param name = "nMin">The lower value range (integer).</param>
        /// <param name = "nMax">The upper value range (integer).</param>
        /// <returns>
        /// Returns the status of the check (boolean).
        /// </returns>
        public static bool InRange(int nNumber, int nMin, int nMax)
        {
            return ((nNumber >= nMin) && (nNumber <= nMax));
        }

        /// <summary>
        /// Method to get the available celestial body names.
        /// </summary>
        /// <returns>
        /// Returns a list of names for all celestial bodies found in the GameDatabase (string).
        /// </returns>
        public static List<string> GetCelestialBodyList()
        {
            var szBodyLoaderNames = new List<string>();

            foreach (var Celestial in FlightGlobals.Bodies)
            {
                //  Add the body name to the list. We are saving them to
                //  lowercase, since it is the lowest common denominator.

                if (Celestial != null)
                {
                    szBodyLoaderNames.Add(Celestial.bodyName.ToLower());
                }
            }

            return szBodyLoaderNames;
        }

        /// <summary>
        /// Method to get the operating system graphics renderer.
        /// </summary>
        /// <returns>
        /// Returns one of the following renderer types: D3D9, D3D11, OpenGL or Unknown (string).
        /// </returns>

        public static string GetGraphicsRenderer
        {
            get
            {
                var RendererType = SystemInfo.graphicsDeviceVersion;

                string RendererName;

                if (RendererType.StartsWith("Direct3D 9", StringComparison.InvariantCultureIgnoreCase))
                {
                    RendererName = "D3D9";
                }
                else if (RendererType.StartsWith("Direct3D 11", StringComparison.InvariantCultureIgnoreCase))
                {
                    RendererName = "D3D11";
                }
                else if (RendererType.StartsWith("OpenGL", StringComparison.InvariantCultureIgnoreCase))
                {
                    RendererName = "OpenGL";
                }
                else
                {
                    RendererName = "Unknown";
                }

                return RendererName;
            }
        }

        /// <summary>
        /// Method to get the operating system type.
        /// </summary>
        /// <returns>
        /// Returns one the following operating system types: Linux, OSX, Windows or Unknown (string).
        /// </returns>

        public static string GetPlatformType
        {
            get
            {
                var PlatformType = Application.platform;

                string PlatformTypeName;

                switch (PlatformType)
                {
                    case RuntimePlatform.LinuxPlayer:

                        PlatformTypeName = "Linux";

                        break;

                    case RuntimePlatform.OSXPlayer:

                        PlatformTypeName = "OSX";

                        break;

                    case RuntimePlatform.WindowsPlayer:

                        PlatformTypeName = "Windows";

                        break;

                    default:

                        PlatformTypeName = "Unknown";

                        break;
                }

                return PlatformTypeName;
            }
        }

        /// <summary>
        /// Method to check if the verbose debug option is active.
        /// </summary>
        /// <returns>
        /// Returns the status of the verbose debug option (boolean).
        /// </returns>

        public static bool IsVerboseDebugEnabled
        {
            get { return GameSettings.VERBOSE_DEBUG_LOG; }
        }
    }

    #endregion

    #region RSSVE Version Functions

    /// <summary>
    /// Class to get the assembly version information.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// Method to get the assembly version.
        /// </summary>
        /// <returns>
        /// Returns the assembly version (major.minor.revision.build) (string).
        /// </returns>
        public static string GetAssemblyVersion()
        {
            var AssemblyVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            return string.Format("{0}.{1}.{2}.{3}", AssemblyVersion.FileMajorPart, AssemblyVersion.FileMinorPart,
                AssemblyVersion.FileBuildPart, AssemblyVersion.FilePrivatePart);
        }
    }

    #endregion
}