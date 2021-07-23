using iRacingSdkWrapper;
using iRacingSimulator;
using iRacingSimulator.Drivers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iRacingReplayControl
{
    public class Cam : ReplayEvent
    {
        private int _carIdx;
        private int _camNumber;

        public Cam(Cam cam) : base(cam.FrameNum)
        {
            _carIdx = cam.CarIdx;
            _camNumber = cam.CamNumber;
        }

        public Cam()
        {
        }
        public override string Label => Driver == null ? "" : $"{Driver.ShortName}. {CamName}";

        public int CarIdx
        {
            get => _carIdx;
            set
            {
                _carIdx = value;
                OnPropertyChanged("Label");
            }
        }

        public int CamNumber
        {
            get => _camNumber;
            set
            {
                _camNumber = value;
                OnPropertyChanged("Label");
            }
        }

        public string CamName
        {
            get
            {
                SessionInfo info = Sim.Instance.SessionInfo;
                var query = info["CameraInfo"]["Groups"]["GroupNum", CamNumber];
                return query["GroupName"].GetValue();
            }
        }

        public void Apply()
        {
            Sim.Instance.Sdk.Camera.SwitchToCar(CarNumber, CamNumber);
        }

        private Driver Driver
        {
            get
            {
                if (CarIdx == 0 || Sim.Instance.SessionInfo == null) return null;
                return Driver.FromSessionInfo(Sim.Instance.SessionInfo, CarIdx);
            }
        }

        private int CarNumber => int.Parse(Driver.CarNumber);
    }
}
