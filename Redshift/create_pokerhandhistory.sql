create table pokerhandhistory (
       DateOfHandUtc	VARCHAR(50)
       , HandId		BIGINT
       , DealerButtonPosition	SMALLINT	
       , TableName		VARCHAR(50)
       , GameDescription	VARCHAR(200)
       , NumPlayersActive	SMALLINT
       , NumPlayersSeated	SMALLINT
       , Rake			REAL
       , ComumnityCards		VARCHAR(10)
       , TotalPot		REAL
       , PlayerName		VARCHAR(50)
       , HoleCards		VARCHAR(4)
       , StartingStack		REAL
       , SeatNumber		SMALLINT
       , ActionNumber		SMALLINT
       , Amount			REAL
       , HandActionType		VARCHAR(20)
       , CurrentPostSize	REAL
       , Street			VARCHAR(20)
       , IsAggressiveAction	VARCHAR(5)
       , IsAllIn		VARCHAR(5)
       , IsAllInAction		VARCHAR(5)
       , IsBlinds		VARCHAR(5)
       , IsGameAction		VARCHAR(5)
       , IsPreFlopRaise		VARCHAR(5)
       , IsRaise		VARCHAR(5)
       , IsWinningsAction 	VARCHAR(5)
);


copy pokerhandhistory from 's3://winthropstage/nick.888.backlog.1935.bigfile.csv.gz'
credentials 'aws_access_key_id=HERE;aws_secret_access_key=HERE'
delimiter ',' gzip;

unload ('select * from action_orderings')
to 's3://winthropstage/pokerhands/action_ordering_extract-'
credentials 'aws_access_key_id=HERE;aws_secret_access_key=HERE'
allowoverwrite;

unload ('select * from holecards_by_actiontype')
to 's3://winthropstage/pokerhands/holecards_by_actiontype_extract-'
credentials 'aws_access_key_id=HERE;aws_secret_access_key=HERE'
allowoverwrite;
