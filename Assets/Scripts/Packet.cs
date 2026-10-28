using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Packet
{
	public int Seq;
	public int LastAckSeq;
}

public static class PacketUtils
{
	public static byte[] Serialize<T>(T packet) where T : struct
	{
		byte[] buffer = new byte[UnsafeUtility.SizeOf<T>()];
		MemoryMarshal.Write(buffer, ref packet);

		return buffer;
	}

	public static T Deserialize<T>(byte[] buffer) where T : struct
	{
		return MemoryMarshal.Read<T>(buffer);
	}
}
