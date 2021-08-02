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

        public override bool Apply(State state)
        {
            state.CarNumber = _carNumber;
            state.CamNumber = _camNumber;
            return true;
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
