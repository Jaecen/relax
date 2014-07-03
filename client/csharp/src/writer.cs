using System;
using Castle.Zmq;

public class Program
{
	public static void Main(string[] args)
	{
		if(args.Length < 1 || args.Length > 2)
		{
			Console.WriteLine("Usage: writer <data address> [message size]");
			return;
		}

		var dataAddress = args[0];
		var messageSize = args.Length == 2 ? Int32.Parse(args[1]) : 1024;

		var messageData = new byte[messageSize];
		for(var i = 0; i < messageData.Length; i++)
			messageData[i] = (byte)(i % Byte.MaxValue);

		using(var zmqContext = new Context())
		using(var dataSocket = zmqContext.CreateSocket(SocketType.Push))
		{
			dataSocket.Connect(dataAddress);

			for(long count = 0; ; count++)
			{
				dataSocket.Send(messageData);

				if(count % 1000 == 0)
					Console.WriteLine("Sent {0}", count);
			}
		}
	}
}
