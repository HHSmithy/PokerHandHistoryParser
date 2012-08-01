using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;

namespace HandHistories.Parser.UnitTests.Parsers.HandParserTests.Players
{
    class HandParserPlayersTestsMergeImpl : HandParserPlayersTests
    {
        public HandParserPlayersTestsMergeImpl()
            : base("Merge")
        {
        }

        protected override PlayerList ExpectedNoHoleCardsPlayers
        {
            get 
            {
                /*
                        <player seat="0" nickname="sportslove011" balance="$101.50" dealtin="true" />
                        <player seat="1" nickname="dk2112" balance="$95.28" dealtin="true" />
                        <player seat="2" nickname="nikcname1" balance="$101.50" dealtin="true" />
                        <player seat="3" nickname="rossiwstaxi" balance="$104.98" dealtin="true" />
                        <player seat="4" nickname="cashgreedy00" balance="$102.74" dealtin="true" />
                        <player seat="5" nickname="bebenwong" balance="$111.16" dealtin="true" />
                        <player seat="6" nickname="MrJohn1" balance="$30.20" dealtin="true" />
                        <player seat="7" nickname="JennaMaroney" balance="$154.40" dealtin="true" />
                        <player seat="8" nickname="VegasGrinder03" balance="$64.24" dealtin="true" /> */

                return new PlayerList()
                           {
                               new Player("sportslove011", 101.5m, 0),
                               new Player("dk2112", 95.28m, 1),    
                               new Player("nikcname1", 101.5m, 2),                               
                               new Player("rossiwstaxi", 104.98m, 3),                               
                               new Player("cashgreedy00", 102.74m, 4),
                               new Player("bebenwong", 111.16m, 5),
                               new Player("MrJohn1", 30.2m, 6),
                               new Player("JennaMaroney", 154.4m, 7),
                               new Player("VegasGrinder03", 64.24m, 8)
                           };
            }
        }

        protected override PlayerList ExpectedWithShowdownPlayers
        {
            get
            {
                /*
		            <player seat="0" nickname="GODEXISTSJK" balance="$269.96" dealtin="true" />
		            <player seat="1" nickname="anica11" balance="$201.10" dealtin="true" />
		            <player seat="2" nickname="fasthand007" balance="$302.79" dealtin="true" />
		            <player seat="3" nickname="m0ney2Blow" balance="$230.23" dealtin="true" />
		            <player seat="4" nickname="Hateordiehere" balance="$186.83" dealtin="true" />
		            <player seat="5" nickname="dugaly" balance="$222.84" dealtin="true" />
                 * 
		            <cards type="SHOWN" cards="Kc,Kh" player="1"/>
		            <cards type="SHOWN" cards="Ad,Ah" player="5"/>                 
                 */
                return new PlayerList()
                           {
                               new Player("GODEXISTSJK", 269.96m, 0),
                               new Player("anica11", 201.1m, 1)
                                   {
                                       HoleCards = HoleCards.FromCards("KcKh")
                                   },                               
                               new Player("fasthand007", 302.79m, 2),                               
                               new Player("m0ney2Blow", 230.23m, 3),                               
                               new Player("Hateordiehere", 186.83m, 4),
                               new Player("dugaly", 222.84m, 5)
                                   {
                                       HoleCards = HoleCards.FromCards("AdAh")
                                   }
                           };
            }
        }

        protected override PlayerList ExpectedWithSittingOutPlayers
        {
            get
            {
                /*
                        		<player seat="0" nickname="sportslove011" balance="$101.50" dealtin="true" />
		<player seat="1" nickname="dk2112" balance="$95.28" dealtin="true" />
		<player seat="2" nickname="nikcname1" balance="$101.50" dealtin="true" />
		<player seat="3" nickname="rossiwstaxi" balance="$104.98" dealtin="true" />
		<player seat="4" nickname="cashgreedy00" balance="$102.74" dealtin="true" />
		<player seat="5" nickname="bebenwong" balance="$111.16" dealtin="true" />
		<player seat="6" nickname="MrJohn1" balance="$30.20" dealtin="false" />
		<player seat="7" nickname="JennaMaroney" balance="$154.40" dealtin="true" />
		<player seat="8" nickname="VegasGrinder03" balance="$64.24" dealtin="false" />*/

                return new PlayerList()
                           {
                               new Player("sportslove011", 101.5m, 0),
                               new Player("dk2112", 95.28m, 1),    
                               new Player("nikcname1", 101.5m, 2),                               
                               new Player("rossiwstaxi", 104.98m, 3),                               
                               new Player("cashgreedy00", 102.74m, 4),
                               new Player("bebenwong", 111.16m, 5),
                               new Player("MrJohn1", 30.2m, 6)
                                   {
                                       IsSittingOut = true
                                   },
                               new Player("JennaMaroney", 154.4m, 7),
                               new Player("VegasGrinder03", 64.24m, 8)
                                   {
                                       IsSittingOut = true
                                   }
                           };                
            }
        }

        protected override PlayerList ExpectedOmahaShowdownPlayers
        {
            get
            {
                /*
                 * 		<player seat="0" nickname="girlpoker69" balance="$325.50" dealtin="true" />
		<player seat="4" nickname="IlikePLO" balance="$863.50" dealtin="true" />
		<player seat="5" nickname="TomthegianT" balance="$600.00" dealtin="false" />

                 * 		<cards type="SHOWN" cards="7d,6s,Qh,Th" player="4"/>
		                <cards type="MUCKED" cards="3c,Js,Jh,6d" player="0"/> */
                return new PlayerList()
                           {
                               new Player("girlpoker69", 325.5m, 0)
                                   {
                                       HoleCards = HoleCards.FromCards("3cJsJh6d")
                                   },
                               new Player("IlikePLO", 863.5m, 4)
                                   {
                                       HoleCards = HoleCards.FromCards("7d6sQhTh")
                                   },
                               new Player("TomthegianT", 600m, 5)
                                   {
                                       IsSittingOut = true
                                   }
                           };                
            }
        }

        protected override PlayerList ExpectedOmahaHiLoShowdownPlayers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
