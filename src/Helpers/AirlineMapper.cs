// <copyright file="AirlineMapper.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    /// <summary>
    /// Maps Asobo airline names to ICAO airline codes.
    /// </summary>
    public class AirlineMapper
    {
        private static readonly Lazy<AirlineMapper> MapperInstance = new(() => new AirlineMapper());
        private List<AirlineEntry> airlineList;

        // Private constructor to prevent instantiation
        private AirlineMapper()
        {
            this.LoadAirlineData();
        }

        /// <summary>
        /// Gets an instance of the AirlineMapper.
        /// </summary>
        public static AirlineMapper Instance => MapperInstance.Value;

        /// <summary>
        /// Returns the ICAO airline code for a given Asobo airline name.
        /// </summary>
        /// <param name="asoboAirline">The Asobo airline name.</param>
        /// <returns>The ICAO code for the airline, or null if not found.</returns>
        public string GetAirline(string asoboAirline)
        {
            if (string.IsNullOrEmpty(asoboAirline))
            {
                return null;
            }

            return this.airlineList
                .FirstOrDefault(entry => entry.AsoboAirline == asoboAirline)?.IcaoAirline;
        }

        // Loads data from JSON file
        private void LoadAirlineData()
        {
            string jsonFilePath = "Data\\airlines.json";
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"The file '{jsonFilePath}' was not found.");
            }

            var jsonString = File.ReadAllText(jsonFilePath);
            this.airlineList = JsonSerializer.Deserialize<List<AirlineEntry>>(jsonString) ?? throw new InvalidOperationException("Failed to parse the JSON file.");
        }

        // Class to represent the JSON objects
        private class AirlineEntry
        {
            public string AsoboAirline { get; set; }

            public string IcaoAirline { get; set; }
        }
    }
}