using Randtech.RS232FileTransfer.CommonLibrary;
using System;
using System.IO.Ports;

namespace Send
{
	/// <summary>
	/// Main application for SENDING files over RS232
	/// </summary>
	class Program : ProgramAbstractBase
	{

		[STAThread]
		static void Main(string[] args)
		{
			new Program(Functions.GetSerialPort(true), Settings.SendFileName);
		}

		private Program(SerialPort _port, string fileName)
		{
			Log.Info("Send application launched.");
			Functions.GetConfigStatus(Log); Log.Info("Send app constructor called.");
			Port = _port;
			try
			{
				Log.Info($"Trying to send file '{fileName}' using port '{Port.PortName}'.");
				Functions.SendTextFile(Port, fileName);
				Log.Info(Settings.MessageSuccess);
				Console.WriteLine(Settings.MessageSuccess);
			}
			catch(Exception ex)
			{
				Log.Debug("Error sending file!");
				Log.Debug(Settings.MessageFail);
				Console.WriteLine(Settings.MessageFail);

				Log.Debug(ex);
				throw ex;
			}
		}
	}
}