using System;
using System.IO;

namespace CloudFileTransfer
{
    public class TransferLog
    {
        #region Fields
        #endregion
        #region Constructor
        public TransferLog(string filePath)
        {
            FilePath = filePath;
        }
        #endregion
        #region Properties
        public string FilePath { get; private set; }
        #endregion
        #region Methods
        public void RequestSync(string identifier)
        {
            LogEntry syncRequest = LogExtensions.CreateLogEntry(FileTransferStatus.SyncRequested);
            syncRequest.Instructions = identifier;
            File.AppendAllText(FilePath, syncRequest.ToCSV());
        }
        public void NotifyOnHold()
        {
            File.AppendAllText(FilePath, LogExtensions.CreateLogEntry(FileTransferStatus.ModifyOpsOnHold).ToCSV());
        }
        public void NotifyFileInCloud(string fileName)
        {
            LogEntry inCloudEntry = LogExtensions.CreateLogEntry(FileTransferStatus.NewFileInCloud);
            inCloudEntry.Instructions = fileName;
            File.AppendAllText(FilePath, inCloudEntry.ToCSV());
        }
        public void NotifyComplete()
        {
            File.AppendAllText(FilePath, LogExtensions.CreateLogEntry(FileTransferStatus.TransferComplete).ToCSV());
        }
        public LogEntry GetCurrentLogEntry()
        {
            return LogExtensions.FromCSV(GetMostRecentFileLine());
        }
        string GetMostRecentFileLine()
        {
            if (!File.Exists(FilePath)) { return null; }
            string returnVar;
            try
            {
                returnVar = File.ReadAllText(FilePath);
            }
            catch
            {
                return null;
            }
            if (string.IsNullOrEmpty(returnVar)) { return null; }
            int lastNewLine = returnVar.LastIndexOf("\r\n", returnVar.Length-3); // "The resulting string does not contain the terminating carriage return and/or line feed."
            if (lastNewLine == -1) { lastNewLine = 0; }
            else { lastNewLine += 2; }
            return returnVar.Substring(lastNewLine, returnVar.Length-lastNewLine-2);
        }
        public bool UpdateIsRequested(object identifier)
        {
            var entry = GetCurrentLogEntry();
            return (entry.TransferStatus == FileTransferStatus.SyncRequested && entry.Instructions == identifier.ToString());
        }
        #endregion
    }
}
