// <copyright file="ISaveDialogService.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Interfaces
{
    /// <summary>
    /// Provides an interface to show file save dialog to the user.
    /// </summary>
    public interface ISaveDialogService
    {
        /// <summary>
        /// Returns an instance of a save dialog service.
        /// </summary>
        public static readonly ISaveDialogService Instance;

        /// <summary>
        /// Shows a save dialog to the user.
        /// </summary>
        /// <returns>The filename to save to, or null if no filename specified.</returns>
        string ShowDialog();
    }
}
