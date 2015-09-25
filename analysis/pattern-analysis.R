split_on_comma = function(s) { strsplit(s, ',') }
d = read.csv('/home/ubuntu/action_orderings/action_ordering.csv', header=FALSE, sep='|', stringsAsFactors = FALSE)
names(d) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")
d2 = data.frame(lapply(d[, 2:ncol(d)], split_on_comma))
names(d2) = c("handid","actionorder","seatnumberorder","amountorder","num_big_blinds_in_amountorder","amount_pct_into_currentpotorder","num_big_blinds_in_currentpotorder")
