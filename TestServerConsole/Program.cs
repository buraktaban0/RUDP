using RUDP.Packets;
using RUDP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestServerConsole
{
	class Program : IUDPServer
	{
		public static UDPServer server;

		static void Main(string[] args)
		{
			Console.WriteLine("SERVER");

			Program program = new Program();

			IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001);

			server = new UDPServer(program);
			server.Start(localEP);


			while (true)
			{
				string line = Console.ReadLine();
				server.Send(line, remoteEP);
			}
		}

		public void OnBindFailed(Exception ex)
		{
			Console.WriteLine("Server.OnBindFailed -> " + ex);
		}

		public void OnBindSuccessful()
		{
			Console.WriteLine("Server.OnBindSuccessful");
		}

		public void OnClientConnected(UDPClientHandler client)
		{
			throw new NotImplementedException();
		}

		public void OnClientDisconnected(UDPClientHandler client)
		{
			throw new NotImplementedException();
		}

		public void OnClientReconnected(UDPClientHandler oldClient, UDPClientHandler newClient)
		{
			throw new NotImplementedException();
		}

		public void OnClientPacketReceived(Packet packet, UDPClientHandler client)
		{
			throw new NotImplementedException();
		}


		public void OnPacketReceived(Packet packet)
		{
			Console.WriteLine("Server.OnPacketReceived -> " + packet.Text);
		}
	}
}
