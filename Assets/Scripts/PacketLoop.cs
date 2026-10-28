using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PacketLoop
{
	private readonly int waitMs;
	private readonly ITransponder transponder;

	private int lastAckedSeq = -1;
	private int lastSentSeq = -1;
	private readonly CancellationTokenSource cts = new();

	public PacketLoop(int rateHz, ITransponder transponder)
	{
		waitMs = 1000 / rateHz;
		this.transponder = transponder;
		transponder.OnReceive += ReceiveHandle;
		_ = Task.Run(() => StartLoop(cts.Token));
	}

	private void ReceiveHandle(byte[] buffer)
	{
		Packet p = PacketUtils.Deserialize<Packet>(buffer);
		lastAckedSeq = p.Seq > lastAckedSeq ? p.Seq : lastAckedSeq;
		Debug.Log($"Received ack for seq: {lastAckedSeq}");
	}

	private async Task StartLoop(CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				await Task.Delay(waitMs, token);
				lastSentSeq++;
				Packet p = new()
				{
					Seq = lastSentSeq,
					LastAckSeq = lastAckedSeq,
				};

				byte[] buffer = PacketUtils.Serialize(p);
				transponder.Transmit(buffer);
				Debug.Log($"Sending seq : {lastSentSeq}");
			}
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}

	public void Stop()
	{
		cts.Cancel();
		transponder.OnReceive -= ReceiveHandle;
	}
}
