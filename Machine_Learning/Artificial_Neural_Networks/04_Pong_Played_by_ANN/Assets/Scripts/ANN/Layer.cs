using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int numNeurons;
    public List<Neuron> neurons = new List<Neuron>();

    //numberOfNeurons - how many neurons are in the layers
    //numberOfNeuronsInputs - how many inputs each neuron has (in other words,
    //the number of neurons in the previous layer)
    public Layer(int numberOfNeurons, int numberOfNeuronsInputs)
    {
        numNeurons = numberOfNeurons;
        for (int i = 0; i < numNeurons; i++)
        {
            neurons.Add(new Neuron(numberOfNeuronsInputs));
        }
    }
}
