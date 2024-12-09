// <copyright file="MainWindow.xaml.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace VmrGenerator
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using VmrGenerator.Services;
    using VmrGenerator.ViewModels.ModelMatching;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ModelMatchingViewModel modelMatchingViewModel;
        private HwndSource hwndSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.modelMatchingViewModel = new ModelMatchingViewModel
            {
                MessageBoxService = MessageBoxService.Instance,
                SaveDialogService = SaveDialogService.Instance,
            };
            this.DataContext = this.modelMatchingViewModel;
        }

        /// <inheritdoc/>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            this.hwndSource.AddHook(this.modelMatchingViewModel.HandleWindowsEvent);
            this.modelMatchingViewModel.WindowHandle = new WindowInteropHelper(this).Handle;
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            if (this.hwndSource != null)
            {
                this.hwndSource.RemoveHook(this.modelMatchingViewModel.HandleWindowsEvent);
                this.hwndSource = null;
            }

            base.OnClosed(e);
        }
    }
}
