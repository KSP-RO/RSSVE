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
//
//  Based on the InstallChecker from the Kethane mod for Kerbal Space Program:
//
//      • https://github.com/Majiir/Kethane/blob/master/Plugin/Kethane/Utilities/InstallChecker.cs
//
//  Original copyright by Majiir, CC0 Public Domain:
//
//      • http://creativecommons.org/publicdomain/zero/1.0
//
//  Forum thread:
//
//      • http://forum.kerbalspaceprogram.com/index.php?showtopic=59388
//
//  This file has been modified extensively and is released under the same license.
//  ================================================================================

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RSSVE
{
    /// <summary>
    /// Version and file system integrity checker class. Operates only in the Main Menu scene.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class InstallChecker : MonoBehaviour
    {
        /// <summary>
        /// Method to destroy any active GameObjects.
        /// </summary>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        private void OnDestroy()
        {
            try
            {
                Notification.Logger(Constants.AssemblyName, null, "Removing the EVE configuration event handler...");

                GameEvents.OnGameDatabaseLoaded.Remove(OnGameDatabaseLoaded);
            }
            catch (Exception ExceptionStack)
            {
                Notification.Logger(Constants.AssemblyName, "Error",
                    string.Format("InstallChecker.OnDestroy() caught an exception: {0},\n{1}\n", ExceptionStack.Message,
                        ExceptionStack.StackTrace));
            }
            finally
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// Method to register the EVE configuration file validation routine when a GameDatabase reload is invoked.
        /// </summary>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        void OnGameDatabaseLoaded()
        {
            Notification.Logger(Constants.AssemblyName, null, "Reloading GameDatabase...");

            EVEConfigChecker.GetValidateConfig(Utilities.GetCelestialBodyList());
        }

        /// <summary>
        /// Method to start the installation checker.
        /// </summary>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        void Start()
        {
            try
            {
                //  Check if the mod's assembly is placed at the correct location. This will
                //  also detect duplicate copies because only one can be in the right place.

                var baseAssembly = AssemblyLoader.loadedAssemblies
                    .Where(asm => asm.assembly.GetName().Name.Equals(Assembly.GetExecutingAssembly().GetName().Name))
                    .Where(asm => asm.url != Constants.AssemblyPath);

                if (baseAssembly.Any())
                {
                    var badPaths = baseAssembly.Select(asm => asm.path).Select(p =>
                        Uri.UnescapeDataString(new Uri(Path.GetFullPath(KSPUtil.ApplicationRootPath))
                            .MakeRelativeUri(new Uri(p)).ToString().Replace('/', Path.DirectorySeparatorChar)));

                    var badPathsString = string.Join("\n", badPaths.ToArray());

                    Notification.Logger(Constants.AssemblyName, "Error",
                        $"Incorrect installation, bad path(s): {badPathsString}");

                    Notification.Dialog("BaseAssemblyChecker",
                        $"Incorrect {Constants.AssemblyName} Installation", "#F0F0F0",
                        string.Format(
                            "{0} has been installed incorrectly and will not function properly. All files should be located under the GameData" +
                            Path.AltDirectorySeparatorChar + Constants.AssemblyName +
                            "folder. Do not move any files from inside that folder!\n\nIncorrect path(s):\n    •    {1}",
                            Constants.AssemblyName, badPathsString), "#F0F0F0");
                }
                else
                {
                    string MissingDependenciesNames = string.Empty;

                    //  Check if the following dependencies are installed:
                    //
                    //  • Environmental Visual Enhancements
                    //  • Module Manager
                    //  • Real Solar System

                    bool AssemblyEVELoaded = AssemblyLoader.loadedAssemblies.Any(asm =>
                        asm.assembly.GetName().Name
                            .StartsWith("EVEManager", StringComparison.InvariantCultureIgnoreCase) &&
                        asm.url.ToLower().Equals(Constants.AssemblyEVEPath));
                    bool AssemblyMMLoaded = AssemblyLoader.loadedAssemblies.Any(asm =>
                        asm.assembly.GetName().Name
                            .StartsWith("ModuleManager", StringComparison.InvariantCultureIgnoreCase) &&
                        asm.url.ToLower().Equals(Constants.AssemblyMMPath));
                    bool AssemblyRSSLoaded = AssemblyLoader.loadedAssemblies.Any(asm =>
                        asm.assembly.GetName().Name
                            .StartsWith("RealSolarSystem", StringComparison.InvariantCultureIgnoreCase) &&
                        asm.url.ToLower().Equals(Constants.AssemblyRSSPath));

                    //  If a dependency is not installed then we add it in the missing dependencies list.

                    if (!AssemblyEVELoaded)
                    {
                        MissingDependenciesNames = string.Concat(MissingDependenciesNames,
                            "  •  Environmental Visual Enhancements\n");

                        Notification.Logger(Constants.AssemblyName, "Error",
                            "Missing or incorrectly installed Environmental Visual Enhancements!");
                    }

                    if (!AssemblyMMLoaded)
                    {
                        MissingDependenciesNames = string.Concat(MissingDependenciesNames, "  •  Module Manager\n");

                        Notification.Logger(Constants.AssemblyName, "Error",
                            "Missing or incorrectly installed Module Manager!");
                    }

                    if (!AssemblyRSSLoaded)
                    {
                        MissingDependenciesNames = string.Concat(MissingDependenciesNames, "  •  Real Solar System\n");

                        Notification.Logger(Constants.AssemblyName, "Error",
                            "Missing or incorrectly installed Real Solar System!");
                    }

                    //  Warn the user if any of the dependencies are missing.

                    if (!string.IsNullOrEmpty(MissingDependenciesNames))
                    {
                        Notification.Dialog("DependencyChecker", "Missing Dependencies", "#F0F0F0",
                            string.Format(
                                "{0} requires the following listed mods in order to function correctly:\n\n  {1}",
                                Constants.AssemblyName, MissingDependenciesNames.Trim()), "#F0F0F0");

                        Notification.Logger(Constants.AssemblyName, "Error", "Required dependencies missing!");
                    }

                    //  Validate all possible EVE configuration files loaded in the GameDatabase.

                    Notification.Logger(Constants.AssemblyName, null, "Initiating the EVE configuration validation...");

                    EVEConfigChecker.GetValidateConfig(Utilities.GetCelestialBodyList());

                    Notification.Logger(Constants.AssemblyName, null, "Adding the EVE configuration event handler...");

                    GameEvents.OnGameDatabaseLoaded.Add(OnGameDatabaseLoaded);
                }
            }
            catch (Exception ExceptionStack)
            {
                Notification.Logger(Constants.AssemblyName, "Error",
                    string.Format("InstallChecker.Start() caught an exception: {0},\n{1}\n", ExceptionStack.Message,
                        ExceptionStack.StackTrace));
            }
            finally
            {
                Destroy(this);
            }
        }
    }
}