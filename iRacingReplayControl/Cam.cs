using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace iRacingReplayControl
{
    public class Cam : INotifyPropertyChanged
    {
        private int _replayFrameNum;
        private int _carIdx;
        private int _camNumber;

        public event PropertyChangedEventHandler PropertyChanged;

        public Cam(Cam cam)
        {
            _replayFrameNum = cam.ReplayFrameNum;
            _carIdx = cam.CarIdx;
            _camNumber = cam.CamNumber;
        }
        public Cam()
        {
        }

        public int ReplayFrameNum
        {
            get => _replayFrameNum;
            set
            {
                _replayFrameNum = value;
                OnPropertyChanged();
            }
        }

        public int CarIdx
        {
            get => _carIdx;
            set
            {
                _carIdx = value;
                OnPropertyChanged();
            }
        }

        public int CamNumber
        {
            get => _camNumber;
            set
            {
                _camNumber = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
