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

using System.Collections;

namespace Rakis.Logging
{
    /**
     * <summary>The common functionality for all loggers</summary>
     */
    public abstract class LoggerBase : ILogger
    {
        public ILoggingSink Sink { get; init; }
        public string LoggerType { get; init; }

        public string Name { get; init; }
        public string FullName { get; init; }
        public LogLevel Threshold { get; init; }

        public bool IsEnabled(LogLevel level) => level >= Threshold;

        public bool IsTraceEnabled => IsEnabled(LogLevel.TRACE);
        public ILeveledLogger Trace { get; init; }

        public bool IsDebugEnabled => IsEnabled(LogLevel.DEBUG);
        public ILeveledLogger Debug { get; init; }

        public bool IsInfoEnabled => IsEnabled(LogLevel.INFO);
        public ILeveledLogger Info { get; init; }

        public bool IsWarnEnabled => IsEnabled(LogLevel.WARN);
        public ILeveledLogger Warn { get; init; }

        public bool IsErrorEnabled => IsEnabled(LogLevel.ERROR);
        public ILeveledLogger Error { get; init; }

        public bool IsFatalEnabled => IsEnabled(LogLevel.FATAL);
        public ILeveledLogger Fatal { get; init; }

        public LoggerBase(ILoggingSink sink, string name, string fullName = null, LogLevel threshold= LogLevel.INFO)
        {
            Sink = sink;
            LoggerType = sink.LoggingType;
            if (fullName == null)
            {
                FullName = name;
                int lastDot = name.LastIndexOf('.');
                Name = (lastDot == -1) ? name : name.Substring(lastDot + 1);
            }
            else
            {
                Name = name;
                FullName = fullName;
            }
            Threshold = threshold;

            Trace = IsTraceEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.TRACE) : null;
            Debug = IsDebugEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.DEBUG) : null;
            Info = IsInfoEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.INFO) : null;
            Warn = IsWarnEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.WARN) : null;
            Error = IsErrorEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.ERROR) : null;
            Fatal = IsFatalEnabled ? new DefaultLeveledLogger(this, sink, LogLevel.FATAL) : null;
        }

        public ILeveledLogger GetLogger(LogLevel level) => level switch
        {
            LogLevel.TRACE => Trace,
            LogLevel.DEBUG => Debug,
            LogLevel.INFO => Info,
            LogLevel.WARN => Warn,
            LogLevel.ERROR => Error,
            LogLevel.FATAL => Fatal,
            _ => throw new System.NotImplementedException()
        };

        public void Log(LogLevel level, LogEntry entry)
        {
            GetLogger(level)?.Log(entry);
        }

        public void Log(LogLevel level, string message)
        {
            GetLogger(level)?.Log(message);
        }

        public void Log(LogLevel level, string fmt, params object[] args)
        {
            GetLogger(level)?.Log(fmt, args);
        }

        public abstract void Flush();

        public abstract void Dispose();
    }
}
