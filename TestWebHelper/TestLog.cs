using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using WebHelper.Log;

namespace TestWebHelper
{
    [TestClass]
    public class TestLog
    {
        ILogger logger;
        const string Test = "test";
        const string JsonTest = @"{
'startDate':'2015-10-25',

'endDate':'2015-10-27',

'topX':'1-5',

'destId':684,

'currencyCode':'EUR',

'catId':0,

'subCatId':0,

'dealsOnly':false,

'sortOrder':'PRICE_FROM_A'
}";

        [TestMethod]
        public void TestLogProperties()
        {
            const string LogDir = "Log";
            Logger.LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogDir);
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogDir);
            var testPath = Path.Combine(baseDir,String.Format("log_{0}.txt", DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss")));
            Assert.IsTrue(string.Equals(Logger.LogFilePath, testPath), "Something wrong with file path");
        }

        [TestMethod]
        public void TestWriteMessages()
        {
            logger.Message(Test);
            Thread.Sleep(1000);
            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                var content = streamReader.ReadLine();
                Assert.IsTrue(content.Contains(Test), "Message didn't write to the log file");
            }

            logger.Message(Test, Test, Test);
            Thread.Sleep(1000);
            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                streamReader.ReadLine();
                streamReader.ReadLine();
                streamReader.ReadLine();
                var anotherContent = streamReader.ReadLine().Trim();
                Assert.IsTrue(anotherContent.Equals("test,test", StringComparison.InvariantCulture), "Message didn't write to the log file");
            }

            logger.Message(Test, LogType.String, "Another test");
            Thread.Sleep(1000);
            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                streamReader.ReadLine();
                streamReader.ReadLine();
                streamReader.ReadLine();
                streamReader.ReadLine();
                streamReader.ReadLine();
                streamReader.ReadLine();
                var anotherContent = streamReader.ReadLine();
                Assert.IsTrue(anotherContent.Contains("Another test"), "Message didn't write to the log file");
            }
        }

        [TestMethod]
        public void TestWriteJsonMessages()
        {
            if (File.Exists(Logger.LogFilePath))
            {
                File.Delete(Logger.LogFilePath);
            }

            logger.Message("Json test", LogType.Json, JsonTest);
            Thread.Sleep(1000);
            
            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                var list = new List<string>();
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }

                var strContent = string.Join(" ", list.Skip(1));
                var contentObj = JsonConvert.DeserializeObject(strContent);
                var contentStr = JsonConvert.SerializeObject(contentObj);
                var testObj = JsonConvert.DeserializeObject(JsonTest);
                var testStr = JsonConvert.SerializeObject(testObj);
                Assert.AreEqual(testStr, contentStr, "Message didn't write a json  to the log file");
            }
        }

        
        [TestMethod]
        public void TestWriteExceptionMessages()
        {
            if (File.Exists(Logger.LogFilePath))
            {
                File.Delete(Logger.LogFilePath);
            }

            var exception = new Exception("TestException");
            var testStr = "Message: TestException  Stack trace:  Source:";
            logger.Message("Test exception", LogType.Exception, exception);
            Thread.Sleep(1000);
            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                var list = new List<string>();
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }

                var strContent = string.Join(" ", list.Skip(1)).Trim();

                Assert.IsTrue(string.Equals(strContent, testStr), "Message didn't write an exceptions to the log file");
            }
        }

    }
}
