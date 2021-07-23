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
        private readonly int _carIdx;
        private readonly int _carNumber;
        private readonly int _camNumber;
        private readonly Driver _driver;
        private readonly string _label;

        public Cam(int frameNum, int carIdx, int camNumber) : base(frameNum)
        {
            _carIdx = carIdx;
            _camNumber = camNumber;
            _driver = GetDriver();
            _carNumber = int.Parse(_driver.CarNumber);
            _label = $"{_driver.ShortName}. {GetCameName()}";
        }

        public override string Label => _label;
        public void Apply()
        {
            Sim.Instance.Sdk.Camera.SwitchToCar(_carNumber, _camNumber);
        }

        private Driver GetDriver()
        {
            return Driver.FromSessionInfo(Sim.Instance.SessionInfo, _carIdx);
        }

        private string GetCameName()
        {
            SessionInfo info = Sim.Instance.SessionInfo;
            YamlQuery camQuery = info["CameraInfo"]["Groups"]["GroupNum", _camNumber];
            return camQuery["GroupName"].GetValue();
        }
    }
}
