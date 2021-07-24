﻿using iRacingSimulator;
using iRacingSimulator.Drivers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace iRacingReplayControl
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ReplayEvent _lastApplied;

        public Cam CurrentCam { get; private set; }
        public CamCollection Cams { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            Cams = new CamCollection();

            // Bind data to interface
            DataContext = this;
            itemsControl.ItemsSource = Cams.ObservableCollection;

            Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
            Sim.Instance.Start();
        }

        private void AddCam(object sender, RoutedEventArgs e)
        {
            Cams.Add(CurrentCam);
        }

        private void EventuallySwitchToCam()
        {
            ReplayEvent camToApply = Cams.Current(CurrentCam.FrameNum);
 
            if (camToApply == null || camToApply == _lastApplied)
              return;
             
            _lastApplied = camToApply;
            camToApply.Apply();
            itemsControl.SelectedItem = camToApply;

            // Autoscroll
            int index = itemsControl.SelectedIndex;
            object item = itemsControl.Items.GetItemAt(index);
            itemsControl.ScrollIntoView(item);
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (Sim.Instance.SessionInfo == null) return;

            CurrentCam = new Cam(
                e.TelemetryInfo.ReplayFrameNum.Value,
                e.TelemetryInfo.CamCarIdx.Value,
                e.TelemetryInfo.CamGroupNumber.Value
            );

            OnPropertyChanged("CurrentCam");
            EventuallySwitchToCam();
        }

        private void JumpToCam(Cam cam)
        {
            if (cam == null)
                return;

            cam.JumpTo();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Sim.Instance.Stop();
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}