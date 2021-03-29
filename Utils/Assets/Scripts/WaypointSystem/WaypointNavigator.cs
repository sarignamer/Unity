using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterNavigationController))]
public class WaypointNavigator : MonoBehaviour
{
    private CharacterNavigationController controller;
    [SerializeField]
    private Waypoint currentWaypoint;
    [SerializeField]
    private int direction;
    [SerializeField]
    private bool stopAtEnd = false;

    public Waypoint CurrentWaypoint { get => currentWaypoint; set => currentWaypoint = value; }

    private void Awake()
    {
        controller = GetComponent<CharacterNavigationController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller.SetDestination(currentWaypoint.GetRandomPosition());
        direction = (int)Mathf.Sign(Random.Range(-1f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.ReachedDestination && (currentWaypoint != null))
        {
            bool shouldBranch = false;

            if ((currentWaypoint.Branches != null) && (currentWaypoint.Branches.Count > 0))
            {
                shouldBranch = (Random.Range(0, 1f) <= currentWaypoint.BranchRatio) ? true : false;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.Branches[Random.Range(0, currentWaypoint.Branches.Count)];
            }

            if ((currentWaypoint + direction) == null)
            {
                if (stopAtEnd)
                {
                    return;
                }
                else
                {
                    direction *= -1;
                }
            }

            if (!shouldBranch)
            {
                currentWaypoint = currentWaypoint + direction;
            }

            if (currentWaypoint != null)
            {
                controller.SetDestination(currentWaypoint.GetRandomPosition());
            }

        }
    }
}
