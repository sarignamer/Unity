using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHidden;
    public int numNeuronsPerHidden;
    public double alpha; //how much percentage each training set is going to affect the ANN
    List<Layer> layers = new List<Layer>();

    public ANN(int numberOfInputs,
               int numberOfOutputs,
               int numberOfHiddenLayers,
               int numberOfNeuronsPerHidden,
               double alpha)
    {
        numInputs = numberOfInputs;
        numOutputs = numberOfOutputs;
        numHidden = numberOfHiddenLayers;
        numNeuronsPerHidden = numberOfNeuronsPerHidden;
        this.alpha = alpha;

        if (numHidden > 0)
        {
            layers.Add(new Layer(numNeuronsPerHidden, numInputs));
            for (int i = 0; i < numHidden - 1; i++)
            {
                layers.Add(new Layer(numNeuronsPerHidden, numNeuronsPerHidden));
            }
            layers.Add(new Layer(numOutputs, numNeuronsPerHidden));
        }
        else
        {
            layers.Add(new Layer(numOutputs, numInputs));
        }
    }

    public List<double> Train(List<double> inputValues, List<double> desiredOutputs)
    {
        List<double> outputs = CalculateOutput(inputValues, desiredOutputs);
        UpdateWeights(outputs, desiredOutputs);
        return outputs;
    }

    public List<double> CalculateOutput(List<double> inputValues, List<double> desiredOutputs)
    {
        //outputs - the output of each layer
        List<double> outputs = new List<double>();

        if (inputValues.Count != numInputs)
        {
            Debug.Log("ERROR: number of inputs must be: " + numInputs);
            return outputs;
        }

        //inputs - the inputs for each layers
        List<double> inputs = new List<double>(inputValues);
        for (int i = 0; i < numHidden + 1; i++)
        {
            //i = 0 is the input layer
            //if i > 0 then its an hidden layer and the inputs for each
            //hidden layer is the outputs of the previous layer
            if (i > 0)
            {
                inputs = new List<double>(outputs);
            }
            outputs.Clear();

            //this loop is for looping through all the of neurons of each layer
            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                //N is the total weight of each neuron calculated by multiplying each input by it's weight
                //N = i1 * w1 + i2 * w2 + .... + ik * wk - bias
                double N = 0;
                //clear all previous inputs of the neuron
                layers[i].neurons[j].inputs.Clear();

                //loop for filling up the inputs of each nureons and doing inputs * weights
                for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                {
                    //adding the current input to the the neuron
                    //reminder: inputs here is the output of the previuos layer (unless its the first
                    //layer, and then its just the inputValues)
                    layers[i].neurons[j].inputs.Add(inputs[k]);
                    //adding to N ik * wk
                    N += layers[i].neurons[j].weights[k] * inputs[k];
                }

                //here we are negating the bias instead of adding it like we
                //did in the perceptron code
                //we just need to remember to negate it again in the UpdateWeights function again
                N -= layers[i].neurons[j].bias;

                if (i == numHidden)
                {
                    layers[i].neurons[j].output = ActivationFunctionOutputLayer(N);
                }
                else
                {
                    layers[i].neurons[j].output = ActivationFunction(N);
                }

                //in the next loop itteration, the outputs will be passed as inputs
                outputs.Add(layers[i].neurons[j].output);
            }
        }

        return outputs;
    }

    private void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
    {
        double error;

        //back propagade through all the layers
        for (int i = numHidden; i >= 0; i--)
        {
            //for each neuron in layer[i]
            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                //if we are at the output layer, we can calculate the error
                if (i == numHidden)
                {
                    error = desiredOutputs[j] - outputs[j];
                    //the error gradient is how much each neuron is responsible for the general error
                    layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
                    //errorGradiant is calculated with Delta Rule: en.wikipedia.org/wiki/Delta_rule
                }
                else
                {
                    layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);
                    //errorGradientSum is the sum total of all the errors of all the neurons that are in the
                    //output of a specific neuron, or in other words, all the neurons that accepts this neuron
                    //output as input.
                    double errorGradientSum = 0;
                    for (int p = 0; p < layers[i+1].numNeurons; p++)
                    {
                        errorGradientSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];
                    }
                    layers[i].neurons[j].errorGradient *= errorGradientSum;
                }

                //update the weights for each neuron in each layer
                for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                {
                    if (i == numHidden)
                    {
                        error = desiredOutputs[j] - outputs[j];
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
                    }
                    else
                    {
                        layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
                    }
                }

                //update the bias of each neuron
                //because we negated the bias in the "Go" function:
                //N -= layers[i].neurons[j].bias;
                //we need to multiply by -1 here to negate it back
                layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
            }
        }
    }

    public string PrintWeights()
    {
        string weightsStr = "";
        foreach (Layer layer in layers)
        {
            foreach (Neuron neuron in layer.neurons)
            {
                foreach (double w in neuron.weights)
                {
                    weightsStr += w + ",";
                }
            }
        }

        return weightsStr;
    }

    public void LoadWeights(string weightsStr)
    {
        if (weightsStr == "")
        {
            return;
        }

        string[] weightsValues = weightsStr.Split(',');
        int w = 0;

        foreach (Layer layer in layers)
        {
            foreach (Neuron neuron in layer.neurons)
            {
                for (int i = 0; i < neuron.weights.Count; i++)
                {
                    neuron.weights[i] = System.Convert.ToDouble(weightsValues[w]);
                    w++;
                }
            }
        }
    }

    //for full list of activation functions:
    //see en.wikipedia.org/wiki/Activation_function
    double ActivationFunction(double value)
    {
        return TanH(value);
    }

    double ActivationFunctionOutputLayer(double value)
    {
        return TanH(value);
    }

    private double Sigmoid(double value)
    {
        double k = (double)System.Math.Exp(value);
        return k / (1.0f + k);
    }

    double TanH(double value)
    {
        return 2 * (Sigmoid(2 * value)) - 1;
    }

    double ReLU(double value)
    {
        return (value > 0) ? value : 0;
    }

    double LeakyReLU(double value)
    {
        return (value > 0) ? value : 0.01 * value;
    }

    double Step(double value) //aka binary step
    {
        return (value > 0) ? 1 : 0;
    }

    double Sinusoid(double value)
    {
        return Math.Sin(value);
    }

    double ArcTan(double value)
    {
        return Math.Atan(value);
    }

    double SoftSign(double value)
    {
        return value / (1 + Math.Abs(value));
    }
}