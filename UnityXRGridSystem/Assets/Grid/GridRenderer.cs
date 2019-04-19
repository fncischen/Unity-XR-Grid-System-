using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    private Plane plane;
    public Plane Plane { get { return plane; } }

    private Vector3[,] xStart;
    private Vector3[,] xEnd;

    private Vector3[,] yStart;
    private Vector3[,] yEnd;

    private Vector3[,] zStart;
    private Vector3[,] zEnd;

    private int xDimension;
    private int yDimension;
    private int zDimension;

    private float CellSize;


    #region public interface 
    public void UpdateGridRenderer(GridVerticiesPayload v)
    {
        xStart = v.xStart;
        xEnd = v.xEnd;

        yStart = v.yStart;
        yEnd = v.yEnd;

        zStart = v.zStart;
        zEnd = v.zEnd;

        xDimension = v.xDimension;
        yDimension = v.yDimension;
        zDimension = v.zDimension;

    }

    #endregion

    #region Using Unity's Visual Debugging Tools
    /// <summary>
    /// For debugging purposes / can be used as a CoRoutine
    /// </summary>
    private void OnDrawGizmos()
    {
        // render xLines
        for (int x = 0; x < xDimension; x++)
        {
            for (int z = 0; z < zDimension; z++)
            {
                Gizmos.color = Color.blue; 
                Gizmos.DrawLine(xStart[x, z], xEnd[x, z]);
                
            }
        }
        // render yLines
        for (int y = 0; y < yDimension; y++)
        {
            for (int z = 0; z < zDimension; z++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(yStart[y, z], yEnd[y, z]);
                
            }
        }
        // render zLines
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                Gizmos.DrawLine(zStart[x, y], zEnd[x, y]);
                Gizmos.color = Color.green;
            }
        }
    }

    #endregion

    #region Custom Sprite Grid Rendering

    private void OnDrawGrid()
    {

    }

    #endregion
}
