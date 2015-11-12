library(depmixS4)
if(!exists('pdata')) {
pdata = read.csv('/home/ubuntu/aohc.txt', sep = '|', header = FALSE)
# Order the data by sequence action number
# This is important otherwise models won't have the proper sequence of action and will output shit.
pdata = pdata[order(pdata$V1, pdata$V4, pdata$V7), ]
}

# Sample data and train a hidden markov model on pokerhands
sthmm = function(hand_list, model=list(V5~1, V6~1, V8~1), numstates=2, fam=list(multinomial(), multinomial(), multinomial()), multiseries=TRUE) {
	# Get all the hands from the originial dataset
temp = pdata[pdata$V2 %in% hand_list, ]

# Aggregate counts so we know what actions are part of an independant series
# We don't want the model to treat this as a single time series because each new hand play is independant
if(multiseries) {
iseq = aggregate(V2 ~ V1 + V4, temp, length)
iseq = iseq[order(iseq$V1, iseq$V4), ]
}
else {iseq = NULL}

# Ensure the data is ordered properly by handid, playerid, and actionnumber
temp_ordered = temp[order(temp$V1, temp$V4, temp$V7), ]

print("Creating HMM Model...")
# Create a 2 state model
dmm = depmix(model
, data=temp_ordered
, nstates=numstates
, ntimes=iseq[,3]
, family = fam)

print("Training HMM Parameters...")
# Optimize the model parameters
fit.dmm = fit(dmm)

print("Creating Evaluation Matrix...")
eval.dmm = cbind(temp_ordered, fit.dmm@posterior)
eval.matrix = aggregate(V1 ~ state + V2, eval.dmm, length)
# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
r.object = list(eval.dmm, iseq, dmm, fit.dmm, eval.matrix)
names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
return(r.object)
}

if(!exists('AK')) {
# Make a sample dataset with subset of hands for a 2 state HMM.
# Patterns that "look like" player is holding AK or they are NOT holding AK
# pdataAK = pdata[pdata$V2 %in% c('AKo'), ]
# pdataAK = pdataAK[order(pdataAK$V1, pdataAK$V4, pdataAK$V7), ]
AK = stss(c('AKo'))
}
# > head(pdataAK)
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
if(!exists('iseq')) {
iseq = aggregate(V2 ~ V1 + V4, pdataAK, length)
iseq = iseq[order(iseq$V1, iseq$V4), ]
}

# if(!exists('iseq_all')){
# print("Aggregating ..")
# iseq_all = aggregate(V2 ~ V1 + V4, pdata, length)
# print("Sorting..")
# iseq_all = iseq_all[order(iseq_all$V1, iseq_all$V4), ]
# }

# create a multivariate HMM -- 2 states since we only care about AKo .. either you got it or you don't.
if(!exists('dmmR')) { dmmR = depmix(list(V2~V5, V2~V6, V2~V8), data=pdataAK, nstates=2,
		      	     ntimes=iseq[,3],
			     family = list(multinomial(), multinomial(), multinomial())) }


# dmmR3 = depmix(list(V2~V5, V2~V6, V2~V8)
#                , data=pdataAK
# 	       , nstates=2
# 	       ,family = list(multinomial(), multinomial(), multinomial()))

# fit.dmmR3 = fit(dmmR3)

if(!exists('dmmR2')) {
dmmR2 = depmix(list(V5~1, V6~1, V8~1)
, data=pdataAK
, nstates=1
, ntimes=iseq[,3]
, family = list(multinomial(), multinomial(), multinomial()))

fit.dmmR2 = fit(dmmR2)
}

# Optimize the HMM parameters
if(!exists('fit.dmmR')) { fit.dmmR = fit(dmmR) }

# Use the parameters from previous training to see how the model holds when we add more hands.
# pdatasampled = pdata[pdata$V2 %in% c('AKo', '8Ao', 'JTs'), ]
# pdatasampled = pdatasampled[order(pdatasampled$V1, pdatasampled$V4, pdatasampled$V7), ]
# pars = getpars(fit.dmmR2)
# responseinits = pars[7:npar(fit.dmmR2)]
# flags = c(unlist(getpars(fit.dmmR2, "fixed")))
# filter = flags[7:npar(fit.dmmR2)]
# responseinits_filter = responseinits[!filter]
# helper = function(x) { return(c(x, runif(2))) }
# responseinitsfinal = c(unlist(lapply(responseinits_filter, helper)))
# names(responseinitsfinal) = NULL

# Leverage parameters from one fitted HMM model to optimize another HMM model
blendHMM = function(
	fittedHmmToBlend
	# Below are parameters for the new HMM model
	, model=list(V5~1, V6~1, V8~1)
  , hand_list
	, numstates=2
	, fam=list(multinomial(), multinomial(), multinomial())
	, startresp=NULL
	, starttr=NULL
	, startinit=NULL
	, multiseries=TRUE
	, fix) {

		temp = pdata[pdata$V2 %in% hand_list, ]

		# Aggregate counts so we know what actions are part of an independant series
		# We don't want the model to treat this as a single time series because each new hand play is independant
		if(multiseries) {
		iseq = aggregate(V2 ~ V1 + V4, temp, length)
		iseq = iseq[order(iseq$V1, iseq$V4), ]
		}
		else {iseq = NULL}

		# Ensure the data is ordered properly by handid, playerid, and actionnumber
		temp_ordered = temp[order(temp$V1, temp$V4, temp$V7), ]

		print("Creating Blended HMM Model...")
		bhmm = depmix(model
			         , data=temp_ordered
				 , nstates=numstates
				 , respstart=startresp
				 , trstart=starttr
				 , instart=startinit
				 , family=fam

		print("Fitting Parameters to Model...")
		fit.bhmm = fit(bhmm, emcontrol=em.control(rand=FALSE, maxit=20), equal=fix)

		print("Creating Evaluation Matrix...")
		eval.bhmm = cbind(temp_ordered, fit.bhmm@posterior)
		eval.matrix = aggregate(V1 ~ state + V2, eval.bhmm, length)
		# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
		r.object = list(eval.bhmm, iseq, bhmm, fit.bhmm, eval.matrix)
		names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
		return(r.object)
	}
