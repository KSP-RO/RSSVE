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
using System.Reflection;
using UnityEngine;

namespace RSSVE
{
    /// <summary>
    /// Startup system checker. Operates only in the Loading scene.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class StartupChecker : MonoBehaviour
    {
        /// <summary>
        /// Method to start the startup checker.
        /// </summary>
        private void Start()
        {
            try
            {
                //  Log some basic information that might be of interest when debugging installations.

                Notification.Logger(Constants.AssemblyName, null,
                    $"Assembly location: {Assembly.GetExecutingAssembly().Location}");
                Notification.Logger(Constants.AssemblyName, null,
                    $"Assembly version: {Version.GetAssemblyVersion()}");
                Notification.Logger(Constants.AssemblyName, null,
                    $"Assembly compatible: {CompatibilityChecker.IsCompatible()}");

                //  The following information fields are only active if the
                //  "Verbose Logging" option in the KSP settings is enabled.

                if (Utilities.IsVerboseDebugEnabled)
                {
                    Notification.Logger(Constants.AssemblyName, null,
                        $"Using Unity player: {Utilities.GetPlatformType}");
                    Notification.Logger(Constants.AssemblyName, null,
                        $"Using renderer: {Utilities.GetGraphicsRenderer}");
                    Notification.Logger(Constants.AssemblyName, null,
                        $"Using Maximum Texture Size: {SystemInfo.maxTextureSize}");
                }

                //  Check if the graphics accelerator installed supports at
                //  least the 8K texture size required by the RSSVE assets.

                if (SystemInfo.maxTextureSize >= 8192) return;
                Notification.Dialog("TextureChecker", "Unsupported Graphics Accelerator", "#F0F0F0",
                    $"{Constants.AssemblyName} is not supported by the current graphics accelerator installed.",
                    "#F0F0F0");

                Notification.Logger(Constants.AssemblyName, "Error",
                    $"Unsupported minimum texture size (using {SystemInfo.maxTextureSize})!");
            }
            catch (Exception ExceptionStack)
            {
                Notification.Logger(Constants.AssemblyName, "Error",
                    $"StartupChecker.Start() caught an exception: {ExceptionStack.Message},\n{ExceptionStack.StackTrace}\n");
            }
            finally
            {
                Destroy(this);
            }
        }
    }
}