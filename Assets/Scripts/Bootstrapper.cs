using System.Net;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField] private TransportClient client;
	[SerializeField] private string address;
	[SerializeField] private TransportServer server;

	private void Start()
	{
		if (server != null)
		{
			server.StartServer(3030);
		}

		if (client != null)
		{
			if (IPAddress.TryParse(address, out IPAddress ipAddress))
			{
				if (server != null)
				{
					ipAddress = IPAddress.Loopback;
				}
				client.ConnectClient(ipAddress, 3030);
				client.Send("Sending first message");
			}
			else
			{
				Debug.LogError($"Malformed address : {address}");
			}
		}
	}
}
