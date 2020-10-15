using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    int dnaLength = 5;
    private int maxDnaValue = 50;
    public DNA dna;
    public GameObject eyes;
    public float distanceTraveled = 0;
    public float timeAlive = 0;
    public bool seeUpWall = false;
    public bool seeDownWall = false;
    public bool seeTop = false;
    public bool seeBottom = false;
    float viewDistance = 2f;
    public Vector3 startPosition;
    public int crash = 0;
    public bool alive = false;
    Rigidbody2D rb;

    enum GeneOrder
    {
        UpWall,
        DownWall,
        Top,
        Bottom,
        Normal
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            distanceTraveled = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "top") ||
            (collision.gameObject.tag == "bottom") ||
            (collision.gameObject.tag == "upwall") ||
            (collision.gameObject.tag == "downwall"))
        {
            crash++;
        }
    }

    public void Init()
    {
        //Initialize DNA
        //0 UpWall
        //1 DownWall
        //2 Top
        //3 Bottom
        //4 Normal
        dna = new DNA(dnaLength, maxDnaValue);
        this.transform.Translate(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f), 0);
        distanceTraveled = 0;
        alive = true;
        startPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        rb = this.GetComponent<Rigidbody2D>();
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

        seeBottom = false;
        seeDownWall = false;
        seeTop = false;
        seeUpWall = false;

        Debug.DrawRay(eyes.transform.position, -eyes.transform.right * viewDistance, Color.red);
        Debug.DrawRay(eyes.transform.position, eyes.transform.up * viewDistance, Color.red);
        Debug.DrawRay(eyes.transform.position, -eyes.transform.up * viewDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.right, viewDistance);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "upwall")
            {
                seeUpWall = true;
            }
            else if (hit.collider.tag == "downwall")
            {
                seeDownWall = true;
            }
        }

        hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, viewDistance);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "top")
            {
                seeTop = true;
            }
        }

        hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, viewDistance);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "Bottom")
            {
                seeBottom = true;
            }
        }

        timeAlive += PopulationManager.elapsed;
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        //read DNA
        float upForce = 0;
        float forwardForce = -4.0f;

        if (seeUpWall)
        {
            upForce = dna.GetGene((int)GeneOrder.UpWall);
        }
        else if (seeDownWall)
        {
            upForce = dna.GetGene((int)GeneOrder.DownWall);
        }
        else if (seeTop)
        {
            upForce = dna.GetGene((int)GeneOrder.Top);
        }
        else if (seeBottom)
        {
            upForce = dna.GetGene((int)GeneOrder.Bottom);
        }
        else
        {
            upForce = dna.GetGene((int)GeneOrder.Normal);
        }

        rb.AddForce(this.transform.right * forwardForce);
        rb.AddForce(this.transform.up * upForce);

        distanceTraveled = Vector3.Distance(this.transform.position, startPosition);
    }
}
