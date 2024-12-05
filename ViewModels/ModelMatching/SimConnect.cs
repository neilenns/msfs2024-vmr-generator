#nullable enable
using System;
using System.Collections.Generic;
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

		const int WM_USER_SIMCONNECT = 0x0402;
		SimConnect? _simConnect;

		/// <summary>
		/// Opens a connection to the simulator.
		/// </summary>
		/// <param name="handle">The window handle of the app</param>
		public void ConnectToSim()
		{
			if (IsConnected || WindowHandle == IntPtr.Zero)
			{
				return;
			}

			try
			{
				_simConnect = new SimConnect("WMR Generator", WindowHandle, WM_USER_SIMCONNECT, null, 0);
				_simConnect.OnRecvOpen += SimConnect_OnRecvOpen;
				_simConnect.OnRecvQuit += SimConnect_OnRecvQuit;
				_simConnect.OnRecvException += SimConnect_OnRecvException;
				_simConnect.OnRecvEnumerateInputEvents += SimConnect_OnRecvEnumerateInputEvents;
			}
			catch (COMException ex)
			{
				ErrorMessage = $"Error connecting to simulator: {ex.Message}";
			}
		}

		public bool CanConnectToSim()
		{
			return !(IsConnected || WindowHandle == IntPtr.Zero);
		}

		/// <summary>
		/// Sends a request to the sim for the list of liveries. This is an async process.
		/// Items will be added to the Liveries property as they are received.
		/// </summary>
		public void GetLiveries()
		{
			if (!IsConnected || _simConnect == null)
			{
				return;
			}

			_simConnect.EnumerateInputEvents(RequestID.GetInputEvents);
		}

		/// <summary>
		/// Handles exceptions received from SimConnect.
		/// </summary>
		/// <param name="sender">Sender of the exception</param>
		/// <param name="data">Details of the exception</param>
		private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
		{
			ErrorMessage = $"Error receiving data from simulator: {data.dwException}";
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
					liveriesToAdd.Add(new Livery()
					{
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
						if (_simConnect == null)
						{
							handled = true;
							return IntPtr.Zero;
						}

						try
						{
							this._simConnect.ReceiveMessage();
						}
						catch (Exception ex)
						{
							ErrorMessage = $"Error receiving data from simulator: {ex.Message}";
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