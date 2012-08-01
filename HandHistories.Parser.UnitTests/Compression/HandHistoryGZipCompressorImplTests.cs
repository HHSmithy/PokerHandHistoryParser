using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Compression;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Compression
{
    [TestFixture]
    [Ignore("Low Priority")]
    class HandHistoryGZipCompressorImplTests
    {
        private IHandHistoryCompressor _handHistoryCompressor = new HandHistoryGZipCompressorImpl();

        [Test]
        public void CompressHandHistory_HandHistory_GZipsText()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void CompressHandHistory_Text_GZipsText()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UnCompressHandHistory_UnGzipsText()
        {
            throw new NotImplementedException();
        }
    }
}
