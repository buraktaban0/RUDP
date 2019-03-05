using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP.Internal
{
	internal static class Utility
	{

		public static byte[] SubArray(byte[] arr, int offset, int count)
		{
			byte[] sub = new byte[count];
			Buffer.BlockCopy(arr, offset, sub, 0, count);
			return sub;
		}


		public static byte[] Combine(byte[] first, byte[] second)
		{
			int len = 0;
			byte[] ret = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, ret, len, first.Length);
			len += first.Length;
			Buffer.BlockCopy(second, 0, ret, len, second.Length);
			return ret;
		}

		public static byte[] Combine(byte[] first, byte[] second, byte[] third)
		{
			int len = 0;
			byte[] ret = new byte[first.Length + second.Length + third.Length];
			Buffer.BlockCopy(first, 0, ret, len, first.Length);
			len += first.Length;
			Buffer.BlockCopy(second, 0, ret, len, second.Length);
			len += second.Length;
			Buffer.BlockCopy(third, 0, ret, len, third.Length);
			return ret;
		}

		public static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth)
		{
			int len = 0;
			byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length];
			Buffer.BlockCopy(first, 0, ret, len, first.Length);
			len += first.Length;
			Buffer.BlockCopy(second, 0, ret, len, second.Length);
			len += second.Length;
			Buffer.BlockCopy(third, 0, ret, len, third.Length);
			len += third.Length;
			Buffer.BlockCopy(fourth, 0, ret, len, fourth.Length);
			return ret;
		}

		public static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth, byte[] fifth)
		{
			int len = 0;
			byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length];
			Buffer.BlockCopy(first, 0, ret, len, first.Length);
			len += first.Length;
			Buffer.BlockCopy(second, 0, ret, len, second.Length);
			len += second.Length;
			Buffer.BlockCopy(third, 0, ret, len, third.Length);
			len += third.Length;
			Buffer.BlockCopy(fourth, 0, ret, len, fourth.Length);
			len += fourth.Length;
			Buffer.BlockCopy(fifth, 0, ret, len, fifth.Length);
			return ret;
		}

		public static byte[] Combine(params byte[][] arrays)
		{
			byte[] ret = new byte[arrays.Sum(x => x.Length)];
			int offset = 0;
			foreach (byte[] data in arrays)
			{
				Buffer.BlockCopy(data, 0, ret, offset, data.Length);
				offset += data.Length;
			}
			return ret;
		}


		public static uint FromLittleEndian(byte[] bytes)
		{
			int pos = 0;
			uint result = 0;
			int length = bytes.Length;
			for (int i = 0; i < length; i += 1)
			{
				result |= ((uint)bytes[i] << pos);
				pos += 8;
			}
			return result;
		}
		public static ulong FromLittleEndianLong(byte[] bytes)
		{
			int pos = 0;
			ulong result = 0;
			int length = bytes.Length;
			for (int i = 0; i < length; i += 1)
			{
				result |= ((uint)bytes[i] << pos);
				pos += 8;
			}
			return result;
		}

		/// <summary>
		/// Convert an unsigned integer value to a byte array, using little endian approach.
		/// </summary>
		/// <param name="val"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public static byte[] ToLittleEndian(uint val, int len)
		{
			byte[] bytes = new byte[len];
			int pos = 0;
			for (int i = 0; i < len; i += 1)
			{
				bytes[i] = (byte)(val >> pos);
				pos += 8;
			}
			return bytes;
		}
		public static byte[] ToLittleEndian(ulong val, int len)
		{
			byte[] bytes = new byte[len];
			int pos = 0;
			for (int i = 0; i < len; i += 1)
			{
				bytes[i] = (byte)(val >> pos);
				pos += 8;
			}
			return bytes;
		}

		public static uint GetRandomUInt()
		{
			byte[] bytes = new byte[4];
			new Random().NextBytes(bytes);
			return FromLittleEndian(bytes);
		}


		public static byte PackBytes(byte a, byte b)
		{
			return (byte)((a << 4) | b);
		}

		public static Tuple<byte, byte> UnpackBytes(byte pack)
		{
			byte a = (byte)(pack >> 4);
			byte b = (byte)(pack - (a << 4));

			return Tuple.Create(a, b);
		}

		public static ulong PackUInts(uint a, uint b)
		{
			return ((ulong)a << 32) | b;
		}

		public static Tuple<uint, uint> UnpackULong(ulong v)
		{
			uint b = (uint)v;
			uint a = (uint)(v >> 32);
			return Tuple.Create(a, b);
		}


		public static long GetUnixTimestamp()
		{
			return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
		}



		public static byte[] GenerateChecksum(byte[] a)
		{
			byte[] chc = new byte[2];
			byte b1 = 85;
			byte b2 = (byte)~b1;
			int max = a.Length - 1;
			for (int i = 0; i < max; i += 2)
			{
				b1 = (byte)((b1 & a[i]) | a[i + 1]);
				b2 = (byte)((b2 | a[i]) & a[i + 1]);
			}

			chc[0] = b1;
			chc[1] = b2;

			return chc;
		}

	}
}
