install.packages(c('Matrix', 'HMM', 'ngram'))
library(HMM)
library(ngram)
split_on_comma = function(s) { strsplit(s, ',') }

d = read.transactions('/home/ubuntu/PokerHandHistoryParser/analysis/action-ordering-data/action_ordering_basketformat.tsv', format="basket", sep="|")
names(d) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")

# Split betting actions into a format that can be read by pattern mining algorithm
d_actions = apply(d[, 1:2], c(1,2), split_on_comma)
#d2 = apply(d, c(1,2), split_on_comma)
#names(d2) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")

d_actions_string = concat(unlist(d_actions[, 2]), collapse=' ')
d_actions_2grams = ngram(d_actions_string, n=2)
d_actions_3grams = ngram(d_actions_string, n=3)
d_actions_4grams = ngram(d_actions_string, n=4)

write(d_actions_2grams, file='2grams.txt')
