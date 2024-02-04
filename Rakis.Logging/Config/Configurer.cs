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

using System.IO;
using System;
using System.Collections.Generic;
using static System.Environment;
using Rakis.Logging.Sinks;

namespace Rakis.Logging.Config
{
    public partial class Configurer
    {

        public const string RootLoggerName = "rootLogger";

        /**
         * <summary>The configuration file to read. If left <code>null</code>, only a default configuration will be used.</summary>
         */
        public string ConfigPath { get; private set; }

        /**
         * <summary>The base path where any log files should be stored.</summary>
         */
        public string TargetBasePath { get; private set; }

        /**
         * <summary>If using ProgramData or AppData, the files require an owner and app name.</summary>
         */
        public string TargetOwner { get; private set; }

        /**
         * <summary>If using ProgramData or AppData, the files require an owner and app name.</summary>
         */
        public string TargetName { get; set; }

        private bool targetNamesRequired = false;

        private List<LogConfig> configs = new();
        private bool haveRootConfig = false;
        private LogConfig rootConfig;

        /**
         * <summary>Create a new <see cref="Configurer"/> which will save log files under the current working directory.</summary>
         */
        public Configurer()
        {
            ConfigPath = null;
            TargetBasePath = ".";
        }

        /**
         * <summary>Create a new <see cref="Configurer"/> which will save log files under the current working directory and is set to load
         * configuration from <paramref name="path"/>.</summary>
         */
        public Configurer(string path)
        {
            ConfigPath = path;
            TargetBasePath = ".";
        }

        /**
         * <summary>Add <paramref name="config"/> to the list of configurations.</summary>
         */
        public Configurer AddConfig(LogConfig config)
        {
            if (config.Name == RootLoggerName)
            {
                rootConfig = config;
                haveRootConfig = true;
            }
            else
            {
                configs.Add(config);
            }
            return this;
        }

        /**
         * <summary>Load the configuration from the given path.</summary>
         */
        public Configurer FromFile(string path)
        {
            ConfigPath = path;
            return this;
        }

        // FileLogger settings we may want to define once.

        /**
         * <summary>Store any logfiles in the "ProgramData" directory. This implies an Owner and AppName must be set.</summary>
         */
        public Configurer UsingProgramData()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + "\\ProgramData");
            targetNamesRequired = true;
            return this;
        }

        /**
         * <summary>Store any logfiles in the user's "AppData\Local" directory. This implies an Owner and AppName must be set.</summary>
         */
        public Configurer UsingAppDataLocal()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Local");
            targetNamesRequired = true;
            return this;
        }

        /**
         * <summary>Store any logfiles in the user's "AppData\LocalLow" directory. This implies an Owner and AppName must be set.</summary>
         */
        public Configurer UsingAppDataLocalLow()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "LocalLow");
            targetNamesRequired = true;
            return this;
        }

        /**
         * <summary>Store any logfiles in the user's "AppData\Roaming" directory. This implies an Owner and AppName must be set.</summary>
         */
        public Configurer UsingAppDataRoaming()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Roaming");
            targetNamesRequired = true;
            return this;
        }

        /**
         * <summary>Store any logfiles in the user's home-directory.</summary>
         */
        public Configurer UsingHome()
        {
            TargetBasePath = GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH");
            return this;
        }

        /**
         * <summary>Store any logfiles in the user's "Documents" folder.</summary>
         */
        public Configurer UsingDocuments()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "Documents");
            return this;
        }

        /**
         * <summary>Add <paramref name="owner"/> as an "owner"-level subdirectory name.</summary>
         */
        public Configurer UsingOwner(string owner)
        {
            TargetOwner = owner;
            return this;
        }

        /**
         * <summary>Add <paramref name="name"/> as an "app-name" subdirectory name.</summary>
         */
        public Configurer UsingAppName(string name)
        {
            TargetName = name;
            return this;
        }

        /**
         * <summary>Start a <see cref="ConsoleLoggerConfigurer"/> for the root-logger.</summary>
         */
        public ConsoleLoggerConfigurer WithRootConsoleLogger(LogLevel level = LogLevel.INFO)
        {
            return new ConsoleLoggerConfigurer(this).withName(RootLoggerName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="ConsoleLoggerConfigurer"/>.</summary>
         */
        public ConsoleLoggerConfigurer WithConsoleLogger(string fullName = null, LogLevel level = LogLevel.INFO)
        {
            return new ConsoleLoggerConfigurer(this).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="ConsoleLoggerConfigurer"/>.</summary>
         */
        public ConsoleLoggerConfigurer WithConsoleLogger(string name, string fullName, LogLevel level = LogLevel.INFO)
        {
            return new ConsoleLoggerConfigurer(this).withName(name).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Copy this central <see cref="Configurer"/>'s logfile settings to the new <paramref name="configurer"/>.</summary>
         */
        private FileLoggerConfigurer AddCommonSettings(FileLoggerConfigurer configurer)
        {
            return configurer
                .UsingOwner(TargetOwner)
                .UsingAppName(TargetName)
                .UsingTargetBasePath(TargetBasePath, targetNamesRequired);
        }

        /**
         * <summary>Start a <see cref="FileLoggerConfigurer"/> for the root-logger.</summary>
         */
        public FileLoggerConfigurer WithRootFileLogger(string path = null, LogLevel level = LogLevel.INFO)
        {
            return AddCommonSettings(new FileLoggerConfigurer(this))
                .withName(RootLoggerName)
                .UsingPath(path)
                .withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="FileLoggerConfigurer"/>.</summary>
         */
        public FileLoggerConfigurer WithFileLogger(string fullName = null, LogLevel level = LogLevel.INFO)
        {
            return AddCommonSettings(new FileLoggerConfigurer(this)).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="FileLoggerConfigurer"/>.</summary>
         */
        public FileLoggerConfigurer WithFileLogger(string name, string fullName, LogLevel level = LogLevel.INFO)
        {
            return AddCommonSettings(new FileLoggerConfigurer(this)).withName(name).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="ReactiveLoggerConfigurer"/> as the root-logger.</summary>
         */
        public ReactiveLoggerConfigurer WithReactiveRootLogger(LogLevel level = LogLevel.INFO)
        {
            return new ReactiveLoggerConfigurer(this).withFullName(RootLoggerName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="ReactiveLoggerConfigurer"/>.</summary>
         */
        public ReactiveLoggerConfigurer WithReactiveLogger(string fullName = null, LogLevel level = LogLevel.INFO)
        {
            return new ReactiveLoggerConfigurer(this).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="ReactiveLoggerConfigurer"/>.</summary>
         */
        public ReactiveLoggerConfigurer WithReactiveLogger(string name, string fullName, LogLevel level = LogLevel.INFO)
        {
            return new ReactiveLoggerConfigurer(this).withName(name).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="CustomLoggerConfigurer"/> as the root-logger.</summary>
         */
        public CustomLoggerConfigurer WithCustomRootLogger(LogLevel level = LogLevel.INFO)
        {
            return new CustomLoggerConfigurer(this).withFullName(RootLoggerName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="CustomLoggerConfigurer"/>.</summary>
         */
        public CustomLoggerConfigurer WithCustomLogger(string fullName = null, LogLevel level = LogLevel.INFO)
        {
            return new CustomLoggerConfigurer(this).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Start a <see cref="CustomLoggerConfigurer"/>.</summary>
         */
        public CustomLoggerConfigurer WithCustomLogger(ILoggingSink sink, string name, string fullName, LogLevel level = LogLevel.INFO)
        {
            return new CustomLoggerConfigurer(this).UsingSink(sink).withName(name).withFullName(fullName).withThreshold(level);
        }

        /**
         * <summary>Check if the given <paramref name="line"/> has any contents, ignoring comment lines. If so, split the line at the first equals-sign,
         * and return the individual parts as, respectively, <paramref name="path"/> and <paramref name="value"/>.</summary>
         */
        private bool ParseLine(string line, out string path, out string value)
        {
            path = "";
            value = "";
            line = line.Trim();
            if (line.StartsWith("#") || line.StartsWith(";"))
            {
                return false;
            }
            var index = line.IndexOf('=');
            path = (index == -1) ? line : line.Substring(0, index).Trim();
            value = (index == -1) ? "" : line.Substring(index+1).Trim();

            return path != "";
        }

        /**
         * <summary>Load the configuration file specified.</summary>
         */
        public Configurer Load(string path = null)
        {
            if (path != null)
            {
                ConfigPath = path;
            }
            if (ConfigPath == null)
            {
                throw new LogConfigurationException("No configuration file provided to load.");
            }
            if (!File.Exists(ConfigPath))
            {
                throw new LogConfigurationException($"Configuration file '{ConfigPath}' not found.");
            }

            using StreamReader f = new(ConfigPath);
            string line;
            while ((line = f.ReadLine()) != null)
            {
                if (ParseLine(line, out string fullName, out string value))
                {
                    var name = fullName;
                    var lastDot = fullName.LastIndexOf(".");
                    if (lastDot != -1)
                    {
                        name = fullName.Substring(lastDot + 1);
                    }
                    LogConfig config = new(name, fullName);
                    string type = null;

                    foreach (string token in value.Split(","))
                    {
                        var trimmedToken = token.Trim();
                        // First alternative: LoggingLevel
                        if (Enum.TryParse(trimmedToken, out LogLevel level))
                        {
                            config.Level = level;
                            continue;
                        }

                        // Second alternative: Logger Type
                        if (string.Equals(trimmedToken, ConsoleLogger.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            type = ConsoleLogger.Type;
                            continue;
                        }
                        if (string.Equals(trimmedToken, FileLogger.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            type = FileLogger.Type;
                            continue;
                        }

                        // Last alternative: Logger options
                        if (type != FileLogger.Type)
                        {
                            throw new LogConfigurationException($"Unknown option '{trimmedToken}' for logger type '{type}'");
                        }
                        int equalsIndex = trimmedToken.IndexOf("=");
                        if (equalsIndex == -1)
                        {
                            config.Options.Add(trimmedToken, "true");
                        }
                        else
                        {
                            config.Options.Add(trimmedToken.Substring(0, equalsIndex), trimmedToken.Substring(equalsIndex + 1));
                        }
                    }
                    config.Type = type;
                    if (config.Level == null)
                    {
                        throw new LogConfigurationException("Every line must specify a level.");
                    }
                    if (type == ConsoleLogger.Type) {
                        config.Sink = new ConsoleLogger();
                    }
                    if (fullName == RootLoggerName)
                    {
                        rootConfig = config;
                        haveRootConfig = true;
                    }
                    else
                    {
                        configs.Add(config);
                    }
                }
            }
            return this;
        }

        /**
         * <summary>Process all settings and configure the loggers.</summary>
         */
        public void Build()
        {
            if (haveRootConfig)
            {
                Logger.RootLogger = BuildLogger(rootConfig);
            }
            configs.Sort((left, right) => left.FullName.CompareTo(right.FullName));
            foreach (var config in configs)
            {
                Logger.AddLogger(BuildLogger(config));
            }
            Logger.RootLogger.Info?.Log($"Logger initialized with root level '{Logger.RootLogger.Threshold}'");
        }

        /**
         * <summary>Build a <see cref="Logger"/> from the given <paramref name="config"/>.</summary>
         */
        private ILogger BuildLogger(LogConfig config)
        {
            // Find parent logger. If null then this is the RootLogger
            ILogger parent = (config.Name == RootLoggerName) ? null : Logger.FindLogger(config.FullName);

            // Fix Type and Sink
            if (config.Type == null)
            {
                if (config.Sink != null)
                {
                    config.Type = config.Sink.LoggingType;
                }
                else if (parent == null)
                {
                    config.Sink = new ConsoleLogger();
                    config.Type = ConsoleLogger.Type;
                }
                else
                {
                    config.Sink = parent.Sink;
                    config.Type = parent.Sink.LoggingType;
                }
            }
            else if (config.Sink == null)
            {
                if (config.Type == ConsoleLogger.Type)
                {
                    config.Sink = ConsoleLogger.FromConfig(config);
                }
                else if (config.Type == FileLogger.Type)
                {
                    config.Sink = FileLogger.FromConfig(config);
                }
                else
                {
                    throw new LogConfigurationException($"No sink provided for logger '{config.FullName}'");
                }
            }
            return new Logger(config.Name, config.Sink, config.FullName, (LogLevel)config.Level);
        }
    }
}
