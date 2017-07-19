using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace WebHelper.Log.Estimate
{
    /// <summary>
    /// Estimate the performance of code 
    /// <example> 
    /// <code>
    /// using (Estimator estimator = new Estimator(CounterType.Common, "SetHeaders", CounterMeasureType.Ticks))
    /// {
    ///     //code for estimate
    /// }
    /// </code>
    /// </summary>
    public class Estimator : IDisposable
    {
#pragma warning disable 169, 649
        private ILogger _logger; //it is OK. It uses an extension
#pragma warning restore 169, 649

        private Stopwatch _stopWatch;
        private CounterType _type;
        private CounterMeasureType _measure;
        private string _description;


        public Estimator(CounterType counterType = CounterType.Common, string description = null, CounterMeasureType measureType = CounterMeasureType.MilliSec)
        {
            Init(counterType, description, measureType);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                if (_stopWatch == null)
                {
                    return;
                }
                _stopWatch.Stop();
                var elapsedTime = String.Empty;
                switch (_measure)
                {
                    case CounterMeasureType.Sec:
                        elapsedTime = String.Format("{0} sec.", _stopWatch.Elapsed.Seconds);
                        break;
                    case CounterMeasureType.MilliSec:
                        elapsedTime = String.Format("{0} millisecond.", _stopWatch.ElapsedMilliseconds);
                        break;
                    case CounterMeasureType.Ticks:
                        elapsedTime = String.Format("{0} ticks.", _stopWatch.ElapsedTicks);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _logger.Message(String.Format("{0}[{1}]:{2}", _type, _description, elapsedTime));
            }
            catch (Exception ex)
            {
                _logger.Message("Estimator raises an Exception ", LogType.Exception, ex);
            }
        }

        [Conditional("DEBUG")]
        private void Init(CounterType counterType, string description, CounterMeasureType measureType)
        {
            _stopWatch = Stopwatch.StartNew();
            _type = counterType;
            _description = description;
            _measure = measureType;
            _stopWatch.Start();
        }
    }
}
