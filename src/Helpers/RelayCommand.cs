// <copyright file="RelayCommand.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    /// <summary>
    /// Provides an easy way for view models to expose Commands to WPF.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </remarks>
    /// <param name="execute">The method to invoke when the command executed.</param>
    /// <param name="canExecute">The method to invoke to verify if the command can execute.</param>
    // From https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090051
    public class RelayCommand(Action<object> execute, Predicate<object> canExecute) : ICommand
    {
        private readonly Action<object> execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Predicate<object> canExecute = canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The method to invoke when the command executed.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <inheritdoc/>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
