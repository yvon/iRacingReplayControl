using iRacingSimulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayControl
{
    public class Jump : ReplayEvent
    {
        public override string Label => "Jump";

        public Jump(int frameNum) : base(frameNum)
        {
        }

        public override void Apply(ReplayEvent lastApplied)
        {
            if (Next != null)
            {
                ReplayEvent target = Next == lastApplied ? Prev : Next;
                target.JumpTo();
                Sim.Instance.Sdk.Replay.SetPlaybackSpeed(1);
            }
        }
    }
}
