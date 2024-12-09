#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.FlightSimulator.SimConnect;

namespace vmr_generator.ViewModels.ModelMatching
{
    public partial class ModelMatchingViewModel
    {
        enum RequestID
        {
            GetInputEvents
        }

        private SimConnect? _simConnect;

        // This gets initialized in ConnectCommand.cs.
        /// <summary>
        /// The connection to the simulator.
        /// </summary>
        public SimConnect? SimConnect
        {
            get => _simConnect;
            set
            {
                if (_simConnect != value)
                {
                    _simConnect = value;
                    OnPropertyChanged(nameof(SimConnect));
                }
            }
        }

        const int WM_USER_SIMCONNECT = 0x0402;

        /// <summary>
        /// Opens a connection to the simulator.
        /// </summary>
        /// <param name="handle">The window handle of the app</param>
        public void ConnectToSim()
        {
            if (!IsSimRunning || IsConnected || WindowHandle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                SimConnect = new SimConnect("WMR Generator", WindowHandle, WM_USER_SIMCONNECT, null, 0);
                SimConnect.OnRecvOpen += SimConnect_OnRecvOpen;
                SimConnect.OnRecvQuit += SimConnect_OnRecvQuit;
                SimConnect.OnRecvException += SimConnect_OnRecvException;
                SimConnect.OnRecvEnumerateInputEvents += SimConnect_OnRecvEnumerateInputEvents;
            }
            catch (COMException ex)
            {
                // This is just when the sim isn't running, no need to warn the user just log
                // it to debug and return.
                if (ex.HResult == -2147467259)
                {
                    Debug.WriteLine("Error connecting to the simulator, it probably isn't running or ready to accept connections.");
                    return;
                }

                ErrorMessage = String.Format(_resourceManager.GetString("SimConnectionErrror") ?? "", ex.Message);
            }
        }

        /// <summary>
        /// Handles exceptions received from SimConnect.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">Details of the exception</param>
        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            ErrorMessage = String.Format(_resourceManager.GetString("OnRecvExceptionMessage") ?? "", data.dwException);
        }

        /// <summary>
        /// Handles the loss of connection to the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">SimConnect additional details</param>
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            IsConnected = false;
            Debug.WriteLine("Disconnected from simulator.");
        }

        /// <summary>
        /// Handles when a connection is established with the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception</param>
        /// <param name="data">SimConnect additional details</param>
        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            IsConnected = true;
            Debug.WriteLine("Connected to simulator.");
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
                        if (SimConnect == null)
                        {
                            handled = true;
                            return IntPtr.Zero;
                        }

                        try
                        {
                            SimConnect.ReceiveMessage();
                        }
                        catch (Exception ex)
                        {
                            IsConnected = false;

                            // This happens when the sim is closed
                            if (ex.HResult == -1073741648)
                            {
                                Debug.WriteLine("Lost connection to the simulator.");
                            }
                            else
                            {
                                ErrorMessage = String.Format(_resourceManager.GetString("ReceiveMessageExceptionMessage") ?? "", ex.Message);
                            }
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

    }
}