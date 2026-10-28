using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UdpServer : MonoBehaviour
{
	private System.Net.Sockets.UdpClient client;
	private CancellationTokenSource cts;
	private Task pollTask;

	public void StartServer(int port)
	{
		try
		{
			Debug.Log("Starting Server");
			client = new System.Net.Sockets.UdpClient(port);
			cts = new CancellationTokenSource();
			Debug.Log("Polling messages");
			pollTask = Task.Run(() => PollMessages(cts.Token), cts.Token);
		}
		catch (Exception e)
		{
			Debug.LogError($"Something went wrong while starting the server: {e.Message}");
		}
	}

	private Task PollMessages(CancellationToken cts)
	{
		IPEndPoint ep = null;
		while (true)
		{
			cts.ThrowIfCancellationRequested();
			byte[] rxBytes = client.Receive(ref ep);
			string rxMsg = Encoding.UTF8.GetString(rxBytes);
			Debug.Log($"From: {ep.Address} -> Message: {rxMsg}");
		}
	}

	private void OnApplicationQuit()
	{
		if (cts != null)
		{
			cts.Cancel();
			cts = null;
			pollTask = null;
		}
	}
}
