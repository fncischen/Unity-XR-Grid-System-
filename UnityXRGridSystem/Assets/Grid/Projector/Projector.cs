using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class Projector : MonoBehaviour
    {

        #region public methods
        // place this method in the Projector GameObject     
        public void shiftGraph(GridVerticiesPayload g, Vector3 cameraShift)
        {

            #region notes
            //  how to shift using the gridVerticiesPayload? 

            // what type of problem am I solving // refactoring 

            // the problem I'm having is -> 
            // I hate individually changing each individual variable because
            // it requires me to remember each variable.


            // I prefer calling methods which manipulate multiple variables at once, rather than
            // try to do it at every step 

            // it adds extra cognitive load to the process, by repeating the steps needed t

            // what are the pros and cons of each use case. 


            #endregion

            #region steps 

            // I am interested in refactoring the gridPayload because 
            // I feel that there will be multiple use cases I haven't accounted for
            // that I want to prepare for. // think about single responsibility problem

            // question -> should this be triggered by an event listener? 

            // i.e. everytime there is a Time.deltaTime change, send an event to the delegate
            // so that this grid can make that change 

            // step 0) -> From the public interface, have a 
            // public method that allows for the shifting of the graph 
            // i.e. g.shiftGraph(cameraShift);

            // step 1 -> in ShiftGraph, access the existing from and to
            // public variables, shift the opposite direction of movement,
            // and 

            // step 2 -> change the start lengths and end lengths,
            // based on the SHIFT of the to and from variables. Internally
            // call g.changeGridLengths() method 

            // step 4) recalculate the unitDimensions, using the private method
            // g.changeDimensions()

            // step 5) g.setGridDimensions() // this will parse through all the 
            // grid private variables & set up grid dimensions. 

            // step 6) g.generateGridVerticies() this will autopopulate the grid dimensions
            // with vertice coordinates 

            // step 7) call the gridRenderer to rerender the  

            // this means that we need access to the 
            // to and from variables 

            #endregion

        }

        #endregion

        #region private methods 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridPayload"></param>
        /// <param name="i"></param>
        private void RenderGridTiles(GridVerticiesPayload gridPayload, Interactable i)
        {
            // use a circular render texture, that  
            //Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
            //RaycastHit hit;

            //if (Physics.Raycast(cameraRay, out hit, 100f) & hit.collider.GetComponent<Floor>())
            //{
            //    Vector3 floorPos = hit.collider.GetComponent<Floor>().transform.position;
            //    circle.SetActive(true);

            //    circle.transform.position = new Vector3(hit.point.x, floorPos.y, hit.point.z);

            //    // You need a way to map the grid shaders w/ 
            //    // relative to the mouse movement

            //}
            //else
            //{
            //    circle.SetActive(false);


            //}

            //for (int x = 0; x < g.xDimension; x++)
            //{
            //    for (int z = 0; z < g.zDimension; z++)
            //    {

            //    }
            //}
        }

        private void gridDetectionMethod()
        {

        }

        #endregion
    }
}