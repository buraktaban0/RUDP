using RUDP.Client;
using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestClientConsole
{
	class Program : IUDPClient
	{
		public static UDPClient client;

		static void Main(string[] args)
		{
			Program program = new Program();

			IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3001);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);

			Console.WriteLine("CLIENT - " + localEP);
			

			client = new UDPClient(program);
			client.Start(localEP);

			while (true)
			{
				string line = Console.ReadLine();
				client.Send(line, remoteEP);
			}
		}

		public void OnBindFailed(Exception ex)
		{
			Console.WriteLine("Client.OnBindFailed -> " + ex);
		}

		public void OnBindSuccessful()
		{
			Console.WriteLine("Client.OnBindSuccessful");
		}

		public void OnPacketReceived(Packet packet)
		{
			Console.WriteLine("Client.OnPacketReceived -> " + packet.Text);
		}
	}
}
