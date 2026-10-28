using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TransportServer : MonoBehaviour
{
	private UdpClient client;
	private CancellationTokenSource cts;

	public void StartServer(int port)
	{
		try
		{
			Debug.Log("Starting Server");
			client = new UdpClient(port);
			cts = new CancellationTokenSource();
			Debug.Log("Polling messages");
			Task.Run(() => PollMessages(cts.Token), cts.Token);
		}
		catch (Exception e)
		{
			Debug.LogError($"Something went wrong while starting the server: {e.Message}");
		}
	}

	private void PollMessages(CancellationToken token)
	{
		IPEndPoint ep = null;
		while (!token.IsCancellationRequested)
		{
			try
			{
				byte[] rxBytes = client.Receive(ref ep);
				string rxMsg = Encoding.UTF8.GetString(rxBytes);
				Debug.Log($"From: {ep.Address} -> Message: {rxMsg}");
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
