// <copyright file="ModelMatchingViewModel.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

namespace VmrGenerator.ViewModels.ModelMatching
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Timers;
    using System.Windows.Input;
    using System.Xml.Serialization;
    using VmrGenerator.Helpers;
    using VmrGenerator.Interfaces;
    using VmrGenerator.Models;

    /// <summary>
    /// View model for the main window of the application.
    /// </summary>
    [XmlRoot("ModelMatchRuleSet")]
    public partial class ModelMatchingViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Timer that polls to see if the simulator is running.
        /// </summary>
        private readonly System.Timers.Timer checkForSimTimer;

        private string? errorMessage;

        private bool isConnected;

        private bool isSimRunning;

        /// <summary>
        /// Provides localized strings for various properties in the view model.
        /// </summary>
        private ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelMatchingViewModel"/> class.
        /// </summary>
        public ModelMatchingViewModel()
        {
            this.resourceManager = new ResourceManager("VmrGenerator.Properties.Resources", typeof(ModelMatchingViewModel).Assembly);

            // Set up a timer to check for the sim every second.
            this.checkForSimTimer = new System.Timers.Timer(1000);
            this.checkForSimTimer.Elapsed += this.CheckForSim;
            this.checkForSimTimer.Start();
        }

        /// <summary>
        /// Fires when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a message box to display information to the user.
        /// </summary>
        public IMessageBoxService? MessageBoxService { get; set; }

        /// <summary>
        /// Gets or sets a file save dialog to get a file name from the user.
        /// </summary>
        public ISaveDialogService? SaveDialogService { get; set; }

        /// <summary>
        /// Gets or sets the window handle of the parent view. Must be set before calling any
        /// of the SimConnect commands.
        /// </summary>
        public IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Gets or sets the error text for any errors encountered by the view model.
        /// </summary>
        public string? ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                this.errorMessage = value;
                Debug.WriteLine(value);
                this.MessageBoxService?.ShowError(value, this.resourceManager.GetString("MessageBoxErrorTitle"));
                this.OnPropertyChanged(nameof(this.ErrorMessage));
            }
        }

        /// <summary>
        /// Gets or sets a list of all liveries retrieved from MSFS2024.
        /// </summary>
        public RangeObservableCollection<Livery> Liveries { get; set; } = [];

        /// <summary>
        /// Gets a list of Liveries flattened into single entries by CallsignPrefix and TypeCode.
        /// Multiple entries with the same CallsignPrefix and TypeCode are grouped into a single entry
        /// with the ModelName values separated by two slashes (//).
        ///
        /// This list can then be directly written to XML to be in the correct format for a vPilot model
        /// matching file.
        /// </summary>
        public List<Livery> FlattenedList
        {
            get
            {
                return [.. this.Liveries.GroupBy(l => new { l.CallsignPrefix, l.TypeCode, l.FlightNumberRange }).Select(g => new Livery
                {
                    CallsignPrefix = g.Key.CallsignPrefix,
                    TypeCode = g.Key.TypeCode,
                    FlightNumberRange = g.Key.FlightNumberRange,
                    ModelName = string.Join("//", g.Select(l => l.ModelName)),
                })];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SimConnect is connected to MSFS2024.
        /// </summary>
        public bool IsConnected
        {
            get => this.isConnected;
            set
            {
                // Only poll for a sim connection when the sim isn't connected.
                this.checkForSimTimer.Enabled = !value;

                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.OnPropertyChanged(nameof(this.IsConnected));
                    this.OnPropertyChanged(nameof(this.SimConnectedStateMessage));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Microsoft Flight Simulator 2024 is running.
        /// </summary>
        public bool IsSimRunning
        {
            get => this.isSimRunning;
            set
            {
                if (this.isSimRunning != value)
                {
                    this.isSimRunning = value;

                    this.OnPropertyChanged(nameof(this.IsSimRunning));
                }

                // This ensures SimConnect keeps trying to connect to the sim when it is
                // running but a connection hasn't been established. This happens when
                // the sim first launches, but isn't ready to accept incoming SimConnect
                // requests.
                if (value && !this.IsConnected)
                {
                    this.ConnectToSim();
                }
            }
        }

        /// <summary>
        /// Gets a localized string indicating the current sim connected state.
        /// </summary>
        public string SimConnectedStateMessage
        {
            get
            {
                if (this.IsConnected)
                {
                    return this.resourceManager.GetString("SimConnectedMessage") ?? string.Empty;
                }
                else
                {
                    return this.resourceManager.GetString("WaitingForSimulatorMessage") ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Adds a bunch of sample data to the list of liveries for testing purposes.
        /// </summary>
        public void AddSampleData()
        {
            this.Liveries.AddRange([

                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CL60" ModelName="FSLTL_GA_C25C_ZZZ//FSLTL_GA_C25C_M-MIKE//FSLTL_GA_C25C_PS-CSF" />
                new Livery()
                {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_ZZZ",
                    TypeCode = "CL60",
                },
                new Livery()
                {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_M-MIKE",
                    TypeCode = "CL60",
                },
                new Livery()
                {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_PS-CSF",
                    TypeCode = "CL60",
                },

                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CRJX" ModelName="FSLTL_CRJ7_ZZZZ" />
                new Livery()
                {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_CRJ7_ZZZZ",
                    TypeCode = "CRJX",
                },

                // <ModelMatchRule TypeCode="C172" ModelName="FSLTL_GA_C172_ZZZ" />
                new Livery()
                {
                    ModelName = "FSLTL_GA_C172_ZZZ",
                    TypeCode = "C172",
                },

                // <ModelMatchRule CallsignPrefix="DAL" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_SSW//FSLTL_FAIB_B739_DAL-Delta_WL" />
                new Livery()
                {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_SSW",
                    TypeCode = "B739",
                },
                new Livery()
                {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                },

                // Should be standalone
                // <ModelMatchRule CallsignPrefix="DAL" FlightNumberRange="4439-4858" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_WL" />
                new Livery()
                {
                    CallsignPrefix = "DAL",
                    FlightNumberRange = "4439-4858",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                },
            ]);
        }

        /// <summary>
        /// Saves the list of models to the specified file as XML.
        /// </summary>
        /// <param name="fileName">The file name to save the XML to.</param>
        public void ToXml(string fileName)
        {
            var xmlRoot = new XmlRootAttribute("ModelMatchRuleSet");
            var serializer = new XmlSerializer(typeof(List<Livery>), xmlRoot);

            using var writer = new StreamWriter(fileName);
            serializer.Serialize(writer, this.FlattenedList);
            Debug.WriteLine($"Saved to {fileName}");
        }

        /// <summary>
        /// Triggers the PropertyChanged event handler.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested(); // Refresh CanExecute for commands
        }

        private void CheckForSim(object? sender, ElapsedEventArgs e)
        {
            this.IsSimRunning = Process.GetProcessesByName("flightsimulator2024").Length > 0;
        }
    }
}
