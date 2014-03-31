using System;
using System.Globalization;

namespace CloudFileTransfer
{
    public enum FileTransferStatus
    {
        NoRequests=0,
        SyncRequested,
        ModifyOpsOnHold,
        NewFileInCloud,
        TransferComplete
    }
    public class LogEntry
    {
        public FileTransferStatus TransferStatus { get; internal set; }
        public DateTime UTCdate { get; internal set; }
        /// <summary>
        /// if sync requested, will be the identifier
        /// if newfileincloud, will be the filename (Not including path as this will vary between computers)
        /// </summary>
        public string Instructions { get; internal set; }
    }
    internal static class LogExtensions
    {
        public static LogEntry CreateLogEntry(FileTransferStatus fileTransferStatus)
        {
            return new LogEntry
            {
                TransferStatus = fileTransferStatus,
                UTCdate = DateTime.UtcNow
            };
        }
        public static string ToCSV(this LogEntry logEntry)
        {
            string returnVar = logEntry.TransferStatus.ToString() + ',' + logEntry.UTCdate.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern);
            if (string.IsNullOrEmpty(logEntry.Instructions))
            {
                return returnVar + "\r\n";
            }
            return returnVar + ',' + logEntry.Instructions + "\r\n";
        }
        public static LogEntry FromCSV(string line)
        {
            if (string.IsNullOrEmpty(line)) 
            {
                return new LogEntry(); 
            }
            string[] vals = line.Split(',');
            LogEntry returnVar = new LogEntry
            {
                TransferStatus = (FileTransferStatus)Enum.Parse(typeof(FileTransferStatus), vals[0], false),
                UTCdate = DateTime.ParseExact(vals[1], DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern, CultureInfo.InvariantCulture)
            };
            if (vals.Length == 3)
            {
                returnVar.Instructions = vals[2];
            }
            return returnVar;
        }
    }
}
