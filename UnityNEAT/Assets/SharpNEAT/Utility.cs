using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using SharpNeat.Genomes.Neat;
using System.Xml;
using System;
using SharpNeat.Domains;

public class Utility : MonoBehaviour {

    public static bool DebugLog = false;

	/// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static void Log(string message)
    {
        if (DebugLog)
        {
            Debug.Log(message);
        }
    }

    public static float DeltaTime()
    {
        return Time.deltaTime;
    }

    public static float GetDistance(Vector3 A, Vector3 B)
    {
        if (A != null && B != null)
        {
            Vector2 a = new Vector2(A.x, A.z);
            Vector2 b = new Vector2(B.x, B.z);
            return Vector2.Distance(a, b);
        }
        else
        {
            if (A == null)
            {
                throw new ArgumentNullException("GameObject A is null");
            }
            else
            {
                throw new ArgumentNullException("GameObject B is null");
            }
        } 
    }

    public static float GetDistance(GameObject A, GameObject B)
    {
        if (A != null && B != null)
        {
            Vector2 a = new Vector2(A.transform.position.x, A.transform.position.z);
            Vector2 b = new Vector2(B.transform.position.x, B.transform.position.z);
            return Vector2.Distance(a, b);
        }
        else
        {
            if (A == null)
            {
                throw new ArgumentNullException("GameObject A is null");
            }
            else
            {
                throw new ArgumentNullException("GameObject B is null");
            }
        }
        return 0.0f;
    }

    /// <summary>
    /// Clamps a value between 0 and 1
    /// </summary>
    /// <param name="val">value to clamp</param>
    /// <returns>clamped value between 0 and 1</returns>
    public static float Clamp(float val)
    {
        return Clamp(val, 0, 1);
    }

    /// <summary>
    /// Clamps a value between specfified min and max
    /// </summary>
    /// <param name="val">Value to clamp</param>
    /// <param name="min">Minimum clamped value</param>
    /// <param name="max">Maximum clamped value</param>
    /// <returns></returns>
    public static float Clamp(float val, float min, float max)
    {
        if (val < 0)
        {
            return 0;
        }
        if (val > 1)
        {
            return 1;
        }
        return val;
    }

    public static float GenerateNoise(float threshold)
    {
        return UnityEngine.Random.Range(-threshold, threshold);
    }

    //public static IBlackBox LoadBrain(string filePath)
    //{
    //    OptimizationExperiment experiment = new OptimizationExperiment();
    //    XmlDocument xmlConfig = new XmlDocument();
    //    TextAsset textAsset = (TextAsset)Resources.Load("phototaxis.config");
    //    //      xmlConfig.Load(OptimizerParameters.ConfigFile);
    //    xmlConfig.LoadXml(textAsset.text);
    // //   experiment.SetOptimizer(this);
    //    experiment.Initialize(OptimizerParameters.Name, xmlConfig.DocumentElement, OptimizerParameters.NumInputs, OptimizerParameters.NumOutputs);
    //    return LoadBrain(filePath, experiment);
    //}

    public static IBlackBox LoadBrain(string filePath, INeatExperiment experiment)
    {
        NeatGenome genome = null;


        // Try to load the genome from the XML document.
        try
        {
            using (XmlReader xr = XmlReader.Create(filePath))
                genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];
        }
        catch (Exception e1)
        {
            print(filePath + " Error loading genome from file!\nLoading aborted.\n"
                                      + e1.Message + "\nJoe: " + filePath);
            return null;
        }

        // Get a genome decoder that can convert genomes to phenomes.
        var genomeDecoder = experiment.CreateGenomeDecoder();

        // Decode the genome into a phenome (neural network).
        var phenome = genomeDecoder.Decode(genome);

        return phenome;
    }
}
