using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTestSerialPorts
	{

		public Randtech.RS232FileTransfer.CommonLibrary.Functions MyFunctions { get; set; } = new Randtech.RS232FileTransfer.CommonLibrary.Functions();

		[TestMethod]
		public void ShouldHaveSomePorts()
		{
			List<string> portNames = MyFunctions.GetAllComPortNames();

			Debug.WriteLineIf(portNames.Any(), $"Found {portNames.Count} ports. Names: {string.Join(", ", portNames)}");
			Assert.IsTrue(portNames.Any());
		}

		[TestMethod]
		public void ShouldOpenComPort()
		{
			using (var myPort = new System.IO.Ports.SerialPort("COM1"))
			{
				if (myPort.IsOpen == false) //if not open, open the port
					myPort.Open();
				//do your work here	`
				myPort.Close();

				Assert.IsFalse(myPort.IsOpen);
			}
		}


		[TestMethod]
		public void ShouldSend()
		{
			
		}

		
	}
}
