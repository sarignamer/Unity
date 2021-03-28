using Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericGrid;

public class Tester : MonoBehaviour
{
    private GenericGrid<GridObject> grid;
    int gridWidth = 10;
    int gridHeight = 10;
    int cellWidth = 10;
    int cellHeight = 10;

    void Start()
    {
        grid = new GenericGrid<GridObject>(gridWidth, gridHeight, cellWidth, cellHeight, GridObject.CreateGridObject, Vector3.zero);
        grid.ShowDebugGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridObject g = grid.GetGridObject(MouseUtils.GetMousePosition3D());
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = MouseUtils.GetMousePosition3D();

            if (mousePos.magnitude != 0)
            {
                TextCreator.CreatePopupText(mousePos.ToString(), 10, 1, null, mousePos, Camera.main);
            }
        }
    }
}

public class GridObject
{
    private GenericGrid<GridObject> grid;
    private int x;
    private int z;

    private GridObject(GenericGrid<GridObject> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }

    public static GridObject CreateGridObject(GenericGrid<GridObject> grid, int x, int y)
    {
        return new GridObject(grid, x, y);
    }

    public override string ToString()
    {
        return x + "," + z;
    }
}
