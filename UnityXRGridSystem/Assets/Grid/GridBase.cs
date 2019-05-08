using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public struct GridVerticiesPayload
{
    public Vector3 origin; 

    public Vector3[,] xStart;
    public Vector3[,] xEnd;

    public Vector3[,] yStart;
    public Vector3[,] yEnd;

    public Vector3[,] zStart;
    public Vector3[,] zEnd;

    public int xDimension;
    public int yDimension;
    public int zDimension;

    public float CellSize; 
}

namespace UnityEngine.XR.ARFoundation
{
    [RequireComponent(typeof(BoxCollider))]
    public class GridBase : MonoBehaviour
    {
        #region Public variables
        //public Camera camera;
        public float CellSize;

        public delegate void OnVariableChangeDelegate();
        public event OnVariableChangeDelegate OnFromChange;
        public event OnVariableChangeDelegate OnToChange;

        public Vector3 gridOrigin;
        public Vector3 from;
        public Vector3 to;

        public GridRenderer gridRenderer;

        // https://answers.unity.com/questions/1206632/trigger-event-on-variable-change.html

        // use for camera modifications
        public Vector3 From
        {
            get
            {
                return from;
            }

            set
            {
                if (from == value)
                    return;

                from = value;
                Debug.Log("Changing FROM!");
                OnFromChange?.Invoke();
            }
        }
        public Vector3 To
        {
            get
            {
                return to;
            }

            set
            {
                if (to == value)
                    return;

                to = value;
                Debug.Log("Changing TO!");
                OnToChange?.Invoke();
            }
        }



        private Vector3[,] testArray;
        // these from and to drive the grid size 


        #endregion

        #region Private variables

        private Vector3[,] vertices;
        private Vector3[] boundaries;

        //private Vector3 gridOrigin;

        private GridVerticiesPayload gridPayload;

        private Plane world = new Plane(Vector3.up, Vector3.zero); //world plane to draw the grid on

        private BoxCollider collider;

        #endregion

        #region public events & delegates
        public delegate void OnGridHit(Collider other);
        public event OnGridHit allowSnapping;
        #endregion

        #region Public Callback Methods
        public virtual void Start()
        {
            initalizeGrid();

            OnFromChange += UpdateGrid;
            OnToChange += UpdateGrid;

        }

        // https://xinyustudio.wordpress.com/2014/01/11/unity3d-inspector-control-value-changed-event-handling/
        public void Update()
        {

        }

        /// <summary>
        /// update grid, if there is change in From or To Vectors 
        /// </summary>
        public virtual void UpdateGrid()
        {
            gridPayload = GenerateVerticies(gridOrigin, From, To, CellSize);
            GenerateCells(gridPayload);
            ColliderConfiguration(gridPayload, gridOrigin);
            gridRenderer.UpdateGridRenderer(gridPayload);
        }

        // we need a condition that allows the object to unsnap from the grid 
        public void OnTriggerStay(Collider other)
        {
            Debug.Log("On TriggerStay");
            if (other.GetComponent<Interactable>())
            {
                Debug.Log("Snapping");
                SnappingToGrid(other);
            }

        }

        #endregion

        #region Private Configuration Methods 

        // initialize grid on Start at Beginning of RunTime.
        private void initalizeGrid()
        {
            // for non AR
            //gridOrigin = transform.position;

            gridPayload = GenerateVerticies(gridOrigin, from, to, CellSize);
            gridRenderer.UpdateGridRenderer(gridPayload);
            ColliderConfiguration(gridPayload, gridOrigin);
            //GenerateCells(gridPayload);
            // for AR 
        }

        private void ColliderConfiguration(GridVerticiesPayload g, Vector3 colliderOrigin)
        {
            collider = GetComponent<BoxCollider>();

            // take into account that the scale units (i.e. a cube scale of 20 means a collider width of 1 will mean
            // the actual dimensions will be 20.

            Vector3 scale = transform.localScale;
            Debug.Log(scale);

            // collider.size = new Vector3((g.xDimension * CellSize)/scale.x, g.yDimension*CellSize, (g.zDimension * CellSize)/scale.z);
            collider.center = colliderOrigin;
        }
        #endregion

        // inspiration https://www.youtube.com/watch?v=QBO1m-AFntQ
        #region Private rendering / cell calculation methods

        /// <summary>
        /// Generate vertices for the vertex array; 
        /// </summary>
        /// <param name="gridOrigin"></param>
        private GridVerticiesPayload GenerateVerticies(Vector3 gridOrigin, Vector3 from, Vector3 to, float cellSize)
        {
            GridVerticiesPayload g = new GridVerticiesPayload();

            // check type sensitivity here 
            for (int i = 0; i < 3; i++)
            {
                from[i] = Mathf.Clamp(from[i], -Mathf.Infinity, 0);
                to[i] = Mathf.Clamp(to[i], 0, Mathf.Infinity);
            }

            // private variables -> call this the 
            // changeGridLengths();

            int xStartLength = Mathf.FloorToInt(from.x - gridOrigin.x);
            int yStartLength = Mathf.FloorToInt(from.y - gridOrigin.y);
            int zStartLength = Mathf.FloorToInt(from.z - gridOrigin.z);

            int xEndLength = Mathf.FloorToInt(to.x - gridOrigin.x);
            int yEndLength = Mathf.FloorToInt(to.y - gridOrigin.y);
            int zEndLength = Mathf.FloorToInt(to.z - gridOrigin.z);

            g.xDimension = Mathf.Abs(xEndLength - xStartLength);
            g.yDimension = Mathf.Abs(yEndLength - yStartLength);
            g.zDimension = Mathf.Abs(zEndLength - zStartLength);

            //Debug.Log("Grid Origin: " + gridOrigin);
            //Debug.Log(xStartLength + " , " + yStartLength + " , " + zStartLength);
            //Debug.Log(xEndLength + " , " + yEndLength + " , " + zEndLength);
            //Debug.Log(g.xDimension + " , " + g.yDimension + " , " + g.zDimension);

            g.origin = gridOrigin;
            g.CellSize = cellSize;

            // x axis end 
            g.xStart = new Vector3[g.xDimension, g.zDimension];
            g.xEnd = new Vector3[g.xDimension, g.zDimension];

            // y axis end
            g.yStart = new Vector3[g.yDimension, g.zDimension];
            g.yEnd = new Vector3[g.yDimension, g.zDimension];

            // z axis end 
            g.zStart = new Vector3[g.xDimension, g.yDimension];
            g.zEnd = new Vector3[g.xDimension, g.yDimension];


            // for math purposes, convert to abs value 
            xStartLength = Mathf.Abs(xStartLength);
            yStartLength = Mathf.Abs(yStartLength);
            zStartLength = Mathf.Abs(zStartLength);

            xEndLength = Mathf.Abs(xEndLength);
            yEndLength = Mathf.Abs(yEndLength);
            zEndLength = Mathf.Abs(zEndLength);


            // x lines
            for (int x = 0; x < g.xDimension; x++)
            {
                for (int z = 0; z < g.zDimension; z++)
                {
                    g.xStart[x, z] = new Vector3(g.CellSize * (x - xStartLength + gridOrigin.x), g.CellSize * (-yStartLength + gridOrigin.y), g.CellSize * (z - zStartLength + gridOrigin.z));
                    g.xEnd[x, z] = new Vector3(g.CellSize * (x - xStartLength + gridOrigin.x), g.CellSize * (yEndLength + gridOrigin.y), g.CellSize * (z - zStartLength + gridOrigin.z));
                }
            }

            // y lines
            for (int y = 0; y < g.yDimension; y++)
            {
                for (int z = 0; z < g.zDimension; z++)
                {
                    g.yStart[y, z] = new Vector3(g.CellSize * (-xStartLength + gridOrigin.x), g.CellSize * (y - yStartLength + gridOrigin.y), g.CellSize * (z - zStartLength + gridOrigin.z));
                    g.yEnd[y, z] = new Vector3(g.CellSize * (xEndLength + gridOrigin.x), g.CellSize * (y - yStartLength + gridOrigin.y), g.CellSize * (z - zStartLength + gridOrigin.z));
                }
            }

            // z lines
            for (int x = 0; x < g.xDimension; x++)
            {
                for (int y = 0; y < g.yDimension; y++)
                {
                    g.zStart[x, y] = new Vector3(g.CellSize * (x - xStartLength + gridOrigin.x), g.CellSize * (y - yStartLength + gridOrigin.y), g.CellSize * (-zStartLength + gridOrigin.z));
                    g.zEnd[x, y] = new Vector3(g.CellSize * (x - xStartLength + gridOrigin.x), g.CellSize * (y - yStartLength + gridOrigin.y), g.CellSize * (zEndLength + gridOrigin.z));
                }
            }


            return g;
        }

        /// <summary>
        /// Add cells that are within the Camera boundaries. 
        /// </summary>
        private void GenerateCells(GridVerticiesPayload g)
        {
            for (int x = 0; x < g.xDimension - 1; x++)
            {
                for (int y = 0; y < g.yDimension - 1; y++)
                {
                    for (int z = 0; y < g.zDimension - 1; z++)
                    {
                        float shiftedX = x - g.origin.x + g.CellSize / 2;
                        float shiftedY = y - g.origin.y + g.CellSize / 2;
                        float shiftedZ = z - g.origin.z + g.CellSize / 2;

                        Vector3 cellPos = new Vector3(shiftedX, shiftedY, shiftedZ);
                        //cells[i] = new Cell(cellPos, g.CellSize);
                    }
                }
            }
        }


        #endregion

        #region Public Grid Helper / Utility Methods
        /// <summary>
        /// Calculate the camera's look position when it hits a plane.
        /// </summary>
        /// <returns></returns>
        public Vector3 CalculateLookPosition(Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            float rayDist = Vector3.Magnitude(camera.transform.position - Vector3.zero);
            world.Raycast(ray, out rayDist);
            return ray.GetPoint(rayDist);
        }

        /// <summary>
        /// Calculate the camera boundaries where the frustm boundaries hit the plane. 
        /// </summary>
        /// <param name="a_camera"></param>
        /// <param name="gridPlane"></param>
        /// <returns></returns>
        public Vector3[] CalculateCameraBoundaries(Camera a_camera, Plane gridPlane)
        {
            // https://answers.unity.com/questions/168156/screen-vs-viewport-what-is-the-difference.html
            Vector3[] cameraBoundaries = new Vector3[4];

            Ray bottomLeftRay = a_camera.ViewportPointToRay(new Vector3(0, 0, 0));
            Ray topLeftRay = a_camera.ViewportPointToRay(new Vector3(0, 1, 0));
            Ray topRightRay = a_camera.ViewportPointToRay(new Vector3(1, 1, 0));
            Ray bottomRightRay = a_camera.ViewportPointToRay(new Vector3(1, 0, 0));

            float bottomLeftRayDist;
            float topLeftRayDist;
            float topRightRayDist;
            float bottomRightRayDist;

            gridPlane.Raycast(bottomLeftRay, out bottomLeftRayDist);
            gridPlane.Raycast(topLeftRay, out topLeftRayDist);
            gridPlane.Raycast(topRightRay, out topRightRayDist);
            gridPlane.Raycast(bottomRightRay, out bottomRightRayDist);

            cameraBoundaries[0] = bottomLeftRay.GetPoint(bottomLeftRayDist);
            cameraBoundaries[1] = topLeftRay.GetPoint(topLeftRayDist);
            cameraBoundaries[2] = topRightRay.GetPoint(topRightRayDist);
            cameraBoundaries[3] = bottomRightRay.GetPoint(topRightRayDist);

            return cameraBoundaries;
        }

        /// <summary>
        /// Calculate the closest cell position by interfacing directly with the cell data sturcture. 
        /// </summary>
        /// <returns></returns>
        /// 
        public void SnappingToGrid(Collider other)
        {
            other.transform.position = snapToGrid(other.transform.position);
        }

        public void UnsnappingToGrid(Collider other)
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y + 15.0f, other.transform.position.z);
        }

        public Vector3 snapToGrid(Vector3 pos)
        {
            float shiftedX = Mathf.RoundToInt(pos.x / CellSize);
            //float shiftedY = Mathf.RoundToInt(pos.y / CellSize) + CellSize/2 + gridOrigin.y + 0.25f;
            float shiftedY = gridOrigin.y + 5f;
            float shiftedZ = Mathf.RoundToInt(pos.z / CellSize);

            Debug.Log("Before: " + new Vector3(pos.x, pos.y, pos.z));
            Debug.Log(new Vector3(shiftedX * CellSize + CellSize / 2 + gridOrigin.x, shiftedY, shiftedZ * CellSize + CellSize / 2 + gridOrigin.z));

            // clamp based on boundaries of grid -> not necessary if the object will be leaving collider 
            //shiftedX = Mathf.Clamp(shiftedX, from.x - gridOrigin.x, to.x - gridOrigin.x);
            //shiftedY = Mathf.Clamp(shiftedY, from.y - gridOrigin.y, to.y - gridOrigin.y);
            //shiftedZ = Mathf.Clamp(shiftedZ, from.z - gridOrigin.z, to.z - gridOrigin.z);

            Vector3 snappedPosition = new Vector3(shiftedX * CellSize + CellSize / 2, shiftedY, shiftedZ * CellSize + CellSize / 2 + gridOrigin.z);

            return snappedPosition;
        }

        /// <summary>
        /// Call method when object collides with grid. Snap to grid. 
        /// </summary>
        /// <param name="other"></param>
        public void CollideWithGrid(Collision other)
        {
            if (other.collider)
            {
                other.collider.transform.position = snapToGrid(other.collider.transform.position);
            }
        }
        #endregion

    }
}