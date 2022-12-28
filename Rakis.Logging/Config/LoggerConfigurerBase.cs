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

using static Rakis.Logging.Config.Configurer;

namespace Rakis.Logging.Config
{
    public abstract class LoggerConfigurerBase<T> : ILoggerConfigurer<T> where T : LoggerConfigurerBase<T>
    {
        private readonly Configurer configurer;

        private string _name;
        public string Name {
            get => _name ?? LastPartOf(FullName);
            set => _name = value;
        }
        private string _fullName;
        public string FullName { 
            get => _fullName ?? Name;
            set => _fullName = value;
        }
        public LogLevel Threshold { get; set; }

        protected LoggerConfigurerBase(Configurer configurer)
        {
            this.configurer = configurer;
        }

        private static string LastPartOf(string s)
        {
            if (s == null) return "";
            var lastDot = s.LastIndexOf('.');
            return lastDot == -1 ? s : s.Substring(lastDot + 1);
        }
        public T withName(string name)

        {
            Name = name;
            return (T)this;
        }

        public T withFullName(string fullName)
        {
            FullName = fullName;
            return (T)this;
        }

        public T withThreshold(LogLevel level)
        {
            Threshold = level;
            return (T)this;
        }

        public abstract LogConfig BuildConfig();

        public Configurer AddToConfig()
        {
            return configurer.AddConfig(BuildConfig());
        }

    }
}
