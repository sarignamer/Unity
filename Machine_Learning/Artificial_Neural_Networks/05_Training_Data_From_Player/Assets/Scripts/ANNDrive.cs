using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ANNDrive : MonoBehaviour
{
    ANN ann;
    public float visibleDistance = 200f;
    public int epochs = 1000;
    public float speed = 1500f;
    public float rotationSpeed = 2000f;

    bool trainingDone = false;
    float trainingProgress = 0f;
    double sse = 0;
    double lastSSE = 1;

    public float translation;
    public float rotation;

    // Start is called before the first frame update
    void Start()
    {
        ann = new ANN(5, 2, 1, 10, 0.5);
        StartCoroutine(LoadTrainingSet());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained:" + trainingProgress);
    }

    IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/TrainingData.txt";

        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            string line;
            StreamReader tdf = File.OpenText(path);
            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < epochs; i++)
            {
                sse = 0;
                tdf.BaseStream.Position = 0;
                //save current weights incase the training was bad
                string currentWeights = ann.PrintWeights();
                while ((line = tdf.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    float thisError = 0;
                    if ((System.Convert.ToDouble(data[5]) != 0) && (System.Convert.ToDouble(data[6]) != 0))
                    {
                        inputs.Clear();
                        outputs.Clear();
                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        double o1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                        outputs.Add(o1);
                        double o2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                        outputs.Add(o2);

                        calcOutputs = ann.Train(inputs, outputs);
                        thisError = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                                     Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2)) / 2f;
                    }

                    sse += thisError;
                }

                trainingProgress = (float)i / (float)epochs;
                sse /= lineCount;

                //if current sse is larger then lastsse it means the training got worsen and we
                //want to load previous weights and ignore the last training, and decrease alpha
                if (lastSSE < sse)
                {
                    ann.LoadWeights(currentWeights);
                    ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
                }
                else //increase alpha
                {
                    lastSSE = sse;
                    ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                }

                yield return null;
            }
        }

        trainingDone = true;
    }

    float Map(float newFrom, float newTo, float origFrom, float origTo, float value)
    {
        if (value <= origFrom)
        {
            return value;
        }
        else if (value >= origTo)
        {
            return value;
        }

        return (newTo - newFrom) * ((value - origFrom) / (origTo - origFrom)) + newFrom;
    }

    float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!trainingDone)
        {
            return;
        }

        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        RaycastHit hit;
        float fDist = 0;
        float rDist = 0;
        float lDist = 0;
        float r45Dist = 0;
        float l45Dist = 0;

        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            fDist = 1 - Round(hit.distance / visibleDistance);
        }

        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rDist = 1 - Round(hit.distance / visibleDistance);
        }

        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            lDist = 1 - Round(hit.distance / visibleDistance);
        }

        if (Physics.Raycast(transform.position, (transform.forward + transform.right).normalized, out hit, visibleDistance))
        {
            r45Dist = 1 - Round(hit.distance / visibleDistance);
        }

        if (Physics.Raycast(transform.position, (transform.forward - transform.right).normalized, out hit, visibleDistance))
        {
            l45Dist = 1 - Round(hit.distance / visibleDistance);
        }

        inputs.Add(fDist);
        inputs.Add(rDist);
        inputs.Add(lDist);
        inputs.Add(r45Dist);
        inputs.Add(l45Dist);
        outputs.Add(0);
        outputs.Add(0);
        calcOutputs = ann.CalculateOutput(inputs, outputs);
        float translationInput = Map(0, 1, -1, 1, (float)calcOutputs[0]);
        float rotationInput = Map(0, 1, -1, 1, (float)calcOutputs[1]);
        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;
        this.transform.Translate(0, 0, translation);
        this.transform.Rotate(0, rotation, 0);

    }
}
