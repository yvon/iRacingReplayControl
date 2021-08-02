using iRacingSdkWrapper;
using iRacingSimulator;

namespace iRacingReplayControl
{
    public class CamTransition : Transition
    {
        private int _carIdx;
        private readonly string _label = null;
        private Driver _driver = null;

        public int CarIdx
        {
            get => _carIdx;
            set
            {
                _carIdx = value;
                _driver = Driver.FromCarIdx(value);
            }
        }

        public int CamNumber { get; set; }

        public override string Label => $"{_driver.ShortName}. {CamName}";

        public CamTransition()
        {

        }

        public CamTransition(int frameNum, int carIdx, int camNumber) : base(frameNum)
        {
            CarIdx = carIdx;
            CamNumber = camNumber;
        }

        public override bool Apply(State state)
        {
            state.CarNumber = CarNumber;
            state.CamNumber = CamNumber;
            return true;
        }

        private int CarNumber => _driver.CarNumber;

        private string CamName
        {
            get
            {
                SessionInfo info = Sim.Instance.SessionInfo;
                YamlQuery camQuery = info["CameraInfo"]["Groups"]["GroupNum", CamNumber];
                return camQuery["GroupName"].GetValue();
            }
        }
    }
}
