
-- working section .. 

-- frequency distribution of holecards
-- FYI, this will be a distribution of hands held until showdown because we do not know what a player has
-- unless their hands makes it to a showdown.
select
	count(distinct handid) as freq_count 
from pokerhandhistory_showdowns
where holecards like 'A_T_' or holecards like 'T_A_'
order by count(distinct handid) desc;


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
order by count(distinct handid) desc
limit 100;

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
       ,pct_of_starting_stack
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

create table temp_extract as select handid, actionorder, seatnumberorder, pct_of_starting_stackorder, num_big_blinds_in_amountorder, amount_pct_into_currentpotorder, currentpotsizeorder, num_big_blinds_in_currentpotorder from action_orderings;

-- look at hand by amount won, their frequency "count" and amountwon/hand
select 
       holecards_simple
       , round(avg(avgpctofstackatrisk)) as avgpctofstackatrisk
       , round((sum(winamount) / sum(avgstartingstack)) * 100) as avgpctofstackwon
       , round(avg(winamount)) as avgwin
       , round(case when avg(avglossamout) = 0 then 1 else avg(avglossamout) end) as avgloss
       , round((round(avg(avgpctofstackatrisk)) / round((sum(winamount) / sum(avgstartingstack)) * 100)), 2) as risktorewardratio
       , round(sum(winamount) / round(case when sum(avglossamout) = 0 then 1 else sum(avglossamout) end), 2) as winlossratio
       , sum(winamount) as totalwinnings
       , round(case when sum(avglossamout) = 0 then 1 else sum(avglossamout) end) as totalloss
       , sum("count") as freq
--       , round(sum(winamount) / sum("count")) as winningsperhand
from holecards_by_actiontype_features 
group by holecards_simple --, avglossamout
order by round(avg(winamount) / case when avg(avglossamout) = 0 then 1 else avg(avglossamout) end, 2) desc;

select 
       holecards_simple
       ,street
       ,handactiontype
       ,num_big_blinds_in_amount
       ,count(handid)
from pokerhandhistory_showdowns
where handactiontype not in ('MUCKS', 'WINS', 'UNCALLED_BET', 'FOLD', 'SHOW') and holecards_simple != 's' and isallin = 'False' and handactiontype not in ('BIG_BLIND', 'SMALL_BLIND')
group by holecards_simple, num_big_blinds_in_amount, handactiontype, street
having count(handid) > 9
order by holecards_simple, count(handid) asc,  street, handactiontype
limit 1000;
-- end working section


-- create a contatenated list of actions so we can do some frequency analysis on what patterns occur most frequently
drop table aohc;
create table aohc as
select 
       handid
       , holecards_simple
       , comumnitycards
       , listagg(handactiontype || ':' || street, ',') within group (order by actionnumber) as actionorder
       , listagg(currenthandrank, ',') within group (order by actionnumber) as currenthandrankorder
       , listagg(amount, ',') within group (order by actionnumber) as amountorder
       , listagg(pct_of_starting_stack, ',') within group (order by actionnumber) as pct_of_starting_stackorder
       , listagg(num_big_blinds_in_amount, ',') within group (order by actionnumber) as num_big_blinds_in_amountorder
       , listagg(amount_pct_into_currentpot, ',') within group (order by actionnumber) as amount_pct_into_currentpotorder
       , listagg(currentpostsize, ',') within group (order by actionnumber) as currentpotsizeorder
       , listagg(num_big_blinds_in_currentpot, ',') within group (order by actionnumber) as num_big_blinds_in_currentpotorder
       , listagg(outs, ',') within group (order by actionnumber) as outsorder
from pokerhandhistory_showdowns
group by holecards_simple, handid, comumnitycards;
