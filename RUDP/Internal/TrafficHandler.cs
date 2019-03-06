using RUDP.Collections;
using RUDP.Commons;
using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

		private BlockingQueue<Packet> pendingOut;

		private Thread sendThread;
		private bool isRunning;

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

			pendingOut = new BlockingQueue<Packet>();
		}

		public void Start(IPEndPoint localEP)
		{
			isRunning = true;

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

			sendThread = new Thread(Run_Send);
			sendThread.Start();

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

			Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP, new AsyncCallback(ReceiveCallback), null);

			if (packet != null)
			{
				packetReceivedCallbacks[packet.reliability].Invoke(packet);
			}
		}


		private void OnAckReceived(Packet packet)
		{
			reliableMonitor.Remove(packet);
		}

		private void OnReliableReceived(Packet packet)
		{
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

			if (latestReceived.Contains(packet) == false)
			{
				latestReceived.Add(packet);
				eventHandler.OnPacketReceived(packet);
			}
		}


		private void Run_Send()
		{
			while (isRunning)
			{
				Packet packet = pendingOut.Dequeue();
				Socket.SendTo(packet.GetBytes(), SocketFlags.None, packet.RemoteEP);
			}
		}


		internal void SendInternal(Packet packet)
		{
			double random = new Random().NextDouble();

			if (random < 0.4) // Simulate packet loss
			{
				return;
			}

			pendingOut.Enqueue(packet);
		}


		public void SendReliable(Packet packet, IPEndPoint remoteEP)
		{
			packet.RemoteEP = remoteEP;
			packet.reliability = Packet.Reliability.Reliable;
			reliableMonitor.Add(packet);
			SendInternal(packet);
		}

		public void Send(Packet packet, IPEndPoint remoteEP)
		{
			packet.RemoteEP = remoteEP;
			packet.reliability = Packet.Reliability.Unreliable;
			SendInternal(packet);
		}

		public void Send(string text, IPEndPoint remoteEP, bool isReliable = false)
		{
			Packet packet = Packet.FromText(text);

			if (isReliable)
			{
				SendReliable(packet, remoteEP);
			}
			else
			{
				Send(packet, remoteEP);
			}

		}

	}
}
