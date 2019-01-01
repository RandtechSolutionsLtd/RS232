using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;

namespace Randtech.RS232FileTransfer.CommonLibrary
{
	/// <summary>
	/// Configuration class exposing config values through properties
	/// </summary>
	public class Settings
	{
		static Settings()
		{
			ReceiveConfigPathName = Path.Combine(ConfigFolderName, ReceiveConfigFileName);
			List<string> receiveConfig = new List<string>(File.ReadLines(ReceiveConfigPathName));
			ReceiveFolderName = receiveConfig[0];
			ReceivePortName = receiveConfig[1];

			SendConfigPathName = Path.Combine(ConfigFolderName, SendConfigFileName);
			List<string> sendConfig = new List<string>(File.ReadLines(SendConfigPathName));
			SendFileName = sendConfig[0];
			SendPortName = sendConfig[1];
		}

		public static string ConfigFolderName { get; set; } = ConfigurationManager.AppSettings["configFolder"];
		public static string SendConfigFileName { get; set; } = ConfigurationManager.AppSettings["configFileSend"];
		public static string ReceiveConfigFileName { get; set; } = ConfigurationManager.AppSettings["configFileReceive"];


		public static int BaudRate { get; } = int.Parse(ConfigurationManager.AppSettings["baudRate"] ?? "9600");
		public static string MessageFail { get; set; } = ConfigurationManager.AppSettings[" "];
		public static string MessageSuccess { get; set; } = ConfigurationManager.AppSettings["messageSuccess"];
		public static int MaximumRetries { get; set; } = int.Parse(ConfigurationManager.AppSettings["maxTries"] ?? "20");

		public static int DataBits { get; internal set; } = int.Parse(ConfigurationManager.AppSettings["dataBits"] ?? "1");



		public static bool RtsEnable { get; internal set; } = false;
		public static bool DtrEnable { get; internal set; } = false;
		public static int ReadTimeout { get; internal set; }  = -1;



		public static StopBits StopBits { get; internal set; } = StopBits.One;
		public static Parity Parity { get; internal set; } = Parity.None;
		public static Handshake Handshake { get; internal set; } = Handshake.RequestToSend;
		public static int WriteTimeout { get; internal set; } = 4600;




		public static string SendConfigPathName { get; internal set; }
		public static string ReceiveConfigPathName{ get; internal set; }

		public static string SendFileName { get; internal set; }
		public static string ReceiveFolderName { get; internal set; }

		public static string SendPortName { get; set; } 
		public static string ReceivePortName { get; set; }
	}
}