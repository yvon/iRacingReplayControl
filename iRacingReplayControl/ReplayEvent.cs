using iRacingSimulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace iRacingReplayControl
{
    public abstract class ReplayEvent
    {
        public ReplayEvent(int frameNum)
        {
            FrameNum = frameNum;
        }

        public abstract string Label { get; }
        public int FrameNum { get; set; }
        public string Time => TimeSpan.FromSeconds(FrameNum / 60).ToString();

        public void JumpTo()
        {
            Sim.Instance.Sdk.Replay.SetPosition(FrameNum);
        }
    }
}
