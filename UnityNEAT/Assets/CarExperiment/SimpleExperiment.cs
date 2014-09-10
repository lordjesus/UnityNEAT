using UnityEngine;
using System.Collections;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNEAT.Core;
using System;

public class SimpleExperiment : INeatExperiment
{

    NeatEvolutionAlgorithmParameters _eaParams;
    NeatGenomeParameters _neatGenomeParams;
    string _name;
    int _populationSize;
    int _specieCount;
    NetworkActivationScheme _activationScheme;
    string _complexityRegulationStr;
    int? _complexityThreshold;
    string _description;
    Optimizer _optimizer;
    int _inputCount;
    int _outputCount;

    public string Name
    {
        get { return _name; }
    }

    public string Description
    {
        get { return _description; }
    }

    public int InputCount
    {
        get { return _inputCount; }
    }

    public int OutputCount
    {
        get { return _outputCount; }
    }

    public int DefaultPopulationSize
    {
        get { return _populationSize; }
    }

    public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
    {
        get { return _eaParams; }
    }

    public NeatGenomeParameters NeatGenomeParameters
    {
        get { return _neatGenomeParams; }
    }

    public void SetOptimizer(Optimizer se)
    {
        this._optimizer = se;
    }


    public void Initialize(string name, XmlElement xmlConfig)
    {
        Initialize(name, xmlConfig, 6, 3);
    }

    public void Initialize(string name, XmlElement xmlConfig, int input, int output)
    {
        _name = name;
        _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
        _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
        _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
        _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
        _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
        _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");

        _eaParams = new NeatEvolutionAlgorithmParameters();
        _eaParams.SpecieCount = _specieCount;
        _neatGenomeParams = new NeatGenomeParameters();
        _neatGenomeParams.FeedforwardOnly = _activationScheme.AcyclicNetwork;

        _inputCount = input;
        _outputCount = output;
    }

    public List<NeatGenome> LoadPopulation(XmlReader xr)
    {
        NeatGenomeFactory genomeFactory = (NeatGenomeFactory)CreateGenomeFactory();
        return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
    }

    public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
    {
        NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
    }

    public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
    {
        return new NeatGenomeDecoder(_activationScheme);
    }

    public IGenomeFactory<NeatGenome> CreateGenomeFactory()
    {
        return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(string fileName)
    {
        List<NeatGenome> genomeList = null;
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();
        try
        {
            if (fileName.Contains("/.pop.xml"))
            {
                throw new Exception();
            }
            using (XmlReader xr = XmlReader.Create(fileName))
            {
                genomeList = LoadPopulation(xr);
            }
        }
        catch (Exception e1)
        {
            Utility.Log(fileName + " Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message + "\nJoe: " + fileName);

            genomeList = genomeFactory.CreateGenomeList(_populationSize, 0);

        }



        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
    {
        return CreateEvolutionAlgorithm(_populationSize);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
    {
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

        List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
    {
        IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
        ISpeciationStrategy<NeatGenome> speciationStrategy = new KMeansClusteringStrategy<NeatGenome>(distanceMetric);

        IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

        NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

        // Create black box evaluator       
        SimpleEvaluator evaluator = new SimpleEvaluator(_optimizer);

        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = CreateGenomeDecoder();


        IGenomeListEvaluator<NeatGenome> innerEvaluator = new UnityParallelListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, evaluator, _optimizer);

        IGenomeListEvaluator<NeatGenome> selectiveEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(innerEvaluator,
            SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

        //ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
        ea.Initialize(innerEvaluator, genomeFactory, genomeList);

        return ea;
    }
}