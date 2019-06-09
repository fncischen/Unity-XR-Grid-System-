using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class SetGameConfigration : MonoBehaviour
    {
        // place all necessary managers here to handle configuration 
        public GridMain gridManager;
        public TileManager tileManager;

        public void Start()
        {
            gridManager.SetGrids();
            Debug.Log("grid:" + gridManager.gridTable.g.yDimension);
            tileManager.GenerateTilesByGrid();
        }
    }
}