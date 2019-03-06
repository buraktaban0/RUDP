using RUDP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Collections
{
	public class ClientCollection
	{
		public int Capacity { get; set; }


		private Dictionary<int, UDPClientHandler> clients;
		private Dictionary<IPEndPoint, UDPClientHandler> ipsToClients;

		public ClientCollection(int capacity)
		{
			this.Capacity = capacity;
			clients = new Dictionary<int, UDPClientHandler>();
			ipsToClients = new Dictionary<IPEndPoint, UDPClientHandler>();
		}


		public UDPClientHandler Get(int id)
		{
			UDPClientHandler client;
			if (clients.TryGetValue(id, out client))
			{
				return client;
			}
			return null;
		}


		public bool ContainsId(int id)
		{
			return clients.ContainsKey(id);
		}

		public bool ContainsEndPoint(IPEndPoint ep)
		{
			return ipsToClients.ContainsKey(ep);
		}


		public void AddTo(UDPClientHandler client, int id)
		{
			clients.Add(id, client);
			ipsToClients.Add(client.RemoteEP, client);
		}

		public int GetFirstAvailableId()
		{
			for (int i = 0; i < Capacity; i += 1)
			{
				if (!ContainsId(i))
				{
					return i;
				}
			}

			return -1;
		}

		public int AddToFirstAvailable(UDPClientHandler client)
		{
			for (int i = 0; i < Capacity; i += 1)
			{
				if (!ContainsId(i))
				{
					clients.Add(i, client);
					return i;
				}
			}

			return -1;
		}


		public void Remove(UDPClientHandler client)
		{
			clients.Remove(client.Id);
			ipsToClients.Remove(client.RemoteEP);
		}

	}
}
