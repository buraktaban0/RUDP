using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Packets
{
	public static class PacketFactory
	{
		private static ushort currentId = ushort.MaxValue;

		public static Packet CreatePacket()
		{
			return new Packet();
		}

		public static Packet RecyclePacket()
		{
			currentId += 1;
			return new Packet() { Id = currentId, type = Packet.Type.None };
		}

	}
}
