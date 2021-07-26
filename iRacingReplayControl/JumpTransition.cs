using iRacingSimulator;

namespace iRacingReplayControl
{
    public class JumpTransition : Transition
    {
        public override string Label => "Time jump";

        public JumpTransition(int frameNum) : base(frameNum)
        {
        }

        public override bool Apply(int playBackSpeed, State state)
        {
            if (Next == null || playBackSpeed == 0)
                return false;

            Transition target = playBackSpeed > 0 ? Next : this;
            state.FrameNum = target.FrameNum;
            state.PlayBackSpeed = playBackSpeed;
            return true;
        }
    }
}
