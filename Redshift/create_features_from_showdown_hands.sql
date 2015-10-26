\timing on
-- create a helper function to sort pokerhands otherwise, AdTs is a different hand than TsAd

drop function sortstartinghand(varchar(4));
create function sortstartinghand (h varchar(4))
  returns varchar(4)
immutable
as $$
return ''.join(sorted([h[i:i+2] for i in range(0, len(h), 2)]))
$$ language plpythonu;

drop function sortstring(varchar);
create function sortstring (s varchar)
  returns varchar
immutable
as $$
return ''.join(sorted(s))
$$ language plpythonu;

-- extract a win from an actionorder string
drop function getwin(s varchar(65535));
create function getwin (s varchar(65535))
       returns int
immutable
as $$
sa = s.split(',')
return int(sa[len(sa) - 1].split(':')[2])
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
nn       ,numplayersseated
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
       ,sortstring(substring(holecards from 1 for 1) || substring(holecards from 3 for 1)) || case when substring(holecards from 2 for 1) = substring(holecards from 4 for 1) then 's' else 'o' end as holecards_simple
       ,startingstack
       ,round(startingstack / CAST(substring(gamedescription from 27 for 1) as INT)) as bb_in_startingstack
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
       , round(amount / CAST(substring(gamedescription from 27 for 1) as INT)) as num_big_blinds_in_amount
       -- potsize in big blinds
       , round(currentpostsize / CAST(substring(gamedescription from 27 for 1) as INT)) as num_big_blinds_in_currentpot
       ,handactiontype
       ,outs
       ,cardouts
       ,currenthandrank
       ,currentpostsize
       ,street
       ,isaggressiveaction
       ,isallin
       ,isallinaction
       ,isblinds
       ,isgameaction
       ,ispreflopraise
       ,israise
       ,iswinningsaction
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
);

-- create a contatenated list of actions so we can do some frequency analysis on what patterns occur most frequently
drop table action_orderings;
create table action_orderings as 
select 
       handid
       , listagg(handactiontype || ':' || street, ',') within group (order by actionnumber) as actionorder
       , listagg(playername, ',') within group (order by actionnumber) as playeractionorder
       , listagg(case when holecards = '  ' then 'NA' else holecards end, ',') within group (order by actionnumber) as holecardsorder
       , listagg(seatnumber, ',') within group (order by actionnumber) as seatnumberorder
       , listagg(amount, ',') within group (order by actionnumber) as amountorder
       , listagg(pct_of_starting_stack, ',') within group (order by actionnumber) as pct_of_starting_stackorder
       , listagg(num_big_blinds_in_amount, ',') within group (order by actionnumber) as num_big_blinds_in_amountorder
       , listagg(amount_pct_into_currentpot, ',') within group (order by actionnumber) as amount_pct_into_currentpotorder
       , listagg(currentpostsize, ',') within group (order by actionnumber) as currentpotsizeorder
       , listagg(num_big_blinds_in_currentpot, ',') within group (order by actionnumber) as num_big_blinds_in_currentpotorder
       , listagg(outs, ',') within group (order by actionnumber) as outsorder
from pokerhandhistory_showdowns
group by handid;

-- Breakdown of betting patterns by individual holecards
-- this lets us look at any frequent betting patters specific holecards might have
drop table holecards_by_actiontype;
create table holecards_by_actiontype as
select 
       handid
       , playername
       , startingstack
       , bb_in_startingstack
       , holecards
       , holecards_simple
       , round(abs(sum(case when amount < 0 then num_big_blinds_in_amount else 0 end))) as ttl_bb_bet_pot
       , round(abs(sum(case when amount < 0 then num_big_blinds_in_amount else 0 end)) / bb_in_startingstack * 100) as pctofstackatrisk
       , listagg(handactiontype || ':' || street || ':' || num_big_blinds_in_amount, ',') within group (order by actionnumber) as actionorderappends
       , listagg(handactiontype, ',') within group (order by actionnumber) as actionorder
       , listagg(num_big_blinds_in_amount, ',') within group (order by actionnumber) as numbigblindsorder
from pokerhandhistory_showdowns
where holecards != '  ' -- we don't care if we can't see their cards
group by handid, playername, bb_in_startingstack, holecards, holecards_simple, startingstack;
select * from holecards_by_actiontype where holecards_simple='38o' limit 10;

drop table holecards_by_actiontype_freq;
create table holecards_by_actiontype_freq as
select 
       holecards_simple
       , count(handid)
       , case when actionorderappends like '%WINS%' then 1 else 0 end as iswin
       , round(avg(ttl_bb_bet_pot)) as avgttlbbbetpot
       , round(avg(pctofstackatrisk)) as avgpctofstackatrisk
       , round(avg(bb_in_startingstack)) as avgstartingstack
       , actionorderappends
       , actionorder
       , numbigblindsorder
from holecards_by_actiontype 
-- where holecards_simple='AAo'
group by holecards_simple, actionorder, actionorderappends, numbigblindsorder
order by holecards_simple, count(handid) desc; --limit 20
select * from holecards_by_actiontype_freq limit 10;


drop table holecards_by_actiontype_features;
create table holecards_by_actiontype_features as
select 
       holecards_simple
       ,"count"
       , iswin
       , case when iswin = 1 then cast(getwin(actionorderappends) as int) else 0 end as winamount
       , case when iswin = 0 then avgttlbbbetpot else 0 end as avglossamout
       , avgpctofstackatrisk
       , avgstartingstack
       , actionorderappends
       , actionorder
       , numbigblindsorder
from holecards_by_actiontype_freq;
select * from holecards_by_actiontype_features limit 10;
