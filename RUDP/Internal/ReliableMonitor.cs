using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RUDP.Internal
{
	internal class ReliableMonitor
	{
		private static readonly int WAIT_BEFORE_RESEND = 50; // ms

		private readonly object locker = new object();
		private readonly object emptyLocker = new object();

		private bool isRunning;

		private Thread thread;

		private List<Packet> packets;

		private TrafficHandler trafficHandler;

		public ReliableMonitor(TrafficHandler trafficHandler)
		{
			this.trafficHandler = trafficHandler;
			thread = new Thread(Run);
			packets = new List<Packet>();
		}


		public void Start()
		{
			isRunning = true;
			thread.Start();
		}

		public void Stop()
		{
			isRunning = false;
			Monitor.Enter(locker);
			Monitor.PulseAll(locker);
			Monitor.Enter(emptyLocker);
			Monitor.PulseAll(emptyLocker);
			Monitor.Exit(emptyLocker);
			Monitor.Exit(locker);
			thread.Join(3000);
		}

		public void Add(Packet packet)
		{
			packet.timeout = Utility.GetUnixTimestamp() + WAIT_BEFORE_RESEND;
			Monitor.Enter(locker);
			packets.Add(packet);
			if (packets.Count == 1)
			{
				// TEST WITHOUT ENTER
				Monitor.Enter(emptyLocker);
				Monitor.PulseAll(emptyLocker);
				Monitor.Exit(emptyLocker);
			}
			Monitor.Exit(locker);
		}

		public void Remove(Packet packet)
		{
			Monitor.Enter(locker);

			for (int i = 0; i < packets.Count; i += 1)
			{
				if (packets[i].UniqueID == packet.UniqueID)
				{
					packets.RemoveAt(i);
					break;
				}
			}

			Monitor.Exit(locker);
		}

		private void Run()
		{
			Monitor.Enter(locker);

			while (isRunning)
			{
				if (packets.Count == 0)
				{
					// TEST ORDER
					Monitor.Enter(emptyLocker);
					Monitor.Exit(locker);
					Monitor.Wait(emptyLocker);
					Monitor.Exit(emptyLocker);
					Monitor.Enter(locker);
				}

				int waitFor = WAIT_BEFORE_RESEND;
				long now = Utility.GetUnixTimestamp();

				for (int i = 0, j = 0; i < packets.Count; i += 1)
				{

					Packet packet = packets[j];
					if (packet.timeout > now)
					{
						waitFor = (int)(packet.timeout - now);
						break;
					}
					else
					{
						packet.timeout = now + WAIT_BEFORE_RESEND;
						packets.RemoveAt(0);
						packets.Add(packet);
						trafficHandler.SendInternal(packet);
					}

				} // For end

				Monitor.Wait(locker, waitFor);

			}// While end

			Monitor.Exit(locker);

		} // Run end


	}
}
