// <copyright file="SaveLiveriesCommand.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.ViewModels.ModelMatching
{
    using System.Windows.Input;
    using VmrGenerator.Helpers;

    /// <summary>
    /// Implements the SaveLiveries command for the view model.
    /// </summary>
    public partial class ModelMatchingViewModel
    {
        private RelayCommand saveLiveriesCommand;

        /// <summary>
        /// Gets the command.
        /// </summary>
        public ICommand SaveLiveriesCommand => this.saveLiveriesCommand ??= new RelayCommand(
            param => this.SaveLiveries(),
            param => this.CanSaveLiveries());

        /// <summary>
        /// Saves the liveries to a file.
        /// </summary>
        public void SaveLiveries()
        {
            var fileName = this.SaveDialogService.ShowDialog();

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            this.ToXml(fileName);
        }

        /// <summary>
        /// Gets whether the save liveries command is available.
        /// </summary>
        /// <returns>True if the command is available, false otherwise.</returns>
        public bool CanSaveLiveries()
        {
            return this.Liveries.Count > 0;
        }
    }
}
