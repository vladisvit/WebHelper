using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using WebHelper.Log;
using WebHelper.Log.Estimate;

namespace TestWebHelper
{
    [TestClass]
    public class TestEstimator
    {
        [TestMethod]
        public void Estimate()
        {
            Logger.LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Estimator");
            using (Estimator estimator = new Estimator(CounterType.Common, "SumIntegers", CounterMeasureType.Ticks))
            {
                var sum = 0;
                foreach (var item in Enumerable.Range(1, 1000))
                {
                    sum += item;
                }
            }

            using (StreamReader streamReader = new StreamReader(Logger.LogFilePath))
            {
                var content = streamReader.ReadLine();
                Assert.IsTrue(content.Contains("ticks."), "Message didn't write to the log file");
            }
        }
    }
}
