using log4net;
using System.IO.Ports;

namespace Randtech.RS232FileTransfer.CommonLibrary
{
	/// <summary>
	/// Abstract base class for the SEND and GET application classes to hold common functionality
	/// </summary>
	public class ProgramAbstractBase
	{
		/// <summary>
		/// Our log4net log
		/// </summary>
		protected ILog Log => Functions.GetLogger();

		/// <summary>
		/// Our RSR232 serial port object
		/// </summary>
		public SerialPort Port { get; set; }
	}
}
