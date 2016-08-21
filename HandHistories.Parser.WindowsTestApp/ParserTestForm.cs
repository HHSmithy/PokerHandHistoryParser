using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Factory;
using System.Diagnostics;
using HandHistories.Parser.Parsers.FastParser.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Utils;

namespace HandHistories.Parser.WindowsTestApp
{
    public partial class ParserTestForm : Form
    {
        public ParserTestForm()
        {
            InitializeComponent();

            listBoxSite.Items.Add(SiteName.BossMedia);
            listBoxSite.Items.Add(SiteName.PokerStars);
            listBoxSite.Items.Add(SiteName.PokerStarsFr);
            listBoxSite.Items.Add(SiteName.PokerStarsIt);
            listBoxSite.Items.Add(SiteName.PokerStarsEs);
            listBoxSite.Items.Add(SiteName.FullTilt);
            listBoxSite.Items.Add(SiteName.PartyPoker);
            listBoxSite.Items.Add(SiteName.IPoker);
            listBoxSite.Items.Add(SiteName.OnGame);
            listBoxSite.Items.Add(SiteName.OnGameFr);
            listBoxSite.Items.Add(SiteName.OnGameIt);
            listBoxSite.Items.Add(SiteName.Pacific);
            listBoxSite.Items.Add(SiteName.Entraction);
            listBoxSite.Items.Add(SiteName.Merge);
            listBoxSite.Items.Add(SiteName.WinningPoker);
            listBoxSite.Items.Add(SiteName.MicroGaming);
            listBoxSite.Items.Add(SiteName.Winamax);
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            if (listBoxSite.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Please pick a site");
                return;
            }

            IHandHistoryParserFactory factory = new HandHistoryParserFactoryImpl();
            var handParser = factory.GetFullHandHistoryParser((SiteName) listBoxSite.SelectedItem);
            bool validate = checkBox_validateHands.Checked;

            try
            {
                string text = richTextBoxHandText.Text;

                int parsedHands = 0;
                Stopwatch SW = new Stopwatch();
                SW.Start();

                HandHistoryParserFastImpl fastParser = handParser as HandHistoryParserFastImpl;

                var hands = fastParser.SplitUpMultipleHandsToLines(text.Trim());
                foreach (var hand in hands)
                {
                    var parsedHand = fastParser.ParseFullHandHistory(hand, true);
                    if (validate)
                    {
                        HandIntegrity.Assert(parsedHand);
                    }
                    parsedHands++;
                }

                SW.Stop();

                string message = string.Format("Parsed {0} hands. {1}ms",
                    parsedHands,
                    Math.Round(SW.Elapsed.TotalMilliseconds, 2));

                MessageBox.Show(this, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, "Error");
            }
        }
    }
}
