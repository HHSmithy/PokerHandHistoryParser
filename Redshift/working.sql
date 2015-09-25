
-- working section .. 

-- frequency distribution of holecards
-- FYI, this will be a distribution of hands held until showdown because we do not know what a player has
-- unless their hands makes it to a showdown.
select
       holecards, count(distinct handid) as freq_count 
from pokerhandhistory_showdowns36
-- where holecards like 'A_T_' or holecards like 'T_A_'
group by holecards
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
order by count(distinct handid) desc;

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
-- end working section
