using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RUDP.Internal;
using RUDP.Packets;

namespace RUDP.Server
{
	public class UDPServer : ITrafficHandler
	{

		public IPEndPoint LocalEP
		{
			get
			{
				return trafficHandler.LocalEP;
			}
		}

		private TrafficHandler trafficHandler;

		private IUDPServer eventHandler;

		public UDPServer(IUDPServer eventHandler)
		{
			this.eventHandler = eventHandler;
			trafficHandler = new TrafficHandler(this);
		}

		public void Start(IPEndPoint localEP)
		{
			trafficHandler.Start(localEP);
		}


		public void Send(string text, IPEndPoint remoteEP)
		{
			trafficHandler.Send(text, remoteEP);
		}


		public void OnBindFailed(Exception ex)
		{
			eventHandler.OnBindFailed(ex);
		}

		public void OnBindSuccessful()
		{
			eventHandler.OnBindSuccessful();
		}

		public void OnPacketReceived(Packet packet)
		{
			eventHandler.OnPacketReceived(packet);
		}
	}
}
