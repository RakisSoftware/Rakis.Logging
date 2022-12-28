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
using System;

namespace Rakis.Logging.Config
{
    public class ReactiveLoggerConfigurer : LoggerConfigurerBase<ReactiveLoggerConfigurer>
    {

        private ReactiveLogger logger = new();

        public ReactiveLoggerConfigurer(Configurer configurer) : base(configurer)
        {
        }

        public ReactiveLoggerConfigurer WithObserver(IObserver<LogEntry> observer)
        {
            logger.Subscribe(observer);
            return this;
        }

        public ReactiveLoggerConfigurer OnNext(Action<LogEntry> callback)
        {
            logger.Subscribe(callback);
            return this;
        }

        public override LogConfig BuildConfig()
        {
            LogConfig config = new(Name, FullName, Threshold);
            config.Type = ReactiveLogger.Type;
            config.Sink = logger;
            return config;
        }
    }
}
