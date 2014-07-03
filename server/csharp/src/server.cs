using System;
using System.Collections.Concurrent;
using System.Threading;
using Castle.Zmq;

public class Program
{
	public static void Main(string[] args)
	{
		if(args.Length < 1 || args.Length > 2)
		{
			Console.WriteLine("Usage: client.exe <request address> <data address>");
			return;
		}

		var requestAddress = args[0];
		var dataAddress = args[1];
		var zmqContext = new Context();
		var queue = new BlockingCollection<byte[]>();

		var writeThread = new Thread(() =>
			{
				using(var reqSocket = zmqContext.CreateSocket(SocketType.Rep))
				{
					reqSocket.Bind(requestAddress);

					for(long count = 0; ; count++)
					{
						reqSocket.Recv();
						reqSocket.Send(queue.Take());

						if(count % 1000 == 0)
							Console.WriteLine("Sent {0}", count);
					}
				}
			});

		var readThread = new Thread(() =>
			{
				using(var dataSocket = zmqContext.CreateSocket(SocketType.Pull))
				{
					dataSocket.Bind(dataAddress);

					for(long count = 0; ; count++)
					{
						queue.Add(dataSocket.Recv());

						if(count % 1000 == 0)
							Console.WriteLine("Received {0}", count);
					}
				}
			});

		writeThread.Start();
		readThread.Start();

		Console.WriteLine("Press return to quit");
		Console.ReadLine();

		writeThread.Abort();
		readThread.Abort();
	}
}
