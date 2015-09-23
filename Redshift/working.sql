-- create a table for only hands which have a showdown.  We only care to analyze patterns from these hands since
-- they will have hole cards. 
drop table pokerhandhistory_showdowns;
create table pokerhandhistory_showdowns as (
select
       dateofhandutc
       ,handid
       ,dealerbuttonposition
       ,tablename
       ,gamedescription
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
order by handid);


-- frequency distribution of holecards
-- FYI, this will be a distribution of hands held until showdown because we do not know what a player has
-- unless their hands makes it to a showdown.
select 
       holecards, count(distinct handid) as freq_count 
from pokerhandhistory 
-- where holecards like 'A_T_' or holecards like 'T_A_'
group by holecards
order by count(distinct handid) desc;



