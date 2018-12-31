using Randtech.RS232FileTransfer.CommonLibrary;
using System;
using System.IO;
using System.IO.Ports;

namespace Get
{
	/// <summary>
	/// Main application for GETTING files over RS232
	/// </summary>
	public class Program : ProgramAbstractBase
	{
		/// <summary> String object to hold incoming file text until the end is reached and it can be saved to disk </summary>
		public string Buffer { get; set; }
		public int TryCount { get; set; } = 1;
		public int MaxTryCount { get; set; } = Settings.MaximumRetries;

		[STAThread]
		static void Main(string[] args)
		{
			new Program(Functions.GetSerialPort(false), "");
		}

		public Program(SerialPort port, string buffer)
		{

			Log.Info("Get application launched.");
			Functions.GetConfigStatus(Log);
			Log.Info("Get app constructor called.");
			Port = port;
			Buffer = buffer;

			Log.Info($"Port '{Port.PortName}' is now open and awaiting file...");

			while (TryCount <= MaxTryCount)
			{
				string msg = $"Try {TryCount}/{MaxTryCount}";
				Log.Info(msg);
				Console.WriteLine(msg);
				try
				{
					Init();

					Console.WriteLine("Hit ESC to quit");
					while (Console.ReadKey().Key != ConsoleKey.Escape){
					} // todo replace with better await functionality

					return;
				}
				catch(UnauthorizedAccessException ex)
				{
					TryCount++;

					if (TryCount < MaxTryCount)
					{
						Console.WriteLine(Settings.MessageFail);
						Console.WriteLine(ex.Message);
						Log.Debug(Settings.MessageFail);
						Log.Debug(ex.ToString());
					}
					else
					{
						Console.WriteLine("Fatal exception. Exceeded maximum number of tries. See log file for information.");
						Console.WriteLine(ex.ToString());
					}
					Port.Close();
					Console.WriteLine($"HINT: Verify that no other application is using the required GET port '{Port.PortName}''.");
				}
				catch (Exception ex)
				{
					TryCount++;

					if (TryCount < MaxTryCount)
					{
						Console.WriteLine(Settings.MessageFail);
						Console.WriteLine(ex.Message);
						Log.Debug(Settings.MessageFail);
						Log.Debug(ex.ToString());
					} else{
						Console.WriteLine("Fatal exception. Exceeded maximum number of tries. See log file for information.");
						Console.WriteLine(ex.ToString());
					}
					Port.Close();
					Console.WriteLine($"HINT: Check the log file for exception details.");
				}
			}

			Console.WriteLine("Shutting down...");
			return;
		}

		private void Init()
		{
			Log.Info($"Port '{Port.PortName}' is now open and awaiting file...");
			Console.WriteLine($"{DateTime.Now.ToLongTimeString()}. Awaiting file...");

			Log.Info("Attaching events and emptying received buffer.");
			Port.DataReceived += Port_DataReceived;
			Port.ErrorReceived += Port_ErrorReceived;
			Buffer = string.Empty;

			Log.Info("Checking if port is open.");
			if (!Port.IsOpen)
			{
				Log.Info("No. Opening port.");
				Port.Open();
			}
			else
			{
				Log.Info("Port already open.");
			}
		}

		/// <summary>
		/// Event for error handling
		/// </summary>
		private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			Log.Info("Port error received.");
		}

		/// <summary>
		/// Event for Data Received handling
		/// </summary>
		private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			Log.Info("Receiving data event raised...");
			try
			{
				GetIncomingFile();
			}
			catch (Exception ex)
			{
				Log.Debug("Error getting file!");
				Log.Debug(Settings.MessageFail);
				Console.WriteLine(Settings.MessageFail);

				Log.Debug(ex);
				throw ex;
			}
		}

		private void GetIncomingFile()
		{
			Log.Info("Getting incoming file! ");
			string incoming = Port.ReadExisting();
			Buffer += incoming;

			bool isEOF = false;
			while (!isEOF)
			{
				if (Buffer.Contains("\r\n"))
				{
					isEOF = true;
					Log.Info("Found termination character in incoming response buffer.");
					Log.Info("Get path to save the incoming data received from serial port.");

					Log.Info("Calling Functions to get file name.");
					//string fileName = "incoming.txt";
					string fileName = Functions.CalculateSaveFileName(Buffer);
					string path = Path.Combine(Settings.ReceiveFolderName, fileName); // todo get fle name from incoming				
					Log.Info($"Save file path: {path}");
					Log.Info("Checking if file exists?");
					if (File.Exists(path))
					{
						Log.Info("File exists. Deleting.");
						File.Delete(path);
					}
					else
					{
						Log.Info("File does not exist.");
					}

					Log.Info("Adding newline character to end of incoming text.");
					string createText = Buffer + Environment.NewLine;

					Log.Info($"Writing all text to path. Text char length: {createText.ToCharArray().Length}");
					File.WriteAllText(path, createText);

					Log.Info(Settings.MessageSuccess);
					Console.WriteLine(Settings.MessageSuccess);

					string log = $"Wrote {createText.ToCharArray().Length} characters of incoming data to file '{path}'";

					if(File.Exists(path)){
						Log.Info(log);
						Console.WriteLine(DateTime.Now.ToString() + ": " + log);
					}
				}
			}

			Buffer = string.Empty;

			Log.Info("EOF received. Application terminated.");
		}
	}
}
