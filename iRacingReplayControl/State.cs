using iRacingSimulator;
using System.Diagnostics;

namespace iRacingReplayControl
{
    public class State
    {
        public int? PlayBackSpeed { get; set; }
        public int? FrameNum { get; set; }
        public int? CarNumber { get; set; }
        public int? CamNumber { get; set; }

        public void Apply(CamTransition currentCam)
        {
            Debug.Print($"Apply state: FrameNum: {FrameNum}, CarNumber: {CarNumber}, CamNumber: {CamNumber}");

            if (FrameNum != null && currentCam.FrameNum < FrameNum)
            {
                Sim.Instance.Sdk.Replay.SetPosition((int)FrameNum);
                Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
            }

            if (CarNumber != null && CamNumber != null)
            {
                Sim.Instance.Sdk.Camera.SwitchToCar((int)CarNumber, (int)CamNumber);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State)) return false;
            State state = (State)obj;

            return state.FrameNum == FrameNum &&
                state.CarNumber == CarNumber &&
                state.CamNumber == CamNumber;
        }

        public override int GetHashCode()
        {
            return (int)(FrameNum ^ CamNumber ^ CarNumber);
        }

        public bool IsEmpty()
        {
            return FrameNum == null && CarNumber == null && CamNumber == null;
        }
    }
}
