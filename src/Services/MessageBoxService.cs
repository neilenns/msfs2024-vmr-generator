// <copyright file="MessageBoxService.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Services
{
    using System.Windows;
    using VmrGenerator.Interfaces;

    /// <summary>
    /// Provides a Win32 MessageBox via the IMessageBoxService interface.
    /// </summary>
    public class MessageBoxService : IMessageBoxService
    {
        private static readonly IMessageBoxService InstanceValue = new MessageBoxService();

        /// <summary>
        /// Gets an instance of the MessageBoxService.
        /// </summary>
        public static IMessageBoxService Instance => InstanceValue;

        /// <inheritdoc/>
        public void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc/>
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}