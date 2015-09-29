-- create a helper function to sort pokerhands otherwise, AdTs is a different hand than TsAd

create function sortstartinghand (h varchar(4))
  returns varchar(4)
immutable
as $$
return ''.join(sorted([h[i:i+2] for i in range(0, len(h), 2)]))
$$ language plpythonu;

create function sortstring (s varchar)
  returns varchar
immutable
as $$
return ''.join(sorted(s))
$$ language plpythonu;


-- create a table for only hands which have a showdown.  We only care to analyze patterns from these hands since
-- they will have hole cards. 
drop table pokerhandhistory_showdowns;
create table pokerhandhistory_showdowns as (
select
       dateofhandutc
       -- convert string to real timestamp
       -- redshift does not support the to_timestamp function so you have to convert string to a unix timestamp
       -- then convert the unix timestamp back to a timestamp
       ,TIMESTAMP 'epoch' + (extract(epoch from cast(dateofhandutc as timestamp))) * INTERVAL '1 second' as dateofhandutc_ts
       ,handid
       ,dealerbuttonposition
       ,tablename
       ,gamedescription
       ,CAST(substring(gamedescription from 24 for 1) as INT) as small_blind
       ,CAST(substring(gamedescription from 27 for 1) as INT) as big_blind
       ,numplayersactive
       ,numplayersseated
       ,rake
       ,comumnitycards
       ,totalpot
       ,playername
       ,holecards
       -- split out hole cards to make them easier to query
       ,substring(holecards from 1 for 2) as first_card
       ,substring(holecards from 3 for 2) as second_card
       ,substring(holecards from 1 for 1) as first_card_value
       ,substring(holecards from 3 for 1) as second_card_value
       ,substring(holecards from 2 for 1) as first_card_suit
       ,substring(holecards from 4 for 1) as second_card_suit
       ,substring(holecards from 2 for 1) = substring(holecards from 4 for 1) as suited
       -- reduce holecards to suited or offsuit to simplify analysis and reduce the number of distinct hands
       ,(sortstring(first_card_value || second_card_value) || case when suited then 's' else 'o' end) as holecards_simple
       ,startingstack
       ,seatnumber
       ,actionnumber
       ,amount
       -- percentage of pot
       -- there are some instances where folds happend before blinds are posted creating a current pot size of 0
       -- we do this below to avoid a divide by zero
       , round((amount / startingstack) * 100) as pct_of_starting_stack
       , round(amount / case when lag(currentpostsize, 1) over (order by handid, actionnumber) = 0 then 1 
       	 	       else lag(currentpostsize, 1) over (order by handid, actionnumber) end * 100) as amount_pct_into_currentpot
       -- amount in big blinds
       , round(amount / CAST(substring(gamedescription from 27 for 1) as INT), 1) as num_big_blinds_in_amount
       -- potsize in big blinds
       , round(currentpostsize / CAST(substring(gamedescription from 27 for 1) as INT), 1) as num_big_blinds_in_currentpot
       ,handactiontype
       ,currentpostsize
       ,street
from
    pokerhandhistory
where
    handid in (
        select
            distinct handid
        from
            pokerhandhistory
        where
            handactiontype = 'SHOW'
    )
order by handid, actionnumber
);



-- create a contatenated list of actions so we can do some frequency analysis on what patterns occur most frequently
drop table action_orderings;
create table action_orderings as 
select 
       handid
       , listagg(handactiontype, ',') within group (order by actionnumber) as actionorder
       , listagg(playername, ',') within group (order by actionnumber) as playeractionorder
       , listagg(case when holecards = '  ' then 'NA' else holecards end, ',') within group (order by actionnumber) as holecardsorder
       , listagg(seatnumber, ',') within group (order by actionnumber) as seatnumberorder
       , listagg(amount, ',') within group (order by actionnumber) as amountorder
       , listagg(pct_of_starting_stack, ',') within group (order by actionnumber) as pct_of_starting_stackorder
       , listagg(num_big_blinds_in_amount, ',') within group (order by actionnumber) as num_big_blinds_in_amountorder
       , listagg(amount_pct_into_currentpot, ',') within group (order by actionnumber) as amount_pct_into_currentpotorder
       , listagg(currentpostsize, ',') within group (order by actionnumber) as currentpotsizeorder
       , listagg(num_big_blinds_in_currentpot, ',') within group (order by actionnumber) as num_big_blinds_in_currentpotorder
from pokerhandhistory_showdowns
group by handid;

-- Breakdown of betting patterns by individual holecards
-- this lets us look at any frequent betting patters specific holecards might have
create table holecards_by_actiontype as
select 
       handid
       , playername
       , holecards
       , listagg(handactiontype, ',') within group (order by actionnumber) as actionorder
from pokerhandhistory_showdowns
where holecards != '  ' -- we don't care if we can't see their cards
group by handid, playername, holecards;
