using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Server
{
	public interface IUDPServer
	{
		void OnBindFailed(Exception ex);

		void OnBindSuccessful();

		
		void OnClientConnected(UDPClientHandler client);

		void OnClientDisconnected(UDPClientHandler client);

		void OnClientPacketReceived(Packet packet, UDPClientHandler client);

		void OnPacketReceived(Packet packet);


	}

}
