using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using System.Collections;
using UnityEngine;

namespace SharpNEAT.Core
{
    class UnityListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
	{
        
        readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        readonly IPhenomeEvaluator<TPhenome> _phenomeEvaluator;

        #region Constructor

        /// <summary>
        /// Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
        /// </summary>
        public UnityListEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
                                         IPhenomeEvaluator<TPhenome> phenomeEvaluator)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
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
                    yield return Coroutiner.StartCoroutine(_phenomeEvaluator.Evaluate(phenome));

                    FitnessInfo fitnessInfo = _phenomeEvaluator.GetLastFitness(phenome);

                    genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                    genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                }
            }
        }

        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }
    }
}
