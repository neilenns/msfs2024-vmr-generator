// <copyright file="IMessageBoxService.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Interfaces
{
    /// <summary>
    /// Provides an interface to show a message to the user.
    /// </summary>
    public interface IMessageBoxService
    {
        /// <summary>
        /// Returns an instance of a message box service.
        /// </summary>
        public static readonly IMessageBoxService Instance;

        /// <summary>
        /// Shows an error to the user.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="title">The title for the dialog.</param>
        void ShowError(string message, string title);

        /// <summary>
        /// Shows a message to the user.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title for the dialog.</param>
        void ShowMessage(string message, string title);
    }
}