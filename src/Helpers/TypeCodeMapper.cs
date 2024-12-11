// <copyright file="TypeCodeMapper.cs" company="Neil Enns">
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
    /// Maps Asobo type codes names to ICAO type codes.
    /// </summary>
    public class TypeCodeMapper
    {
        private static readonly Lazy<TypeCodeMapper> MapperInstance = new(() => new TypeCodeMapper());
        private List<TypeCodeEntry> typeCodeList;

        // Private constructor to prevent instantiation
        private TypeCodeMapper()
        {
            this.LoadTypeCodeData();
        }

        /// <summary>
        /// Gets an instance of the TypeCodeMapper.
        /// </summary>
        public static TypeCodeMapper Instance => MapperInstance.Value;

        /// <summary>
        /// Returns the ICAO type code code for a given Asobo type code.
        /// </summary>
        /// <param name="asoboTypeCode">The Asobo type cod.</param>
        /// <returns>The ICAO type code, or null if not found.</returns>
        public string GetTypeCode(string asoboTypeCode)
        {
            if (string.IsNullOrEmpty(asoboTypeCode))
            {
                return null;
            }

            return this.typeCodeList
                .FirstOrDefault(entry => entry.AsoboTypeCode == asoboTypeCode)?.IcaoTypeCode;
        }

        // Loads data from JSON file
        private void LoadTypeCodeData()
        {
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data\\typecodes.json");
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"The file '{jsonFilePath}' was not found.");
            }

            var jsonString = File.ReadAllText(jsonFilePath);
            this.typeCodeList = JsonSerializer.Deserialize<List<TypeCodeEntry>>(jsonString) ?? throw new InvalidOperationException("Failed to parse the JSON file.");
        }

        // Class to represent the JSON objects
        private class TypeCodeEntry
        {
            public string AsoboTypeCode { get; set; }

            public string IcaoTypeCode { get; set; }
        }
    }
}