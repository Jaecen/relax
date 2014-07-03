using System;
using Castle.Zmq;

public class Program
{
	public static void Main(string[] args)
	{
		if(args.Length != 2)
		{
			Console.WriteLine("Usage: client.exe <request address> <data address>");
			return;
		}

		var requestAddress = args[0];
		var dataAddress = args[1];

		var taskRequestData = new byte[] { 0 };

		var messageData = new byte[1024];
		for(var i = 0; i < messageData.Length; i++)
			messageData[i] = (byte)(i % 255);

		using(var zmqContext = new Context())
		using(var reqSocket = zmqContext.CreateSocket(SocketType.Req))
		using(var dataSocket = zmqContext.CreateSocket(SocketType.Push))
		{
			reqSocket.Connect(requestAddress);
			dataSocket.Connect(dataAddress);
			
			while(true)
			{
				reqSocket.Send(taskRequestData);

				var requestData = reqSocket.Recv();
				var count = requestData[0];

				if(count == 0x00)
				{
					// Generate 10K messages
					messageData[0] = 0x01;
					for(var i = 0; i < 10000; i++)
						dataSocket.Send(messageData);
				}
				else if(count < 0x05)
				{
					messageData[0] = (byte)(count + 1);
					dataSocket.Send(messageData);
				}
			}
		}
	}
}
