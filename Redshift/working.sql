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
       ,startingstack
       ,seatnumber
       ,actionnumber
       ,amount
       -- percentage of pot
       -- there are some instances where folds happend before blinds are posted creating a current pot size of 0
       -- we do this below to avoid a divide by zero
       , round(amount / startingstack) as pct_of_starting_stack
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



-- frequency distribution of holecards
-- FYI, this will be a distribution of hands held until showdown because we do not know what a player has
-- unless their hands makes it to a showdown.
select
       holecards, count(distinct handid) as freq_count 
from pokerhandhistory_showdowns36
-- where holecards like 'A_T_' or holecards like 'T_A_'
group by holecards
order by count(distinct handid) desc;


-- create a contatenated list of actions so we can do some frequency analysis on what patterns occur most frequently
drop table action_orderings;
create table action_orderings as 
select 
       handid
       , listagg(handactiontype, ',') within group (order by actionnumber) as actionorder
       , listagg(playername, ',') within group (order by actionnumber) as playeractionorder
       , listagg(seatnumber, ',') within group (order by actionnumber) as seatnumberorder
       , listagg(amount, ',') within group (order by actionnumber) as amountorder
       , listagg(num_big_blinds_in_amount, ',') within group (order by actionnumber) as num_big_blinds_in_amountorder
       , listagg(amount_pct_into_currentpot, ',') within group (order by actionnumber) as amount_pct_into_currentpotorder
       , listagg(currentpostsize, ',') within group (order by actionnumber) as currentpotsizeorder
       , listagg(num_big_blinds_in_currentpot, ',') within group (order by actionnumber) as num_big_blinds_in_currentpotorder
from pokerhandhistory_showdowns
group by handid;

-- frequency distribution of hand actions
select 
       count(distinct handid)
       , actionorder
       -- , playeractionorder
       -- , seatnumberorder
       -- , amountorder
       -- , currentpotsizeorder
from action_orderings 
group by actionorder --, playeractionorder, seatnumberorder, amountorder, currentpotsizeorder
order by count(distinct handid) desc;


-- working section .. 
select 
--       dateofhandutc
       handid
--       ,dealerbuttonposition
--       ,tablename
--       ,gamedescription
--       ,numplayersactive
--       ,numplayersseated
--       ,rake
       ,comumnitycards
       ,totalpot
       ,playername
       ,holecards
--       ,first_card
--       ,second_card
--       ,first_card_value
--       ,second_card_value
--       ,first_card_suit
--       ,second_card_suit
--       ,suited
       ,startingstack
--       ,seatnumber
       ,actionnumber
       ,amount
       ,num_big_blinds_in_amount
       ,amount_pct_into_currentpot
       ,handactiontype
       ,currentpostsize
       ,num_big_blinds_in_currentpot
       ,street

from pokerhandhistory_showdowns
where big_blind=6 -- handid = '331688942' -- in (select handid from pokerhandhistory where amount_pct_into_currentpot = 17722)
order by handid, actionnumber
limit 1000;

select distinct handactiontype from pokerhandhistory;

select handid, playername, seatnumber, actionnumber, amount, num_big_blinds_in_amount, currentpostsize, pct_of_pot, num_big_blinds_in_pot, handactiontype from pokerhandhistory_showdowns order by handid, actionnumber limit 1000;
-- end working section
