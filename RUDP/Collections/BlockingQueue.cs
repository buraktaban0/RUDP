using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RUDP.Collections
{
	internal class BlockingQueue<T>
	{
		private readonly object locker = new object();


		public int Count { get { return queue.Count; } }

		private Queue<T> queue;

		public BlockingQueue()
		{
			queue = new Queue<T>();
		}


		public void Enqueue(T item)
		{
			Monitor.Enter(locker);
			queue.Enqueue(item);
			if (queue.Count == 1)
			{
				Monitor.PulseAll(locker);
			}
			Monitor.Exit(locker);
		}

		public T Dequeue()
		{
			Monitor.Enter(locker);

			while (queue.Count == 0)
			{
				Monitor.Wait(locker);
			}

			T item = queue.Dequeue();

			Monitor.Exit(locker);

			return item;
		}

	}
}
