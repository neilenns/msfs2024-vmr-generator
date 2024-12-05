#nullable enable
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
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
        enum RequestID
        {
            GetInputEvents
        }

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
        public RangeObservableCollection<Livery> Liveries { get; set; } = [];

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
            this.simConnect.OnRecvEnumerateInputEvents += SimConnect_OnRecvEnumerateInputEvents; ;
        }

        /// <summary>
        /// Sends a request to the sim for the list of liveries. This is an async process.
        /// Items will be added to the Liveries property as they are received.
        /// </summary>
        public void GetLiveries()
        {
            if (!IsConnected || this.simConnect == null)
            {
                return;
            }

            this.simConnect.EnumerateInputEvents(RequestID.GetInputEvents);
        }

        /// <summary>
        /// Saves the list of models to the specified file as XML.
        /// </summary>
        /// <param name="fileName">The file name to save the XML to</param>
        public void ToXml(string fileName)
        {
            var serializer = new XmlSerializer(typeof(ModelMatchingViewModel));

            using var writer = new StreamWriter(fileName);
            serializer.Serialize(writer, this);
            Debug.WriteLine($"Saved to {fileName}");
        }

        /// <summary>
        /// Handles exceptions received from SimConnect.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">Details of the exception</param>
        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Debug.WriteLine($"SimConnect exception {data.dwException}");
        }

        /// <summary>
        /// Handles the loss of connection to the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">SimConnect additional details</param>
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            this.IsConnected = false;
            Debug.WriteLine("Disconnected from simulator.");
        }

        /// <summary>
        /// Handles when a connection is established with the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">SimConnect additional details</param>
        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.IsConnected = true;
            Debug.WriteLine("Connected to simulator.");
        }

        /// <summary>
        /// Handles receiving input events from the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">SimConnect additional details</param>
        private void SimConnect_OnRecvEnumerateInputEvents(SimConnect sender, SIMCONNECT_RECV_ENUMERATE_INPUT_EVENTS data)
        {
            List<Livery> liveriesToAdd = [];

            foreach (object item in data.rgData)
            {
                if (item is SIMCONNECT_INPUT_EVENT_DESCRIPTOR descriptor)
                {
                    liveriesToAdd.Add(new Livery() { 
                        ModelName = descriptor.Name,
                        TypeCode = descriptor.Hash.ToString(),
                    });
                }
            }

            this.Liveries.AddRange(liveriesToAdd);
        }

        /// <summary>
        /// Looks for SimConnect specific messages from Win32 and makes the SimConnect library process them.
        /// </summary>
        /// <param name="message">The message to process</param>
        /// <returns>True if processed</returns>
#pragma warning disable IDE0060 // Remove unused parameter
        public IntPtr HandleWindowsEvent(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
#pragma warning restore IDE0060 // Remove unused parameter
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