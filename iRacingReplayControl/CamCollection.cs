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
                Prepend(replayEvent);
                return;
            }

            // Chain events
            current.Next = replayEvent;
            replayEvent.Prev = current;

            // Insert in collection
            int index = ObservableCollection.IndexOf(current) + 1;
            ObservableCollection.Insert(index, replayEvent);
        }

        public bool Remove(ReplayEvent replayEvent)
        {
            ReplayEvent prev = replayEvent.Prev;

            if (prev != null)
            {
                prev.Next = replayEvent.Next;
            }

            return ObservableCollection.Remove(replayEvent);
        }

        public ReplayEvent Current(int frameNum)
        {
            return ObservableCollection.LastOrDefault(e => e.FrameNum <= frameNum);
        }

        private void Prepend(ReplayEvent replayEvent)
        {
            ReplayEvent first = ObservableCollection.FirstOrDefault();
            ObservableCollection.Insert(0, replayEvent);

            if (first != null)
            {
                first.Prev = replayEvent;
                replayEvent.Next = first;
            }
        }
    }
}
