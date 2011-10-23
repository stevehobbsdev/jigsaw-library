using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Diggins.Jigsaw
{
    public class Notifier<T> : IObservable<T>
    {
        private List<IObserver<T>> observers = new List<IObserver<T>>();

        public Notifier()
        { }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public void Subscribe(Action<T> action)
        {
            Subscribe(new Observer<T>(action));
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void Notify(T subject)
        {
            foreach (var observer in observers)
                observer.OnNext(subject);
        }

        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }

    public class Observer<T> : IObserver<T>, IDisposable
    {
        Notifier<T> tracker;
        IDisposable unsubscriber;
        Action<T> action;

        public Observer(Action<T> action)
        {
            this.action = action;
        }

        public void Subscribe(Notifier<T> tracker)
        {
            this.tracker = tracker;
            unsubscriber = tracker.Subscribe(this);
        }

        public void Unsubscribe()
        {
            if (unsubscriber != null)
                unsubscriber.Dispose();               
        }

        public void OnCompleted()
        {
            Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(T value)
        {
            action(value);
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }

    public class ObservableValue<T> : Notifier<T>
    {
        T value;

        public T Get() 
        { 
            return value; 
        }

        public void Set(T x)
        {
            if (value.Equals(x))
                return;
            value = x;
            Notify(value);
        }
    }

    public class ComputedValue<T> : Notifier<T>
    {
        T cache;
        
        Observer<T> observer;
        
        public ComputedValue(Func<T, T> f)
        {
            observer = new Observer<T>(x => { UpdateCache(f(x)); });
        }
        
        public T Get() 
        { 
            return cache; 
        }
        
        private void UpdateCache(T x)
        {
            if (cache.Equals(x))
                return;
            cache = x;
            Notify(x);  
        }
    }
}
