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
                Transition target = Next == lastApplied ? Prev : Next;
                target.JumpTo();
                Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
            }
        }
    }
}
