using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

namespace CodeAlchemy.CommonUtils
{
    [System.Serializable]
    public struct IntRange
    {
        public int min, max;

        public int RandomFromRange()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    public static class ExtensionUtils
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class EventsManager
    {

        private static Dictionary<string, Action<Dictionary<string, object>>> eventDictionary;

        private static void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<Dictionary<string, object>>>();
            }
        }

        public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            Init();
            Action<Dictionary<string, object>> thisEvent;

            if (eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            Init();
            Action<Dictionary<string, object>> thisEvent;
            if (eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName, Dictionary<string, object> message)
        {
            Init();
            Action<Dictionary<string, object>> thisEvent = null;
            if (eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                Debug.Log(string.Format("[EventsManager] TRIGGERED EVENT: {0}", eventName));
                thisEvent.Invoke(message);
            }
        }
    }

    public interface IObservable<T> : IDisposable where T : IEquatable<T>, IComparable<T>
    {
        public void Subscribe(Action<T> callback);
    }

    public class Observable<T> : IObservable<T> where T : IEquatable<T>, IComparable<T>
    {
        private Action<T> onValueChanged;

        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                onValueChanged.Invoke(_value);
            }
        }
        public Observable(T newVal)
        {
            _value = newVal;
        }
        public void Subscribe(Action<T> callback)
        {
            onValueChanged += callback;
        }

        public void UnSubscribe(Action<T> callback)
        {
            onValueChanged -= callback;
        }

        public void Dispose()
        {
        }
    }

    public static class Logger
    {
        public static bool logEnabled = true;
        public static void Log(string msg)
        {
            if(logEnabled)
                Debug.Log(msg);
        }


    }
}