using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using iRacingSimulator;

namespace iRacingReplayControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Cam _currentCam;
        private CollectionViewSource _viewSource; 

        public CamCollection Cams { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            InitializeComponent();

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            _currentCam = new Cam();
            Cams = new CamCollection();
            _viewSource = new CollectionViewSource();
            _viewSource.Source = Cams;
            _viewSource.SortDescriptions.Add(new SortDescription("Frame", ListSortDirection.Ascending));
            _viewSource.IsLiveSortingRequested = true;

            DataContext = _currentCam;
            grid.ItemsSource = _viewSource.View;             

            Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
            Sim.Instance.Start();
        }

        public void AddCam(object sender, RoutedEventArgs e)
        {
            Cams.Add(new Cam(_currentCam));
        }

        private void DeleteCam(object sender, RoutedEventArgs e)
        {
            Cam cam = (sender as Label).DataContext as Cam;
            Cams.Remove(cam);
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            _currentCam.ReplayFrameNum = e.TelemetryInfo.ReplayFrameNum.Value;
            _currentCam.CarIdx = e.TelemetryInfo.CamCarIdx.Value;
            _currentCam.CamNumber = e.TelemetryInfo.CamGroupNumber.Value;
        }
    }
}