using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TransportClient : MonoBehaviour, ITransponder
{
	private System.Net.Sockets.UdpClient client;
	public event Action<byte[]> OnReceive;
	private CancellationTokenSource cts;

	public void ConnectClient(IPAddress address, int port)
	{
		try
		{
			client = new System.Net.Sockets.UdpClient();
			client.Connect(address, port);
			cts = new CancellationTokenSource();
			Debug.Log("Polling messages");
			Task.Run(() => PollPackets(cts.Token), cts.Token);
		}
		catch (Exception e)
		{
			Debug.LogError($"Something went wrong while creating client or connecting to address {address}. Error: {e.Message}");
		}
	}

	public void Receive(byte[] buffer)
	{
		throw new NotImplementedException();
	}

	public void Transmit(byte[] buffer)
	{
		if (client == null)
		{
			Debug.LogError("Client is null");
			return;
		}

		try
		{
			client.Send(buffer, buffer.Length);
		}
		catch (Exception e)
		{
			Debug.LogError($"Error while sending byte buffer: {e.Message}");
		}
	}

	private void PollPackets(CancellationToken token)
	{
		IPEndPoint ep = null;
		while (!token.IsCancellationRequested)
		{
			try
			{
				byte[] rxBytes = client.Receive(ref ep);
				OnReceive?.Invoke(rxBytes);
			}
			catch (Exception e)
			{
				Debug.LogError($"Client receive failed: {e}");
				return;
			}
		}
	}

	private void OnApplicationQuit()
	{
		cts?.Cancel();
		client?.Close();
		cts = null;
	}
}
