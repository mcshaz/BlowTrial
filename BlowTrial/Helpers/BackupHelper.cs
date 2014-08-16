using Ionic.Zip;
using log4net;
using System;
using System.IO;

namespace BlowTrial.Helpers
{
    static class BackupHelper
    {
        static ILog GetLog()
        {
            return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="cloudDir"></param>
        /// <returns>Whether the action was succesful</returns>
        public static bool ZipVerifyAndPutInCloudDir(string filepath, string cloudDir)
        {
            int dotIndx = filepath.LastIndexOf('.');
            string filepathWithoutExtension = dotIndx==-1
                ?filepath
                :filepath.Substring(0,dotIndx);
            string tempFilename = filepathWithoutExtension + ".zip";
            bool isFixed;
            ILog log = null;
            using (ZipFile zip = new ZipFile())
            {
                zip.ParallelDeflateThreshold = -1;
                /*
                zip.BufferSize = 1000000;
                zip.CodecBufferSize = 1000000;
                 * */

                zip.AddFile(filepath, "");
                zip.Save(tempFilename);

                using (var sw = new StringWriter())
                {
                    if (ZipFile.CheckZip(tempFilename, true, sw))
                    {
                        isFixed = false;
                    }
                    else
                    {
                        sw.Flush();
                        log = GetLog();
                        log.WarnFormat(sw.ToString());
                        isFixed = true;
                    }
                }
            }
            try
            {
                if (isFixed)
                {
                    File.Delete(tempFilename);
                    File.Move(filepathWithoutExtension + "_Fixed.zip", tempFilename);
                }

                using (ZipFile zip = ZipFile.Read(tempFilename))
                {
                    zip[0].Extract(Stream.Null);
                }
                string destName = Path.Combine(cloudDir, Path.GetFileName(tempFilename));
                if (File.Exists(destName))
                {
                    File.Delete(destName);
                }
                File.Move(tempFilename, destName);
                return true;
            }
            catch (Exception e)
            {
                if (log == null) { log = GetLog(); }
                log.Error("Failed to create zip file - error on test extraction:", e);
                return false;
            }
        }
    }
}
