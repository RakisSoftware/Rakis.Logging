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
    public enum LogLevel
    {
        TRACE,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }


    /**
     * <summary>An ILogger allows you to log messages. It will ignore attempts to log below the set threshold.</summary>
     */
    public interface ILogger : IDisposable
    {

        /**
         * <summary>The short name of this logger.</summary>
         */
        public string Name { get; init; }

        /**
         * <summary>The full name of this logger, typically a fully qualified classname.</summary>
         */
        public string FullName { get; init; }

        /**
         * <summary>The threshold for this logger. Below this level the returned ILeveledLoggers will be null.</summary>
         */
        public LogLevel Threshold { get; init; }

        /**
         * <summary>The actual ILoggingSink used.</summary>
         */
        public ILoggingSink Sink { get; init; }

        /**
         * <summary>Return if a specific LogLevel is enabled.</summary>
         */
        public bool IsEnabled(LogLevel level);

        /**
         * <summary>Return the logger for a specific <paramref name="level"/>.</summary>
         */
        public ILeveledLogger GetLogger(LogLevel level);

        /**
         * <summary>Trace level is for high-volume messages that normally are ignored but can provide help tracing bugs.</summary>
         */
        public bool IsTraceEnabled { get; }
        public ILeveledLogger Trace { get; init; }

        /**
         * <summary>Debug level is for useful debugging messages that can be ignored under normal circumstances.</summary>
         */
        public bool IsDebugEnabled { get; }
        public ILeveledLogger Debug { get; init; }

        /**
         * <summary>Info level is for informational messages that provide useful insights during normal operation, but may be ignored.</summary>
         */
        public bool IsInfoEnabled { get; }
        public ILeveledLogger Info { get; init; }

        /**
         * <summary>Warn level is for important warnings that do not cause (persistent) problems for the application.</summary>
         */
        public bool IsWarnEnabled { get; }
        public ILeveledLogger Warn { get; init; }

        /**
         * <summary>Error level is for messages about things that went wrong and caused functionality not to work as intended, but are recoverable.</summary>
         */
        public bool IsErrorEnabled { get; }
        public ILeveledLogger Error { get; init; }

        /**
         * <summary>Fatal level is for errors that could not be ignored and are not recoverable. Typically an application will abort after a Fatal level message.</summary>
         */
        public bool IsFatalEnabled { get; }
        public ILeveledLogger Fatal { get; init; }

        /**
         * <summary>Dispatch <paramref name="entry"/> at level <paramref name="level"/>.</summary>
         */
        public void Log(LogLevel level, LogEntry entry);

        /**
         * <summary>Dispatch <paramref name="message"/> at level <paramref name="level"/>.</summary>
         */
        public void Log(LogLevel level, string message);

        /**
         * <summary>Dispatch <paramref name="message"/> at level <paramref name="level"/>, treating <paramref name="message"/> as a format string using <paramref name="args"/>.</summary>
         */
        public void Log(LogLevel level, string message, params object[] args);

        /**
         * <summary>If relevant, flush any data to storage.</summary>
         */
        public void Flush();
    }
}
