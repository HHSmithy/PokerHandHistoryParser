starttime = proc.time()
source('betting-sequence-model.R')

handids = unique(pdata$V1)
pdata.train.handids = sample(handids, length(handids) * .6)
pdata.val.handids = handids[!handids %in% pdata.train.handids]

# Load Monitoring Data
if(!exists('mdata')){
mdata = read.csv('./monitoring.txt', sep = '|', header = FALSE)
mdata$V1 = as.character(mdata$V1)
print(head(mdata))
}

if(!exists('Mr')) {
    print("Mr")		   
    Mr = sthmm(mdata
	      , element_list=c('0532d304f8383d6520c4a5f0a230587397b054ae', '0b921e64d729de16c470563f567ef0382457beb5')
	      , seqids = c('0532d304f8383d6520c4a5f0a230587397b054ae', '0b921e64d729de16c470563f567ef0382457beb5')
    	      , model=list(V12~1)
	      , numstates=2
	      , fam=list(gaussian())
	      , multiseries=FALSE
	  )
}


if(!exists('AK')) {
    print("AK")
    AK = sthmm(pdata
		, element_list=c('AKo', '27o')
		, seqids=pdata.train.handids
		, model=list(V2~1, V5~1, V6~1, V8~1)
		, numstates=2
		, fam=list(multinomial(), multinomial(), multinomial(), multinomial())
	      )
}

if(!exists('AKr')) {
    print("AKr")		   
    AKr = sthmm(pdata
	      , element_list=c('AKo', '27o')
	      , seqids = pdata.train.handids
    	      , model=list(V2~V5, V2~V6, V2~V8)
	      , numstates=2
	      , fam=list(multinomial(), multinomial(), multinomial())
	  )
}


if(!exists('AKf')){
print("AKf")
AKf = blendHMM(pdata
  , fittedHmmToBlend=AKr$fit.dmm
  , handsFromHmmToBlend=c('AKo', '27o')
  , seqids = pdata.val.handids
  , model=list(V2~V5, V2~V6, V2~V8)
  , numhands=4)
  # State 2 is what we want to fix since in AKr model above is was accurate on AKo
#  , startresp = exp(responseinitsfinal) / (1+exp(responseinitsfinal))
#  , starttr = pars[3:6]
#  , startinit = pars[1:2]
#  , fix=c(rep(1, skipnum-1), rep(1,14*length(hands)), rep(0, 14*length(hands))))
}


# if(!exists('AKr10')){
#     print("AKr10")
#     AKr10 = sthmm(pdata, element_list=c('AKo', as.character(sample(unique(pdata$V2), 9))), model=list(V2~V5, V2~V6, V2~V8), numstates=2, fam=list(multinomial(), multinomial(), multinomial()))
# }

stoptime = proc.time()
print(stoptime - starttime)


# blendHMM = function(
# 	fittedHmmToBlend
# 	# Below are parameters for the new HMM model
# 	, model=list(V5~1, V6~1, V8~1)
#   , element_list
# 	, numstates=2
# 	, fam=list(multinomial(), multinomial(), multinomial())
# 	, startresp=NULL
# 	, starttr=NULL
# 	, startinit=NULL
# 	, multiseries=TRUE
# 	, fix
# 	, numhands) {


