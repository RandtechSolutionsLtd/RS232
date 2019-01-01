using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace Randtech.RS232FileTransfer.CommonLibrary
{
	/// <summary>
	/// Contains functionality that may be shared between send and receive program
	/// </summary>
	public class Functions
	{
		#region Public Methods

		/// <summary>
		/// Debug information output
		/// </summary>
		public static void GetConfigStatus(ILog log)
		{
			log.Info($"Config folder: {Settings.ConfigFolderName}");
			bool isFolderFound = Directory.Exists(Settings.ConfigFolderName.ToString());
			log.Info($"Folder found? {isFolderFound}");

			log.Info($"Send config file: {Settings.SendConfigFileName}");
			log.Info($"Send config path: {Settings.SendConfigPathName}");
			bool isSendFileFound = File.Exists(Settings.SendConfigPathName);
			log.Info($"Send config file found at path? {isSendFileFound}");


			log.Info($"Receive config file: {Settings.ReceiveConfigFileName}");
			log.Info($"Receive config path: {Settings.ReceiveConfigPathName}");
			bool isReceiveFileFound = File.Exists(Settings.ReceiveConfigPathName);
			log.Info($"Receive config file found at path? {isReceiveFileFound}");

			if (!isFolderFound)
			{
				throw new DirectoryNotFoundException($"Check folder '{Settings.ConfigFolderName}' exists.");
			}

			if (!isSendFileFound)
			{
				throw new FileNotFoundException($"Send config file not found. Check path '{Settings.SendConfigPathName}' exists.");
			}

			if (!isReceiveFileFound)
			{
				throw new FileNotFoundException($"Receive config file not found. Check path '{Settings.ReceiveConfigPathName}' exists.");
			}

			log.Info($"Send port name: {Settings.SendPortName}");
			log.Info($"Receive port name: {Settings.ReceivePortName}");

			log.Info($"Send from: {Settings.SendFileName}");
			log.Info($"Receive to: {Settings.ReceiveFolderName}");

			log.Info($"Success message: {Settings.MessageSuccess}");
			log.Info($"Fail message: {Settings.MessageFail}");
		}

		/// <summary>
		/// Instantiates a SerialPort object using configuration
		/// </summary>
		/// <param name="isSendPort">Optional. Indicates we want to use the Send port</param>
		/// <returns>A SerialPort object</returns>
		public static SerialPort GetSerialPort(bool isSendPort = true)
		{
			string desiredPortName = GetPortName(isSendPort);

			VerifyPortName(desiredPortName);

			var port = new SerialPort
			{
				BaudRate = Settings.BaudRate,
				DataBits = Settings.DataBits,
				PortName = desiredPortName,
				StopBits = Settings.StopBits,
				Parity = Settings.Parity,
				Handshake = Settings.Handshake,

				RtsEnable = Settings.RtsEnable,
				DtrEnable = Settings.DtrEnable,
				ReadTimeout = Settings.ReadTimeout,
				WriteTimeout = Settings.WriteTimeout
			};

			EnsurePortIsClosed(port);

			return port;
		}

		/// <summary> Works out the name of the file that will be saved, by looking at the file buffer's first 5 characters </summary>
		/// <param name="buffer">File buffer to read filename from</param>
		/// <returns>The five digit file name, with the .txt extension</returns>
		public static string CalculateSaveFileName(string buffer)
		{
			char[] delimiter = Environment.NewLine.ToString().ToCharArray();
			var lines = new List<string>(buffer.Split(delimiter));
			string fileName = lines[0].Trim();

			if (fileName.Length != 5)
			{
				throw new Exception("Invalid file name length");
			}
			fileName += ".txt";

			return fileName;
		}

		/// <summary>
		/// Gets a file as an array of bytes
		/// </summary>
		/// <param name="fileName">File to get as bytes</param>
		/// <returns>Byte[] version of file</returns>
		public static byte[] ReadByteArrayFromFile(string fileName)
		{
			byte[] buff = null;
			var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			var br = new BinaryReader(fs);
			long numBytes = new FileInfo(fileName).Length;
			buff = br.ReadBytes((int)numBytes);
			return buff;
		}

		/// <summary>
		/// Transmits a file with the Path supplied over a SerialPort
		/// </summary>
		/// <param name="port">SerialPort to send file over</param>
		/// <param name="pathName">Path to file</param>
		public static void SendTextFile(SerialPort port, string pathName)
		{
			port.Open();
			port.Write(File.OpenText(pathName).ReadToEnd());
			port.Close();
		}

		/// <summary> Transmits a file with the Path supplied over a SerialPort as a Binary file. </summary>
		/// <param name="port">SerialPort to send file over</param>
		/// <param name="pathName">Path to file</param>
		public static void SendBinaryFile(SerialPort port, string pathName)
		{
			port.Open();
			using (FileStream fs = File.OpenRead(pathName))
				port.Write((new BinaryReader(fs)).ReadBytes((int)fs.Length), 0, (int)fs.Length);

			port.Close();
		}

		/// <summary>
		/// Get a log4net logger
		/// </summary>
		/// <returns></returns>
		public static ILog GetLogger()
		{
			return LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}

		/// <summary>
		/// Gets the names of all com ports
		/// </summary>
		/// <returns>A list of port names</returns>
		public List<string> GetAllComPortNames()
		{
			var allPorts = new List<string>();
			foreach (string portName in SerialPort.GetPortNames())
			{
				allPorts.Add(portName);
			}
			return allPorts;
		}

		#endregion

		#region Private Methods

		private static string GetPortName(bool isSendPort)
		{
			return isSendPort ? Settings.SendPortName : Settings.ReceivePortName;
		}

		private static void EnsurePortIsClosed(SerialPort port)
		{
			while (port.IsOpen)
			{
				Debug.WriteLine($"Port  '{port.PortName}' is open. Attempting to close.");
				port.Close();
			}
		}

		private static void VerifyPortName(string desiredPortName)
		{
			var names = new List<string>(SerialPort.GetPortNames()); // existing ports
			if (!names.Contains(desiredPortName)){
				throw new InvalidOperationException($"There is no port on this machine with the required port name '{desiredPortName}'. Check configuration.");
			}
		} 
		#endregion
	}
}