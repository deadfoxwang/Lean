/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using QuantConnect.Data;
using Python.Runtime;
namespace QuantConnect.Algorithm.CSharp
{
    public class BasicPythonIntegrationTemplateAlgorithm : QCAlgorithm
    {

        public override void Initialize()
        {
            SetStartDate(2018, 11, 10);  //Set Start Date
            SetEndDate(2018, 12, 1);
            SetCash(100000);             //Set Strategy Cash
            AddEquity("SPY", Resolution.Minute);

        }

        private decimal ComputeSin(decimal value)
        {
            using (Py.GIL())
            {
                dynamic np = Py.Import("numpy");
                return (decimal)np.sin(value);
            }
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            if (!Portfolio.Invested)
            {
            	SetHoldings("SPY", 1);
            	var sin = ComputeSin(10);
            	var sinOfTen = Math.Sin(10);
            	Log($"According to Python, the value of sin(10) is: {sin}");
            	Log($"According to C#, the value of sin(10) is: {sinOfTen}");
            }
        }
    }
}