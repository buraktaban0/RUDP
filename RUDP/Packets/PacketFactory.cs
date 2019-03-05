using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Packets
{
	public static class PacketFactory
	{
		private static ulong currentId = 0;

		public static Packet GetPacket()
		{
			return new Packet() { UniqueID = currentId++, type = Packet.Type.None };
		}

	}
}
