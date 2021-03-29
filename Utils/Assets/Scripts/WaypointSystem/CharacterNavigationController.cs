using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNavigationController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 120f;
    [SerializeField]
    private float stopDistance = .5f;
    [SerializeField]
    private Vector3 destination;
    [SerializeField]
    private bool reachedDestination = false;

    public bool ReachedDestination { get => reachedDestination; set => reachedDestination = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position != destination) || ReachedDestination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                ReachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                ReachedDestination = true;
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        ReachedDestination = false;
    }
}
