library(tm)
library(HMM)
library(NLP)

split_on_comma = function(s) { strsplit(s, ',') }

d = read.csv('/home/ubuntu/PokerHandHistoryParser/analysis/action-ordering-data/action_ordering.csv', sep="|", header=FALSE)
names(d) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")

# Split betting actions into a format that can be read by pattern mining algorithm
d_actions = apply(d[, 1:2], c(1,2), split_on_comma)
#d2 = apply(d, c(1,2), split_on_comma)
#names(d2) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")

# d_actions_string = concat(unlist(d_actions[, 2]), collapse=' ')
d_actions_2grams = ngrams(unlist(d_actions[, 2]), 2L)
d_actions_2grams = vapply(d_actions_2grams, paste, "", collapse= " ")

d_actions_3grams = ngrams(unlist(d_actions[, 2]), 3L)
d_actions_3grams = vapply(d_actions_3grams, paste, "", collapse= " ")
