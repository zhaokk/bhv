using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;
using System.Diagnostics;
using BrowserHistoryViewer.shared;
using Ariely_AlphaVSS_Sample02;
using Newtonsoft.Json;
using BrowserHistoryViewer.IEHistory;

namespace browserHistoryBrowser
{
    public class IEHistory : IWebsiteHistorySQLFunction
    {

        private DBReader _m_db;
        private List<Url> History;
        public string RunningDir;
        private string _fileLocation;
        private Lazy<List<string>> _m_tables;
        public bool CopyHistoryFile()
        {
            //unlockDbFile();
            string userName = Environment.UserName;
            string baseAddress = @"C:\Users\{0}\AppData\Local\Microsoft\Windows\WebCache\WebCacheV01.dat";
            string directory = string.Format(baseAddress, userName);
            RunningDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _fileLocation = RunningDir + @"\thirdPartyTool";
            try
            {
                vss.copy(_fileLocation);
                //TheProgram.Main();
                //System.IO.File.Copy(directory, _fileLocation);
            }
            catch (Exception e)
            {
                throw;
            }
            return true;
        }
        public void unlockDbFile()
        {
            int count1 = Process.GetProcesses().
                                 Where(pr => pr.ProcessName == "dllhost").Count();
            Helper.KillProcessByName("dllhost");
            int count = Process.GetProcesses().
                                 Where(pr => pr.ProcessName == "dllhost").Count();
            if (count > 0)
            {
                Helper.KillProcessByName("dllhost");
            }
        }
        public List<Url> RetriveHistoryAsJson()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WorkingDirectory = @"..\..\BrowserHistoryViewer\thirdPartyTool\";
            string runningDir = RunningDir + @"\thirdPartyTool\";
            startInfo.WorkingDirectory = runningDir;
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ESEDatabaseView.exe /table WebCacheV01.dat Container_21 /sjson IEHistory.txt";
            process.StartInfo = startInfo;
            process.Start();
            string fileDir = runningDir + "IEHistory.txt";
            string jsonString = File.ReadAllText(fileDir);
            var deserializedItemsFromItems = JsonConvert.DeserializeObject<List<IEUrlModel>>(jsonString);
            return ConvertIEUrlToUrl(deserializedItemsFromItems);
        }
        public List<Url> ConvertIEUrlToUrl(List<IEUrlModel> ieModel)
        {
            List<Url> result = new List<Url>();
            foreach (IEUrlModel ieUrl in ieModel)
            {
                result.Add(new Url
                {
                    url = ieUrl.Url,
                    visitCount = ieUrl.AccessCount,
                    lastVisitTime = ieUrl.AccessedTime
                });
            }
            return result;

        }
        //public void RetriveHistoryFromThridParty()
        //{
        //    System.Diagnostics.Process process = new System.Diagnostics.Process();
        //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //    //startInfo.WorkingDirectory = @"..\..\BrowserHistoryViewer\thirdPartyTool\";
        //    startInfo.WorkingDirectory = @"\thirdPartyTool\";
        //    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //    startInfo.FileName = "cmd.exe";
        //    startInfo.Arguments = "/C BrowsingHistoryView.exe /LoadIE 1 /HistorySource 2 /sxml IEHistory.xml";
        //    process.StartInfo = startInfo;
        //    process.Start();
        //}
        public List<Url> GetHistory(int langId)
        {
            CopyHistoryFile();
            try
            {
                List<Url> IEUrlList = RetriveHistoryAsJson();
                ConvertUrlsToCsv(IEUrlList);
                return IEUrlList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:");
                Console.WriteLine("Exception happened while retrive data form IE exception message: " + e.Message);
            }

            return new List<Url>();
        }
        public void ConvertUrlsToCsv(List<Url> urls)
        {
            if (urls == null)
            {
                Console.WriteLine("IE history not available");
                Console.ReadLine();
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("title,url,visitCount,lastVisstTime");
                foreach (var data in urls)
                {
                    sb.AppendLine(data.title + "," + data.url + ", " + data.visitCount + ", " + data.lastVisitTime);
                }
                string filePath = RunningDir + @"\output\IEresult.csv";
                File.WriteAllText(filePath, sb.ToString());
            }
        }
    }
}
