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

		public IPEndPoint RemoteEP;


		private TrafficHandler trafficHandler;
		private UDPServer server;

		internal UDPClientHandler(UDPServer server, TrafficHandler trafficHandler)
		{
			this.server = server;
			this.trafficHandler = trafficHandler;
		}

		public void Send(Packet packet)
		{
			trafficHandler.Send(packet, RemoteEP);
		}

		public void SendReliable(Packet packet)
		{
			trafficHandler.SendReliable(packet, RemoteEP);
		}

	}
}
