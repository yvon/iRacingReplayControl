namespace iRacingReplayControl
{
    public class JumpTransition : Transition
    {
        public override string Label => "Time jump";

        public JumpTransition()
        {
        }

        public JumpTransition(int frameNum) : base(frameNum)
        {
        }

        public override bool Apply(State state)
        {
            if (Next == null) return false;

            state.FrameNum = Next.FrameNum;
            // Immediatly apply next transition to avoid flickering
            return Next.Apply(state);
        }
    }
}
