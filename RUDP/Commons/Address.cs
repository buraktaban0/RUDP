using RUDP.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Commons
{
	public class Address
	{
		public IPAddress IPAddress { get; set; }
		public int Port { get; set; }

		public Address(IPAddress iPAddress, int port)
		{
			IPAddress = iPAddress;
			Port = port;
		}

		public static ulong Pack(IPEndPoint ep)
		{
			byte[] address = ep.Address.GetAddressBytes();
			ulong packed = Utility.FromLittleEndian(address);
			packed = packed | (((ulong)ep.Port) << 32);
			return packed;
		}

		public static IPEndPoint Unpack(ulong val)
		{
			int port = (int)(val >> 32);
			byte[] addressBytes = Utility.ToLittleEndian((uint)val, 4);
			return new IPEndPoint(new IPAddress(addressBytes), port);
		}

		public static IPEndPoint DuplicateEndPoint(IPEndPoint ep)
		{
			return Unpack(Pack(ep));
		}

	}
}
