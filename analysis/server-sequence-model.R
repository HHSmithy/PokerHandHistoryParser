# Load Monitoring Data
if(!exists('mdata')){
mdata = read.csv('./monitoring.txt', sep = '|', header = FALSE)
mdata$V1 = as.character(mdata$V1)
print(head(mdata))
}


msthmm = function(
		    mydata
		  , element_list
		  , seqids
		  , model
		  , numstates=2
		  , fam
		  , multiseries=TRUE
		) {

# Get all the servers from the originial dataset
print("Creating Validation Data...")
#mdata[is.na(mdata$V12)] = 0
#mdata[is.na(mdata$V11)] = 0
temp = mydata[mydata$V3 %in% seqids, ]
temp = temp[temp$V3 %in% element_list, ]

# Aggregate counts so we know what actions are part of an independant series
# We don't want the model to treat this as a single time series because each new server play is independant
print("Creating Counts of Independant Series (Server Sequences)...")
if(multiseries) {
iseq = aggregate(V3~V1, temp, length)

}
else {iseq = NULL}

print("Ordering the dataset into sequential order...")
# Ensure the data is ordered properly by serverid, playerid, and actionnumber
temp_ordered = temp[order(temp$V3, temp$V1, temp$V8), ]

print("Creating HMM Model...")
# Create a 2 state model
dmm = depmix(model
, data=temp_ordered
, nstates=numstates
, ntimes=iseq[,2]
, family = fam)

print("Training HMM Parameters...")
# Optimize the model parameters
fit.dmm = fit(dmm, emcontrol=em.control(maxit=20))

print("Creating Evaluation Matrix...")
eval.dmm = cbind(temp_ordered, fit.dmm@posterior)
eval.matrix = aggregate(V1 ~ state + V3, eval.dmm, length)
# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
r.object = list(eval.dmm, iseq, dmm, fit.dmm, eval.matrix)
names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
return(r.object)
}


# Leverage parameters from one fitted HMM model to optimize another HMM model
mblendHMM = function(
	  mydata
	, fittedHmmToBlend
	, serversFromHmmToBlend
	# Below are parameters for the new HMM model
	, seqids
	, model
	, numstates=2
	, fam
	, startresp=NULL
	, starttr=NULL
	, startinit=NULL
	, multiseries=TRUE
	, fixedserver
	, numservers
	, fix=NULL) {
	  	print("Creating Validation Data..")

		temp = mydata[mydata$V3 %in% seqids, ]

		servers = as.character(unique(temp$V3))

		servers= c(fixedserver, sample(servers, numservers))
		servers = sort(servers)

		temp = temp[temp$V3 %in% servers, ]

		print("Extracting Parameters for fixed servers...")
		# Extract parameters (transition / emission) probabilities for the server we want to fix
		pars=getpars(fittedHmmToBlend)
		totalparams = npar(fittedHmmToBlend)
		initialparams = fittedHmmToBlend@nstates+(fittedHmmToBlend@nstates**2) + 1
		responseinits = pars[initialparams:totalparams]
		idx = which(fixedserver == sort(serversFromHmmToBlend))
		serverpars = responseinits[seq(idx, length(responseinits), idx)]
		helper = function(x, y) { return(c(rep(runif(1), idx-1), x, rep(runif(1), y-idx-1))) }
		responseinitsfinal = unlist(lapply(serverpars, function(z) helper(z,fittedHmmToBlend@nstates)))
		print(length(responseinitsfinal))

		# Aggregate counts so we know what actions are part of an independant series
		# We don't want the model to treat this as a single time series because each new server play is independant
		print("Creating Counts of Independant Series (Server Sequences)...")
		if(multiseries) {
		iseq = aggregate(V3~V1, temp, length)
		}
		else {iseq = NULL}

		# Ensure the data is ordered properly by serverid, playerid, and actionnumber
		print("Ordering Servers...")
		temp_ordered = temp[order(temp$V3, temp$V1, temp$V8), ]
		
		print("Creating Blended HMM Model...")
		bhmm = depmix(response=model
			         , data=temp_ordered
				 , nstates=numstates
				 , respstart=exp(responseinitsfinal) / (1+exp(responseinitsfinal))
				 , trstart=pars[numstates:numstates**2]
				 , instart=pars[1:numstates]
				 , ntimes=iseq[,2]
				 , family=fam)

		print("Fitting Parameters to Model...")
		fit.bhmm = fit(bhmm, verbose=TRUE, emcontrol=em.control(maxit=20), fixed=fix)

		print("Creating Evaluation Matrix...")
		eval.bhmm = cbind(temp_ordered, fit.bhmm@posterior)
		eval.matrix = aggregate(V1 ~ state + V3, eval.bhmm, length)

		# Return the dataset, the count of independant series, the single state model, and the fitted parameters of the model
		r.object = list(eval.bhmm, iseq, bhmm, fit.bhmm, eval.matrix)
		names(r.object) = c('eval.dataset', 'iseq', 'dmm', 'fit.dmm', 'eval.matrix')
		return(r.object)
	}
