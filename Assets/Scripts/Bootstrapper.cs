using System.Net;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField] private int port = 3030;
	[SerializeField] private TransportClient client;
	[SerializeField] private string address;
	[SerializeField] private TransportServer server;

	private PacketLoop loop;

	private void Start()
	{
		if (server != null)
		{
			server.StartServer(port);
			loop = new PacketLoop(1, server);
			return;
		}

		if (client != null)
		{
			if (IPAddress.TryParse(address, out IPAddress ipAddress))
			{
				client.ConnectClient(ipAddress, port);
				loop = new PacketLoop(1, client);
			}
			else
			{
				Debug.LogError($"Malformed address : {address}");
			}
		}
	}

	private void OnApplicationQuit()
	{
		if (loop == null)
		{
			return;
		}

		loop.Stop();
	}
}
