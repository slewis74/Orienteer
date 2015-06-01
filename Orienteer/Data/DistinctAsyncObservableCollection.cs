using System.Collections.Generic;

namespace Orienteer.Data
{
    public class DistinctAsyncObservableCollection<T> : DispatchingObservableCollection<T>
    {
        public DistinctAsyncObservableCollection()
        {
        }

        public DistinctAsyncObservableCollection(IEnumerable<T> list) : base(list)
        {
        }

        protected override void InsertItem(int index, T item)
        {
            if (Contains(item))
                return;
            base.InsertItem(index, item);
        }

        public new virtual void Add(T item)
        {
            if (Contains(item))
                return;
            base.Add(item);
        }

        protected override void SetItem(int index, T item)
        {
            if (Contains(item))
                return;
            base.SetItem(index, item);
        }
    }
}