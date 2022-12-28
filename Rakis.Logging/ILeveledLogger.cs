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

namespace Rakis.Logging
{
    /**
     * <summary>A class implementing this interfacve will dispatch messages to the configured <see cref="Sink"/> at the configured <see cref="Level"/>.</summary>
     */
    public interface ILeveledLogger
    {
        public LogLevel Level { get; init; }
        public ILoggingSink Sink { get; init; }

        /**
         * <summary>Send a <see cref="LogEntry"/> to the <see cref="Sink"/>. Note the level in the entry will not be adjusted.</summary>
         */
        public void Log(LogEntry entry);

        /**
         * <summary>Create a <see cref="LogEntry"/> from the given message and dispatch it to the <see cref="Sink"/>.</summary>
         */
        public void Log(string msg);

        /**
         * <summary>Create a <see cref="LogEntry"/> from the given message and dispatch it to the <see cref="Sink"/>.</summary>
         */
        public void Log(string fmt, params object[] args);
    }
}
