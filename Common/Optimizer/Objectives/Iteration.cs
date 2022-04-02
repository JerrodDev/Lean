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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace QuantConnect.Optimizer.Objectives
{
    /// <summary>
    /// Defines the optimization iterations
    /// </summary>
    public class Iteration
    {
        /// <summary>
        /// Returns the initialization status of the object
        /// </summary>
        public IterationType Type { get; set; }

        /// <summary>
        /// Defines the start date for the optimization iteration
        /// </summary>
        [JsonProperty("start-date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Defines the end date for the optimization iteration
        /// </summary>
        [JsonProperty("end-date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Initialize Iteration properties
        /// </summary>
        /// <param name="iterationType">Type of iteration this instance represents</param>
        /// <param name="startDate">Start date of optimization</param>
        /// <param name="endDate">End date of optimization</param>
        public Iteration(IterationType iterationType, DateTime startDate, DateTime endDate)
        {
            Type = iterationType;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    /// <summary>
    /// Type of iteration this instance represents
    /// </summary>
    public enum IterationType
    {
        /// <summary>
        /// Default none-typed iteration
        /// </summary>
        Default,

        /// <summary>
        /// In sample iteration
        /// </summary>
        InSample,

        /// <summary>
        /// Out of sample iteration
        /// </summary>
        OutOfSample
    }
}
