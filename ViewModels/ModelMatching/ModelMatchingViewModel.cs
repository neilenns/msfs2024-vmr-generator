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
            var serializer = new XmlSerializer(typeof(RangeObservableCollection<Livery>), xmlRoot);

            using var writer = new StreamWriter(fileName);
            serializer.Serialize(writer, Liveries);
            Debug.WriteLine($"Saved to {fileName}");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            CommandManager.InvalidateRequerySuggested(); // Refresh CanExecute for commands
        }
    }
}
