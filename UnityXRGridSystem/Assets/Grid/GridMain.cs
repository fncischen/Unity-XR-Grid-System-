using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class GridMain : MonoBehaviour
    {
        public Grid gridTable;
        public Grid largerGrid;

        public Vector3 gridOrigin;

        public Vector3 worldOrigin;
        public Vector3 from;
        public Vector3 to;

        public bool enableGridTableCollider;
        public bool enableWorldGridCollider;

        public float scale;
        /// <summary>
        /// This acts as the main control center / intermediary between two grids. 
        /// </summary>
        private void Start()
        {
            // I want to be able to change the gridOrigin here every time 
            //gridTable.SetColliderStatus(enableGridTableCollider);
            //gridTable.SetGridDimensions(gridOrigin, from, to);
            //gridTable.ConfigureGrid();
            //gridTable.RenderGrid();

            // But, I want to keep this static. 
            //largerGrid.SetColliderStatus(enableWorldGridCollider);
            //largerGrid.SetGridDimensions(worldOrigin, from, to);
            //largerGrid.RescaleGrid(scale);
            //largerGrid.ConfigureGrid();
            //largerGrid.RenderGrid();

            //SetupEventSubscriptions();
        }

        public void SetGrids()
        {
            // I want to be able to change the gridOrigin here every time 
            gridTable.SetColliderStatus(enableGridTableCollider);
            gridTable.SetGridDimensions(gridOrigin, from, to);
            gridTable.ConfigureGrid();
            gridTable.RenderGrid();

            // But, I want to keep this static. 
            largerGrid.SetColliderStatus(enableWorldGridCollider);
            largerGrid.SetGridDimensions(worldOrigin, from, to);
            largerGrid.RescaleGrid(scale);
            largerGrid.ConfigureGrid();
            largerGrid.RenderGrid();
        }

        //private void SetupEventSubscriptions()
        //{
        //    gridTable.sendToLargerGrid += GridTranslator;
        //}

        //private void GridTranslator(Vector3 tableCoordinates, Interactable interactable)
        //{
        //    // step 1
        //    Vector3 gridCoords = gridTable.TranslateFromCoordinateSpaceToGridSpace(tableCoordinates);
        //    // step 2
        //    Vector3 largerWorldGridCoords = largerGrid.TranslateFromGridSpaceToCoordinateSpace(gridCoords);
        //    // step 3 
        //    Interactable newLink = largerGrid.generateInteractableClone(largerWorldGridCoords, interactable);

        //}


        // this is like an interface which directly interacts with diff parts of the grid

    }

}