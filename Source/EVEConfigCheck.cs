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

namespace RSSVE
{
    /// <summary>
    /// Environmental Visual Enhancements configuration file integrity checker class.
    /// </summary>
    static class EVEConfigChecker
    {
        /// <summary>
        /// Method to check if a specific EVE configuration file is valid.
        /// </summary>
        /// <param name = "szBodyLoaderNames">A list with all celestial body names found in the GameDatabase</param>
        /// <param name = "szEVENodeToCheck">The name of the configuration file to be checked (string)</param>
        /// <returns>
        /// Returns the number of the specific EVE configuration files found.
        /// </returns>
        public static int GetCheckConfig(List<string> szBodyLoaderNames, string szEVENodeToCheck)
        {
            //  Check if other EVE configuration files are present in the GameDatabase.
            //
            //  Configuration files that refer to non-existent bodies do (and WILL) break
            //  the EVE loader.

            int nEVENodeCount = 0;

            if (string.IsNullOrEmpty(szEVENodeToCheck)) return nEVENodeCount;
            //  Scan the GameDatabase for all loaded EVE configuration files.

            foreach (ConfigNode EVENode in GameDatabase.Instance.GetConfigNodes(szEVENodeToCheck))
            {
                //  Search for all available EVE body objects.

                foreach (ConfigNode EVECloudsObject in EVENode.GetNodes("OBJECT"))
                {
                    if (EVENode != null && EVECloudsObject.HasValue("body"))
                    {
                        // Get the raw body name from the body object.

                        string szBodyName = EVECloudsObject.GetValue("body");

                        //  Check if the body name exists in the celestial body database.

                        if (Array.IndexOf(szBodyLoaderNames.ToArray(), szBodyName.ToLower()) < 0)
                        {
                            //  Print the invalid body name (for debug purposes).

                            if (Utilities.IsVerboseDebugEnabled)
                            {
                                Notification.Logger(Constants.AssemblyName, "Warning",
                                    string.Format("Incompatible {0} body detected (name: {1})!", szEVENodeToCheck,
                                        szBodyName));
                            }

                            //  Remove the invalid EVE configuration file from the
                            //  GameDatabase.
                            //
                            //  Note: this actually removes the offending ConfigNode
                            //  **completely** from the GameDatabase (and so from the
                            //  actual .cfg file).
                            //
                            //  Will need to find a better solution for this in the
                            //  future but for now this will do the job just fine.

                            EVENode.ClearNodes();
                        }
                    }
                }

                //  Increment the EVE node counter.

                nEVENodeCount++;
            }

            //  Return the number of EVE configuration files found (default: zero).

            return nEVENodeCount;
        }

        /// <summary>
        /// Method to validate a set of EVE configuration files.
        /// </summary>
        /// <param name = "szBodyLoaderNames">A list with all celestial body names found in the GameDatabase</param>
        /// <returns>
        /// Does not return a value.
        /// </returns>
        public static void GetValidateConfig(List<string> szBodyLoaderNames)
        {
            string[] szEVEConfigToCheck =
                {"EVE_ATMOSPHERE", "EVE_CITY_LIGHTS", "EVE_CLOUDS", "EVE_SHADOWS", "EVE_TERRAIN", "PQS_MANAGER"};

            foreach (string szEVENodeToCheck in szEVEConfigToCheck)
            {
                //  Before we start, check if the celestial body list has been populated.

                if (szBodyLoaderNames.Count > 0)
                {
                    //  Try to validate each one of the EVE objects.

                    int nEVENodesFound = GetCheckConfig(szBodyLoaderNames, szEVENodeToCheck);

                    //  Make a note if no EVE configuration files of that type have been installed.

                    if (nEVENodesFound == 0)
                    {
                        Notification.Logger(Constants.AssemblyName, "Warning",
                            $"No {szEVENodeToCheck} configuration files found!");
                    }
                    else
                    {
                        //  Print the total number of EVE configuration files loaded (for debug purposes).

                        if (Utilities.IsVerboseDebugEnabled)
                        {
                            Notification.Logger(Constants.AssemblyName, null,
                                $"{szEVENodeToCheck} configuration file found (count: {nEVENodesFound})!");
                        }
                    }
                }
            }
        }
    }
}