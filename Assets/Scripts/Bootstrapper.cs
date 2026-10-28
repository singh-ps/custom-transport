using System.Net;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
	[SerializeField] private UdpClient client;
	[SerializeField] private UdpServer server;

	private void Start()
	{
		server.StartServer(3030);
		client.ConnectClient(IPAddress.Loopback, 3030);
		client.SendMessage("Sending first message"); 
	}
}
