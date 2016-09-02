using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Band
{
    public class ObservableObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected bool SetProperty<T>(ref T storage, T value, 
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetPropertyDebounced<T>(ref T storage, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            FireDebounce(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Dictionary<string, CancellationTokenSource> taskDict;
        private void FireDebounce(string propertyName)
        {
            if (taskDict == null) taskDict = new Dictionary<string, CancellationTokenSource>();

            if (taskDict.ContainsKey(propertyName) && !taskDict[propertyName].IsCancellationRequested)
            {
                taskDict[propertyName].Cancel();
            }

            taskDict[propertyName] = CreateDebounceTask(propertyName);
        }

        private CancellationTokenSource CreateDebounceTask(string propertyName)
        {
            var cts = new CancellationTokenSource();
            Task.Delay(500, cts.Token).ContinueWith((task) =>
            {
                if (task.IsCanceled) return;
                OnPropertyChanged(propertyName);
            });
            return cts;
        }
    }
}

