using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float visibleDistance = 200f;
    List<string> collectedTrainingData = new List<string>();
    StreamWriter tdf;

    private void Start()
    {
        string path = Application.dataPath + "/trainingData.txt";
        tdf = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        foreach (string td in collectedTrainingData)
        {
            tdf.WriteLine(td);
        }

        tdf.Close();
    }

    float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2f;
    }

    void Update()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        translationInput = Round(translationInput);
        rotationInput = Round(rotationInput);

        float translation = translationInput * speed * Time.deltaTime;
        float rotation = rotationInput * rotationSpeed * Time.deltaTime;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, (transform.forward + transform.right).normalized * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, (transform.forward - transform.right).normalized * visibleDistance, Color.red);

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

        string td = fDist + "," + rDist + "," + lDist + "," + r45Dist + "," + l45Dist + "," + translationInput + "," + rotationInput;

        if (!collectedTrainingData.Contains(td))
        {
            collectedTrainingData.Add(td);
        }
    }
}
