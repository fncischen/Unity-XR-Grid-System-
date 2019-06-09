using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class TileManager : MonoBehaviour
    {
        public float TileFallHeight;
        public float TileBaseHeight;
        public Transform LandingSpot;

        public Grid gridTiles;
        public Bounds streetBounds;

        private Vector3[,,] tilePositions;
        public DynamicTile tile;

        public AnimationCurve movingCurve;
        public float TileScale; 

        // needed for retrieving tile information 
        private DynamicTile[,,] dynamicTiles;

        // Start is called before the first frame update

        #region public methods
        public void EnableEventSubscriptions()
        {

        }

        public void DisableEventSubscriptions()
        {

        }
        #endregion

        #region public tile rising / falling methods   

        public void MakeTilesRise()
        {
            SetTileAtFloor();
            SetTilesRise();
        }

        public void MakeTilesFall()
        {
            SetTileAtHeight();
            SetTilesFall();
        }

        public void GenerateTilesByGrid()
        {
            tilePositions = gridTiles.g.gridCellsPositions;
            dynamicTiles = new DynamicTile[gridTiles.g.xDimension, gridTiles.g.yDimension, gridTiles.g.zDimension];

            Debug.Log(gridTiles.g.xDimension);
            Debug.Log(gridTiles.g.yDimension);
            Debug.Log(gridTiles.g.zDimension);
            Debug.Log("Cell Size: " + gridTiles.g.CellSize);

            for (int x = 0; x < gridTiles.g.xDimension; x++)
            {
                for (int y = 0; y < gridTiles.g.yDimension; y++)
                {
                    for (int z = 0; z < gridTiles.g.zDimension; z++)
                    {
                        Quaternion q = new Quaternion(1,0,0,1);
                        GameObject h = Instantiate(tile.gameObject, tilePositions[x, y, z], q);
                        h.transform.position = new Vector3(h.transform.position.x, TileFallHeight, h.transform.position.z);
                        h.transform.localScale = new Vector3(TileScale,TileScale,TileScale);
                        dynamicTiles[x, y, z] = h.GetComponent<DynamicTile>();
                        Debug.Log("Writing grid:", dynamicTiles[x, y, z]);
                    }
                }
            }
        }

        #endregion

        #region tileMethods 
        private void SetTileAtFloor()
        {

            for (int x = 0; x < gridTiles.g.xDimension; x++)
            {
                for (int y = 0; y < gridTiles.g.yDimension; y++)
                {
                    for (int z = 0; z < gridTiles.g.zDimension; z++)
                    {

                        float xPos = dynamicTiles[x, y, z].gameObject.transform.position.x;
                        float zPos = dynamicTiles[x, y, z].gameObject.transform.position.z;
                        dynamicTiles[x, y, z].gameObject.transform.position = new Vector3(xPos, TileBaseHeight, zPos);

                    }
                }
            }
        }

        private void SetTileAtHeight()
        { 

            for (int x = 0; x < gridTiles.g.xDimension; x++)
            {
                for (int y = 0; y < gridTiles.g.yDimension; y++)
                {
                    for (int z = 0; z < gridTiles.g.zDimension; z++)
                    {
                        float xPos = dynamicTiles[x, y, z].gameObject.transform.position.x;
                        float zPos = dynamicTiles[x, y, z].gameObject.transform.position.z;
                        dynamicTiles[x, y, z].gameObject.transform.position = new Vector3(xPos,TileFallHeight,zPos);
                    }
                }
            }
        }

        private void SetTilesFall()
        {

            for (int x = 0; x < gridTiles.g.xDimension; x++)
            {
                for (int y = 0; y < gridTiles.g.yDimension; y++)
                {
                    for (int z = 0; z < gridTiles.g.zDimension; z++)
                    {
                        StartCoroutine(InitiateTileFall(dynamicTiles[x, y, z].gameObject, TileBaseHeight));
                    }
                }
            }
        }

        private void SetTilesRise()
        {
            for (int x = 0; x < gridTiles.g.xDimension; x++)
            {
                for (int y = 0; y < gridTiles.g.yDimension; y++)
                {
                    for (int z = 0; z < gridTiles.g.zDimension; z++)
                    {
                        StartCoroutine(InitiateTileRise(dynamicTiles[x, y, z].gameObject, TileFallHeight));
                    }
                }
            }
        }

        

        #endregion

        #region mainTileAnimationScripts

        // how to animate physics animation 
        IEnumerator InitiateTileFall(GameObject tile, float baselineHeight)
        {
            float animationTimePos = 0;
            Vector3 targetPos = new Vector3(tile.transform.position.x, baselineHeight, tile.transform.position.z);
            while (!IsTileTouchingFloor(tile, baselineHeight))
            {
                tile.transform.position = Vector3.Lerp(tile.transform.position, targetPos, movingCurve.Evaluate(animationTimePos));
                animationTimePos += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator InitiateTileRise(GameObject tile, float mainHeight)
        {
            float animationTimePos = 0;
            Vector3 targetPos = new Vector3(tile.transform.position.x, mainHeight, tile.transform.position.z);
            while (!HasTileReachedMainHeight(tile, mainHeight))
            {
                tile.transform.position = Vector3.Lerp(tile.transform.position, targetPos, movingCurve.Evaluate(animationTimePos));
                animationTimePos += Time.deltaTime;
                yield return null;
            }

        }
        #endregion

        #region helperMethods
        private bool IsTileTouchingFloor(GameObject tile, float baselineHeight)
        {
            if (tile.transform.position.y <= baselineHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasTileReachedMainHeight(GameObject tile, float mainHeight)
        {
             // Debug.Log("tile current Height: "+ tile.transform.position.y + " main Height: " + mainHeight);
            if (tile.transform.position.y <= mainHeight)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion


    }
}
