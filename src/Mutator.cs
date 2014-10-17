﻿// --------------------------------------------------------------------------------
// Copyright (c) 2014, XLR8 Development
// --------------------------------------------------------------------------------
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// --------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;

using Nito.KitchenSink.OptionParsing;

using Common.Logging;

namespace tsql2pgsql
{
    using visitors;

    public class Mutator
    {
        /// <summary>
        /// Logger for instance
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Application options
        /// </summary>
        private static AppOptions _options;

        static void MutateFile(string filePath)
        {
            Console.WriteLine("> {0}", filePath);

            var fileContent = File.ReadAllLines(filePath);

            fileContent = (new FormattingVisitor(
                fileContent)).Process();
            _log.Debug("MutationVisitor: Start");
            fileContent = (new MutationVisitor(
                fileContent)).Process();

            var prevLine = string.Empty;
            foreach (var line in fileContent)
            {
                var trimLine = line.TrimEnd();
                if (trimLine != string.Empty || prevLine != string.Empty)
                    Console.WriteLine(trimLine);

                prevLine = trimLine;
            }
        }

        static void Main(string[] args)
        {
            try {
				_options = OptionParser.Parse<AppOptions>();

                // determine the number of threads to use while processing
				if (_options.Threads == 0)
					_options.Threads = Environment.ProcessorCount;

                _options.Files.ForEach(MutateFile);

                Console.WriteLine();
            } catch( OptionParsingException e ) {
				Console.Error.WriteLine(e.Message);
				AppOptions.Usage();
			}
        }
    }
}
