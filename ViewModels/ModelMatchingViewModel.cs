#nullable enable
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace vmr_generator.ViewModels
{
    [XmlRoot("ModelMatchRuleSet")]
    public class ModelMatchingViewModel : INotifyPropertyChanged
    {
        const int WM_USER_SIMCONNECT = 0x0402;
        SimConnect? simConnect;

        /// <summary>
        /// Fires when the value of a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The list of all liveries retrieved from MSFS2024.
        /// </summary>
        [XmlElement("ModelMatchRule")]
        public ObservableCollection<Livery> Liveries { get; set; } = [];

        private bool _isConnected;
        /// <summary>
        /// True if SimConnect is connected to MSFS2024.
        /// </summary>
        [XmlIgnore]
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
            Liveries.Add(new Livery()
            {
                ModelName = "Hello",
                TypeCode = "B172"
            }
            );
        }

        /// <summary>
        /// Opens a connection to the simulator.
        /// </summary>
        /// <param name="handle">The window handle of the app</param>
        public void ConnectToSim(IntPtr handle)
        {
            if (IsConnected)
            {
                return;
            }

            this.simConnect = new SimConnect("WMR Generator", handle, WM_USER_SIMCONNECT, null, 0);
            this.simConnect.OnRecvOpen += SimConnect_OnRecvOpen;
            this.simConnect.OnRecvQuit += SimConnect_OnRecvQuit;
            this.simConnect.OnRecvException += SimConnect_OnRecvException;
        }

        public void ToXml()
        {
            var serializer = new XmlSerializer(typeof(ModelMatchingViewModel));

            using var writer = new StringWriter();
            serializer.Serialize(writer, this);
            Debug.WriteLine(writer.ToString());
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Debug.WriteLine($"SimConnect exception {data.dwException}");
        }

        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            this.IsConnected = false;
            Debug.WriteLine("Disconnected from simulator.");
        }

        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.IsConnected = true;
            Debug.WriteLine("Connected to simulator.");
        }

        /// <summary>
        /// Handles Win32 messages and looks for SimConnect specific messages.
        /// </summary>
        /// <param name="message">The message to process</param>
        /// <returns>True if processed</returns>
        public IntPtr HandleWindowsEvent(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (message)
            {
                case WM_USER_SIMCONNECT:
                    {
                        if (simConnect == null)
                        {
                            handled = true;
                            return IntPtr.Zero;
                        }

                        try
                        {
                            this.simConnect.ReceiveMessage();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error processing SimConnect message: {ex.Message}");
                        }

                        handled = true;
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            return IntPtr.Zero;
        }

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}