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
using System.IO;

namespace RSSVE
{
    /// <summary>
    /// Configuration options management class. Operates only in the Difficulty Settings page.
    /// </summary>
    public class RSSVESettings : GameParameters.CustomParameterNode
    {
        /// <summary>
        /// The name of the ConfigNode where all settings are stored.
        /// </summary>
        const string szConfigNodeName = "RSSVESETTINGS";

        /// <summary>
        /// Parameter to set whether the EVE city lights should be visible.
        /// </summary>
        [GameParameters.CustomParameterUI("Enable City Lights", toolTip = "Enables or disables the Earth city lights")]
        public bool EnableCityLights = true;

        /// <summary>
        /// Parameter to set whether the EVE cloud shadows should be visible.
        /// </summary>
        [GameParameters.CustomParameterUI("Enable Cloud Shadows",
            toolTip = "Enables or disables the cloud shadow effects")]
        public bool EnableCloudShadows = true;

        /// <summary>
        /// Parameter to set whether the EVE volumetric clouds should be visible.
        /// </summary>
        [GameParameters.CustomParameterUI("Enable Volumetric Clouds",
            toolTip = "Enables or disables the volumetric cloud particles")]
        public bool EnableVolumetricClouds = true;

        /// <summary>
        /// Display a notification to the user that a restart is required after changing any of the parameters.
        /// </summary>
        [GameParameters.CustomStringParameterUI("Restart Notification", title = null, lines = 4)]
        public string szRestartNotification = "\n\nA restart is required for the changes to take effect.";

        /// <summary>
        /// Method to set the localized title of the Difficulty Options entry.
        /// </summary>

        public override string DisplaySection => "RSS Visual Enhancements";

        /// <summary>
        /// Method to set the applicable GameModes for the mod Difficulty Options.
        /// </summary>

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        /// <summary>
        /// Method to set the presets of the mod Difficulty Options.
        /// </summary>

        public override bool HasPresets => false;

        /// <summary>
        /// Method to load the configuration options.
        /// </summary>
        /// <param name = "node">The ConfigNode object to be loaded.</param>
        public override void OnLoad(ConfigNode node)
        {
            try
            {
                //  Define the variables.

                uint nConfigNodeCount = 0;

                //  Get all available RSSVE ConfigNodes from the GameDatabase.

                foreach (ConfigNode RSSVESettings in GameDatabase.Instance.GetConfigNodes(szConfigNodeName))
                {
                    //  Get the values of the parameters.

                    if (RSSVESettings != null)
                    {
                        RSSVESettings.TryGetValue("EnableCityLights", ref EnableCityLights);
                        RSSVESettings.TryGetValue("EnableCloudShadows", ref EnableCloudShadows);
                        RSSVESettings.TryGetValue("EnableVolumetricClouds", ref EnableVolumetricClouds);
                    }

                    nConfigNodeCount++;
                }

                //  Log some basic information that might be of interest when debugging installations.

                if (!Utilities.IsVerboseDebugEnabled) return;
                Notification.Logger(Constants.AssemblyName, null,
                    $"{szConfigNodeName} config found (count: {nConfigNodeCount})!");
                Notification.Logger(Constants.AssemblyName, null,
                    $"City lights enabled: {EnableCityLights}");
                Notification.Logger(Constants.AssemblyName, null,
                    $"Cloud shadows enabled: {EnableCloudShadows}");
                Notification.Logger(Constants.AssemblyName, null,
                    $"Volumetric clouds enabled: {EnableVolumetricClouds}");
            }
            catch (Exception ExceptionStack)
            {
                Notification.Logger(Constants.AssemblyName, "Error",
                    $"Settings.OnLoad() caught an exception: {ExceptionStack.Message},\n{ExceptionStack.StackTrace}\n");
            }
        }

        /// <summary>
        /// Method to save the configuration options.
        /// </summary>
        /// <param name = "node">The ConfigNode object to be saved.</param>
        public override void OnSave(ConfigNode node)
        {
            try
            {
                // Create a new ConfigNode object.

                var RSSVEConfigNode = new ConfigNode();

                //  Assemble the path where the configuration file resides.

                string RSSVEConfigFilename = KSPUtil.ApplicationRootPath + Constants.ConfigurationFilePath + Path.AltDirectorySeparatorChar +
                                             Constants.ConfigurationFileName;

                //  Create a new configuration file directory.

                if (!Directory.Exists(KSPUtil.ApplicationRootPath + Constants.ConfigurationFilePath))
                {
                    Directory.CreateDirectory(KSPUtil.ApplicationRootPath + Constants.ConfigurationFilePath);
                }

                //  Create a new empty configuration file.

                if (!File.Exists(RSSVEConfigFilename))
                {
                    Notification.Logger(Constants.AssemblyName, "Warning", "No RSSVE settings file found! Creating...");

                    File.Create(RSSVEConfigFilename).Dispose();
                }

                //  Clear any previous ConfigNode objects.

                RSSVEConfigNode.RemoveNodes(szConfigNodeName);

                //  Create the settings node.

                var RSSVEDataNode = RSSVEConfigNode.AddNode(szConfigNodeName);

                //  Save the user-defined RSSVE settings to the configuration file.

                RSSVEDataNode.SetValue("EnableCityLights", EnableCityLights, true);
                RSSVEDataNode.SetValue("EnableCloudShadows", EnableCloudShadows, true);
                RSSVEDataNode.SetValue("EnableVolumetricClouds", EnableVolumetricClouds, true);

                //  Save the configuration node settings.

                RSSVEConfigNode.Save(RSSVEConfigFilename);
            }
            catch (Exception ExceptionStack)
            {
                Notification.Logger(Constants.AssemblyName, "Error",
                    $"Settings.OnSave() caught an exception: {ExceptionStack.Message},\n{ExceptionStack.StackTrace}\n");
            }
        }

        /// <summary>
        /// Method to set the internal section name of the Difficulty Options entry.
        /// </summary>

        public override string Section => "RSSVE Settings";

        /// <summary>
        /// Method to set the position of the Difficulty Options entry.
        /// </summary>

        public override int SectionOrder => 1;

        /// <summary>
        /// Method to set the title of the Difficulty Options entry.
        /// </summary>

        public override string Title => "Global Settings";
    }
}