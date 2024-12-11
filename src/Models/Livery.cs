// <copyright file="Livery.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Models
{
    using System.Xml.Serialization;
    using VmrGenerator.Helpers;

    /// <summary>
    /// Information about an individual livery in MSFS2024.
    /// </summary>
    [XmlType("ModelMatchRule")]
    public class Livery
    {
        private string liveryName;

        /// <summary>
        /// Gets or sets the airline the livery is for.
        /// </summary>
        /// <example>ASA for Alaska Airlines liveries.</example>
        [XmlAttribute("CallsignPrefix")]
        public string CallsignPrefix { get; set; }

        /// <summary>
        /// Gets or sets the flight number range for the livery.
        /// </summary>
        /// <example>1200-1300.</example>
        [XmlAttribute("FlightNumberRange")]
        public string FlightNumberRange { get; set; }

        /// <summary>
        /// Gets or sets the type code for the livery.
        /// </summary>
        /// <example>B738.</example>
        [XmlAttribute("TypeCode")]

        public string TypeCode { get; set; }

        /// <summary>
        /// Gets or sets the model name for the livery.
        /// </summary>
        /// <example>Asobo PassiveAircraft B777-300ER.</example>
        [XmlAttribute("ModelName")]

        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the TypeCode that comes from Asobo.
        /// </summary>
        [XmlIgnore]
        public string AsoboTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the Airline that comes from Asobo.
        /// </summary>
        [XmlIgnore]
        public string AsoboAirline { get; set; }

        /// <summary>
        /// Gets or sets the livery name.
        /// </summary>
        /// <example>B777_300ER_UNITEDAIRLINES.</example>
        [XmlAttribute("LiveryName")]
        public string LiveryName
        {
            get => this.liveryName;
            set
            {
                if (this.liveryName != value)
                {
                    this.liveryName = value;
                    this.ExtractTypeCodeAndAirline(value);
                }
            }
        }

        /// <summary>
        /// Sets AsoboTypeCode and AsoboAirline to the appropriate values from a livery name.
        /// Assumes the Airline is everything after the last _ and the TypeCode is everything
        /// before the last _.
        /// </summary>
        /// <param name="liveryName">The livery name to process.</param>
        private void ExtractTypeCodeAndAirline(string liveryName)
        {
            var cleanedLiveryName = liveryName.Replace("_AIRLINES", string.Empty);
            int lastUnderscore = cleanedLiveryName.LastIndexOf('_');

            if (lastUnderscore == -1)
            {
                return;
            }

            this.AsoboAirline = cleanedLiveryName[(lastUnderscore + 1)..];
            this.AsoboTypeCode = cleanedLiveryName[..lastUnderscore];
            this.CallsignPrefix = AirlineMapper.Instance.GetAirline(this.AsoboAirline) ?? string.Empty;
            this.TypeCode = TypeCodeMapper.Instance.GetTypeCode(this.AsoboTypeCode) ?? string.Empty;
        }
    }
}