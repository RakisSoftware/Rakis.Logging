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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace Rakis.Logging.UnitTests
{
    [TestClass]
    public class LogConfigurationTest
    {
        /**
         * <summary>Verify that the <paramref name="expectedThreshold"/> is correctly set in <paramref name="logger"/>.</summary>
         */
        private static void TestThreshold(ILogger logger, LogLevel expectedThreshold)
        {
            Assert.AreEqual(logger.Threshold, expectedThreshold, $"Logger threshold for '{logger.FullName}' should be {expectedThreshold} but was {logger.Threshold}.");

            foreach (var level in (LogLevel[])Enum.GetValues(typeof(LogLevel)))
            {
                Assert.AreEqual(logger.IsEnabled(level), level >= logger.Threshold, $"With threshold {expectedThreshold}, IsEnabled({level}) should return {level >= logger.Threshold}.");
            }
        }

        /**
         * <summary>Verify the leveled loggers work as intended.</summary>
         */
        private static void TestLeveledLoggers(ILogger logger)
        {
            if (logger.IsTraceEnabled)
            {
                Assert.IsNotNull(logger.Trace, "Trace logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Trace, "Trace logging should be disabled.");
            }
            if (logger.IsDebugEnabled)
            {
                Assert.IsNotNull(logger.Debug, "Debug logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Debug, "Debug logging should be disabled.");
            }
            if (logger.IsInfoEnabled)
            {
                Assert.IsNotNull(logger.Info, "Info logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Info, "Info logging should be disabled.");
            }
            if (logger.IsWarnEnabled)
            {
                Assert.IsNotNull(logger.Warn, "Warn logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Warn, "Warn logging should be disabled.");
            }
            if (logger.IsErrorEnabled)
            {
                Assert.IsNotNull(logger.Error, "Error logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Error, "Error logging should be disabled.");
            }
            if (logger.IsFatalEnabled)
            {
                Assert.IsNotNull(logger.Fatal, "Fatal logging should be enabled.");
            }
            else
            {
                Assert.IsNull(logger.Fatal, "Fatal logging should be disabled.");
            }
        }

        /**
         * <summary>Retrieve a logger and verify if the threshold is set. Then produce a line at all levels.</summary>
         */
        private static void TestConfiguredLevels(Type type, LogLevel threshold)
        {
            using ILogger logger = Logger.GetLogger(type);

            TestThreshold(logger, threshold);

            foreach (var level in (LogLevel[])Enum.GetValues(typeof(LogLevel)))
            {
                logger.GetLogger(level)?.Log($"A logmessage at level {level}.");
            }
        }

        [TestMethod]
        public void TestDefaultConfiguration()
        {
            Logger.ClearLoggers();

            var logger = Logger.GetLogger(typeof(LogConfigurationTest));
            Assert.IsNotNull(logger, "I must have a logger without doing configuration.");

            TestThreshold(logger, LogLevel.INFO);
            TestLeveledLoggers(logger);

            logger.Info.Log("This should go to the Console.");
        }

        [TestMethod]
        public void TestExplicitDefaultConfiguration()
        {
            Logger.DefaultConfiguration().Build();
            var logger = Logger.GetLogger(typeof(LogConfigurationTest));
            Assert.IsNotNull(logger, "I must have a logger DefaultConfiguration().Build.");

            TestThreshold(logger, LogLevel.INFO);
            TestLeveledLoggers(logger);

            logger.Info.Log("This should go to the Console.");
        }

        [TestMethod]
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

            TestConfiguredLevels(typeof(int), LogLevel.WARN);
            TestConfiguredLevels(typeof(Logger), LogLevel.DEBUG);
            TestConfiguredLevels(typeof(ConsoleLogger), LogLevel.TRACE);
            TestConfiguredLevels(typeof(LogConfigurationTest), LogLevel.ERROR);
            TestConfiguredLevels(typeof(Configurer), LogLevel.DEBUG);
        }

        public void CheckLogFile(string path, int expectedNrOfLines =-1)
        {
            using StreamReader f = new(path);
            string? line;
            int count = 0;
            while ((line = f.ReadLine()) != null)
            {
                count++;
            }
            if (expectedNrOfLines >= 0)
            {
                Assert.AreEqual(expectedNrOfLines, count, $"Expected {expectedNrOfLines} lines in logfile '{path}' but found {count}.");
            }
        }

        [TestMethod]
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
            CheckLogFile(testLog, 2);
        }

        [TestMethod]
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
            CheckLogFile(Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Roaming", "Rakis", "Logging", testLog1), 1);

            using (var logger = Logger.GetLogger(typeof(LogConfigurationTest)))
            {
                logger.Info.Log("Hi there!");
                logger.Info.Log("Hi there again!");
            }
            CheckLogFile(testLog2, 2);
        }

        [TestMethod]
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
            Assert.AreEqual(output.Count, 1, "The reactive logger should have deposited log lines.");
        }
    }
}