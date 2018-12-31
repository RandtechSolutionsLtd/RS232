using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace ConsoleAppForPorts
{
	class Program
	{
		static void Main(string[] args)
		{
			var names = new List<string>(SerialPort.GetPortNames());
			Console.WriteLine("Serial ports:");
			foreach (string name in names)
				Console.WriteLine(name);

			string portName = "";
			while (!names.Contains(portName))
			{
				Console.Write("Choose one:");
				portName = Console.ReadLine();
			}

			var port = new SerialPort(portName);

			port.DataReceived += new SerialDataReceivedEventHandler(p_DataReceived);
			port.Open();
			string line;
			do
			{
				line = Console.ReadLine();
				port.Write(line);
			} while (line != "quit");
			port.Close();


		}

		static void p_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			Console.WriteLine(
				(sender as SerialPort).ReadExisting());
		}
	}
}
