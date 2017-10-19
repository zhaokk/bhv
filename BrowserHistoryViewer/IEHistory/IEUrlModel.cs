using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserHistoryViewer.IEHistory
{
    public class IEUrlModel
    {
        public string EntryId { get; set; }
        public string ContainerId { get; set; }
        public string CacheId { get; set; }
        public string UrlHash { get; set; }
        public string SecureDirectory { get; set; }
        public string FileSize { get; set; }
        public string Type { get; set; }
        public string Flags { get; set; }
        public string AccessCount { get; set; }
        public string SyncTime { get; set; }
        public string CreationTime { get; set; }
        public string ExpiryTime { get; set; }
        public string ModifiedTime { get; set; }
        public string AccessedTime { get; set; }
        public string PostCheckTime { get; set; }
        public string SyncCount { get; set; }
        public string ExemptionDelta { get; set; }
        public string Url { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponseHeaders { get; set; }
        public string RedirectUrl { get; set; }
        public string Group { get; set; }
        public string ExtraData { get; set; }
    }
}
