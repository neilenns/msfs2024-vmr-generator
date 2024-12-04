using Microsoft.FlightSimulator.SimConnect;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using vmr_generator.ViewModels;

namespace vmr_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ModelMatchingViewModel modelMatchingViewModel = new();
        private HwndSource _hwndSource;
        private IntPtr _hwnd;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = modelMatchingViewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            _hwndSource.AddHook(modelMatchingViewModel.HandleWindowsEvent);
            _hwnd = new WindowInteropHelper(this).Handle;

        }

        protected override void OnClosed(EventArgs e)
        {
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(modelMatchingViewModel.HandleWindowsEvent);
                _hwndSource = null;
            }

            base.OnClosed(e);
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            this.modelMatchingViewModel.ConnectToSim(_hwnd);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Model Matching Rule Sets (*.vmr)|*.vmr|All Files (*.*)|*.*",
                Title = "Save model matching file",
                FileName = "MSFS2024.vmr"
            };

            bool result = saveFileDialog.ShowDialog() ?? false;
            if (!result)
            {
                return;
            }

            this.modelMatchingViewModel.ToXml(saveFileDialog.FileName);
        }
    }
}
