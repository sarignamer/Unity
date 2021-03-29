using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private Waypoint previousWaypoint;
    [SerializeField]
    private Waypoint nextWaypoint;
    [SerializeField]
    [Range(0, 5)]
    private float width;

    [SerializeField]
    private List<Waypoint> branches;
    [SerializeField]
    private Waypoint branchedFrom;
    [SerializeField]
    [Range(0, 1f)]
    private float branchRatio = .5f;


    public Waypoint PreviousWaypoint { get => previousWaypoint; set => previousWaypoint = value; }
    public Waypoint NextWaypoint { get => nextWaypoint; set => nextWaypoint = value; }
    public float Width { get => width; set => width = value; }
    public List<Waypoint> Branches { get => branches; set => branches = value; }
    public float BranchRatio { get => branchRatio; set => branchRatio = value; }
    public Waypoint BranchedFrom { get => branchedFrom; set => branchedFrom = value; }

    public Vector3 GetRandomPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0, 1f));
    }

    public static Waypoint operator ++(Waypoint waypoint) => waypoint.nextWaypoint;
    public static Waypoint operator --(Waypoint waypoint) => waypoint.previousWaypoint;

    public static Waypoint operator +(Waypoint waypoint, int n)
    {
        if (n < 0)
        {
            n *= -1;

            return waypoint - n;
        }

        Waypoint result = waypoint;

        for (int i = 0; i < n; i++)
        {
            if (result == null)
            {
                break;
            }
            result++;
        }

        return result;
    }

    public static Waypoint operator -(Waypoint waypoint, int n)
    {
        if (n < 0)
        {
            n *= -1;

            return waypoint + n;
        }

        Waypoint result = waypoint;

        for (int i = 0; i < n; i++)
        {
            if (result == null)
            {
                break;
            }
            result--;
        }

        return result;
    }
}
