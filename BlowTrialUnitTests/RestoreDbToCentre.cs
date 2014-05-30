using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrialUnitTests
{
    [TestClass]
    class RestoreDbToCentre
    {
        RestoreDbToCentre()
        {
            foreach (string dirName in new string[] { "Dummy Cloud Folder", "dummy centre app data", "dummy oversight app data" })
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }
    }
}
