using iRacingSimulator;
using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private State _lastState = null;
        private bool _playing = false;

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

        public bool Playing
        {
            get => _playing;
            private set
            {
                if (_playing != value)
                {
                    _playing = value;
                    OnPropertyChanged();
                    OnPropertyChanged("Paused");
                }
            }
        }

        public bool Paused => !Playing;

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
            itemsControl.ItemsSource = Transitions.Collection;

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

        private void SelectLastAppliedTransition()
        {
            if (_lastApplied == null || itemsControl.SelectedItem == _lastApplied) return;
            SelectTransition(_lastApplied);

            try
            {
                // Autoscroll
                int index = itemsControl.SelectedIndex;
                object item = itemsControl.Items.GetItemAt(index);
                itemsControl.ScrollIntoView(item);
            }
            catch (Exception)
            {
                Debug.Print("Can't scroll"); // Out of range
            }
        }

        private void ApplyTransitions()
        {
            State state = new State();
            Transition lastApplied = Transitions.Apply(_currentCam.FrameNum, state);

            if (state.IsEmpty() || state.Equals(_lastState)) return;

            state.Apply(_currentCam);
            _lastApplied = lastApplied;
            _lastState = state;
        }

        private void OnTelemetryUpdated(object sender, iRacingSdkWrapper.SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (Sim.Instance.SessionInfo == null) return;

            CurrentCam = new CamTransition(
                e.TelemetryInfo.ReplayFrameNum.Value,
                e.TelemetryInfo.CamCarIdx.Value,
                e.TelemetryInfo.CamGroupNumber.Value
            );

            if (Connected == false) // TODO: listen to connection events
            {
                Connected = true;
                int sessionId = Sim.Instance.SessionData.SubsessionId;
                Debug.Print($"SessionId: {sessionId}");
                Transitions.DeSerialize(sessionId);
            }

            if (Sim.Instance.Telemetry.ReplayPlaySpeed.Value != 1)
            {
                Playing = false;
            }

            if (Connected && Playing)
            {
                ApplyTransitions();
                SelectLastAppliedTransition();
            }
        }

        private void JumpTo(Transition transition)
        {
            if (transition == null) return;
            transition.JumpTo();
        }

        private void SelectTransition(Transition transition)
        {
            itemsControl.SelectedItem = transition;
        }

        private void ClickOnTransition(object sender, RoutedEventArgs e)
        {
            Transition transition = (sender as Button).DataContext as Transition;
            JumpTo(transition);
            SelectTransition(transition);
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

        private void ClickOnPlay(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void Play()
        {
            Playing = true;
            Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            Playing = false;
            Sim.Instance.Sdk.Replay.SetPlaybackSpeed(0);
        }
    }
}