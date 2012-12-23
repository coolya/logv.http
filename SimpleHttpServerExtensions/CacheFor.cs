/*
     Copyright 2012 Kolja Dummann <k.dummann@gmail.com>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
namespace logv.http
{
    /// <summary>
    /// Constants for caching times
    /// </summary>
    public enum CacheFor
    {
        /// <summary>
        /// A day caching time
        /// </summary>
        ADay = 86400,
        /// <summary>
        /// An hour
        /// </summary>
        AnHour = 3600,
        /// <summary>
        /// two hours
        /// </summary>
        TwoHours = 7200,
        /// <summary>
        /// A minute
        /// </summary>
        AMinute = 60,
        /// <summary>
        /// half an hour
        /// </summary>
        HalfAnHour = 1800,
        /// <summary>
        /// A week
        /// </summary>
        AWeek = 604800,
        /// <summary>
        /// twelve hours
        /// </summary>
        TwelveHours = 43200
    }
}
