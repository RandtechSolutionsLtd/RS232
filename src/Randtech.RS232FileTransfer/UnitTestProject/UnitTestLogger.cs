using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randtech.RS232FileTransfer.CommonLibrary;
using System.Linq;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTestLogger
	{
		[TestMethod]
		public void VerifyFileAppenderExists()
		{
			var rootAppender = ((Hierarchy)LogManager.GetRepository())
				.Root.Appenders.OfType<RollingFileAppender>()
				.FirstOrDefault();

			string filename = rootAppender != null ? rootAppender.File : string.Empty;

			Assert.IsFalse(string.IsNullOrEmpty(filename));
		}

		/// <summary>
		/// Creates an example log entry
		/// </summary>
		[TestMethod]
		public void CreateLogFile()
		{
			var log = Functions.GetLogger();
			log.Fatal("This is a Fatal message");
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void CheckPort_Send()
		{
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void CheckPort_Get()
		{

		}
	}
}