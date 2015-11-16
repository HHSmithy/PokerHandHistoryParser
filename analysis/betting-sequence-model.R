library(depmixS4)
if(!exists('pdata')) {
pdata = read.csv('/home/ubuntu/aohc.txt', sep = '|', header = FALSE)
# Order the data by sequence action number
# This is important otherwise models won't have the proper sequence of action and will output shit.
pdata = pdata[order(pdata$V1, pdata$V4, pdata$V7), ]
}

# Sample data and train a hidden markov model on pokerhands
msthmm = function(
		    mydata
		  , element_list
		  , seqids
		  , model=list(V5~1, V6~1, V8~1)
		  , numstates=2
		  , fam=list(multinomial(), multinomial(), multinomial())
		  , multiseries=TRUE
		) {

# Get all the hands from the originial dataset
print("Creating Validation Data...")
temp = mydata[mydata$V2 %in% element_list, ]
temp = temp[temp$V1 %in% seqids, ]

# Aggregate counts so we know what actions are part of an independant series
# We don't want the model to treat this as a single time series because each new hand play is independant
print("Creating Counts of Independant Series (Hand Sequences)...")
if(multiseries) {
iseq = aggregate(V2 ~ V1 + V4, temp, length)
iseq = iseq[order(iseq$V1, iseq$V4), ]
}
else {iseq = NULL}

print("Ordering the dataset into sequential order...")
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
fit.dmm = fit(dmm, emcontrol=em.control(maxit=20))

print("Creating Evaluation Matrix...")
eval.dmm = cbind(temp_ordered, fit.dmm@posterior)
eval.matrix = aggregate(V1 ~ state + V2, eval.dmm, length)
# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
r.object = list(eval.dmm, iseq, dmm, fit.dmm, eval.matrix)
names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
return(r.object)
}

# Leverage parameters from one fitted HMM model to optimize another HMM model
blendHMM = function(
	  mydata
	, fittedHmmToBlend
	, handsFromHmmToBlend
	# Below are parameters for the new HMM model
	, seqids
	, model=list(V5~1, V6~1, V8~1)
	, numstates=2
	, fam=list(multinomial(), multinomial(), multinomial())
	, startresp=NULL
	, starttr=NULL
	, startinit=NULL
	, multiseries=TRUE
	, fixedhand='AKo'
	, numhands=3
	, fix=NULL) {
	  	print("Creating Validation Data..")

		temp = mydata[mydata$V1 %in% seqids, ]

		hands = unique(as.character(temp$V2))
		hands= c(fixedhand, sample(hands, numhands))
		hands = sort(hands)

		temp = temp[temp$V2 %in% hands, ]

		print("Extracting Parameters for fixed hands...")
		# Extract parameters (transition / emission) probabilities for the hand we want to fix
		pars=getpars(fittedHmmToBlend)
		totalparams = npar(fittedHmmToBlend)
		initialparams = fittedHmmToBlend@nstates+(fittedHmmToBlend@nstates**2) + 1
		responseinits = pars[initialparams:totalparams]
		idx = which(fixedhand == sort(handsFromHmmToBlend))
		handpars = responseinits[seq(idx, length(responseinits), idx)]
		helper = function(x, y) { return(c(rep(runif(1), idx-1), x, rep(runif(1), length(hands)-idx))) }
		responseinitsfinal = unlist(lapply(handpars, function(z) helper(z,length(hands))))

		# Aggregate counts so we know what actions are part of an independant series
		# We don't want the model to treat this as a single time series because each new hand play is independant
		print("Creating Counts of Independant Series (Hand Sequences)...")
		if(multiseries) {
		iseq = aggregate(V2 ~ V1 + V4, temp, length)
		iseq = iseq[order(iseq$V1, iseq$V4), ]
		}
		else {iseq = NULL}

		# Ensure the data is ordered properly by handid, playerid, and actionnumber
		print("Ordering Hands...")
		temp_ordered = temp[order(temp$V1, temp$V4, temp$V7), ]

		print("Creating Blended HMM Model...")
		bhmm = depmix(response=model
			         , data=temp_ordered
				 , nstates=numstates
				 , respstart=exp(responseinitsfinal) / (1+exp(responseinitsfinal))
				 , trstart=pars[numstates:numstates**2]
				 , instart=pars[1:numstates]
				 , ntimes=iseq[,3]
				 , family=fam)

		print("Fitting Parameters to Model...")
		fit.bhmm = fit(bhmm, verbose=TRUE, emcontrol=em.control(maxit=20), fixed=fix)

		print("Creating Evaluation Matrix...")
		eval.bhmm = cbind(temp_ordered, fit.bhmm@posterior)
		eval.matrix = aggregate(V1 ~ state + V2, eval.bhmm, length)
		# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
		r.object = list(eval.bhmm, iseq, bhmm, fit.bhmm, eval.matrix)
		names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
		return(r.object)
	}
