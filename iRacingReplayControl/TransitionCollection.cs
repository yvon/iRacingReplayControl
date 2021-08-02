using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace iRacingReplayControl
{
    public class TransitionCollection
    {
        private XmlSerializer _serializer = new XmlSerializer(typeof(Transition[]), new Type[] { typeof(CamTransition), typeof(JumpTransition) });
        private string _xmlFilename;
        private bool _loaded = false;

        public ObservableCollection<Transition> Collection { get; private set; }
        public object JsonSerializer { get; private set; }

        public TransitionCollection()
        {
            Collection = new ObservableCollection<Transition>();
            Collection.CollectionChanged += OnCollectionChange;
        }

        private void OnCollectionChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_loaded) Serialize();
        }

        private void Serialize()
        {
            Debug.Print("Serialize");
            StreamWriter myWriter = new StreamWriter(_xmlFilename);
            _serializer.Serialize(myWriter, Collection.ToArray());
            myWriter.Close();
        }

        public void DeSerialize(int sessionId)
        {
            _xmlFilename = $"session{sessionId}.xml";
            Debug.Print($"DeSerialize {_xmlFilename}");

            try
            {
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(_xmlFilename, FileMode.Open);
                // Call the Deserialize method and cast to the object type.
                Transition[] transitions = (Transition[])_serializer.Deserialize(myFileStream);
                myFileStream.Close();
                Array.ForEach(transitions, Add);
            }
            catch (FileNotFoundException)
            {
                Debug.Print("File not found");
            }

            _loaded = true;
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
