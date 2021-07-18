using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using iRacingSimulator;
using System.Diagnostics;
using iRacingSdkWrapper;
using System.Linq;
using iRacingSimulator.Drivers;
using System.Collections.Generic;

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

            // Light or Dark mode according to windows settings
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            _currentCam = new Cam();
            Cams = new CamCollection();

            // Live sort cameras
            _viewSource = new CollectionViewSource();
            _viewSource.Source = Cams;
            _viewSource.SortDescriptions.Add(new SortDescription("ReplayFrameNum", ListSortDirection.Ascending));
            _viewSource.IsLiveSortingRequested = true;

            // Bind data to interface
            DataContext = _currentCam;
            itemsControl.ItemsSource = _viewSource.View;             

            Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
            Sim.Instance.Start();
        }

        public void AddCam(object sender, RoutedEventArgs e)
        {
            Cams.Add(new Cam(_currentCam));
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            _currentCam.ReplayFrameNum = e.TelemetryInfo.ReplayFrameNum.Value;
            _currentCam.CarIdx = e.TelemetryInfo.CamCarIdx.Value;
            _currentCam.CamNumber = e.TelemetryInfo.CamGroupNumber.Value;

            Cam nextCam = (Cam)_viewSource.View.CurrentItem;

            if (nextCam != null && nextCam.ReplayFrameNum <= _currentCam.ReplayFrameNum)
            {
                Sim.Instance.Sdk.Camera.SwitchToCar(nextCam.CarIdx, nextCam.CamNumber);
                _viewSource.View.MoveCurrentToNext();
            }
        }

        private void JumpToCam(Cam cam)
        {
            if (cam == null)
                return;

            Sim.Instance.Sdk.Replay.SetPosition(cam.ReplayFrameNum);
            _viewSource.View.MoveCurrentTo(cam);
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ClickOnCam(object sender, RoutedEventArgs e)
        {
            Cam cam = (sender as Button).DataContext as Cam;
            JumpToCam(cam);
        }

        private void ClickOnDeleteCam(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cam cam = (sender as Button).DataContext as Cam;
            Cams.Remove(cam);
        }
    }
}