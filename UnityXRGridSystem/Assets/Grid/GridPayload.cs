using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPayload : MonoBehaviour
{
    #region public variables 

    private Vector3 gridOrigin;
    private Vector3 from;
    private Vector3 to;

    public Vector3 To { get { return to; } set { to = value; } }
    public Vector3 From { get { return from; } set { from = value; } }
    public Vector3 GridOrigin { get { return gridOrigin} set {gridOrigin = value }

    public GridRenderer gridRenderer;

    public float CellSize;

    #endregion

    #region private variables

    private Vector3[,] xStart;
    private Vector3[,] xEnd;

    private Vector3[,] yStart;
    private Vector3[,] yEnd;

    private Vector3[,] zStart;
    private Vector3[,] zEnd;

    private int xStartLength;
    private int yStartLength;
    private int zStartLength;

    private int xEndLength;
    private int yEndLength;
    private int zEndLength;

    private int xDimension;
    private int yDimension;
    private int zDimension;

    private GridVerticiesPayload g;

    #endregion

    #region public configuration methods

    /// <summary>
    /// Constructor function that accepts a origin, startDimension, and endDimension to generate a grid.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="startDimensions"></param>
    /// <param name="endDimensions"></param>
    public GridPayload(Vector3 origin, Vector3 startDimensions, Vector3 endDimensions)
    {
        gridOrigin = origin;
        from = startDimensions;
        to = endDimensions;
    }

    public void SetGridDimensions(Vector3 origin, Vector3 startDimensions, Vector3 endDimensions)
    {
        gridOrigin = origin;
        from = startDimensions;
        to = endDimensions;
    }

    public void ConfigureGrid()
    {
        calculateGridDimensions();
        setGridDimensions();
        generateGridVerticies();
        g = gridPayloadMaker();
    }

    public void RenderGrid()
    {
        gridRenderer.UpdateGridRenderer(g);
    }

    public void ShiftGrid(Vector3 cameraShift)
    {
        from -= cameraShift;
        to -= cameraShift;

        ConfigureGrid();
        RenderGrid();
    }

    public void RescaleGrid(float scale)
    {
        
    }

    #endregion

    #region public grid interaction methods

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

    #endregion

    #region private helper methods

    private void calculateGridDimensions()
    {
        xStartLength = Mathf.FloorToInt(from.x - gridOrigin.x);
        yStartLength = Mathf.FloorToInt(from.y - gridOrigin.y);
        zStartLength = Mathf.FloorToInt(from.z - gridOrigin.z);

        xEndLength = Mathf.FloorToInt(to.x - gridOrigin.x);
        yEndLength = Mathf.FloorToInt(to.y - gridOrigin.y);
        zEndLength = Mathf.FloorToInt(to.z - gridOrigin.z);

        xStartLength = Mathf.Abs(xStartLength);
        yStartLength = Mathf.Abs(yStartLength);
        zStartLength = Mathf.Abs(zStartLength);

        xEndLength = Mathf.Abs(xEndLength);
        yEndLength = Mathf.Abs(yEndLength);
        zEndLength = Mathf.Abs(zEndLength);
    }

    private void setGridDimensions()
    {
        xDimension = Mathf.Abs(xEndLength - xStartLength);
        yDimension = Mathf.Abs(yEndLength - yStartLength);
        zDimension = Mathf.Abs(zEndLength - zStartLength);
    }

    private void generateGridVerticies()
    {
        // x lines
        for (int x = 0; x < xDimension; x++)
        {
            for (int z = 0; z < zDimension; z++)
            {
                xStart[x, z] = new Vector3(CellSize * (x - xStartLength + gridOrigin.x), CellSize * (-yStartLength + gridOrigin.y), CellSize * (z - zStartLength + gridOrigin.z));
                xEnd[x, z] = new Vector3(CellSize * (x - xStartLength + gridOrigin.x), CellSize * (yEndLength + gridOrigin.y), CellSize * (z - zStartLength + gridOrigin.z));
            }
        }

        for (int y = 0; y < yDimension; y++)
        {
            for (int z = 0; z < zDimension; z++)
            {
                yStart[y, z] = new Vector3(CellSize * (-xStartLength + gridOrigin.x), CellSize * (y - yStartLength + gridOrigin.y), CellSize * (z - zStartLength + gridOrigin.z));
                yEnd[y, z] = new Vector3(CellSize * (xEndLength + gridOrigin.x), CellSize * (y - yStartLength + gridOrigin.y), CellSize * (z - zStartLength + gridOrigin.z));
            }
        }

        // z lines
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                zStart[x, y] = new Vector3(CellSize * (x - xStartLength + gridOrigin.x), CellSize * (y - yStartLength + gridOrigin.y), CellSize * (-zStartLength + gridOrigin.z));
                zEnd[x, y] = new Vector3(CellSize * (x - xStartLength + gridOrigin.x), CellSize * (y - yStartLength + gridOrigin.y), CellSize * (zEndLength + gridOrigin.z));
            }
        }

    }

    private GridVerticiesPayload gridPayloadMaker()
    {
        GridVerticiesPayload g = new GridVerticiesPayload();

        g.xDimension = xDimension;
        g.yDimension = yDimension;
        g.zDimension = zDimension;

        g.origin = gridOrigin; 

        g.CellSize = CellSize;

        g.xStart = xStart;
        g.xEnd = xEnd;

        g.yStart = yStart;
        g.yEnd = yEnd;

        g.zStart = zStart;
        g.zEnd = zEnd;

        return g; 
    }

    #endregion 


}
