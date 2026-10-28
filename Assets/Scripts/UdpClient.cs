using System;
using System.Net;
using System.Text;
using UnityEngine;

public class UdpClient : MonoBehaviour
{
	private System.Net.Sockets.UdpClient client;

	public void ConnectClient(IPAddress address, int port)
	{
		try
		{
			client = new System.Net.Sockets.UdpClient();
			client.Connect(address, port);
		}
		catch (Exception e)
		{
			Debug.LogError($"Something went wrong while creating client or connecting to address {address}. Error: {e.Message}");
		}
	}

	public void Send(string message)
	{
		if (client == null)
		{
			Debug.Log("Client is null");
			return;
		}

		byte[] txMsg = Encoding.UTF8.GetBytes(message);
		
		try
		{
			client.SendAsync(txMsg, txMsg.Length);
		}
		catch (Exception e)
		{
			Debug.LogError($"Error while sending message: {e.Message}");
		}
		
	}

	private void OnApplicationQuit()
	{
		client?.Close();
	}
}
