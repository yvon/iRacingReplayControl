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
            current.Next = transition;
            transition.Prev = current;

            // Insert in collection
            int index = Collection.IndexOf(current) + 1;
            Collection.Insert(index, transition);
        }

        public bool Remove(Transition transition)
        {
            Transition prev = transition.Prev;

            if (prev != null)
                prev.Next = transition.Next;

            return Collection.Remove(transition);
        }

        private Transition ReverseApply(int playBackSpeed, int frameNum, State state)
        {
            Transition lastApplied = null;

            foreach (Transition transition in Collection.Reverse())
            {
                if (transition.Apply(playBackSpeed, state))
                    lastApplied = transition;

                if (transition.FrameNum <= frameNum)
                    break;
            }

            return lastApplied;
        }

        public Transition Apply(int playBackSpeed, int frameNum, State state)
        {
            if (playBackSpeed < 0)
                return ReverseApply(playBackSpeed, frameNum, state);

            Transition lastApplied = null;

            foreach (Transition transition in Collection)
            {
                if (transition.FrameNum > frameNum)
                    break;

                if (transition.Apply(playBackSpeed, state))
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
