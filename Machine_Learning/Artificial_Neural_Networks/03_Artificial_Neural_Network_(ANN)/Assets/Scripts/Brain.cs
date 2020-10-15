using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    ANN ann;
    double sumSquareError = 0;

    int numInputs = 2;
    int numOutputs = 1;
    int numHiddenLayars = 1;
    int numNeuronsPerHidden = 2;
    double alpha = 0.8;

    private void Start()
    {
        ann = new ANN(numInputs, numOutputs, numHiddenLayars, numNeuronsPerHidden, alpha);

        List<double> result;

        for (int i = 0; i < 10000; i++)
        {
            sumSquareError = 0;
            result = Train(1, 1, 0);
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
            result = Train(1, 0, 1);
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 1, 1);
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 0, 0);
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
        }

        Debug.Log("SSE: " + sumSquareError);

        result = Train(1, 1, 0);
        Debug.Log(" 1 1 " + result[0]);
        result = Train(1, 0, 1);
        Debug.Log(" 1 0 " + result[0]);
        result = Train(0, 1, 1);
        Debug.Log(" 0 1 " + result[0]);
        result = Train(0, 0, 0);
        Debug.Log(" 0 0 " + result[0]);
    }

    private List<double> Train(double i1, double i2, double o)
    {
        List<double> inputs = new List<double>();
        List<double> desiredOutputs = new List<double>();
        inputs.Add(i1);
        inputs.Add(i2);
        desiredOutputs.Add(o);

        return ann.Train(inputs, desiredOutputs);
    }
}
