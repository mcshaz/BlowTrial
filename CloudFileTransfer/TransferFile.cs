using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace CloudFileTransfer
{
    public static class TransferFile
    {
        public static void RequestUpdate(TransferLog log, object identifier,Func<string> createFileForTransfer, int millisecondsBetweenChecks = 200, Action onComplete = null)
        {
            log.RequestSync(identifier.ToString());

            var dt = new DispatcherTimer
            {
                 Interval = new TimeSpan(millisecondsBetweenChecks * TimeSpan.TicksPerMillisecond)
            };
            EventHandler checkRequest = null;
            dt.Tick += checkRequest = (o, e) =>
                {
                    if (log.GetCurrentLogEntry().TransferStatus == FileTransferStatus.ModifyOpsOnHold)
                    {
                        dt.Tick -= checkRequest;
                        dt.Stop();
                        dt = null;
                        string fn = createFileForTransfer();
                        log.NotifyFileInCloud(fn);

                        while (log.GetCurrentLogEntry().TransferStatus != FileTransferStatus.TransferComplete)
                        {
                            Thread.Sleep(millisecondsBetweenChecks);
                        }

                        onComplete?.Invoke();
                    }
                };
            dt.Start();
        }

        public static void AllowUpdate(TransferLog log, Action<FileInfo> updateFile,int millisecondsBetweenChecks = 200)
        {
            log.NotifyOnHold();
            LogEntry currentLogEntry;
            while ((currentLogEntry = log.GetCurrentLogEntry()).TransferStatus != FileTransferStatus.NewFileInCloud)
            {
                Thread.Sleep(millisecondsBetweenChecks);
            }
            FileInfo fi = new FileInfo(Path.Combine(Path.GetDirectoryName(log.FilePath), currentLogEntry.Instructions));
            updateFile(fi);
            log.NotifyComplete();
        }
    }
}
