#nullable enable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using vmr_generator.Interfaces;
using vmr_generator.Models;
using vmr_generator.Helpers;
using System.Timers;
using System.Resources;
using System.Collections.Generic;
using System.Linq;

namespace vmr_generator.ViewModels.ModelMatching
{
    [XmlRoot("ModelMatchRuleSet")]
    public partial class ModelMatchingViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Provides localized strings for various properties in the view model.
        /// </summary>
        private ResourceManager _resourceManager;

        /// <summary>
        /// Timer that polls to see if the simulator is running.
        /// </summary>
        private Timer _checkForSimTimer;

        /// <summary>
        /// Provides access to a message box to display information to the user.
        /// </summary>
        public IMessageBoxService? MessageBoxService { get; set; }

        /// <summary>
        /// Provides access to a file save dialog to get a file name from the user.
        /// </summary>
        public ISaveDialogService? SaveDialogService { get; set; }

        /// <summary>
        /// The window handle of the parent view. Must be set before calling any
        /// of the SimConnect commands.
        /// </summary>
        public IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Fires when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _errorMessage;

        /// <summary>
        /// Provides the error text for any errors encountered by the view model.
        /// </summary>
        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                Debug.WriteLine(value);
                MessageBoxService?.ShowError(value, _resourceManager.GetString("MessageBoxErrorTitle"));
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        /// <summary>
        /// The list of all liveries retrieved from MSFS2024.
        /// </summary>
        public RangeObservableCollection<Livery> Liveries { get; set; } = [];

        /// <summary>
        /// Returns a list of Liveries flattened into single entries by CallsignPrefix and TypeCode.
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
                return Liveries.GroupBy(l => new { l.CallsignPrefix, l.TypeCode, l.FlightNumberRange }).Select(g => new Livery
                {
                    CallsignPrefix = g.Key.CallsignPrefix,
                    TypeCode = g.Key.TypeCode,
                    FlightNumberRange = g.Key.FlightNumberRange,
                    ModelName = string.Join("//", g.Select(l => l.ModelName))
                }).ToList();
            }
        }

        private bool _isConnected;
        /// <summary>
        /// True if SimConnect is connected to MSFS2024.
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(SimConnectedStateMessage));
                }
            }
        }

        private bool _isSimRunning;
        /// <summary>
        /// True if Microsoft Flight Simulator 2024 is running.
        /// </summary>
        public bool IsSimRunning
        {
            get => _isSimRunning;
            set
            {
                if (_isSimRunning != value)
                {
                    _isSimRunning = value;

                    OnPropertyChanged(nameof(IsSimRunning));
                }

                // This ensures SimConnect keeps trying to connect to the sim when it is
                // running but a connection hasn't been established. This happens when
                // the sim first launches, but isn't ready to accept incoming SimConnect
                // requests.
                if (value && !IsConnected)
                {
                    ConnectToSim();
                }
            }
        }

        public string SimConnectedStateMessage
        {
            get
            {
                if (IsConnected)
                {
                    return _resourceManager.GetString("SimConnectedMessage") ?? "";
                }
                else
                {
                    return _resourceManager.GetString("WaitingForSimulatorMessage") ?? "";
                }
            }
        }

        public ModelMatchingViewModel()
        {

            _resourceManager = new ResourceManager("vmr_generator.Properties.Resources", typeof(ModelMatchingViewModel).Assembly);

            // Set up a timer to check for the sim every second.
            _checkForSimTimer = new Timer(1000);
            _checkForSimTimer.Elapsed += CheckForSim;
            _checkForSimTimer.Start();
        }

        /// <summary>
        /// Adds a bunch of sample data to the list of liveries for testing purposes.
        /// </summary>
        private void AddSampleData()
        {
            Liveries.AddRange([
                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CL60" ModelName="FSLTL_GA_C25C_ZZZ//FSLTL_GA_C25C_M-MIKE//FSLTL_GA_C25C_PS-CSF" /> 
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_ZZZ",
                    TypeCode = "CL60",
                },
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_M-MIKE",
                    TypeCode = "CL60",
                },
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_GA_C25C_PS-CSF",
                    TypeCode = "CL60",
                },
                // <ModelMatchRule CallsignPrefix="AIB" TypeCode="CRJX" ModelName="FSLTL_CRJ7_ZZZZ" /> 
                new Livery() {
                    CallsignPrefix = "AIB",
                    ModelName = "FSLTL_CRJ7_ZZZZ",
                    TypeCode = "CRJX",
                },
                // <ModelMatchRule TypeCode="C172" ModelName="FSLTL_GA_C172_ZZZ" /> 
                new Livery() {
                    ModelName = "FSLTL_GA_C172_ZZZ",
                    TypeCode = "C172",
                },
                // <ModelMatchRule CallsignPrefix="DAL" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_SSW//FSLTL_FAIB_B739_DAL-Delta_WL" /> 
                new Livery() {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_SSW",
                    TypeCode = "B739",
                },
                new Livery() {
                    CallsignPrefix = "DAL",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                },
                // Should be standalone
                // <ModelMatchRule CallsignPrefix="DAL" FlightNumberRange="4439-4858" TypeCode="B739" ModelName="FSLTL_FAIB_B739_DAL-Delta_WL" />
                new Livery() {
                    CallsignPrefix = "DAL",
                    FlightNumberRange = "4439-4858",
                    ModelName = "FSLTL_FAIB_B739_DAL-Delta_WL",
                    TypeCode = "B739",
                },
            ]);
        }

        private void CheckForSim(object? sender, ElapsedEventArgs e)
        {
            IsSimRunning = Process.GetProcessesByName("flightsimulator2024").Length > 0;
        }

        /// <summary>
        /// Saves the list of models to the specified file as XML.
        /// </summary>
        /// <param name="fileName">The file name to save the XML to</param>
        public void ToXml(string fileName)
        {
            var xmlRoot = new XmlRootAttribute("ModelMatchRuleSet");
            var serializer = new XmlSerializer(typeof(List<Livery>), xmlRoot);

            using var writer = new StreamWriter(fileName);
            serializer.Serialize(writer, FlattenedList);
            Debug.WriteLine($"Saved to {fileName}");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested(); // Refresh CanExecute for commands
        }
    }
}
