using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleMonsters.Utils
{
    public class PriorityQueue<T>
    {
        private bool _isMinFirst;
        private Dictionary<float, T> _sortedList;

        public int Count { get => _sortedList.Count; }

        public PriorityQueue(bool isMinFirst)
        {
            _sortedList = new Dictionary<float, T>();
            _isMinFirst = isMinFirst;
        }

        ~PriorityQueue()
        {
            Clear();
        }

        public void Enqueue(float key, T value)
        {
            _sortedList.Add(key, value);
        }

        public T Dequeue()
        {

            float firstKey = GetFirstKey();
            _sortedList.Remove(firstKey, out T value);
            return value;
        }

        private T Peek()
        {
            float firstKey = GetFirstKey();
            return _sortedList[firstKey];
        }

        private float GetFirstKey()
        {
            float[] keys = _sortedList.Keys.ToArray();
            if (_isMinFirst)
            {
                return Mathf.Min(keys);
            }
            else
            {
                return Mathf.Max(keys);
            }
        }

        public void Clear()
        {
            _sortedList.Clear();
        }

        public bool Remove(float key)
        {
            return _sortedList.Remove(key);
        }
    }
}