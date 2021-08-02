using System.Collections.ObjectModel;
using System.Linq;

namespace iRacingReplayControl
{
    public class TransitionCollection
    {
        public ObservableCollection<Transition> Collection { get; private set; }

        public TransitionCollection()
        {
            Collection = new ObservableCollection<Transition>();
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
            transition.Prev = current;
            transition.Next = current.Next;
            current.Next = transition;

            // Insert in collection
            int index = Collection.IndexOf(current) + 1;
            Collection.Insert(index, transition);
        }

        public bool Remove(Transition transition)
        {
            Transition prev = transition.Prev;
            Transition next = transition.Next;

            if (prev != null)
                prev.Next = transition.Next;
            if (next != null)
                next.Prev = transition.Prev;

            return Collection.Remove(transition);
        }

        public Transition Apply(int frameNum, State state)
        {
            Transition lastApplied = null;

            foreach (Transition transition in Collection)
            {
                if (transition.FrameNum > frameNum)
                    break;

                if (transition.Apply(state))
                    lastApplied = transition;
            }

            return lastApplied;
        }

        public Transition Current(int frameNum)
        {
            return Collection.LastOrDefault(e => e.FrameNum <= frameNum);
        }

        private void Prepend(Transition transition)
        {
            Transition first = Collection.FirstOrDefault();
            Collection.Insert(0, transition);

            if (first != null)
            {
                first.Prev = transition;
                transition.Next = first;
            }
        }
    }
}
