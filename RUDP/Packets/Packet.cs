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
		
		public string Text { get; set; }

		#region Internals

		internal ushort Id { get; set; }

		internal ushort Key { get; set; }

		internal long timeout;

		#endregion

		public Packet Duplicate()
		{
			Packet packet = PacketFactory.CreatePacket();
			packet.RemoteEP = this.RemoteEP;
			packet.type = this.type;
			packet.reliability = this.reliability;
			packet.Id = this.Id;
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
			byte[] idBytes = Utility.ToLittleEndian(Id, 2);
			byte[] saltBytes = Utility.ToLittleEndian(Key, 2);
			byte[] textBytes = Encoding.UTF8.GetBytes(Text);
			byte[] bytes = Utility.Combine(new byte[] { relAndType }, idBytes, saltBytes, textBytes);

			return bytes;
		}

		public static Packet FromText(string text)
		{
			Packet packet = PacketFactory.RecyclePacket();
			packet.Text = text;
			packet.type = Type.Event;
			return packet;
		}

		public static Packet FromBytes(byte[] buffer, int c)
		{
			Packet packet = PacketFactory.CreatePacket();

			byte[] bytes = Utility.SubArray(buffer, 0, c);
			Tuple<byte, byte> tuple = Utility.UnpackBytes(bytes[0]);
			packet.reliability = (Reliability)tuple.Item1;
			packet.type = (Type)tuple.Item2;
			packet.Id = Utility.FromLittleEndianShort(Utility.SubArray(bytes, 1, 2));
			packet.Key = Utility.FromLittleEndianShort(Utility.SubArray(bytes, 3, 2));
			packet.Text = Encoding.UTF8.GetString(Utility.SubArray(bytes, 5, bytes.Length - 5));
			return packet;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Packet)
			{
				Packet other = (Packet)obj;
				return Id == other.Id && RemoteEP.Equals(other.RemoteEP);
			}

			return false;
		}
	}
}
