// <copyright file="GetLiveriesCommand.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.ViewModels.ModelMatching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Microsoft.FlightSimulator.SimConnect;
    using VmrGenerator.Helpers;
    using VmrGenerator.Models;

    /// <summary>
    /// Implements the GetLiveries command for the view model.
    /// </summary>
    public partial class ModelMatchingViewModel
    {
        private RelayCommand getLiveriesCommand;

        /// <summary>
        /// Gets the command.
        /// </summary>
        public ICommand GetLiveriesCommand => this.getLiveriesCommand ??= new RelayCommand(
            param => this.GetLiveries(),
            param => this.CanGetLiveries());

        /// <summary>
        /// Sends a request to the sim for the list of liveries. This is an async process.
        /// Items will be added to the Liveries property as they are received.
        /// </summary>
        public void GetLiveries()
        {
            if (!this.IsConnected || this.SimConnect == null)
            {
                return;
            }

            this.SimConnect.EnumerateSimObjectsAndLiveries(RequestID.GetInputEvents, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT);
        }

        /// <summary>
        /// True if the GetLiveries method can be called.
        /// </summary>
        /// <returns>True or false.</returns>
        public bool CanGetLiveries()
        {
            return this.IsConnected && this.SimConnect != null;
        }

        private void SimConnect_OnRecvEnumerateSimobjectAndLiveryList(SimConnect sender, SIMCONNECT_RECV_ENUMERATE_SIMOBJECT_AND_LIVERY_LIST data)
        {
            List<Livery> liveriesToAdd = [];

            foreach (SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY item in data.rgData.Cast<SIMCONNECT_ENUMERATE_SIMOBJECT_LIVERY>())
            {
                liveriesToAdd.Add(new Livery()
                {
                    ModelName = item.AircraftTitle,
                    TypeCode = item.LiveryName,
                });

                Console.WriteLine(item);
            }

            this.Liveries.AddRange(liveriesToAdd);
        }

        /// <summary>
        /// Handles receiving input events from the simulator.
        /// </summary>
        /// <param name="sender">Sender of the exception.</param>
        /// <param name="data">SimConnect additional details.</param>
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
    }
}
