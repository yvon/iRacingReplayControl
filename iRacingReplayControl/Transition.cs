using iRacingSimulator;
using System;
using System.Xml.Serialization;

namespace iRacingReplayControl
{
    public abstract class Transition
    {
        public Transition()
        {
        }

        public Transition(int frameNum)
        {
            FrameNum = frameNum;
        }

        public abstract string Label { get; }
        public int FrameNum { get; set; }

        [XmlIgnore]
        public Transition Prev = null;

        [XmlIgnore]
        public Transition Next = null;

        [XmlIgnore]
        public string Time => TimeSpan.FromSeconds(FrameNum / 60).ToString();

        public void JumpTo()
        {
            Sim.Instance.Sdk.Replay.SetPosition(FrameNum);
        }

        public abstract bool Apply(State state);
    }
}
