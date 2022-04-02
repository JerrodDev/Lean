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

using QuantConnect.Optimizer.Objectives;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace QuantConnect.Optimizer.Strategies
{
    /// <summary>
    /// Find the best solution in first generation
    /// </summary>
    public class WalkforwardOptimizationStrategy : StepBaseOptimizationStrategy
    {
        private object _locker = new object();

        /// <summary>
        /// Checks whether new lean compute job better than previous and run new iteration if necessary.
        /// </summary>
        /// <param name="result">Lean compute job result and corresponding parameter set</param>
        public override void PushNewResults(OptimizationResult result)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException($"WalkforwardOptimizationStrategy.PushNewResults: strategy has not been initialized yet.");
            }

            lock (_locker)
            {
                if (!ReferenceEquals(result, OptimizationResult.Initial) && string.IsNullOrEmpty(result?.JsonBacktestResult))
                {
                    // one of the requested backtests failed
                    return;
                }

                // check if the incoming result is not the initial seed
                if (result.Id > 0)
                {
                    ProcessNewResult(result);
                    return;
                }

                
                foreach (var iteration in Split((WalkforwardOptimizationStrategySettings)Settings))
                {
                    foreach (var parameterSet in Step(OptimizationParameters))
                    {
                        parameterSet.Value.Add("start-date", iteration.StartDate.ToString(CultureInfo.InvariantCulture));
                        parameterSet.Value.Add("end-date", iteration.EndDate.ToString(CultureInfo.InvariantCulture));
                        switch (iteration.Type)
                        {
                            case IterationType.InSample:
                                continue;
                            case IterationType.OutOfSample:
                                continue;
                            default: throw new InvalidOperationException("Invalid iteration type");
                        }
                        OnNewParameterSet(parameterSet);
                    }
                }
            }
        }

        private static IEnumerable<Iteration> Split(WalkforwardOptimizationStrategySettings settings)
        {
            if(EnforceSampleSplitRules(settings.PercentageInSample, settings.PercentageOutOfSample))
            {
                var iterationList = new List<Iteration>();
                var iterationLength = settings.EndDate.Subtract(settings.StartDate).Divide(settings.Iterations);
                for (double iterator = 0.0; iterator < settings.Iterations; iterator += 1.0)
                {
                    var startDate = settings.StartDate.Add(iterationLength.Multiply(iterator));
                    var endDate = settings.EndDate.Add(iterationLength.Multiply(iterator + 1));
                    var splitPoint = startDate.Add(endDate.Subtract(startDate).Multiply(decimal.ToDouble(settings.PercentageInSample / 100m)));
                    iterationList.Add(new Iteration(IterationType.InSample, startDate, splitPoint));
                    iterationList.Add(new Iteration(IterationType.OutOfSample, splitPoint, endDate));
                }
                return iterationList;
            }
            else
            {
                throw new ArgumentException("Sum of sample split percentages must equal 100.0");
            }
        }

        /// <summary>
        /// Ensure the sum of sample splits equal 100.0
        /// </summary>
        /// <param name="inSample">In-sample percentage for the optimization iteration</param>
        /// <param name="outOfSample">Out-of-sample percentage for the optimization iteration</param>
        /// <param name="validation">Validation percentage for the optimization iteration</param>
        /// <returns>Boolean result of split rule enforcement</returns>
        private static bool EnforceSampleSplitRules(decimal inSample, decimal outOfSample)
        {
            return inSample + outOfSample == 100.0m;
        }
    }
}
