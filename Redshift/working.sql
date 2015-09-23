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
order by handid
);

-- frequency distribution of holecards
-- FYI, this will be a distribution of hands held until showdown because we do not know what a player has
-- unless their hands makes it to a showdown.
select 
       holecards, count(distinct handid) as freq_count 
from pokerhandhistory 
-- where holecards like 'A_T_' or holecards like 'T_A_'
group by holecards
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
       ,first_card
       ,second_card
       ,first_card_value
       ,second_card_value
       ,first_card_suit
       ,second_card_suit
       ,suited
       ,startingstack
       ,seatnumber
--       ,actionnumber
       ,amount
       ,handactiontype
       ,currentpostsize
       ,street

from pokerhandhistory_showdowns
where handid='321776574'
order by handid
limit 1000;

-- end working section
