using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    int dnaLength = 2;
    public float timeAlive;
    public float timeWalking;
    public DNA dna;
    public GameObject eyes;
    public bool alive;
    bool seeGround;
    float viewDistance = 10f;

    public GameObject ethanPrefab;
    GameObject ethan;

    private void OnDestroy()
    {
        Destroy(ethan);
    }

    enum GeneOrder
    {
        SeePlatform,
        CantSeePlatform
    }

    enum Decision
    {
        Forward = 0,
        Left,
        Right,
        ENUM_LENGTH
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            timeAlive = 0;
            timeWalking = 0;
        }
    }

    public void Init()
    {
        //Initialize DNA
        //0 forward
        //1 left
        //2 right
        dna = new DNA(dnaLength, (int)Decision.ENUM_LENGTH);
        timeAlive = 0;
        timeWalking = 0;
        alive = true;
        ethan = Instantiate(ethanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = this.transform;
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

        //Debug.DrawRay(eyes.transform.position, eyes.transform.forward * viewDistance, Color.red);
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * viewDistance, out hit))
        {
            if (hit.collider.tag == "platform")
            {
                seeGround = true;
            }
        }

        timeAlive += Time.deltaTime;

        //read DNA
        float h = 0;
        float v = 0;

        int gene;

        if (seeGround)
        {
            gene = (int)GeneOrder.SeePlatform;
        }
        else
        {
            gene = (int)GeneOrder.CantSeePlatform;
        }

        switch (dna.GetGene(gene))
        {
            case (int)Decision.Forward:
                v = 1f;
                timeWalking += 1;
                break;

            case (int)Decision.Left:
                h = -90f;
                break;

            case (int)Decision.Right:
                h = 90f;
                break;
        }

        this.transform.Translate(0, 0, v * 0.1f);
        this.transform.Rotate(0, h, 0);
    }
}
