using System;

public interface ITransponder
{
	public void Transmit(byte[] buffer);
	public event Action<byte[]> OnReceive;
}
