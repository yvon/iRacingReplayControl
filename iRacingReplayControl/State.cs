using iRacingSimulator;

namespace iRacingReplayControl
{
    public class State
    {
        public int? PlayBackSpeed { get; set; }
        public int? FrameNum { get; set; }
        public int? CarNumber { get; set; }
        public int? CamNumber { get; set; }

        public void Apply()
        {
            if (FrameNum != null)
                Sim.Instance.Sdk.Replay.SetPosition((int)FrameNum);

            if (PlayBackSpeed != null)
                Sim.Instance.Sdk.Replay.SetPlaybackSpeed((int)PlayBackSpeed);

            if (CarNumber != null && CamNumber != null)
                Sim.Instance.Sdk.Camera.SwitchToCar((int)CarNumber, (int)CamNumber);
        }
    }
}
