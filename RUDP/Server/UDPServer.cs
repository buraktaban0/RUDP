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


		private Dictionary<Packet.Type, Action<Packet>> receivedCallbacks;

		public UDPServer(IUDPServer eventHandler)
		{
			this.eventHandler = eventHandler;
			trafficHandler = new TrafficHandler(this);

			receivedCallbacks = new Dictionary<Packet.Type, Action<Packet>>();
			receivedCallbacks[Packet.Type.ConnectionRequest] = OnConnectionRequestReceived;
			receivedCallbacks[Packet.Type.ChallengeResponse] = OnChallengeResponseReceived;
			receivedCallbacks[Packet.Type.Event] = OnEventReceived;
			receivedCallbacks[Packet.Type.Challenge] = OnUnknownPacketReceived;
			receivedCallbacks[Packet.Type.ConnectionAccepted] = OnUnknownPacketReceived;
			receivedCallbacks[Packet.Type.ConnectionRejected] = OnUnknownPacketReceived;
			receivedCallbacks[Packet.Type.None] = OnUnknownPacketReceived;

		}

		public void Start(IPEndPoint localEP)
		{
			trafficHandler.Start(localEP);
		}


		public void Send(string text, IPEndPoint remoteEP)
		{
			trafficHandler.Send(text, remoteEP, true);
		}


		public void OnPacketReceived(Packet packet)
		{
			receivedCallbacks[packet.type].Invoke(packet);
		}


		private void OnConnectionRequestReceived(Packet packet)
		{

		}

		private void OnChallengeResponseReceived(Packet packet)
		{

		}

		private void OnEventReceived(Packet packet)
		{

		}

		private void OnUnknownPacketReceived(Packet packet)
		{

		}


		public void OnBindFailed(Exception ex)
		{
			eventHandler.OnBindFailed(ex);
		}

		public void OnBindSuccessful()
		{
			eventHandler.OnBindSuccessful();
		}
	}
}
