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

using Rakis.Logging.Config;
using Rakis.Logging.Sinks;
using System;
using static System.Environment;
using System.IO;
using System.Collections.Generic;
using Xunit;

namespace Rakis.Logging.UnitTests
{

    public class LogConfigurationTest
    {

        /**
         * <summary>Verify the leveled loggers work as intended.</summary>
         */
        private static void TestLeveledLoggers(ILogger logger)
        {
            if (logger.IsTraceEnabled)
            {
                Assert.NotNull(logger.Trace);
            }
            else
            {
                Assert.Null(logger.Trace);
            }
            if (logger.IsDebugEnabled)
            {
                Assert.NotNull(logger.Debug);
            }
            else
            {
                Assert.Null(logger.Debug);
            }
            if (logger.IsInfoEnabled)
            {
                Assert.NotNull(logger.Info);
            }
            else
            {
                Assert.Null(logger.Info);
            }
            if (logger.IsWarnEnabled)
            {
                Assert.NotNull(logger.Warn);
            }
            else
            {
                Assert.Null(logger.Warn);
            }
            if (logger.IsErrorEnabled)
            {
                Assert.NotNull(logger.Error);
            }
            else
            {
                Assert.Null(logger.Error);
            }
            if (logger.IsFatalEnabled)
            {
                Assert.NotNull(logger.Fatal);
            }
            else
            {
                Assert.Null(logger.Fatal);
            }
        }

        [Fact]
        public void TestDefaultConfiguration()
        {
            Logger.ClearLoggers();

            var logger = Logger.GetLogger(typeof(LogConfigurationTest));
            Assert.NotNull(logger);

            Assert.Equal(LogLevel.INFO, Logger.GetLogger(logger.GetType()).Threshold);

            TestLeveledLoggers(logger);

            logger.Info.Log("This should go to the Console.");
        }

        [Fact]
        public void TestExplicitDefaultConfiguration()
        {
            Logger.DefaultConfiguration().Build();
            var logger = Logger.GetLogger(typeof(LogConfigurationTest));
            Assert.NotNull(logger);

            Assert.Equal(LogLevel.INFO, Logger.GetLogger(logger.GetType()).Threshold);

            TestLeveledLoggers(logger);

            logger.Info.Log("This should go to the Console.");
        }

        [Fact]
        public void TestLogConfigurator()
        {
            Logger.ClearLoggers();

            Logger.DefaultConfiguration()
                .WithRootConsoleLogger(LogLevel.WARN).AddToConfig()
                .WithConsoleLogger("Rakis", LogLevel.INFO).AddToConfig()
                .WithConsoleLogger("Rakis.Logging", LogLevel.DEBUG).AddToConfig()
                .WithConsoleLogger("Rakis.Logging.Sinks", LogLevel.TRACE).AddToConfig()
                .WithConsoleLogger("Rakis.Logging.UnitTests", LogLevel.ERROR).AddToConfig()
                .Build();

            Assert.Equal(LogLevel.WARN, Logger.GetLogger(typeof(int)).Threshold);
            Assert.Equal(LogLevel.DEBUG, Logger.GetLogger(typeof(Logger)).Threshold);
            Assert.Equal(LogLevel.TRACE, Logger.GetLogger(typeof(ConsoleLogger)).Threshold);
            Assert.Equal(LogLevel.ERROR, Logger.GetLogger(typeof(LogConfigurationTest)).Threshold);
            Assert.Equal(LogLevel.DEBUG, Logger.GetLogger(typeof(Configurer)).Threshold);
        }

        public static uint CountLines(string path)
        {
            using StreamReader f = new(path);
            string? line;
            uint count = 0;
            while ((line = f.ReadLine()) != null)
            {
                count++;
            }
            return count;
        }

        [Fact]
        public void TestFileLogger()
        {
            Logger.ClearLoggers();

            string testLog = Path.GetTempFileName();
            Logger.DefaultConfiguration()
                .WithRootFileLogger(testLog).AddToConfig()
                .Build();
            using (var logger = Logger.GetLogger("test"))
            {
                logger.Info.Log("Hi there!");
            }
            Assert.True(CountLines(testLog) == 2);
        }

        [Fact]
        public void TestMixedLoggers()
        {
            Logger.ClearLoggers();

            string testLog1 = Guid.NewGuid().ToString().Replace("-", "").ToLower() + ".log";
            string testLog2 = Path.GetTempFileName();
            Logger.DefaultConfiguration()
                .WithRootConsoleLogger().AddToConfig()
                .WithFileLogger("Rakis.Logging")
                    .UsingAppDataRoaming()
                    .UsingOwner("Rakis").UsingAppName("Logging")
                    .UsingPath(testLog1)
                    .AddToConfig()
                .WithFileLogger("Rakis.Logging.UnitTests")
                    .UsingPath(testLog2)
                    .AddToConfig()
                .Build();
 
            using (var logger = Logger.GetLogger(typeof(Logger)))
            {
                logger.Info.Log("Hi there!");
            }

            string logFile = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Roaming", "Rakis", "Logging", testLog1);
            Assert.True(CountLines(logFile) == 1);

            using (var logger = Logger.GetLogger(typeof(LogConfigurationTest)))
            {
                logger.Info.Log("Hi there!");
                logger.Info.Log("Hi there again!");
            }
            Assert.True(CountLines(testLog2) == 2);
        }

        [Fact]
        public void TestReactiveLoggers()
        {
            Logger.ClearLoggers();
            List<String> output = new();

            Logger.DefaultConfiguration()
                .WithRootConsoleLogger().AddToConfig()
                .WithReactiveLogger("Rakis.Logging")
                    .OnNext(e => output.Add(e.ToString()))
                    .AddToConfig()
                .Build();
            Logger.GetLogger(typeof(Logger)).Info.Log("Hi there!");
            Assert.Single(output);
        }
    }
}