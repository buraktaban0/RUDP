using RUDP.Internal;
using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Server
{
	public class UDPClientHandler
	{

		public int Id { get; internal set; }

		public IPEndPoint RemoteEP { get; set; }

		public ushort ServerKey { get; internal set; }
		public ushort ClientKey { get; internal set; }
		public ushort CombinedKey { get { return (ushort)(ServerKey ^ ClientKey); } }


		private TrafficHandler trafficHandler;
		private UDPServer server;

		internal UDPClientHandler(int id, IPEndPoint remoteEP, UDPServer server, TrafficHandler trafficHandler)
		{
			this.Id = id;
			this.RemoteEP = remoteEP;
			this.server = server;
			this.trafficHandler = trafficHandler;
		}

		public void Send(Packet packet)
		{
			packet.Key = CombinedKey;
			trafficHandler.Send(packet, RemoteEP);
		}

		public void SendReliable(Packet packet)
		{
			packet.Key = CombinedKey;
			trafficHandler.SendReliable(packet, RemoteEP);
		}

	}
}
