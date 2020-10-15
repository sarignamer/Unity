using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    //gene for color
    public float r;
    public float g;
    public float b;

    //gene for scale
    public float scale;

    bool dead = false;
    public float timeOfDeath = 0;

    SpriteRenderer spriteRenderer;
    Collider2D collider2D;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer.color = new Color(r, g, b);
        transform.localScale = Vector3.one * scale;
    }

    private void OnMouseDown()
    {
        dead = true;
        timeOfDeath = PopulationManager.elapsed;
        Debug.Log("Dead At: " + timeOfDeath);
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
    }
}
