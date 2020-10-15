using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    List<int> genes = new List<int>();
    int dnaLength = 0;
    int maxValue = 0;

    public DNA(int length, int maxValue)
    {
        dnaLength = length;
        this.maxValue = maxValue;
        SetRandom();
    }

    private void SetRandom()
    {
        for (int i = 0; i < dnaLength; i++)
        {
            genes.Add(UnityEngine.Random.Range(0, maxValue));
        }
    }

    public void SetInt(int pos, int value)
    {
        genes[pos] = value;
    }

    public int GetGene(int pos)
    {
        return genes[pos];
    }

    public void Mutate()
    {
        genes[UnityEngine.Random.Range(0, dnaLength)] = UnityEngine.Random.Range(0, maxValue);
    }

    public void Combine(DNA d1, DNA d2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            int c = 0;
            if (i < dnaLength / 2.0)
            {
                c = d1.genes[i];
            }
            else
            {
                c = d2.genes[i];
            }
            genes[i] = c;
        }
    }
}
