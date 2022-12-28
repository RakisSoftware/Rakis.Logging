/*
 * Copyright (c) 2022. Bert Laverman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Rakis.Logging
{
    public interface ILoggingSink : IDisposable
    {

        public string LoggingType { get; }

        /**
         * <summary>Actually write out the message.</summary>
         */
        public void Log(LogEntry entry);

        /**
         * <summary>Flush any data to whatever we are logging to, if appropriate.</summary>
         */
        public void Flush();
    }
}
