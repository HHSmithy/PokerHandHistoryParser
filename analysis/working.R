starttime = proc.time()
source('betting-sequence-model.R')
if(!exists('AK')) {
    print("AK")
    AK = sthmm(element_list=c('AKo'))
}

if(!exists('AKr')) {
    print("AKr")		   
    AKr = sthmm(element_list=c('AKo', '27o', '55o', '5Ao'), model=list(V2~V5, V2~V6, V2~V8), numstates=2, fam=list(multinomial(), multinomial(), multinomial()))
}

if(!exists('AKr10')){
    print("AKr10")
    AKr10 = sthmm(element_list=c('AKo', as.character(sample(unique(pdata$V2), 9))), model=list(V2~V5, V2~V6, V2~V8), numstates=2, fam=list(multinomial(), multinomial(), multinomial()))
}

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


if(!exists('AKf')){
print("AKf")
AKf = blendHMM(
  fittedHmmToBlend=AKr$fit.dmm
  , handsFromHmmToBlend=c('AKo', '27o', '55o', '5Ao')
  , model=list(V2~V5, V2~V6, V2~V8)
  , numhands=9)
  # State 2 is what we want to fix since in AKr model above is was accurate on AKo
#  , startresp = exp(responseinitsfinal) / (1+exp(responseinitsfinal))
#  , starttr = pars[3:6]
#  , startinit = pars[1:2]
#  , fix=c(rep(1, skipnum-1), rep(1,14*length(hands)), rep(0, 14*length(hands))))
}

if(!exists('AK5')){
print("AK5")
AK5 = blendHMM(
  fittedHmmToBlend=AKr$fit.dmm
  , element_list = hands
  , numstates=2
  , model=list(V5~1, V6~1, V8~1)
  , fix=NULL)
}

stoptime = proc.time()
print(stoptime - starttime)