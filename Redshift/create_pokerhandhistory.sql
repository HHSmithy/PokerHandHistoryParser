

create table pokerhandhistory (

DateOfHandUtc	VARCHAR(50) -- 1/1/2015 10:00:06 AM
,HandId		BIGINT -- 349921475
,DealerButtonPosition	SMALLINT -- 4
,TableName		VARCHAR(50) -- Provo
,GameDescription	VARCHAR(200) -- Pacific.NoLimitHoldem.$3-$6.Regular.6 Max
,NumPlayersActive	SMALLINT -- 6
,NumPlayersSeated	SMALLINT -- 6
,Rake			REAL -- 4.00
,ComumnityCards		VARCHAR(10) -- 5s2cTc2s7h
,TotalPot		REAL -- 
,PlayerName		VARCHAR(50) -- Beardal1ty
,HoleCards		VARCHAR(4) -- 
,StartingStack		REAL -- 927.12
,SeatNumber		SMALLINT -- 4
,ActionNumber		SMALLINT -- 6
,Amount			REAL -- 0
,HandActionType		VARCHAR(20) -- FOLD
,Outs			SMALLINT -- 0
,CardOuts		VARCHAR(200) -- 
,CurrentHandRank	VARCHAR(25) -- 0
,currentPostSize	REAL -- -24.75
,Street			VARCHAR(20) -- Preflop
,IsAggressiveAction	VARCHAR(5) -- False
,IsAllIn		VARCHAR(5) -- False
,IsAllInAction		VARCHAR(5) -- False
,IsBlinds		VARCHAR(5) -- False
,IsGameAction		VARCHAR(5) -- True
,IsPreFlopRaise		VARCHAR(5) -- False
,IsRaise		VARCHAR(5) -- False
,IsWinningsAction	VARCHAR(6) -- False

);
