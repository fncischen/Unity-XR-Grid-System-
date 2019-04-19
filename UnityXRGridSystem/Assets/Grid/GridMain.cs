using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : MonoBehaviour
{
    public GridPayload gridTable;
    public GridPayload largerGrid; 

    public Vector3 gridOrigin;
    public Vector3 from;
    public Vector3 to;

    public float scale; 

    private void Start()
    {
        gridTable.SetGridDimensions(gridOrigin, from, to);
        gridTable.ConfigureGrid();
        gridTable.RenderGrid();

        largerGrid.SetGridDimensions(gridOrigin, from, to);
        largerGrid.RescaleGrid(scale);
        largerGrid.ConfigureGrid();
        largerGrid.RenderGrid();
    }

    public void GridTranslator(Vector3 tableCoordinates)
    {

    }

    
    // this is like an interface which directly interacts with diff parts of the grid

}
