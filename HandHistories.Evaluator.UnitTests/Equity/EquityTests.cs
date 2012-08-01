using System.Collections.Generic;
using System.Linq;
using HandHistories.HandEvaluator.Equity;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace HandHistories.Evaluator.UnitTests.Equity
{
	[TestFixture]
	class EquityTests
	{
		private IEquityCalculator equityCalculator = new PokerEvalEquityCalculatorImpl();

		[Test]
		public void AsKc_vs_QdQh()
		{
			HoleCards holeCardsAsKc = HoleCards.ForHoldem("TestPlayer1", new Card('A', 's'), new Card('K', 'c'));
			HoleCards holeCardsQdQh = HoleCards.ForHoldem("TestPlayer2", new Card('Q', 'h'), new Card('Q', 'd'));

			/*
			
			1,712,304  games     0.005 secs   342,460,800  games/sec

			Board: 
			Dead:  
			equity 	win 	tie 	      pots won 	pots tied	
			Hand 0: 	42.835%  	42.66% 	00.17% 	        730541 	     2927.00   { AsKc }
			Hand 1: 	57.165%  	56.99% 	00.17% 	        975909 	     2927.00   { QdQh }
			
			 */


			AssertEquityIsCorrect(new HoleCards[] {holeCardsAsKc, holeCardsQdQh}, new long[] {730541, 975909},
								  new long[] {2927*2, 2927*2}, 1712304);
		}

		[Test]
		public void QcQd_vs_JcJh()
		{
			HoleCards holeCardsQcQd = HoleCards.ForHoldem("TestPlayer1", new Card('Q', 'c'), new Card('Q', 'd'));
			HoleCards holeCardsJcJh = HoleCards.ForHoldem("TestPlayer2", new Card('J', 'c'), new Card('J', 'h'));

			/*
				1,712,304  games     0.002 secs   856,152,000  games/sec

				Board: 
				Dead:  

				equity 	win 	tie 	      pots won 	pots tied	
				Hand 0: 	81.966%  	81.75% 	00.22% 	       1399759 	     3756.50   { QcQd }
				Hand 1: 	18.034%  	17.81% 	00.22% 	        305032 	     3756.50   { JcJh }

			 */

			AssertEquityIsCorrect(new HoleCards[] { holeCardsQcQd, holeCardsJcJh }, new long[] { 1399759, 305032 },
								  new long[] { 7513, 7513 }, 1712304);
		}

		[Test]
		public void AsJs_vs_ThTc_vs_9d9h()
		{
			HoleCards holeCardsAsJs = HoleCards.ForHoldem("TestPlayer1", new Card('A', 's'), new Card('J', 's'));
			HoleCards holeCardsThTc = HoleCards.ForHoldem("TestPlayer2", new Card('T', 'h'), new Card('T', 'c'));
			HoleCards holeCards9d9h = HoleCards.ForHoldem("TestPlayer3", new Card('9', 'd'), new Card('9', 'h'));

			/*
			   1,370,754  games     0.003 secs   456,918,000  games/sec

				Board: 
				Dead:  

					equity 	win 	tie 	      pots won 	pots tied	
				Hand 0: 	39.986%  	39.90% 	00.08% 	        546956 	     1153.00   { AsJs }
				Hand 1: 	42.954%  	42.87% 	00.08% 	        587647 	     1153.00   { TcTh }
				Hand 2: 	17.060%  	16.98% 	00.08% 	        232692 	     1153.00   { 9d9h }


			 */

			AssertEquityIsCorrect(new HoleCards[] { holeCardsAsJs, holeCardsThTc, holeCards9d9h }, new long[] { 546956, 587647, 232692 },
								  new long[] { 1153 * 3, 1153 * 3, 1153 * 3 }, 1370754);
		}

		[Test]
		public void KdQc_vs_As6s()
		{
			HoleCards holeCardsKdQc = HoleCards.ForHoldem("TestPlayer1", new Card('K', 'd'), new Card('Q', 'c'));
			HoleCards holeCardsAs6s = HoleCards.ForHoldem("TestPlayer2", new Card('6', 's'), new Card('A', 's'));            

			/*
				 1,712,304  games     0.001 secs     1,712,304,000  games/sec

				Board: 
				Dead:  

					equity 	win 	tie 	      pots won 	pots tied	
				Hand 0: 	39.874%  	39.67% 	00.20% 	        679271 	     3499.50   { KdQc }
				Hand 1: 	60.126%  	59.92% 	00.20% 	       1026034 	     3499.50   { As6s }

			 */

			AssertEquityIsCorrect(new HoleCards[] { holeCardsKdQc, holeCardsAs6s }, new long[] { 679271, 1026034 },
								  new long[] { (long)(3499.50 * 2), (long)(3499.50 * 2) }, 1712304);
		}

		private void AssertEquityIsCorrect(IEnumerable<HoleCards> holeCards, long[] expectedWins, long [] expectedTies, long expectedGamesEnumerated)
		{
			long[] wins;
			long[] ties;
			long[] losses;
			long totalHandsEnumerated;
			equityCalculator.HandOdds(holeCards.ToList(),
									  BoardCards.ForPreflop(),
									  null,
									  out wins,
									  out ties,
									  out losses,
									  out totalHandsEnumerated);

			Assert.AreEqual(expectedGamesEnumerated, totalHandsEnumerated, "Hands Enumerated");

			for (int i = 0; i < expectedWins.Length; i++ )
			{
				Assert.AreEqual(expectedWins[i], wins[i], i + " Wins");
				Assert.AreEqual(expectedTies[i], ties[i], i + " Ties");        
			}           
		}
	}
}