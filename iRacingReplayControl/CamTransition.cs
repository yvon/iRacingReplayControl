using iRacingSdkWrapper;
using iRacingSimulator;

namespace iRacingReplayControl
{
    public class CamTransition : Transition
    {
        private readonly int _carNumber;
        private readonly int _camNumber;
        private readonly string _label;

        public CamTransition(int frameNum, int carIdx, int camNumber) : base(frameNum)
        {
            _camNumber = camNumber;
            Driver driver = Driver.FromCarIdx(carIdx);
            _carNumber = driver.CarNumber;
            _label = $"{driver.ShortName}. {CamName}";
        }

        public override string Label => _label;

        public override void Apply(Transition lastApplied)
        {
            Sim.Instance.Sdk.Camera.SwitchToCar(_carNumber, _camNumber);
        }

        private string CamName
        {
            get
            {
                SessionInfo info = Sim.Instance.SessionInfo;
                YamlQuery camQuery = info["CameraInfo"]["Groups"]["GroupNum", _camNumber];
                return camQuery["GroupName"].GetValue();
            }
        }
    }
}
