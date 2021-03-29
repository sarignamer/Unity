using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    public Transform waypointRoot;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("Box");
            DrawButtons();
            EditorGUILayout.EndVertical();

        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }

        if ((Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Waypoint>() != null))
        {
            if (GUILayout.Button("Create Waypoint Branch"))
            {
                CreateWaypointBranch(false);
            }

            if (GUILayout.Button("Create Bidirectional Waypoint Branch"))
            {
                CreateWaypointBranch(true);
            }

            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }

            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }

            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
        }
    }

    private void CreateWaypointBranch(bool isBidirectional)
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        branchedFrom.Branches.Add(newWaypoint);

        newWaypoint.BranchedFrom = branchedFrom;

        if (isBidirectional)
        {
            if (newWaypoint.Branches == null)
            {
                newWaypoint.Branches = new List<Waypoint>();
            }
            newWaypoint.Branches.Add(branchedFrom);
        }

        newWaypoint.transform.position = branchedFrom.transform.position;
        newWaypoint.transform.forward = branchedFrom.transform.forward;

        Selection.activeGameObject = newWaypoint.gameObject;
    }

    private void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        RemoveWaypointFromBranches(selectedWaypoint);

        if (selectedWaypoint.BranchedFrom != null)
        {
            selectedWaypoint.BranchedFrom.Branches.Remove(selectedWaypoint);
        }

        if (selectedWaypoint.NextWaypoint != null)
        {
            selectedWaypoint.NextWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
            Selection.activeGameObject = selectedWaypoint.NextWaypoint.gameObject;
        }

        if (selectedWaypoint.PreviousWaypoint != null)
        {
            selectedWaypoint.PreviousWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
            Selection.activeGameObject = selectedWaypoint.PreviousWaypoint.gameObject;
        }

        DestroyImmediate(selectedWaypoint.gameObject);
    }

    private void RemoveWaypointFromBranches(Waypoint waypoint)
    {
        if (waypoint.Branches != null)
        {
            foreach (Waypoint waypointBranch in waypoint.Branches)
            {
                waypointBranch.Branches.Remove(waypoint);
            }
        }
    }

    private void CreateWaypointAfter()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.transform.position = selectedWaypoint.transform.position;
        newWaypoint.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.NextWaypoint != null)
        {
            newWaypoint.NextWaypoint = selectedWaypoint.NextWaypoint;
            newWaypoint.NextWaypoint.PreviousWaypoint = newWaypoint;
            newWaypoint.transform.SetSiblingIndex(newWaypoint.NextWaypoint.transform.GetSiblingIndex());
        }
        else
        {

        }

        newWaypoint.PreviousWaypoint = selectedWaypoint;
        selectedWaypoint.NextWaypoint = newWaypoint;


        Selection.activeGameObject = waypointObject;
    }

    private void CreateWaypointBefore()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        newWaypoint.transform.position = selectedWaypoint.transform.position;
        newWaypoint.transform.forward = selectedWaypoint.transform.forward;

        if (selectedWaypoint.PreviousWaypoint != null)
        {
            newWaypoint.PreviousWaypoint = selectedWaypoint.PreviousWaypoint;
            newWaypoint.PreviousWaypoint.NextWaypoint = newWaypoint;
        }

        newWaypoint.NextWaypoint = selectedWaypoint;
        selectedWaypoint.PreviousWaypoint = newWaypoint;

        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

        Selection.activeGameObject = waypointObject;
    }

    private void CreateWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);

        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        if (waypointRoot.childCount > 1)
        {
            //Place the waypoint at the last position
            waypoint.PreviousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.PreviousWaypoint.NextWaypoint = waypoint;
            waypoint.transform.position = waypoint.PreviousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.PreviousWaypoint.transform.forward;
        }

        Selection.activeGameObject = waypointObject;
    }
}
