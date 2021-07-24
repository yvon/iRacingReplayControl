using System.Collections.ObjectModel;
using System.Linq;

namespace iRacingReplayControl
{
    public class CamCollection
    {
        public ObservableCollection<ReplayEvent> ObservableCollection { get; private set; }

        public CamCollection()
        {
            ObservableCollection = new ObservableCollection<ReplayEvent>();
        }

        public void Add(ReplayEvent replayEvent)
        {
            ReplayEvent current = Current(replayEvent.FrameNum);

            if (current == null)
            {
                ObservableCollection.Add(replayEvent);
                return;
            }

            int index = ObservableCollection.IndexOf(current) + 1;
            ObservableCollection.Insert(index, replayEvent);
        }

        public void Remove(ReplayEvent replayEvent)
        {
            ObservableCollection.Remove(replayEvent);
        }

        public ReplayEvent Current(int frameNum)
        {
            return ObservableCollection.LastOrDefault(e => e.FrameNum <= frameNum);
        }
    }
}
