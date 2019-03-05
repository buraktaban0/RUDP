using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RUDP.Internal;
using RUDP.Packets;

namespace RUDP.Client
{
	public class UDPClient : ITrafficHandler
	{
		public IPEndPoint LocalEP
		{
			get
			{
				return trafficHandler.LocalEP;
			}
		}

		private TrafficHandler trafficHandler;

		private IUDPClient eventHandler;

		public UDPClient(IUDPClient eventHandler)
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
			trafficHandler.Send(text, remoteEP, true);
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
