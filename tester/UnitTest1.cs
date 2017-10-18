using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WorkingDirectory = @"..\..\BrowserHistoryViewer\thirdPartyTool\";
            startInfo.WorkingDirectory = @"..\..\..\BrowserHistoryViewer\thirdPartyTool\";
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ESEDatabaseView.exe /table WebCacheV01.dat Container_21 /sjson IEHistory.txt";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
