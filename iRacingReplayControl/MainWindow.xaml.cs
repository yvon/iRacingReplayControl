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
    public partial class MainWindow : MetroWindow
    {
        private Cam _currentCam;
        private Cam _lastApplied;
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

        private void EventuallySwitchToCam()
        {
            IEnumerable<Cam> OrderedCams = _viewSource.View.Cast<Cam>();
            Cam camToApply = OrderedCams.LastOrDefault(cam => cam.ReplayFrameNum <= _currentCam.ReplayFrameNum);

            if (camToApply == null || camToApply == _lastApplied)
                return;

            _lastApplied = camToApply;
            Sim.Instance.Sdk.Camera.SwitchToCar(camToApply.CarIdx, camToApply.CamNumber);
            _viewSource.View.MoveCurrentTo(_lastApplied);

            int index = itemsControl.SelectedIndex;
            object item = itemsControl.Items.GetItemAt(index);
            itemsControl.ScrollIntoView(item);
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            _currentCam.ReplayFrameNum = e.TelemetryInfo.ReplayFrameNum.Value;
            _currentCam.CarIdx = e.TelemetryInfo.CamCarIdx.Value;
            _currentCam.CamNumber = e.TelemetryInfo.CamGroupNumber.Value;

            EventuallySwitchToCam();
        }

        private void SetPositionToCam(Cam cam)
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
            SetPositionToCam(cam);
        }

        private void ClickOnDeleteCam(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cam cam = (sender as Button).DataContext as Cam;
            Cams.Remove(cam);
        }
    }
}