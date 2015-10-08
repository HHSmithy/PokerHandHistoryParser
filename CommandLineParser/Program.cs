using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.FastParser.Base;

namespace CommandLineParser
{
    // the design on "class Card"  "class Hand" and "class HandEvaluator" need some serious re-work.  Stole this code from 7 years ago and it sucks but it works.
    class Card
    {
        // create specific types for card objects
        // assign values because compiler starts at 0 by default and that would be confussing.
        public enum Suit { Spade = 0, Club = 1, Heart = 2, Diamond = 3 };
        public enum Rank
        {
            Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11,
            Queen = 12, King = 13, Ace = 14
        };

        Dictionary<char, Suit> str_to_suit = new Dictionary<char, Suit>();
        Dictionary<char, Rank> str_to_rank = new Dictionary<char, Rank>();
        
        Suit suit;
        Rank rank;
        // constructors
        public Card() { }
        public Card(Suit s, Rank r)
        {
            suit = s;
            rank = r;
        }
        public Card(string c)
        {
            if (c.Length != 2)
            {
                throw new Exception("a string card must equal length of 2 and be in a format of RankSuit, example 'As' or 'Ks' for Ace of Spades, King of Spades");
            }
            str_to_suit.Add('s', Suit.Spade);
            str_to_suit.Add('c', Suit.Club);
            str_to_suit.Add('h', Suit.Heart);
            str_to_suit.Add('d', Suit.Diamond);

            str_to_rank.Add('2', Rank.Two);
            str_to_rank.Add('3', Rank.Three);
            str_to_rank.Add('4', Rank.Four);
            str_to_rank.Add('5', Rank.Five);
            str_to_rank.Add('6', Rank.Six);
            str_to_rank.Add('7', Rank.Seven);
            str_to_rank.Add('8', Rank.Eight);
            str_to_rank.Add('9', Rank.Nine);
            str_to_rank.Add('T', Rank.Ten);
            str_to_rank.Add('J', Rank.Jack);
            str_to_rank.Add('Q', Rank.Queen);
            str_to_rank.Add('K', Rank.King);
            str_to_rank.Add('A', Rank.Ace);
            suit = str_to_suit[c[1]];
            rank = str_to_rank[c[0]];

        }
        // properties. get and set values of the card object.
        public Suit SUIT { get { return suit; } set { suit = value; } }
        public Rank RANK { get { return rank; } set { rank = value; } }
    }
    class Hand
    {
        // this variable is used by the hand evaluator.
        public int MaxHandSize;
        public int NumCardsToRank;
        private List<Card> hand;
        // constructors.
        // for a blank hand with a specific number of cards
        public Hand(int maxHandSize, int numCardsToRank)
        {
            this.MaxHandSize = maxHandSize;
            NumCardsToRank = numCardsToRank;
            hand = new List<Card>(maxHandSize);
            hand.Capacity = maxHandSize;
        }
        public Hand(string cards, int maxHandSize, int numCardsToRank)
        {
            if (cards.Length / 2 > maxHandSize)
            {
                throw new Exception("Error: list of cards > maxHandSize Can't add a list of cards to a hand that is greater than the maximum capacity of the hand.");
            }
            
            hand = new List<Card>(maxHandSize);
            hand.Capacity = maxHandSize;
            this.NumCardsToRank = numCardsToRank;
            this.MaxHandSize = maxHandSize;
            for (int i = 0; i <= cards.Length-2; i += 2)
            {
                hand.Add(new Card(cards.Substring(i, 2)));
            }

        }
        public Hand(List<Card> cards, int maxHandSize, int numCardsToRank)
        {
            this.MaxHandSize = maxHandSize;
            // throw and exception if a list of cards > maxHandSize is added to a hand.
            if (cards.Count > maxHandSize)
            {
                throw new Exception("Error: list of cards > maxHandSize Can't add a list of cards to a hand that is greater than the maximum capacity of the hand.");
            }
            numCardsToRank = NumCardsToRank;
            hand = new List<Card>(maxHandSize);
            hand = cards;
            hand.Capacity = maxHandSize;
        }
        // if no parameters are given the default hand size will be given
        public Hand()
        {
            // Default hand size is 7 (hole cards, flop, turn, river)
            // Default cards to rank is 5
            this.MaxHandSize = 7;
            hand = new List<Card>(7);
            hand.Capacity = 7;
            NumCardsToRank = 5;
        }
        // this function will simply add a list of cards to a hand
        // throws an exception if too many cards are added to the hand.
        public void AddCards(List<Card> cardsToAdd)
        {
            // if the hand does not already contain the card...go ahead and add it to the hand.
            foreach (Card item in cardsToAdd)
            {
                if (!hand.Contains(item))
                {
                    if (hand.Count < MaxHandSize)
                        hand.Add(item);
                    else
                    {
                        //throw new Exception("Error: Tried to add too many cards to the current hand than specified by maxCapacity" +
                        //    "\n\nyou should not see this error...there is a major problem");
                        //System.Windows.Forms.MessageBox.Show("Tried to add a card to the hand that already exists in the hand.");
                    }
                }
            }
        }
        public void AddCard(Card cardToAdd)
        {
            // if the hand doesn't contain the card....go ahead and add it to the hand.
            if (!hand.Contains(cardToAdd))
            {
                if (hand.Count < MaxHandSize)
                    hand.Add(cardToAdd);
                else
                {
                    //throw new Exception("Error: Tried to add too many cards to the current hand than specified by maxCapacity" +
                    //    "\n\nyou should not see this error...there is a major problem");
                    //System.Windows.Forms.MessageBox.Show("Tried to add a card to the hand that already exists in the hand.");
                }
            }

        }
        // should probably change this to do a binary search....rather than linear
        public bool ContainsRank(Card.Rank rank)
        {
            foreach (Card item in hand)
            {
                if (item.RANK.Equals(rank))
                    return true;

            }
            return false;
        }
        // should probably change this to do a binary search.
        public bool ContainsSuit(Card.Suit suit)
        {
            foreach (Card item in hand)
            {
                if (item.SUIT.Equals(suit))
                    return true;

            }
            return false;
        }
        // function to remove a list of cards from the hand.
        public List<Card> RemoveCards(List<Card> cardsToRemove)
        {
            // if the hand contains the card.  then remove it.
            foreach (Card item in cardsToRemove)
            {
                if (hand.Contains(item))
                    hand.Remove(item);
            }
            return hand;
        }
        // function to remove a single card from the hand.
        public List<Card> RemoveCard(Card cardToRemove)
        {
            // if hand contains the card.  then remove it.
            if (hand.Contains(cardToRemove))
                hand.Remove(cardToRemove);
            return hand;
        }
        // debugging purposes. Function to print hand to the console
        public void PrintHand()
        {
            Console.WriteLine("The hand is: ");
            foreach (Card item in hand)
            {
                Console.WriteLine("{0} of {1}s", item.RANK, item.SUIT);
            }
        }
        // properties
        public List<Card> HAND { get { return hand; } set { hand = value; } }



    }
    class HandEvaluator
    {
        private Bins bins;
        private bool[] binFull;
        // keeps track of bins in a Generic Array for easy referencing
        private Hand[] binMap;
        // keeps track of the differences between all sequencial pairs of cards in the straightflush bin
        public HandEvaluator()
        {
            binMap = new Hand[9];
            binFull = new bool[9];
            bins = new Bins();
            // map all the bins to their coresponding rank as index in the ArrayList
            binMap[1] = bins.PairBin;
            binMap[2] = (bins.TwoPairBin);
            binMap[3] = (bins.ThreeKindBin);
            binMap[4] = (bins.StraightBin);
            binMap[6] = (bins.FullHouseBin);
            binMap[7] = (bins.FourKindBin);
            for (int i = 0; i < binFull.Length; i++)
                binFull[i] = false;

        }
        public Bins BINS { get { return bins; } }

        private static int CompareCardsByRank(Card x, Card y)
        {
            int xrank = (int)x.RANK;
            int yrank = (int)y.RANK;
            if (xrank == yrank)
                return 0;
            if (xrank > yrank)
                return -1;
            else
                return 1;
        }
        // merge sort....for some reason bubble sort was faster....then i found a way to do it without writing my own code.
        public List<Card> SortHand(List<Card> hand)
        {
            hand.Sort(CompareCardsByRank);
            return hand;
            //for (int i = 0; i < hand.Count - 1; i++)
            //{
            //    for (int j = i; j < hand.Count - 1; j++)
            //    {
            //        if ((int)hand[j].RANK < ((int)hand[j + 1].RANK))
            //        {
            //            Card temp = hand[j];
            //            hand[j] = hand[j + 1];
            //            hand[j + 1] = temp;
            //        }
            //    }
            //}
            //return hand;
            //List<Card> left = new List<Card>();
            //List<Card> right = new List<Card>();
            //int mid = (hand.Count / 2);
            //List<Card> result;
            //if (hand.Count == 1)
            //    return hand;
            //for (int i = 0; i < mid; i++)
            //{
            //    left.Add(hand[i]);
            //}
            //for (int i = mid; i < hand.Count; i++)
            //{
            //    right.Add(hand[i]);
            //}
            //left = SortHand(left);
            //right = SortHand(right);
            //result = merge(left, right);
            //return result;
        }
        // helper function for SortHand
        private List<Card> merge(List<Card> l, List<Card> r)
        {
            List<Card> result = new List<Card>();
            // whild both counts are greater than zero
            while (l.Count > 0 && r.Count > 0)
            {
                // if the first element in the left side of the array is less than the right add it to the final result
                if ((int)l[0].RANK > (int)r[0].RANK)
                {
                    result.Add(l[0]);
                    l.RemoveAt(0);
                }
                // else add the first element of the right
                else
                {
                    result.Add(r[0]);
                    r.RemoveAt(0);
                }
            } // end while
            if (l.Count > 0)
                result.AddRange(l);
            if (r.Count > 0)
                result.AddRange(r);
            return result;
        }
        public double RankHand(Hand myHand, out string stringRank)
        {   // keep track of the current max full bin..this is used for ranking the cards
            int start = DateTime.Now.Millisecond;
            int maxBin = -1;
            // keeps track of which suit, if any, is the flush
            int finalSuitIndex = -1;
            //long SortingStart = DateTime.Now.Ticks;
            myHand.HAND = SortHand(myHand.HAND);

            // high card will always be the first card in the hand since it is sorted in descending order
            bins.HighCardBin = myHand.HAND[0];
            if (0 > maxBin)
                maxBin = 0;
            binFull[0] = true;
            int i = 0;
            while (i <= myHand.HAND.Count - 1)
            {
                Card card1 = myHand.HAND[i];
                Card card2;
                // this is a special case where we need to check the first and last card since aces are high or low in straights.
                // if we are at the end of the hand assign card2 to the first card to see if it is an ace....otherwise continue scanning as normal
                if (i == myHand.HAND.Count - 1)
                    card2 = myHand.HAND[0];
                else
                    card2 = myHand.HAND[i + 1];

                int suitIndex = (int)card1.SUIT;
                int suitCard2 = (int)card2.SUIT;
                // checking for straight flushes...here we don't want to add aces until the end because they can be high or low.
                // aces won't get checked until we reach the end of the while loop.
                if (binFull[8] == false)
                {
                    if (!card1.RANK.Equals(Card.Rank.Ace))
                    {
                        List<Card> SFtemp = bins.StraightFlushBin[suitIndex, 0];
                        if (SFtemp.Count > 0)
                        {
                            if (Difference(SFtemp[SFtemp.Count - 1], card1) == 1 && (int)card1.SUIT == suitIndex)
                                if (!SFtemp.Contains(card1))
                                {
                                    SFtemp.Add(card1);
                                    if (SFtemp.Count == SFtemp.Capacity)
                                    {
                                        maxBin = 8;
                                        finalSuitIndex = suitIndex;
                                        binFull[8] = true;
                                    }

                                }

                        }
                        else
                            SFtemp.Add(card1);
                    }
                }

                // check to see if the bin is full
                List<Card> temp = bins.FlushBin[suitIndex, 0];
                // if the bin is not full add the card to the flush bin
                if (binFull[5] == false)
                {
                    temp.Add(card1);
                }
                // check to see if the flush bin is full
                if (temp.Count == temp.Capacity)
                {
                    finalSuitIndex = suitIndex;
                    binFull[5] = true;
                    if (5 > maxBin)
                        maxBin = 5;
                }
                // compute the difference between each hand an it's i+1 neighbor

                switch (Difference(card1, card2))
                {
                    // case where the face values differ by one here we would be concerned with only the straight bin or the straight 
                    //flush bin
                    case 1:
                        {
                            // if the straight bin is not full
                            if (binFull[4] == false)
                            {
                                // and the cards differ by one...and the bin doesn't contain that rank add them to the bin
                                if (!bins.StraightBin.ContainsRank(card1.RANK))
                                    bins.StraightBin.AddCard(card1);
                                if (!bins.StraightBin.ContainsRank(card2.RANK))
                                    bins.StraightBin.AddCard(card2);
                                // check if the bin is full
                                if (bins.StraightBin.HAND.Count == bins.StraightBin.HAND.Capacity)
                                {
                                    binFull[4] = true;
                                    if (4 > maxBin)
                                        maxBin = 4;
                                }
                            }
                            // otherwise clear the pair bin
                        } // end case 1
                        ClearPairBins();
                        break;
                    // special case where ace is low card in straight
                    case 12:
                        goto case 1;
                    // case where the cards are equal...here we are interested in a multitude of different bins
                    case 0:
                        {

                            // if pair bin is not full...add them to the pair bin
                            if (binFull[1] == false)
                            {
                                bins.PairBin.AddCard(card1);
                                bins.PairBin.AddCard(card2);
                                // check to see if bin is full....which it should be.
                                if (bins.PairBin.HAND.Count == bins.PairBin.HAND.Capacity)
                                {
                                    binFull[1] = true;
                                    if (1 > maxBin)
                                        maxBin = 1;
                                }
                            }
                            // case where two pair bin is not full

                            if (binFull[2] == false)
                            {
                                // if twopair bin has a pair and the new found pair has a different rank than the current pair
                                if (bins.TwoPairBin.HAND.Count == 2 && card1.RANK != bins.TwoPairBin.HAND[0].RANK)
                                {
                                    // add the new pair to the bin
                                    bins.TwoPairBin.AddCard(card1);
                                    bins.TwoPairBin.AddCard(card2);
                                }
                                // else add the pair to the two pair bin because it should be empty
                                else
                                {
                                    if (bins.TwoPairBin.HAND.Count == 0)
                                    {
                                        bins.TwoPairBin.AddCard(card1);
                                        bins.TwoPairBin.AddCard(card2);
                                    }
                                }
                                // check if bin is full
                                if (bins.TwoPairBin.HAND.Count == bins.TwoPairBin.HAND.Capacity)
                                {
                                    binFull[2] = true;
                                    if (2 > maxBin)
                                        maxBin = 2;
                                }
                            }
                            // case where three of a kind bin is not full
                            if (binFull[3] == false)
                            {
                                // if there are no cards then add the pair.
                                if (bins.ThreeKindBin.HAND.Count == 0)
                                {
                                    bins.ThreeKindBin.AddCard(card1);
                                    bins.ThreeKindBin.AddCard(card2);
                                }
                                else
                                {
                                    if (bins.ThreeKindBin.HAND[0].RANK == card1.RANK)
                                    {
                                        bins.ThreeKindBin.AddCard(card1);
                                        bins.ThreeKindBin.AddCard(card2);
                                    }
                                    else
                                    {
                                        bins.ThreeKindBin.HAND.Clear();
                                    }
                                }
                                // check if bin is full
                                if (bins.ThreeKindBin.HAND.Count == bins.ThreeKindBin.HAND.Capacity)
                                {
                                    binFull[3] = true;

                                    if (3 > maxBin)
                                        maxBin = 3;
                                }
                            }
                            // case where full house is a special case where if the three of a kind bin is full and the two pair bin is full
                            // then we know that our full house is the three of a kind bin plus the pair in the two pair bin whos face value
                            // is different than the three of a kind face value.
                            if (binFull[6] == false)
                            {
                                if (binFull[3] == true && binFull[2] == true)
                                {
                                    // immediatly add the three of a kind since we know they will be part of the full house.
                                    bins.FullHouseBin.AddCards(bins.ThreeKindBin.HAND);
                                    // if the first two cards in the sorted twopair bin have a different rank than the three of a kind...
                                    // then we know that is our maximum full house
                                    if (SortHand(bins.TwoPairBin.HAND)[0].RANK != bins.ThreeKindBin.HAND[0].RANK)
                                        bins.FullHouseBin.AddCards(SortHand(bins.TwoPairBin.HAND).GetRange(0, 2));
                                    // otherwise the highest pair in the twopair bin is also containted in the three kind.
                                    // thus making the pair in our full house the lower of the two pair.
                                    else
                                        bins.FullHouseBin.AddCards(SortHand(bins.TwoPairBin.HAND).GetRange(2, 2));

                                }
                                if (bins.FullHouseBin.HAND.Count == bins.FullHouseBin.HAND.Capacity)
                                {
                                    binFull[6] = true;
                                    if (6 > maxBin)
                                        maxBin = 6;
                                }
                            }
                            // case of four of a kind
                            if (binFull[7] == false)
                            {
                                bins.FourKindBin.AddCard(card1);
                                bins.FourKindBin.AddCard(card2);
                                if (bins.FourKindBin.HAND.Count == bins.FourKindBin.HAND.Capacity)
                                {
                                    binFull[7] = true;
                                    if (7 > maxBin)
                                        maxBin = 7;
                                }
                            }


                        } // end case 0

                        break;
                    default:
                        ClearBins();
                        break;
                } // end switch
                i++;

            }// end while
            // final check to see it the straight flush bin needs any aces to make it's hand
            if (binFull[8] == false)
            {
                // case of high card being ace
                if (bins.HighCardBin.RANK.Equals(Card.Rank.Ace))
                {
                    List<Card> temp = bins.StraightFlushBin[(int)bins.HighCardBin.SUIT, 0];
                    if (temp.Count == 4)
                    {
                        if (Difference(bins.HighCardBin, temp[temp.Count - 1]) == 12)
                            if (!temp.Contains(bins.HighCardBin))
                                temp.Add(bins.HighCardBin);
                        if (Difference(bins.HighCardBin, temp[0]) == 1)
                            if (!temp.Contains(bins.HighCardBin))
                                temp.Insert(0, bins.HighCardBin);
                    }
                }
                // case of a pair of aces that need to be checked
                if (binFull[1])
                {
                    if (bins.PairBin.HAND[0].RANK.Equals(Card.Rank.Ace))
                    {

                        foreach (Card item in bins.PairBin.HAND)
                        {
                            List<Card> temp = bins.StraightFlushBin[(int)item.SUIT, 0];
                            if (temp.Count == 4)
                            {
                                if (Difference(item, temp[temp.Count - 1]) == 12)
                                    if (!temp.Contains(item))
                                        temp.Add(item);
                                if (Difference(item, temp[0]) == 1)
                                    if (!temp.Contains(item))
                                        temp.Add(item);
                            }
                        } // end for each
                    }// end check if pair bin rank is ace
                } // end check if pair bin is full
                // case of three of a kind aces that all need to be checked
                if (binFull[3])
                {
                    if (bins.ThreeKindBin.HAND[0].RANK.Equals(Card.Rank.Ace))
                    {
                        foreach (Card item in bins.ThreeKindBin.HAND)
                        {
                            List<Card> temp = bins.StraightFlushBin[(int)item.SUIT, 0];
                            if (temp.Count == 4)
                            {
                                if (Difference(item, temp[temp.Count - 1]) == 12)
                                    if (!temp.Contains(item))
                                        temp.Add(item);
                                if (Difference(item, temp[0]) == 1)
                                    if (!temp.Contains(item))
                                        temp.Add(item);
                            }
                        } // end foreach
                    } // end check for tkind rank Ace
                } // end if three kind bin full

            } // end check for final straight flush

            for (int j = 0; j < bins.StraightFlushBin.Length; j++)
            {
                if (bins.StraightFlushBin[j, 0].Count == bins.StraightFlushBin[j, 0].Capacity)
                {
                    binFull[8] = true;
                    finalSuitIndex = j;
                    maxBin = 8;
                }
            }
            // remove the cards in the bin from the players current hand and then use the rest of the cards to rank the highest kickers.
            string finalRank = Convert.ToString(maxBin) + ".";
            Hand tempHand = new Hand();
            // in this switch case the hand rank is built by converting the bin index to a string then appending "." to make is a double.  
            // each sucessive cards in their ranking order are multiplied by .01 then parsed into a string and concatinated to the overall rank.
            // for example: AsJs10s9s4s would be ranked 5.1411100904 then converted to a double for easy comparison in dertermining a winning hand.
            // trim all the periods our of the double values
            char[] trims = { '.' };
            switch (maxBin)
            {
                // case 0 and 5 are different cases because they have different types then the rest of the other bins.

                // case of a high card hand
                case 0:
                    // in this case the first five cards will be your best hand.
                    for (int j = 0; j < myHand.HAND.Count; j++)
                    {
                        string temp = Convert.ToString(.01 * (double)myHand.HAND[j].RANK).Replace("0.", "");
                        if (j < myHand.NumCardsToRank)
                            finalRank += temp;
                        else
                            break;
                    }

                    break;
                // case where hand is a flush
                case 5:
                    tempHand.HAND = bins.FlushBin[finalSuitIndex, 0];

                    foreach (Card item in tempHand.HAND)
                    {
                        string temp = Convert.ToString(.01 * (double)item.RANK).Replace("0.", "");
                        finalRank += temp;
                    }

                    break;
                // case where we have a straight flush
                case 8:
                    tempHand.HAND = bins.StraightFlushBin[finalSuitIndex, 0];
                    foreach (Card item in tempHand.HAND)
                    {
                        string temp = Convert.ToString(.01 * (double)item.RANK).Replace("0.", "");
                        finalRank += temp;
                    }
                    break;
                // any other case is kept in the bin map
                default:
                    tempHand = binMap[maxBin];
                    Hand newHand = new Hand(myHand.MaxHandSize, myHand.NumCardsToRank);
                    foreach (Card item in tempHand.HAND)
                    {
                        string temp = Convert.ToString((.01 * (double)item.RANK)).Replace("0.", "");
                        newHand.AddCard(item);
                        finalRank += temp;
                    }
                    List<Card> foo = myHand.RemoveCards(tempHand.HAND);
                    int index = 0;
                    while (newHand.HAND.Count < myHand.NumCardsToRank)
                    {
                        string temp = Convert.ToString(.01 * (double)myHand.HAND[index].RANK).Replace("0.", "");
                        finalRank += temp;
                        newHand.AddCard(myHand.HAND[index]);
                        index++;
                    }
                    myHand.AddCards(newHand.HAND);
                    break;
            }
            int stop = DateTime.Now.Millisecond;


            //Console.WriteLine("It took {0} Milliseconds to Rank the hand", stop - start);
            //if (stop - start > 0)
            //    Console.ReadLine();
            stringRank = finalRank;
            return Convert.ToDouble(finalRank);
        }
        // debugging function
        private void DisplayBins()
        {
            Console.WriteLine("The Four of a kind bin Contains: ");
            bins.FourKindBin.PrintHand();
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Full House");
            bins.FullHouseBin.PrintHand();
            Console.WriteLine("Flush");
            Console.WriteLine("Straight");
            Console.WriteLine("TKind");
            bins.ThreeKindBin.PrintHand();
            Console.WriteLine("Two Pair");
            bins.TwoPairBin.PrintHand();
            Console.WriteLine("Pair");
            bins.PairBin.PrintHand();
        }

        // function to compute the difference between two cards by face value used by rankhand function
        private int Difference(Card card1, Card card2)
        {
            return Math.Abs((int)card1.RANK - (int)card2.RANK);
        }
        // resets bins if the bin is not full
        private void ClearBins()
        {
            if (binFull[1] == false)
                bins.PairBin.HAND.Clear();
            if (binFull[2] == false && bins.TwoPairBin.HAND.Count != 2 && bins.TwoPairBin.HAND.Count != 4)
            {
                bins.TwoPairBin.HAND.Clear();
            }
            if (binFull[3] == false)
            {
                bins.ThreeKindBin.HAND.Clear();
            }
            if (binFull[4] == false)
                bins.StraightBin.HAND.Clear();
            if (binFull[6] == false)
                bins.FullHouseBin.HAND.Clear();
            if (binFull[7] == false)
                bins.FourKindBin.HAND.Clear();

        }
        private void ClearStraightBins()
        {
            if (binFull[4] == false)
                bins.StraightBin.HAND.Clear();

        }
        private void ClearPairBins()
        {
            if (binFull[1] == false)
                bins.PairBin.HAND.Clear();
            if (binFull[2] == false && bins.TwoPairBin.HAND.Count != 2 && bins.TwoPairBin.HAND.Count != 4)
            {
                bins.TwoPairBin.HAND.Clear();
            }
            if (binFull[3] == false)
            {
                bins.ThreeKindBin.HAND.Clear();
            }
            if (binFull[6] == false)
                bins.FullHouseBin.HAND.Clear();
            if (binFull[7] == false)
                bins.FourKindBin.HAND.Clear();
        }
        public class Bins
        {
            // class to hold the data structures of the hand ranks.  Each bin is a hand with a specific capacity.  the reason for making them each
            // a hand is because the Hand class implements it's own add method more suitable to the algorithm than the built in add method to the
            // generic list...the hand class checks to see if the hand contains the card before adding it...with the built in add method cards of
            // the same value could get added...we don't want this.
            Card highCard;
            Hand pair;
            Hand twoPair;
            Hand threeKind;
            Hand straight;
            // flush and fullHouse are special cases
            List<Card>[,] flush;
            Hand fullHouse;
            Hand fourKind;
            List<Card>[,] straightFlush;
            public Bins()
            {
                pair = new Hand(2, 5);
                twoPair = new Hand(4, 5);
                threeKind = new Hand(3, 5);
                straight = new Hand(5, 5);
                fourKind = new Hand(4, 5);
                fullHouse = new Hand(5, 5);

                // this probably isnt the way to do this...
                highCard = new Card();
                pair.HAND = new List<Card>();
                pair.HAND.Capacity = 2;
                twoPair.HAND = new List<Card>();
                twoPair.HAND.Capacity = 4;
                threeKind.HAND = new List<Card>();
                threeKind.HAND.Capacity = 3;
                straight.HAND = new List<Card>();
                straight.HAND.Capacity = 5;
                flush = new List<Card>[5, 1];
                for (int i = 0; i < 5; i++)
                {
                    flush[i, 0] = new List<Card>();
                    flush[i, 0].Capacity = 5;
                }
                fullHouse.HAND = new List<Card>();
                fullHouse.HAND.Capacity = 5;
                fourKind.HAND = new List<Card>();
                fourKind.HAND.Capacity = 4;
                straightFlush = new List<Card>[5, 1];
                for (int i = 0; i < 5; i++)
                {
                    straightFlush[i, 0] = new List<Card>();
                    straightFlush[i, 0].Capacity = 5;
                }

            }
            // properties to access each bin
            public Card HighCardBin { get { return highCard; } set { highCard = value; } }
            public Hand PairBin { get { return pair; } }
            public Hand TwoPairBin { get { return twoPair; } }
            public Hand ThreeKindBin { get { return threeKind; } }
            public Hand StraightBin { get { return straight; } }
            public List<Card>[,] FlushBin { get { return flush; } }
            public Hand FullHouseBin { get { return fullHouse; } }
            public Hand FourKindBin { get { return fourKind; } }
            public List<Card>[,] StraightFlushBin { get { return straightFlush; } }
        }

    }

    class Program
    {


        static void Main(string[] args)
        {// takes a full path to a directory that contains raw *.txt poker history files from 888 poker.
            IHandHistoryParserFactory handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            // Get the correct parser from the factory.
            var handHistoryParser = new Poker888FastParserImpl();
            HandHistoryParserFastImpl fastParser = handHistoryParser as HandHistoryParserFastImpl;

            string handRank, handRank2;
            HandEvaluator he = new HandEvaluator();
            Hand h = new Hand("AsJcKdTs8c9c2h", 7, 5);
            double hr = he.RankHand(h, out handRank);
            he = new HandEvaluator();
            Hand h2 = new Hand("KsJcTs8h2c3h9d", 7, 5);
            double hr2 = he.RankHand(h2, out handRank2);
            Console.WriteLine("hr1 {0}\nhr2 {1}", handRank, handRank2);

            if (hr > hr2)
            {
                Console.WriteLine("hr1 wins");
            }
            else if (hr < hr2)
            {
                Console.WriteLine("hr2 wins");
            }
            else{
                Console.WriteLine("tie");
            }
            Console.ReadLine();
            return;

            try
            {
                // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                // be silent and null will be returned.
                string[] files = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
                int fileCount = files.Length;
                foreach (string file in files)
                {
                    Console.WriteLine("number of files left {0} out of {1}", fileCount--, files.Length);
                    string fileText = new StreamReader(file).ReadToEnd();
                    var hands = fastParser.SplitUpMultipleHandsToLines(fileText);
                    var outputFile = new StreamWriter(file + ".csv");
                    outputFile.WriteLine("DateOfHandUtc,HandId,DealerButtonPosition,TableName,GameDescription,NumPlayersActive,NumPlayersSeated,Rake,ComumnityCards,TotalPot,PlayerName,HoleCards,StartingStack,SeatNumber,ActionNumber,Amount,HandActionType,currentPostSize,Street,IsAggressiveAction,IsAllIn,IsAllInAction,IsBlinds,IsGameAction,IsPreFlopRaise,IsRaise,IsWinningsAction");
                    foreach (var hand in hands)
                    {
                        var parsedHand = fastParser.ParseFullHandHistory(hand, true);

                        // probably not the best way to do this. This should be added to the ParseHandActions function or something.
                        decimal currentPotSize = 0;
                        int actionNumber = 0;
                        foreach (var action in parsedHand.HandActions)
                        {
                            // I tried to do this (this = 'actionNumber++') rtdfgcvdrt properly via the ParseHandActions function in Poker888FastParserImpl.cs file to be 1-1 with the raw handlines, however
                            // I was getting some un-expected behavior when the action was all in and the winning action the handline index would be 0 so you would end up with
                            // actions sequences like 1,2,3,4,5,6,0,8,9,10.
                            actionNumber++;
                            if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.ANTE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BET
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.RAISE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.CALL
                                || action.HandActionType ==  HandHistories.Objects.Actions.HandActionType.ALL_IN)
                            {
                                currentPotSize += action.Amount;
                            }

                            outputFile.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}",
                                parsedHand.DateOfHandUtc
                                , parsedHand.HandId
                                , parsedHand.DealerButtonPosition
                                , parsedHand.TableName
                                , parsedHand.GameDescription
                                , parsedHand.NumPlayersActive
                                , parsedHand.NumPlayersSeated
                                , parsedHand.Rake
                                , parsedHand.ComumnityCards
                                , parsedHand.TotalPot
                                , action.PlayerName
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).HoleCards
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).StartingStack
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).SeatNumber
                                , actionNumber
                                , action.Amount
                                , action.HandActionType
                                , currentPotSize
                                , action.Street
                                , action.IsAggressiveAction
                                , action.IsAllIn
                                , action.IsAllInAction
                                , action.IsBlinds
                                , action.IsGameAction
                                , action.IsPreFlopRaise
                                , action.IsRaise
                                , action.IsWinningsAction
    );
                        }


                    }
                    outputFile.Close();
                }
            }
            catch (Exception ex) // Catch hand-parsing exceptions
            {
                Console.WriteLine("Parsing Error: {0}", ex.Message); // Example logging.
            }
        }
    }
}
