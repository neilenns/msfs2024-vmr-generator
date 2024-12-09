#nullable enable
namespace VmrGenerator.ViewModels.ModelMatching
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.FlightSimulator.SimConnect;

    /// <summary>
    /// Implements the SimConnect methods of the view model.
    /// </summary>
    public partial class ModelMatchingViewModel
    {
        private const int WMUSERSIMCONNECT = 0x0402;

        private SimConnect? simConnect;

        private enum RequestID
        {
            GetInputEvents,
        }

        /// <summary>
        /// Gets or sets connection to the simulator.
        /// </summary>
        // This gets initialized in ConnectCommand.cs.
        public SimConnect? SimConnect
        {
            get => this.simConnect;
            set
            {
                if (this.simConnect != value)
                {
                    this.simConnect = value;
                    this.OnPropertyChanged(nameof(this.SimConnect));
                }
            }
        }

        /// <summary>
        /// Opens a connection to the simulator.
        /// </summary>
        /// <param name="handle">The window handle of the app.</param>
        public void ConnectToSim()
        {
            if (!this.IsSimRunning || this.IsConnected || this.WindowHandle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                this.SimConnect = new SimConnect("WMR Generator", this.WindowHandle, WMUSERSIMCONNECT, null, 0);
                this.SimConnect.OnRecvOpen += this.SimConnect_OnRecvOpen;
                this.SimConnect.OnRecvQuit += this.SimConnect_OnRecvQuit;
                this.SimConnect.OnRecvException += this.SimConnect_OnRecvException;
                this.SimConnect.OnRecvEnumerateInputEvents += this.SimConnect_OnRecvEnumerateInputEvents;
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

                this.ErrorMessage = string.Format(this.resourceManager.GetString("SimConnectionErrror") ?? string.Empty, ex.Message);
            }
        }

        /// <summary>
        /// Looks for SimConnect specific messages from Win32 and makes the SimConnect library process them.
        /// </summary>
        /// <param name="hwnd">The window handle for the window that received the message.</param>
        /// <param name="message">The message to process.</param>
        /// <param name="wParam">The first message parameter.</param>
        /// <param name="lParam">The second message parameter.</param>
        /// <param name="handled">Set to true if the method handled the event.</param>
        /// <returns>True if processed.</returns>
        public IntPtr HandleWindowsEvent(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (message)
            {
                case WMUSERSIMCONNECT:
                    {
                        if (this.SimConnect == null)
                        {
                            handled = true;
                            return IntPtr.Zero;
                        }

                        try
                        {
                            this.SimConnect.ReceiveMessage();
                        }
                        catch (Exception ex)
                        {
                            this.IsConnected = false;

                            // This happens when the sim is closed
                            if (ex.HResult == -1073741648)
                            {
                                Debug.WriteLine("Lost connection to the simulator.");
                            }
                            else
                            {
                                this.ErrorMessage = string.Format(this.resourceManager.GetString("ReceiveMessageExceptionMessage") ?? string.Empty, ex.Message);
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

        /// <summary>
        /// Handles exceptions received from SimConnect.
        /// </summary>
        /// <param name="sender">Sender of the exception.</param>
        /// <param name="data">Details of the exception.</param>
        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            this.ErrorMessage = string.Format(this.resourceManager.GetString("OnRecvExceptionMessage") ?? string.Empty, data.dwException);
        }

        /// <summary>
        /// Handles the loss of connection to the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception.</param>
        /// <param name="data">SimConnect additional details.</param>
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            this.IsConnected = false;
            Debug.WriteLine("Disconnected from simulator.");
        }

        /// <summary>
        /// Handles when a connection is established with the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception.</param>
        /// <param name="data">SimConnect additional details.</param>
        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.IsConnected = true;
            Debug.WriteLine("Connected to simulator.");
        }
    }
}