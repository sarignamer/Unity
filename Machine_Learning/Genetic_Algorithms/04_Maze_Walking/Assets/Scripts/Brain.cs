using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    int dnaLength = 2;
    public float distanceTraveled;
    public DNA dna;
    public GameObject eyes;
    public bool alive;
    bool seeWall;
    float viewDistance = 0.5f;
    public Vector3 origin;
    private int maxDnaValue = 360;

    enum GeneOrder
    {
        Forward,
        AngleTurn
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            distanceTraveled = 0;
        }
    }

    public void Init()
    {
        //Initialize DNA
        //0 forward
        //1 Angle Turn
        dna = new DNA(dnaLength, maxDnaValue);
        distanceTraveled = 0;
        alive = true;
        origin = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            return;
        }

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * viewDistance, Color.red);
        seeWall = false;
        RaycastHit hit;
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit))
        {
            if (hit.collider.tag == "wall")
            {
                seeWall = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //read DNA
        float h = 0;
        float v = dna.GetGene((int)GeneOrder.Forward);

        if (seeWall)
        {
            h = dna.GetGene((int)GeneOrder.AngleTurn);
        }


        distanceTraveled = Vector3.Distance(this.transform.position, origin);

        this.transform.Translate(0, 0, v * 0.001f);
        this.transform.Rotate(0, h, 0);
    }
}
