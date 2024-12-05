using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.FlightSimulator.SimConnect;
using vmr_generator.Models;
using vmr_generator.Helpers;

namespace vmr_generator.ViewModels.ModelMatching
{
  public partial class ModelMatchingViewModel
  {
    RelayCommand _getLiveriesCommand;


    public ICommand GetLiveriesCommand => _getLiveriesCommand ??= new RelayCommand(param => GetLiveries(), param => CanGetLiveries());


    /// <summary>
    /// Sends a request to the sim for the list of liveries. This is an async process.
    /// Items will be added to the Liveries property as they are received.
    /// </summary>
    public void GetLiveries()
    {
      if (!IsConnected || SimConnect == null)
      {
        return;
      }

      SimConnect.EnumerateInputEvents(RequestID.GetInputEvents);
    }

    /// <summary>
    /// True if the GetLiveries method can be called.
    /// </summary>
    /// <returns>True or false</returns>
    public bool CanGetLiveries()
    {
      return IsConnected && SimConnect != null;
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

      Liveries.AddRange(liveriesToAdd);
    }
  }
}