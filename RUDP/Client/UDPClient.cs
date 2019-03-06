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

		public IPEndPoint ServerEP { get; private set; }

		private TrafficHandler trafficHandler;

		private IUDPClient eventHandler;

		private ushort ServerKey { get; set; }
		private ushort ClientKey { get; set; }
		private ushort CombinedKey { get { return (ushort)(ServerKey ^ ClientKey); } }

		public UDPClient(IUDPClient eventHandler)
		{
			this.eventHandler = eventHandler;
			trafficHandler = new TrafficHandler(this);
		}

		public void Start(IPEndPoint localEP, IPEndPoint remoteEP)
		{
			trafficHandler.Start(localEP);
			ServerEP = remoteEP;
		}


		public void Send(string text, IPEndPoint remoteEP)
		{
			trafficHandler.Send(text, remoteEP, true);
		}


		public void Send(Packet packet)
		{
			trafficHandler.Send(packet, ServerEP);
		}

		public void SendReliable(Packet packet)
		{
			trafficHandler.SendReliable(packet, ServerEP);
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
