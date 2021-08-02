using iRacingSimulator;
using System;

namespace iRacingReplayControl
{
    public abstract class Transition
    {
        public Transition(int frameNum)
        {
            FrameNum = frameNum;
        }

        public abstract string Label { get; }
        public int FrameNum { get; set; }
        public Transition Prev = null;
        public Transition Next = null;
        public string Time => TimeSpan.FromSeconds(FrameNum / 60).ToString();

        public void JumpTo()
        {
            Sim.Instance.Sdk.Replay.SetPosition(FrameNum);
        }

        public abstract bool Apply(State state);
    }
}
