/*
 * Copyright (c) 2021, 2022. Bert Laverman
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
using System.Collections.Generic;
using Rakis.Logging.Config;
using Rakis.Logging.Sinks;

namespace Rakis.Logging
{

    public class Logger : LoggerBase, ILogger, IDisposable
    {

        public Logger(string name, ILoggingSink sink, string fullName =null, LogLevel level =LogLevel.INFO) : base(sink, name, fullName, level)
        {

        }

        public Logger(ILogger parent, string fullName) : base(parent.Sink, fullName, threshold: parent.Threshold)
        {

        }

        public override void Flush()
        {
            Sink.Flush();
        }

        public override void Dispose()
        {
            Sink.Dispose();
        }

        public static ILogger RootLogger { get; set; } = new Logger("DefaultConsoleLogger", new ConsoleLogger(), level: LogLevel.INFO);

        private const string CONFIGPATH_DEFAULT = "rakisLog.properties";

        private static Dictionary<string, ILogger> loggers = new();

        public static void ClearLoggers()
        {
            loggers.Clear();
            RootLogger = new Logger("DefaultConsoleLogger", new ConsoleLogger(), level: LogLevel.INFO);
        }

        /**
         * <summary>Return a new <see cref="Configurer"/> to modify the current configuration.</summary>
         */
        public static Configurer Configuration()
        {
            return new Configurer();
        }

        /**
         * <summary>Return a new <see cref="Configurer"/> set to load a configuration file. If not path is specified, <see cref="CONFIGPATH_DEFAULT"/> is used.</summary>
         */
        public static Configurer ConfigurationFromFile(string configPath = CONFIGPATH_DEFAULT)
        {
            return new Configurer(configPath);
        }

        /**
         * <summary>Return a new <see cref="Configurer"/>, after first wiping the current settings.</summary>
         */
        public static Configurer DefaultConfiguration()
        {
            ClearLoggers();

            return new Configurer();
        }

        /**
         * <summary>Add an <see cref="ILogger"/> conforming logger object.</summary>
         */
        public static void AddLogger(ILogger logger)
        {
            loggers.Add(logger.FullName, logger);
        }

        /**
         * <summary>Return the best matching <see cref="ILogger"/> given <paramref name="key"/>.</summary>
         */
        public static ILogger FindLogger(string key)
        {
            if ((key == null) || key.Length == 0)
            {
                return RootLogger;
            }
            if (loggers.TryGetValue(key, out ILogger result))
            {
                return result;
            }
            int index = key.LastIndexOf('.');
            return (index <= 0) ? RootLogger : FindLogger(key.Substring(0, index));
        }

        /**
         * <summary>Try to find the <see cref="ILogger"/> matching <paramref name="name"/>, or add (and return) a new one using the best match as parent.</summary>
         */
        public static ILogger GetLogger(string name)
        {
            var result = FindLogger(name);
            if (result.FullName != name)
            {
                result = new Logger(result, name);
                AddLogger(result);
            }
            return result;
        }

        /**
         * <summary>Call <see cref="GetLogger(string)"/> using the fully qualified type name of <paramref name="type"/>.</summary>
         */
        public static ILogger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

    }

}
