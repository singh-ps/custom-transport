using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
			Debug.LogError($"Something went wrong while creating client or connecting to address {address.ToString()}. Error: {e.Message}");
		}
	}

	public void SendMessage(string message)
	{
		if (client == null)
		{
			Debug.Log("Client is null");
		}

		byte[] txMsg = Encoding.UTF8.GetBytes(message);

		try
		{
			Task.Run(async () => { await client.SendAsync(txMsg, txMsg.Length); });
		}
		catch (Exception e)
		{
			Debug.LogError($"Error while sending message: {e.Message}");
		}
	}

	private void OnApplicationQuit()
	{
		client.Close();
	}
}
