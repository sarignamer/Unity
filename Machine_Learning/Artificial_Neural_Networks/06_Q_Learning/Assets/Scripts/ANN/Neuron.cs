using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
    public int numInputs;
    public double bias;
    public double output;
    //errorGradient sum of all the total errors that are from the neurons that are
    //connected to this neuron output
    public double errorGradient;
    public List<double> weights = new List<double>();
    public List<double> inputs = new List<double>();

    public Neuron(int numberOfInputs)
    {
        bias = UnityEngine.Random.Range(-1f, 1f);
        numInputs = numberOfInputs;
        for (int i = 0; i < numberOfInputs; i++)
        {
            weights.Add(UnityEngine.Random.Range(-1f, 1f));
        }
    }
}
