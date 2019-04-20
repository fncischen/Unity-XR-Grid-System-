using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMain : MonoBehaviour
{
    public Grid gridTable;
    public Grid largerGrid; 

    public Vector3 gridOrigin;
    public Vector3 from;
    public Vector3 to;

    public float scale; 
    /// <summary>
    /// This acts as the main control center / intermediary between two grids. 
    /// </summary>
    private void Start()
    {
        gridTable.SetGridDimensions(gridOrigin, from, to);
        gridTable.ConfigureGrid();
        gridTable.RenderGrid();

        largerGrid.SetGridDimensions(gridOrigin, from, to);
        largerGrid.RescaleGrid(scale);
        largerGrid.ConfigureGrid();
        largerGrid.RenderGrid();

        SetupEventSubscriptions();
    }

    private void SetupEventSubscriptions()
    {
        gridTable.sendToLargerGrid += GridTranslator;
    }

    private void GridTranslator(Vector3 tableCoordinates, Interactable interactable)
    {
        // step 1
        Vector3 gridCoords = gridTable.TranslateFromCoordinateSpaceToGridSpace(tableCoordinates);
        // step 2
        Vector3 largerWorldGridCoords = largerGrid.TranslateFromGridSpaceToCoordinateSpace(gridCoords);
        // step 3 
        largerGrid.generateInteractableClone(largerWorldGridCoords, interactable);

    }

    
    // this is like an interface which directly interacts with diff parts of the grid

}
