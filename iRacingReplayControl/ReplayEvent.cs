using iRacingSimulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayControl
{
    public abstract class ReplayEvent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _frameNum;

        public ReplayEvent(int frameNum)
        {
            _frameNum = frameNum;
        }

        public ReplayEvent()
        {
        }

        public abstract string Label { get; }

        public int FrameNum
        {
            get => _frameNum;
            set
            {
                _frameNum = value;
                OnPropertyChanged();
                OnPropertyChanged("Time");
            }
        }
        public string Time
        {
            get => TimeSpan.FromSeconds(FrameNum / 60).ToString();
        }
        public void JumpTo()
        {
            Sim.Instance.Sdk.Replay.SetPosition(FrameNum);
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
