using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Compression;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.UnitTests.Parsers.Base;
using HandHistories.Parser.UnitTests.Utils.IO;
using Ninject;

namespace HandHistories.Parser.UnitTests.Infrastructure
{
    public class NinjectKernel : StandardKernel
    {
        public NinjectKernel()
        {
            // Utils
            Bind<IFileReader>().To<WindowsFileReaderImpl>();

            // Test Helpers
            Bind<ISampleHandHistoryRepository>().To<SampleHandHistoryRepositoryFileBasedImpl>();

            // Parsers
            Bind<IHandHistoryCompressor>().To<HandHistoryGZipCompressorImpl>();
            Bind<IHandHistoryParserFactory>().To<HandHistoryParserFactoryImpl>();
        }        
    }
}
