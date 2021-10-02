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
 *
*/

using System.Collections.Generic;
using System.Linq;
using QuantConnect.Data;
using QuantConnect.Interfaces;
using QuantConnect.Securities;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// This regression algorithm verifies automatic option contract assignment behavior.
    /// </summary>
    /// <meta name="tag" content="regression test" />
    /// <meta name="tag" content="options" />
    /// <meta name="tag" content="using data" />
    /// <meta name="tag" content="filter selection" />
    public class OptionAssignmentRegressionAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Security Stock;

        private Security CallOption;
        private Symbol CallOptionSymbol;

        private Security PutOption;
        private Symbol PutOptionSymbol;

        public override void Initialize()
        {
            SetStartDate(2015, 12, 23);
            SetEndDate(2015, 12, 24);
            SetCash(100000);
            Stock = AddEquity("GOOG", Resolution.Minute);

            var contracts = OptionChainProvider.GetOptionContractList(Stock.Symbol, UtcTime).ToList();

            PutOptionSymbol = contracts
                .Where(c => c.ID.OptionRight == OptionRight.Put)
                .OrderBy(c => c.ID.Date)
                .First(c => c.ID.StrikePrice == 800m);

            CallOptionSymbol = contracts
                .Where(c => c.ID.OptionRight == OptionRight.Call)
                .OrderBy(c => c.ID.Date)
                .First(c => c.ID.StrikePrice == 600m);

            PutOption = AddOptionContract(PutOptionSymbol);
            CallOption = AddOptionContract(CallOptionSymbol);
        }

        public override void OnData(Slice data)
        {
            if (!Portfolio.Invested && Stock.Price != 0 && PutOption.Price != 0 && CallOption.Price != 0)
            {
                // this gets executed on start and after each auto-assignment, finally ending with expiration assignment
                MarketOrder(PutOptionSymbol, -1);
                MarketOrder(CallOptionSymbol, -1);
            }
        }

        public bool CanRunLocally { get; } = true;
        public Language[] Languages { get; } = {Language.CSharp};

        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "24"},
            {"Average Win", "9.60%"},
            {"Average Loss", "-16.86%"},
            {"Compounding Annual Return", "-99.166%"},
            {"Drawdown", "2.300%"},
            {"Expectancy", "0.046"},
            {"Net Profit", "-2.162%"},
            {"Sharpe Ratio", "-4.698"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "33%"},
            {"Win Rate", "67%"},
            {"Profit-Loss Ratio", "0.57"},
            {"Alpha", "2.375"},
            {"Beta", "-1.252"},
            {"Annual Standard Deviation", "0.199"},
            {"Annual Variance", "0.04"},
            {"Information Ratio", "-9.995"},
            {"Tracking Error", "0.358"},
            {"Treynor Ratio", "0.747"},
            {"Total Fees", "$12.00"},
            {"Estimated Strategy Capacity", "$310000.00"},
            {"Lowest Capacity Asset", "GOOCV VP83T1ZUHROL"},
            {"Fitness Score", "0.5"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "79228162514264337593543950335"},
            {"Return Over Maximum Drawdown", "-50.725"},
            {"Portfolio Turnover", "8.14"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$0"},
            {"Total Accumulated Estimated Alpha Value", "$0"},
            {"Mean Population Estimated Insight Value", "$0"},
            {"Mean Population Direction", "0%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "0%"},
            {"Rolling Averaged Population Magnitude", "0%"},
            {"OrderListHash", "58557574cf0489dd38fb37768f509ca1"}
        };
    }
}
