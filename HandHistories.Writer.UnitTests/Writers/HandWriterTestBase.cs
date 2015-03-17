using HandHistories.Parser.Parsers.Base;
using HandHistories.Writer.Writer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Writer.UnitTests
{
    public abstract class HandWriterTestBase
    {
        string extension;

        IHandHistoryParser parser;

        IHandWriter writer;

        public HandWriterTestBase(IHandHistoryParser Parser, IHandWriter Writer)
        {
            this.parser = Parser;
            this.writer = Writer;
            this.extension = ".txt";
        }

        public HandWriterTestBase(IHandHistoryParser Parser, IHandWriter Writer, string Extension)
        {
            this.parser = Parser;
            this.writer = Writer;
            this.extension = Extension;
        }

        protected void TestHand(string file)
        {
            var path = string.Format(@"SampleHandHistories\{0}\CashGame\{1}{2}",
                parser.SiteName,
                file,
                extension);

            var text = File.ReadAllText(path).Trim();

            var hand = parser.ParseFullHandHistory(text);

            var exportedHand = writer.Write(hand);

            FileInfo FI = new FileInfo(@"Test\" + file + extension);
            if (!FI.Directory.Exists)
            {
                FI.Directory.Create();
            }

            File.WriteAllText(FI.FullName, exportedHand);

            Assert.AreEqual(text, exportedHand);
        }
    }
}
