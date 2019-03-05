using RUDP.Collections;
using RUDP.Commons;
using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Internal
{
	internal class TrafficHandler
	{
		public Socket Socket { get; private set; }

		public IPEndPoint LocalEP { get; private set; }

		private ITrafficHandler eventHandler;

		private ReliableMonitor reliableMonitor;

		private byte[] buffer;

		private EndPoint remoteEP;

		private Dictionary<Packet.Reliability, Action<Packet>> packetReceivedCallbacks;

		private BoundedList<Packet> latestReceived;

		public TrafficHandler(ITrafficHandler eventHandler)
		{
			this.eventHandler = eventHandler;
			reliableMonitor = new ReliableMonitor(this);

			packetReceivedCallbacks = new Dictionary<Packet.Reliability, Action<Packet>>();
			packetReceivedCallbacks[Packet.Reliability.None] = packet => { };
			packetReceivedCallbacks[Packet.Reliability.Ack] = OnAckReceived;
			packetReceivedCallbacks[Packet.Reliability.Reliable] = OnReliableReceived;
			packetReceivedCallbacks[Packet.Reliability.Unreliable] = OnUnreliableReceived;

			latestReceived = new BoundedList<Packet>(1024 * 8);
		}

		public void Start(IPEndPoint localEP)
		{
			this.LocalEP = localEP;
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			try
			{
				Socket.Bind(localEP);
			}
			catch (Exception ex)
			{
				eventHandler.OnBindFailed(ex);
				return;
			}

			eventHandler.OnBindSuccessful();

			reliableMonitor.Start();

			remoteEP = new IPEndPoint(IPAddress.Any, 0);
			buffer = new byte[1024];

			Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(ReceiveCallback), null);
		}


		private void ReceiveCallback(IAsyncResult ar)
		{
			int c = Socket.EndReceiveFrom(ar, ref remoteEP);
			Packet packet = Packet.FromBytes(buffer, c);
			packet.RemoteEP = Address.DuplicateEndPoint((IPEndPoint)remoteEP);

			Console.WriteLine(packet.UniqueID);

			Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(ReceiveCallback), null);

			if (packet != null)
			{
				packetReceivedCallbacks[packet.reliability].Invoke(packet);
			}
		}


		private void OnAckReceived(Packet packet)
		{
			Console.WriteLine("Ack received: " + packet.UniqueID);
			reliableMonitor.Remove(packet);
		}

		private void OnReliableReceived(Packet packet)
		{
			Console.WriteLine("Reliable received: " + packet.UniqueID);
			Packet ack = packet.GetAck();
			SendInternal(ack);

			if (latestReceived.Contains(packet) == false)
			{
				latestReceived.Add(packet);
				eventHandler.OnPacketReceived(packet);
			}
		}

		private void OnUnreliableReceived(Packet packet)
		{
			Console.WriteLine("Unreliable received: " + packet.UniqueID);

			if (latestReceived.Contains(packet) == false)
			{
				latestReceived.Add(packet);
				eventHandler.OnPacketReceived(packet);
			}
		}


		internal void SendInternal(Packet packet)
		{
			double random = new Random().NextDouble();

			if (random < 0.4) // Simulate packet loss
			{
				return;
			}

			Socket.SendTo(packet.GetBytes(), SocketFlags.None, packet.RemoteEP);
		}

		public void Send(string text, IPEndPoint remoteEP, bool isReliable = false)
		{
			Packet packet = Packet.FromText(text);
			packet.RemoteEP = remoteEP;

			if (isReliable)
			{
				packet.reliability = Packet.Reliability.Reliable;
				reliableMonitor.Add(packet);
			}
			else
			{
				packet.reliability = Packet.Reliability.Unreliable;
			}

			SendInternal(packet);
		}

	}
}
