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

using Rakis.Logging.Config;
using System;
using System.IO;

namespace Rakis.Logging.Sinks
{
    public enum LogStrategy
    {
        None = 0,
        Fast,
        Flushing,
        Safe
    }

    public class FileLogger : ILoggingSink
    {

        public const string Type = "file";
        public string LoggingType => Type;

        public string LogPath { get; init; }
        public LogStrategy Strategy { get; set; }
        private FileStream file = null;
        private StreamWriter writer = null;

        public FileLogger(string logPath, LogStrategy logStrategy = LogStrategy.Fast)
        {
            LogPath = logPath;
            Strategy = logStrategy;

            string dirPath = Path.GetDirectoryName(LogPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (Strategy != LogStrategy.Safe)
            {
                Open();
            }
        }

        public static FileLogger FromConfig(LogConfig config)
        {
            if (!config.Options.ContainsKey(LogConfig.OptionPath))
            {
                throw new LogConfigurationException($"Missing path for file logger '{config.FullName}'");
            }

            var result = new FileLogger((string)config.Options[LogConfig.OptionPath]);
            if (config.Options.TryGetValue(LogConfig.OptionStrategy, out object strategy) && ((LogStrategy)strategy != LogStrategy.None))
            {
                result.Strategy = (LogStrategy)strategy;
            }
            else
            {
                result.Strategy = LogStrategy.Safe;
            }
            return result;
        }

        private void Open()
        {
            file = new(LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(file);
        }

        public void Flush()
        {
            if (Strategy != LogStrategy.Fast)
            {
                writer?.Flush();
                file?.Flush();
            }
        }

        private void Close()
        {
            writer?.Dispose();
            writer = null;
            file?.Dispose();
            file = null;
        }

        public void Log(LogEntry entry)
        {
            if (Strategy == LogStrategy.Safe)
            {
                lock (this)
                {
                    using FileStream fs = new(LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    using StreamWriter f = new(fs);
                    f.WriteLine(entry.ToString());
                }
            }
            else
            {
                writer.WriteLine(entry.ToString());
                Flush();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
