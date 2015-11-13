if(!exists('AK')) AK = sthmm(hand_list=c('AKo'))
if(!exists('AKr')) AKr = sthmm(hand_list=c('AKo', '27o', '55o', '5Ao'), model=list(V2~V5, V2~V6, V2~V8), numstates=2, fam=list(multinomial(), multinomial(), multinomial()))

# blendHMM = function(
# 	fittedHmmToBlend
# 	# Below are parameters for the new HMM model
# 	, model=list(V5~1, V6~1, V8~1)
#   , hand_list
# 	, numstates=2
# 	, fam=list(multinomial(), multinomial(), multinomial())
# 	, startresp=NULL
# 	, starttr=NULL
# 	, startinit=NULL
# 	, multiseries=TRUE
# 	, fix
# 	, numhands) {

pars=getpars(AKr$fit.dmm)
AKpars = pars[seq(10,npar(AKr$fit.dmm),4)]
skipnum = (AKr$fit.dmm@nresp * AKr$fit.dmm@nstates) + 1
responseinits = pars[skipnum:npar(AKr$fit.dmm)]
hands= c('AKo', as.character(sample(unique(pdata$V2), 5)))
idx = which('AKo' == sort(hands))
helper = function(x, y) { return(c(rep(runif(1), idx-1), x, rep(runif(1), length(hands)-idx))) }
responseinitsfinal = unlist(lapply(AKpars, function(z) helper(z,length(hands))))


if(!exists('AKf')){
AKf = blendHMM(
  fittedHmmToBlend=AKr$fit.dmm
  , hand_list = hands
  , model=list(V2~V5, V2~V6, V2~V8)
  # State 2 is what we want to fix since in AKr model above is was accurate on AKo
  , startresp = exp(responseinitsfinal) / (1+exp(responseinitsfinal))
  , starttr = pars[3:6]
  , startinit = pars[1:2]
  , fix=c(rep(1, skipnum-1), rep(1,14*length(hands)), rep(0, 14*length(hands))))
}

print("AK15")
AK5 = blendHMM(
  fittedHmmToBlend=AKr$fit.dmm
  , hand_list = hands
  , model=list(V5~1, V6~1, V8~1)
  # State 2 is what we want to fix since in AKr model above is was accurate on AKo
  , startresp = exp(AKpars) / (1+exp(AKpars))
  , starttr = c(.5, .5, 0.95, 0.05)
  , startinit = pars[1:2]
  , fix=c(rep(1, skipnum-1), rep(1,14), rep(0, 14)))
