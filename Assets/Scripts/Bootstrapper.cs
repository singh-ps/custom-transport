using System.Net;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField] private int port = 3030;
	[SerializeField] private TransportClient client;
	[SerializeField] private string address;
	[SerializeField] private TransportServer server;

	private void Start()
	{
		if (server != null)
		{
			server.StartServer(port);
		}

		if (client != null)
		{
			if (IPAddress.TryParse(address, out IPAddress ipAddress))
			{
				if (server != null)
				{
					ipAddress = IPAddress.Loopback;
				}
				client.ConnectClient(ipAddress, port);
				client.Send("Sending first message");
			}
			else
			{
				Debug.LogError($"Malformed address : {address}");
			}
		}
	}
}
