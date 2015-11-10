library(depmixS4)
if(!exists('pdata')) {
pdata = read.csv('/home/ubuntu/aohc.txt', sep = '|', header = FALSE)
# Order the data by sequence action number
# This is important otherwise models won't have the proper sequence of action and will output shit.
pdata = pdata[order(pdata$V1, pdata$V4, pdata$V7), ]
# Make a sample dataset with subset of hands for a 2 state HMM.
# Patterns that "look like" player is holding AK or they are NOT holding AK
pdataAK = pdata[pdata$V2 %in% c('AKo', '27o'), ]
}

# > head(pdata_ordered)
#                                               V1  V2         V3
# 512050  000036366e5344b5b24baa44b989aea35d800578 4Ao 8cKs9d9sTc
# 1134393 000036366e5344b5b24baa44b989aea35d800578 4Ao 8cKs9d9sTc
# 237601  000036366e5344b5b24baa44b989aea35d800578 4Ao 8cKs9d9sTc
# 1794271 000036366e5344b5b24baa44b989aea35d800578 4Ao 8cKs9d9sTc
# 1062592 000036366e5344b5b24baa44b989aea35d800578 4Ao 8cKs9d9sTc
# 1990632 000036366e5344b5b24baa44b989aea35d800578 8Qo 8cKs9d9sTc
#                                               V4          V5      V6 V7    V8
# 512050  7a10747e52a7e1c2ae191ab84c8eaf9c2e24110f   BIG_BLIND Preflop  2 SMALL
# 1134393 7a10747e52a7e1c2ae191ab84c8eaf9c2e24110f        CALL Preflop  4 SMALL
# 237601  7a10747e52a7e1c2ae191ab84c8eaf9c2e24110f       CHECK    Flop  5 SMALL
# 1794271 7a10747e52a7e1c2ae191ab84c8eaf9c2e24110f       CHECK    Turn  7 SMALL
# 1062592 7a10747e52a7e1c2ae191ab84c8eaf9c2e24110f       CHECK   River  9 SMALL
# 1990632 f72089770df258345acc3412c14205fc60586507 SMALL_BLIND Preflop  1 SMALL

# Let the model know we have multiple independant sequences in this dataset
# we treat each players sequence as independant, this isn't the greatest assumption, however,
# for recurring behavior this is OK to make.
if(!exists('ntimes')) { 
ntimes = aggregate(V2 ~ V1 + V4, pdataAK_ordered, length)
ntimes = ntimes[order(ntimes$V1, ntimes$V4), ]
}
# create a multivariate HMM -- 2 states since we only care about AKo .. either you got it or you don't.
if(!exists('dmmR')) { dmmR = depmix(list(V2~V5, V2~V6, V2~V8), data=pdataAK_ordered, nstates=2, ntimes=ntimes[,3], family = list(multinomial(), multinomial(), multinomial())) }

# Optimize the HMM parameters
if(!exists('fit.dmmR')) { fit.dmmR = fit(dmmR) }