using Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenericGrid
{
    /// <summary>
    /// Genereic Grid class
    /// Can store any type or custom TGridObject
    /// TGridObject should also override ToString() function for debug purpose
    /// </summary>
    /// <typeparam name="TGridObject">Type of object to store in the grid</typeparam>
    public class GenericGrid<TGridObject>
    {
        /// <summary>
        /// Event that is called when a grid value is changed.
        /// Triggers automatically when calling SetGridObject.
        /// Can also be triggered manualy by calling TriggerGridObjectChanged,
        /// usually use this when setting a grid value using other function that
        /// bypass the SetGridObject.
        /// </summary>
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs
        {
            public int x;
            public int y;

            public OnGridValueChangedEventArgs(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private int width;
        private int height;
        private int cellWidth;
        private int cellHeight;
        private bool is2D;
        private Vector3 origin;

        private TGridObject[,] gridArray;
        private TextMesh[,] debugArray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">The width of the grid</param>
        /// <param name="height">The height of the grid</param>
        /// <param name="cellWidth">The width of each cell in the grid</param>
        /// <param name="cellHeight">The height of each cell in the grid</param>
        /// <param name="createGridObject">Function that creates the TGridObject</param>
        /// <param name="originPostion">The buttom left corner of the grid in world space</param>
        /// <param name="is2D">Is the grid in 2D world</param>
        public GenericGrid(
            int width, int height, int cellWidth, int cellHeight,
            Func<GenericGrid<TGridObject>, int, int, TGridObject> createGridObject,
            Vector3 originPostion = default(Vector3), bool is2D = false)
        {
            this.width = width;
            this.height = height;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.is2D = is2D;
            this.origin = originPostion;

            gridArray = new TGridObject[width, height];
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            debugArray = new TextMesh[width, height];

            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs e) =>
            {
                if (debugArray[e.x, e.y] != null)
                {
                    debugArray[e.x, e.y].text = gridArray[e.x, e.y].ToString();
                }
            };
        }

        public void ShowDebugGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    debugArray[x, y] = TextCreator.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPositionInMiddleOfCellSize(x, y), 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetWidth()
        {
            return width;
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            if (is2D)
            {
                return new Vector3(x * cellWidth, y * cellHeight) + origin;
            }

            return new Vector3(x * cellWidth, 0, y * cellHeight) + origin;
        }

        private Vector3 GetWorldPositionInMiddleOfCellSize(int x, int y)
        {
            if (is2D)
            {
                return new Vector3(x * cellWidth, y * cellHeight) + new Vector3(cellWidth, cellHeight) * .5f;
            }

            return new Vector3(x * cellWidth, 0, y * cellHeight) + new Vector3(cellWidth, 0, cellHeight) * .5f;
        }

        private Vector2Int GetXYFromWorldPosition(Vector3 worldPosition)
        {
            Vector3 position = worldPosition - origin;
            if (is2D)
            {
                return new Vector2Int(Mathf.FloorToInt(position.x / cellWidth), Mathf.FloorToInt(position.y / cellHeight));
            }

            return new Vector2Int(Mathf.FloorToInt(position.x / cellWidth), Mathf.FloorToInt(position.z / cellHeight));
        }

        public void SetGridObject(int x, int y, TGridObject gridObject)
        {
            if ((x < 0) || (x >= width) || (y < 0) || (y >= height))
            {
                return;
            }

            gridArray[x, y] = gridObject;

            TriggerGridObjectChanged(x, y);
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            if (OnGridValueChanged != null)
            {
                OnGridValueChanged(this, new OnGridValueChangedEventArgs(x, y));
            }
        }

        private void SetGridObject(Vector2Int position, TGridObject gridObject)
        {
            SetGridObject(position.x, position.y, gridObject);
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject gridObject)
        {
            SetGridObject(GetXYFromWorldPosition(worldPosition), gridObject);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if ((x < 0) || (x >= width) || (y < 0) || (y >= height))
            {
                return default(TGridObject);
            }

            return gridArray[x, y];
        }

        private TGridObject GetGridObject(Vector2Int position)
        {
            return GetGridObject(position.x, position.y);
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            return GetGridObject(GetXYFromWorldPosition(worldPosition));
        }
    }
}