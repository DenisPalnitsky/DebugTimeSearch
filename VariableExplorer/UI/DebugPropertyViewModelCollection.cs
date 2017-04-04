using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLocals.UI
{
    class DebugPropertyViewModelCollection : IList<DebugPropertyViewModel>, INotifyPropertyChanged , INotifyCollectionChanged
    {
        List<DebugPropertyViewModel> _internalList = new List<DebugPropertyViewModel>();
                

        public int IndexOf(DebugPropertyViewModel item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, DebugPropertyViewModel item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public DebugPropertyViewModel this[int index]
        {
            get
            {
                return _internalList[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(DebugPropertyViewModel item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _internalList.Clear();
            RiseItemsRemovedEvents();            
        }        

        public bool Contains(DebugPropertyViewModel item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(DebugPropertyViewModel[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(DebugPropertyViewModel item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<DebugPropertyViewModel> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        private void RiseItemsAddedEvents(IEnumerable<DebugPropertyViewModel> newItems)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void RiseItemsRemovedEvents()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Count"));

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<DebugPropertyViewModel> items )
        {
            _internalList.AddRange(items);
            RiseItemsAddedEvents(items);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
