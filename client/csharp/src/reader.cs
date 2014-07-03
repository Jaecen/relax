using System;
using Castle.Zmq;

public class Program
{
	public static void Main(string[] args)
	{
		if(args.Length != 1)
		{
			Console.WriteLine("Usage: reader <request address>");
			return;
		}

		var requestAddress = args[0];

		var taskRequestData = new byte[] { 0 };

		using(var zmqContext = new Context())
		using(var reqSocket = zmqContext.CreateSocket(SocketType.Req))
		{
			reqSocket.Connect(requestAddress);

			for(long count = 0; ; count++)
			{
				reqSocket.Send(taskRequestData);
				reqSocket.Recv();

				if(count % 1000 == 0)
					Console.WriteLine("Sent {0}", count);
			}
		}
	}
}
