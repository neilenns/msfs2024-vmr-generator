#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Xml.Serialization;
using vmr_generator.Interfaces;
using vmr_generator.Models;
using vmr_generator.Helpers;

namespace vmr_generator.ViewModels.ModelMatching
{
    [XmlRoot("ModelMatchRuleSet")]
    public partial class ModelMatchingViewModel : INotifyPropertyChanged
    {
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
                MessageBoxService?.ShowError(value, "Error");
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
                }
            }
        }

        public ModelMatchingViewModel()
        {
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
