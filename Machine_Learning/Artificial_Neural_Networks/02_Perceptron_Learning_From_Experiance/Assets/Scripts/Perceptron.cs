using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour {

    List<TrainingSet> ts = new List<TrainingSet>();
	double[] weights = {0,0};
	double bias = 0;
	double totalError = 0;

    public GameObject npc;

    public void SendInput(double input1, double input2, double output)
    {
        //react
        double result = CalcOutput(input1, input2);
        Debug.Log(result);

        if (result == 0) //duck for cover
        {
            npc.GetComponent<Animator>().SetTrigger("Crouch");
            npc.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            npc.GetComponent<Rigidbody>().isKinematic = true;
        }

        //learn from it for next time
        TrainingSet s = new TrainingSet();
        s.input = new double[2] { input1, input2 };
        s.output = output;
        ts.Add(s);
        Train();
    }

	double DotProductBias(double[] v1, double[] v2) 
	{
		if (v1 == null || v2 == null)
			return -1;
	 
		if (v1.Length != v2.Length)
			return -1;
	 
		double d = 0;
		for (int x = 0; x < v1.Length; x++)
		{
			d += v1[x] * v2[x];
		}

		d += bias;
	 
		return d;
	}

	double CalcOutput(int i)
	{
		return(ActivationFunction(DotProductBias(weights,ts[i].input)));
	}

	double CalcOutput(double i1, double i2)
	{
		double[] inp = new double[] {i1, i2};
		return(ActivationFunction(DotProductBias(weights,inp)));
	}

	double ActivationFunction(double dp)
    {
		return (dp > 0) ? 1 : 0;
	}

	void InitialiseWeights()
	{
		for(int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f,1.0f);
		}
		bias = Random.Range(-1.0f,1.0f);
	}

	void UpdateWeights(int j)
	{
		double error = ts[j].output - CalcOutput(j);
		totalError += Mathf.Abs((float)error);
		for(int i = 0; i < weights.Length; i++)
		{			
			weights[i] = weights[i] + error*ts[j].input[i]; 
		}
		bias += error;
	}

    void Train()
    {
        for (int t = 0; t < ts.Count; t++)
        {
            UpdateWeights(t);
        }
    }

    void LoadWeights()
    {
        string path = Application.dataPath + "/weights.txt";

        if (File.Exists(path))
        {
            Debug.Log("loading");
            var sr = File.OpenText(path);
            string line = sr.ReadLine();
            string[] w = line.Split(',');
            weights[0] = System.Convert.ToDouble(w[0]);
            weights[1] = System.Convert.ToDouble(w[1]);
            bias = System.Convert.ToDouble(w[2]);
            Debug.Log("done loading");
        }
    }

    void SaveWeights()
    {
        string path = Application.dataPath + "/weights.txt";

        Debug.Log("saving");
        var sr = File.CreateText(path);
        sr.WriteLine(weights[0] + "," + weights[1] + "," + bias);
        sr.Close();
        Debug.Log("done saving");
    }

	void Start ()
    {
        InitialiseWeights();
	}
	
	void Update ()
    {
		if (Input.GetKeyDown("s"))
        {
            SaveWeights();
        }
        if (Input.GetKeyDown("l"))
        {
            LoadWeights();
        }
	}
}