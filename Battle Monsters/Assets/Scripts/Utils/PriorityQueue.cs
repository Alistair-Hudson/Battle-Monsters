using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleMonsters.Utils
{
    public class Node<T>
    {
        public T Data;
        public float Priority;
    }

    public class PriorityQueue<T>
    {
        private bool _isMinFirst;
        private List<Node<T>> _itemList;

        public int Count { get => _itemList.Count; }

        public PriorityQueue(bool isMinFirst)
        {
            _itemList = new List<Node<T>>();
            _isMinFirst = isMinFirst;
        }

        ~PriorityQueue()
        {
            Clear();
        }

        public void Enqueue(float priority, T data)
        {
            Node<T> node = new Node<T> { Data = data, Priority = priority };
            _itemList.Add(node);
        }

        public T Dequeue()
        {
            T data = Peek();
            Remove(data);
            return data;
        }

        private T Peek()
        {
            Node<T> node = _itemList[0];
            if (_isMinFirst)
            {
                for (int i = 1; i < _itemList.Count; i++)
                {
                    if (node.Priority > _itemList[i].Priority)
                    {
                        node = _itemList[i];
                    }
                }
            }
            else
            {
                for (int i = 1; i < _itemList.Count; i++)
                {
                    if (node.Priority < _itemList[i].Priority)
                    {
                        node = _itemList[i];
                    }
                }
            }
            return node.Data;
        }

        public void Clear()
        {
            _itemList.Clear();
        }

        public bool Remove(T data)
        {
            foreach (var node in _itemList)
            {
                if (data.Equals(node.Data))
                {
                    return _itemList.Remove(node);
                }
            }
            return false;
        }
    }
}