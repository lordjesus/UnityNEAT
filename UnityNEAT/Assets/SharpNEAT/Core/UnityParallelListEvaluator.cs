using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;

namespace SharpNEAT.Core
{
    class UnityParallelListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {

        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        //readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;
        Optimizer _optimizer;

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
        /// </summary>
        public UnityParallelListEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                         IPhenomeEvaluator<TPhenome> phenomeEvaluator,
                                          Optimizer opt)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _optimizer = opt;
        }

        #endregion

        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        public IEnumerator Evaluate(IList<TGenome> genomeList)
        {
            yield return Coroutiner.StartCoroutine(evaluateList(genomeList));
        }

        private IEnumerator evaluateList(IList<TGenome> genomeList)
        {
            Dictionary<TGenome, TPhenome> dict = new Dictionary<TGenome, TPhenome>();
            Dictionary<TGenome, FitnessInfo[]> fitnessDict = new Dictionary<TGenome, FitnessInfo[]>();
            for (int i = 0; i < _optimizer.Trials; i++)
            {
                Utility.Log("Iteration " + (i + 1));                
                _phenomeEvaluator.Reset();
                dict = new Dictionary<TGenome, TPhenome>();
                foreach (TGenome genome in genomeList)
                {
                    
                    TPhenome phenome = _genomeDecoder.Decode(genome);
                    if (null == phenome)
                    {   // Non-viable genome.
                        genome.EvaluationInfo.SetFitness(0.0);
                        genome.EvaluationInfo.AuxFitnessArr = null;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            fitnessDict.Add(genome, new FitnessInfo[_optimizer.Trials]);
                        }
                        dict.Add(genome, phenome);
                        //if (!dict.ContainsKey(genome))
                        //{
                        //    dict.Add(genome, phenome);
                        //    fitnessDict.Add(phenome, new FitnessInfo[_optimizer.Trials]);
                        //}
                        Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(phenome));


                    }
                }

                yield return new WaitForSeconds(_optimizer.TrialDuration);
                
                foreach (TGenome genome in dict.Keys)
                {
                    TPhenome phenome = dict[genome];
                    if (phenome != null)
                    {

                        FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness(phenome);
                        
                        fitnessDict[genome][i] = fitnessInfo;
                    }
                }
            }
            foreach (TGenome genome in dict.Keys)
            {
                TPhenome phenome = dict[genome];
                if (phenome != null)
                {
                    double fitness = 0;

                    for (int i = 0; i < _optimizer.Trials; i++)
                    {
                     
                        fitness += fitnessDict[genome][i]._fitness;
                       
                    }
                    var fit = fitness;
                    fitness /= _optimizer.Trials; // Averaged fitness
                    
                    if (fit > _optimizer.StoppingFitness)
                    {
                      //  Utility.Log("Fitness is " + fit + ", stopping now because stopping fitness is " + _optimizer.StoppingFitness);
                      //  _phenomeEvaluator.StopConditionSatisfied = true;
                    }
                    genome.EvaluationInfo.SetFitness(fitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessDict[genome][0]._auxFitnessArr;
                }
            }
        }

        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }
    }
}
