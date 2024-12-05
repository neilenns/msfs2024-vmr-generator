using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.FlightSimulator.SimConnect;
using vmr_generator.Helpers;

// This method of adding commands to view models comes from
// https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090051
namespace vmr_generator.ViewModels.ModelMatching
{
	public partial class ModelMatchingViewModel
	{
		RelayCommand _connectCommand;

		/// <summary>
		/// Command to open a connection to the simulator. Must be called before any simulator-related APIs are called.
		/// </summary>
		public ICommand ConnectCommand => _connectCommand ??= new RelayCommand(param => ConnectToSim(), param => CanConnectToSim());

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
					Debug.WriteLine($"Error connecting to the simulator, it probably isn't running: {ex.Message}");
					return;
				}

				ErrorMessage = String.Format(_resourceManager.GetString("SimConnectionErrror"), ex.Message);
			}
		}

		public bool CanConnectToSim()
		{
			return !(IsConnected || WindowHandle == IntPtr.Zero);
		}
	}
}
