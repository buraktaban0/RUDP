using System;
using System.Collections.Generic;
using System.Text;

namespace RUDP.Collections
{
	internal class BoundedList<T>
	{
		private List<T> list;


		public int Count { get { return list.Count; } }
		public int Capacity { get { return list.Capacity; } }

		public BoundedList(int capacity)
		{
			list = new List<T>(capacity);
		}


		public void Add(T item)
		{
			if (list.Count == list.Capacity)
			{
				list.RemoveAt(0);
			}

			list.Add(item);
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, item);
		}

		public void Remove(T item)
		{
			list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public void Clear()
		{
			list.Clear();
		}

	}
}
