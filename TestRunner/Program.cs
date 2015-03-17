using HandHistories.Writer.UnitTests.Writers.PokerStars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new PokerStarsWriterTests();
            
            test.TestFile(@"HandActionTests\3BetHand");
        }
    }
}
