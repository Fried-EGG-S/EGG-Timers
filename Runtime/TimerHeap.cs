using System.Collections.Generic;

namespace FriedEGGS.Timers
{
    internal sealed class TimerHeap
    {
        private readonly List<TimerBase> _items = new();

        public int Count => _items.Count;
        public TimerBase Earliest => _items[0];

        public void AddOrUpdate(TimerBase timer)
        {
            if (timer.HeapIndex < 0)
            {
                timer.HeapIndex = _items.Count;
                _items.Add(timer);
            }
            SiftDown(SiftUp(timer.HeapIndex));
        }

        public void Remove(TimerBase timer)
        {
            int index = timer.HeapIndex;
            if (index < 0) return;

            int last = _items.Count - 1;
            if (index != last) Place(_items[last], index);
            _items.RemoveAt(last);
            timer.HeapIndex = -1;

            if (index < _items.Count) SiftDown(SiftUp(index));
        }

        public void Clear()
        {
            foreach (TimerBase timer in _items) timer.HeapIndex = -1;
            _items.Clear();
        }

        private int SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_items[parent].EndTime <= _items[index].EndTime) break;
                Swap(index, parent);
                index = parent;
            }
            return index;
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int smallest = index;
                int left = index * 2 + 1;
                int right = left + 1;
                if (left < _items.Count && _items[left].EndTime < _items[smallest].EndTime) smallest = left;
                if (right < _items.Count && _items[right].EndTime < _items[smallest].EndTime) smallest = right;
                if (smallest == index) return;
                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            TimerBase first = _items[a];
            Place(_items[b], a);
            Place(first, b);
        }

        private void Place(TimerBase timer, int index)
        {
            _items[index] = timer;
            timer.HeapIndex = index;
        }
    }
}
