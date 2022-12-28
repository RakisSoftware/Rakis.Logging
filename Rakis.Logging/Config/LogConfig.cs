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

using System.Collections.Generic;

namespace Rakis.Logging.Config
{
    public struct LogConfig
    {
        public const string OptionPath = "Path";
        public const string OptionStrategy = "Strategy";
        public const string OptionProgramData = "ProgramData";
        public const string OptionAppDataLocal = "AppDataLocal";
        public const string OptionAppDataLocalLow = "AppDataLocalLow";
        public const string OptionAppDataRoaming = "AppDataRoaming";
        public const string OptionDocuments = "Documents";
        public const string OptionHome = "Home";

        public readonly string Name;
        public readonly string FullName;
        public string Type;
        public ILoggingSink Sink;
        public LogLevel? Level;
        public Dictionary<string, object> Options;

        public LogConfig(string fullName)
        {
            int lastDot = fullName.LastIndexOf('.');

            Name = lastDot == -1 ? fullName : fullName.Substring(lastDot + 1);
            FullName = fullName;
            Type = null;
            Sink = null;
            Level = null;
            Options = new();
        }

        public LogConfig(string fullName, LogLevel level)
        {
            int lastDot = fullName.LastIndexOf('.');

            Name = lastDot == -1 ? fullName : fullName.Substring(lastDot + 1);
            FullName = fullName;
            Type = null;
            Sink = null;
            Level = level;
            Options = new();
        }

        public LogConfig(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
            Type = null;
            Sink = null;
            Level = null;
            Options = new();
        }

        public LogConfig(string name, string fullName, LogLevel level)
        {
            Name = name;
            FullName = fullName;
            Type = null;
            Sink = null;
            Level = level;
            Options = new();
        }
    }

}
