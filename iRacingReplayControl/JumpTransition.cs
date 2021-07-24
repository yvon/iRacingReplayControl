using iRacingSimulator;

namespace iRacingReplayControl
{
    public class JumpTransition : Transition
    {
        public override string Label => "Time jump";

        public JumpTransition(int frameNum) : base(frameNum)
        {
        }

        public override void Apply(Transition lastApplied)
        {
            if (Next != null)
            {
                int playBackSpeed = Sim.Instance.Telemetry.ReplayPlaySpeed.Value;
                Transition target = Next == lastApplied ? this : Next;
                target.JumpTo();
                Sim.Instance.Sdk.Replay.SetPlaybackSpeed(playBackSpeed);
            }
        }
    }
}
