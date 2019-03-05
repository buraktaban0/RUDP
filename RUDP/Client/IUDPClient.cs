using RUDP.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Client
{
	public interface IUDPClient
	{
		void OnPacketReceived(Packet packet);

		void OnBindFailed(Exception ex);

		void OnBindSuccessful();


	}
}
