using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PopulationManager : MonoBehaviour
{
    public GameObject personPrefab;
    public int populationSize = 10;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public float mutaionChance = 0.1f;
    int trialTime = 10;
    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI()
    {
        guiStyle.fontSize = 50;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10f, 10f, 100f, 20f), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10f, 65, 100f, 20f), "Trial Time: " + (int)elapsed, guiStyle);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-8f, 8), UnityEngine.Random.Range(-3.5f, 5.5f), 0);
            GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);
            go.GetComponent<DNA>().r = UnityEngine.Random.Range(0, 1f);
            go.GetComponent<DNA>().g = UnityEngine.Random.Range(0, 1f);
            go.GetComponent<DNA>().b = UnityEngine.Random.Range(0, 1f);
            go.GetComponent<DNA>().scale = UnityEngine.Random.Range(0.1f, 0.3f);
            population.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    private void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();

        //get rid of unfit individuals
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA>().timeOfDeath).ToList();
        population.Clear();

        //breed the upper half of the population
        for (int i = (int)(sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        //destroy all parents and previous generation
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }

        generation++;

    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-8f, 8), UnityEngine.Random.Range(-3.5f, 5.5f), 0);
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        //check for mutation chance
        if (UnityEngine.Random.Range(0, 1f) < mutaionChance)
        {
            offspring.GetComponent<DNA>().r = UnityEngine.Random.Range(0, 1f);
            offspring.GetComponent<DNA>().g = UnityEngine.Random.Range(0, 1f);
            offspring.GetComponent<DNA>().b = UnityEngine.Random.Range(0, 1f);
            offspring.GetComponent<DNA>().scale = UnityEngine.Random.Range(0.8f, 1.2f);
        }
        else //swap parent dna
        {
            offspring.GetComponent<DNA>().r = UnityEngine.Random.Range(0, 1f) < 0.5f ? dna1.r : dna2.r;
            offspring.GetComponent<DNA>().g = UnityEngine.Random.Range(0, 1f) < 0.5f ? dna1.g : dna2.g;
            offspring.GetComponent<DNA>().b = UnityEngine.Random.Range(0, 1f) < 0.5f ? dna1.b : dna2.b;
            offspring.GetComponent<DNA>().scale = UnityEngine.Random.Range(0, 1f) < 0.5f ? dna1.scale : dna2.scale;
        }

        return offspring;
    }
}
