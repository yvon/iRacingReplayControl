using iRacingSimulator;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace iRacingReplayControl
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Transition _lastApplied;
        private bool _connected = false;
        private CamTransition _currentCam;

        public bool Connected
        {
            get => _connected;
            private set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnPropertyChanged();
                }
            }
        }

        public CamTransition CurrentCam
        {
            get => _currentCam;
            private set
            {
                _currentCam = value;
                OnPropertyChanged();
            }
        }

        public TransitionCollection Transitions { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            Transitions = new TransitionCollection();

            // Bind data to interface
            DataContext = this;
            itemsControl.ItemsSource = Transitions.ObservableCollection;

            Sim.Instance.Disconnected += OnDisconnection;
            Sim.Instance.TelemetryUpdated += OnTelemetryUpdated;
            Sim.Instance.Start();
        }

        private void OnDisconnection(object sender, EventArgs e)
        {
            Connected = false;
        }

        private void AddCamTransition(object sender, RoutedEventArgs e)
        {
            Transitions.Add(CurrentCam);
        }

        private void AddJumpTransition(object sender, RoutedEventArgs e)
        {
            Transitions.Add(new JumpTransition(CurrentCam.FrameNum));
        }

        private void ApplyTransition(Transition transition)
        {
            transition.Apply(_lastApplied);
            _lastApplied = transition;
            itemsControl.SelectedItem = transition;

            // Autoscroll
            int index = itemsControl.SelectedIndex;
            object item = itemsControl.Items.GetItemAt(index);
            itemsControl.ScrollIntoView(item);
        }

        private void ApplyTransitions()
        {
            Transition candidate = Transitions.Current(CurrentCam.FrameNum);

            if (candidate != null && candidate != _lastApplied)
            {
                ApplyTransition(candidate);
            }
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (Sim.Instance.SessionInfo == null) return;

            CurrentCam = new CamTransition(
                e.TelemetryInfo.ReplayFrameNum.Value,
                e.TelemetryInfo.CamCarIdx.Value,
                e.TelemetryInfo.CamGroupNumber.Value
            );

            Connected = true; // TODO: listen to connection events
            ApplyTransitions();
        }

        private void JumpTo(Transition transition)
        {
            if (transition == null)
                return;

            transition.JumpTo();
        }

        private void ClickOnTransition(object sender, RoutedEventArgs e)
        {
            Transition transition = (sender as Button).DataContext as Transition;
            JumpTo(transition);
        }

        private void RightClickOnTransition(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Transition transition = (sender as Button).DataContext as Transition;
            _ = Transitions.Remove(transition);
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