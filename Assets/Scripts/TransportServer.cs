using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TransportServer : MonoBehaviour, ITransponder
{
	private UdpClient client;
	private CancellationTokenSource cts;
	public event Action<byte[]> OnReceive;

	private IPEndPoint remote = null;

	public void StartServer(int port)
	{
		try
		{
			Debug.Log("Starting Server");
			client = new UdpClient(port);
			cts = new CancellationTokenSource();
			Debug.Log("Polling Packets");
			Task.Run(() => PollPackets(cts.Token), cts.Token);
		}
		catch (Exception e)
		{
			Debug.LogError($"Something went wrong while starting the server: {e.Message}");
		}
	}

	public void Transmit(byte[] buffer)
	{
		if (remote == null)
		{
			return;
		}

		client.Send(buffer, buffer.Length, remote);
	}

	private void PollPackets(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				byte[] rxBytes = client.Receive(ref remote);
				OnReceive?.Invoke(rxBytes);
			}
			catch (Exception e)
			{
				Debug.LogError($"Server receive failed: {e}");
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
