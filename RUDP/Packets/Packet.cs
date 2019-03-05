using RUDP.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Packets
{
	public class Packet
	{
		public enum Type : byte
		{
			None = 0,
			ConnectionRequest = 1,
			ConnectionAccepted = 2,
			ConnectionRejected = 3,
			Challenge = 4,
			ChallengeResponse = 5,
			Event = 6
		}

		public enum Reliability : byte
		{
			None = 0,
			Unreliable = 1,
			Reliable = 2,
			Ack = 3
		}


		public EndPoint RemoteEP { get; set; }
		public IPEndPoint IPEndPoint { get { return (IPEndPoint)RemoteEP; } }

		public Type type { get; set; }
		public Reliability reliability { get; set; }

		public ulong UniqueID { get; set; }

		public string Text { get; set; }

		#region Internals

		internal long timeout;

		#endregion

		public Packet Duplicate()
		{
			Packet packet = new Packet();
			packet.RemoteEP = this.RemoteEP;
			packet.type = this.type;
			packet.reliability = this.reliability;
			packet.UniqueID = this.UniqueID;
			return packet;
		}

		public Packet GetAck()
		{
			Packet packet = Duplicate();
			packet.reliability = Reliability.Ack;
			packet.Text = Text;
			return packet;
		}

		public byte[] GetBytes()
		{
			byte relAndType = Utility.PackBytes((byte)reliability, (byte)type);
			byte[] idBytes = Utility.ToLittleEndian(UniqueID, 8);
			byte[] textBytes = Encoding.UTF8.GetBytes(Text);
			byte[] bytes = Utility.Combine(new byte[] { relAndType }, idBytes, textBytes);

			return bytes;
		}

		public static Packet FromText(string text)
		{
			Packet packet = PacketFactory.GetPacket();
			packet.Text = text;
			return packet;
		}

		public static Packet FromBytes(byte[] buffer, int c)
		{
			Packet packet = PacketFactory.GetPacket();

			byte[] bytes = Utility.SubArray(buffer, 0, c);
			Tuple<byte, byte> tuple = Utility.UnpackBytes(bytes[0]);
			packet.reliability = (Reliability)tuple.Item1;
			packet.type = (Type)tuple.Item2;
			packet.UniqueID = Utility.FromLittleEndianLong(Utility.SubArray(bytes, 1, 8));
			packet.Text = Encoding.UTF8.GetString(Utility.SubArray(bytes, 9, bytes.Length - 9));
			return packet;
		}

		public override int GetHashCode()
		{
			return UniqueID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Packet)
			{
				return UniqueID == ((Packet)obj).UniqueID;
			}

			return false;
		}
	}
}
