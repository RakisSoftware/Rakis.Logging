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

using Rakis.Logging.Sinks;
using static System.Environment;
using System.IO;

namespace Rakis.Logging.Config
{
    public class FileLoggerConfigurer : LoggerConfigurerBase<FileLoggerConfigurer>
    {
        public string LogPath { get; set; }
        public LogStrategy Strategy { get; set; }

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

        public FileLoggerConfigurer(Configurer configurer) : base(configurer)
        {
            Strategy = LogStrategy.None;
            TargetBasePath = ".";
        }

        public FileLoggerConfigurer UsingTargetBasePath(string path)
        {
            TargetBasePath = path;
            return this;
        }

        public FileLoggerConfigurer UsingTargetBasePath(string path, bool targetNamesRequired)
        {
            TargetBasePath = path;
            this.targetNamesRequired = targetNamesRequired;
            return this;
        }

        /**
         * <summary>Store any logfiles in the "ProgramData" directory. This implies an Owner and AppName must be set.</summary>
         */
        public FileLoggerConfigurer UsingProgramData()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + "\\ProgramData");
            targetNamesRequired = true;
            return this;
        }

        public FileLoggerConfigurer UsingAppDataLocal()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Local");
            targetNamesRequired = true;
            return this;
        }

        public FileLoggerConfigurer UsingAppDataLocalLow()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "LocalLow");
            targetNamesRequired = true;
            return this;
        }

        public FileLoggerConfigurer UsingAppDataRoaming()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "AppData", "Roaming");
            targetNamesRequired = true;
            return this;
        }

        public FileLoggerConfigurer UsingHome()
        {
            TargetBasePath = GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH");
            return this;
        }

        public FileLoggerConfigurer UsingDocuments()
        {
            TargetBasePath = Path.Combine(GetEnvironmentVariable("HOMEDRIVE") + GetEnvironmentVariable("HOMEPATH"), "Documents");
            return this;
        }

        public FileLoggerConfigurer UsingOwner(string owner)
        {
            TargetOwner = owner;
            return this;
        }

        public FileLoggerConfigurer UsingAppName(string name)
        {
            TargetName = name;
            return this;
        }

        public FileLoggerConfigurer UsingPath(string path)
        {
            LogPath = path;
            return this;
        }

        private string FilePath(string path)
        {
            if (targetNamesRequired)
            {
                if (TargetOwner == null)
                {
                    TargetOwner = "RakisUtil";
                }
                if (TargetName == null)
                {
                    TargetName = "Logging";
                }
            }
            string result = TargetBasePath;
            if (TargetOwner != null)
            {
                result = Path.Combine(result, TargetOwner);
            }
            if (TargetName != null)
            {
                result = Path.Combine(result, TargetName);
            }
            return Path.Combine(result, path);
        }

        public override LogConfig BuildConfig()
        {
            LogConfig config = new(Name, FullName, Threshold);

            config.Options.Add(LogConfig.OptionPath, FilePath(LogPath));
            config.Options.Add(LogConfig.OptionStrategy, Strategy);

            config.Type = FileLogger.Type;
            config.Sink = FileLogger.FromConfig(config);

            return config;
        }
    }
}
