using System.Collections.ObjectModel;
using System.Linq;

namespace iRacingReplayControl
{
    public class TransitionCollection
    {
        public ObservableCollection<Transition> ObservableCollection { get; private set; }

        public TransitionCollection()
        {
            ObservableCollection = new ObservableCollection<Transition>();
        }

        public void Add(Transition transition)
        {
            Transition current = Current(transition.FrameNum);

            if (current == null)
            {
                Prepend(transition);
                return;
            }

            // Chain events
            current.Next = transition;
            transition.Prev = current;

            // Insert in collection
            int index = ObservableCollection.IndexOf(current) + 1;
            ObservableCollection.Insert(index, transition);
        }

        public bool Remove(Transition transition)
        {
            Transition prev = transition.Prev;

            if (prev != null)
            {
                prev.Next = transition.Next;
            }

            return ObservableCollection.Remove(transition);
        }

        public Transition Current(int frameNum)
        {
            return ObservableCollection.LastOrDefault(e => e.FrameNum <= frameNum);
        }

        private void Prepend(Transition transition)
        {
            Transition first = ObservableCollection.FirstOrDefault();
            ObservableCollection.Insert(0, transition);

            if (first != null)
            {
                first.Prev = transition;
                transition.Next = first;
            }
        }
    }
}
