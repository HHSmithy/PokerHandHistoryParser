create table pokerhandhistory (

DateOfHandUtc	VARCHAR(50)
,HandId		BIGINT
,DealerButtonPosition	SMALLINT
,TableName		VARCHAR(50)
,GameDescription	VARCHAR(200)
,NumPlayersActive	SMALLINT
,NumPlayersSeated	SMALLINT
,Rake			REAL
,ComumnityCards		VARCHAR(10)
,TotalPot		REAL
,PlayerName		VARCHAR(50)
,HoleCards		VARCHAR(4)
,StartingStack		REAL
,SeatNumber		SMALLINT
,ActionNumber		SMALLINT
,Amount			REAL
,HandActionType		VARCHAR(20)
,Outs			SMALLINT
,CardOuts		VARCHAR(200)
,CurrentHandRank	FLOAT8
,currentPostSize	REAL
,Street			VARCHAR(20)
,IsAggressiveAction	VARCHAR(5)
,IsAllIn		VARCHAR(5)
,IsAllInAction		VARCHAR(5)
,IsBlinds		VARCHAR(5)
,IsGameAction		VARCHAR(5)
,IsPreFlopRaise		VARCHAR(5)
,IsRaise		VARCHAR(5)
,IsWinningsAction	VARCHAR(5)

);
