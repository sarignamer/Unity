﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    Vector3 ballStartPosition;
    Rigidbody2D rb;
    float speed = 400f;
    public AudioSource blip;
    public AudioSource blop;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        ballStartPosition = transform.position;
        ResetBall();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "backwall")
        {
            blop.Play();
        }
        else
        {
            blip.Play();
        }
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 dir = new Vector3(Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
        rb.AddForce(speed * dir);
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown("space"))
        {
            ResetBall();
        }
	}
}
