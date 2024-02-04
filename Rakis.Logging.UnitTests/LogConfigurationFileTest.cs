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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace Rakis.Logging.UnitTests
{

    public class LogConfigurationFileTest
    {
        private readonly ITestOutputHelper output;

        public LogConfigurationFileTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static string run(string command)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            // wrap IDisposable into using (in order to release hProcess) 
            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();

                // Add this: wait until process does its work
                process.WaitForExit();

                // and only then read the result
                return process.StandardOutput.ReadToEnd().Trim();
            }
        }

        private static string getTestPath(string relativePath)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return (dirPath == null) ? Path.Combine("..", "..", "..", "TestFiles", relativePath)  : Path.Combine(Directory.GetParent(dirPath)?.Parent?.Parent?.FullName ?? ".", "TestFiles", relativePath);
        }

        [Fact]
        public void TestCwd()
        {
            Logger.ConfigurationFromFile(getTestPath("rakisLog.properties")).Load().Build();
            var cwd = run("cd");
            output.WriteLine($"Current Working Directory = {cwd}");
            var testD = getTestPath("TestFiles");
            output.WriteLine($"Test files are in = {testD}");

            Logger.GetLogger(typeof(LogConfigurationFileTest)).Fatal?.Log($"Current Working Directory = {cwd}");
        }
    }
}
