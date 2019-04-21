using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SpatialTracking;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARFoundation
{
    public struct GestureRecognizerObject
    {
        Touch touch;
    }

    public class TouchManager : MonoBehaviour
    {
        public ARSessionOrigin session;

        private Vector3 firstTouchPosition;
        private Vector3 lastTouchPosition;

        private int fingerId;

        // minimum distance for a swipe to be registered 
        public float swipeRange;



        // minimum angle to register 
        public float rotationAngularOffset; 

        private enum SwipeDirection
        {
            Left,
            Right,
            Up,
            Down 
        }

        #region public methods 
        public void Update()
        {
            manageTouches();
        }

        /// <summary>
        /// Checks each gesture behaviour ----> depending on the
        /// UI element / GameObject, send an action to that GameObject/Component,
        /// and let the object take care of the object. 
        /// </summary>

        public void manageTouches()
        {
            switch (Input.touchCount) { 

                case 1: 
                    CheckForTapPanOrSwipeGestures();
                    break;
                case 2: 
                    CheckForPinchOrRotateGesture();
                    break;

                    // https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
            }
        }
        #endregion 

        #region touchRecognizers

        // https://stackoverflow.com/questions/9898627/what-is-the-difference-between-pan-and-swipe-in-ios

        private void CheckForTapPanOrSwipeGestures()
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    fingerId = touch.fingerId;
                    lastTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended: 
                    if (touch.tapCount == 1)
                    {
                        TapGestureRecognizer();
                        Debug.Log("Single Tap Counted");
                    }
                    else
                    {
                        firstTouchPosition = Vector3.zero;
                        lastTouchPosition = Vector3.zero;
                        Debug.Log("End swipe or panning movement");
                    }
                    break;
                case TouchPhase.Moved:
                    if (touch.fingerId == fingerId)
                    {
                        firstTouchPosition = touch.position;

                        Vector3 touchDelta = firstTouchPosition - lastTouchPosition;

                        if (Mathf.Abs(firstTouchPosition.x - lastTouchPosition.x) > swipeRange || Mathf.Abs(firstTouchPosition.y - lastTouchPosition.y) > swipeRange)
                        {
                            // https://forum.unity.com/threads/simple-swipe-and-tap-mobile-input.376160/ 
                            if (Mathf.Abs(firstTouchPosition.x - lastTouchPosition.x) > Mathf.Abs(firstTouchPosition.y - lastTouchPosition.y))
                            {
                                if (firstTouchPosition.x > lastTouchPosition.x)
                                {
                                    SwipeGestureRecognizer(SwipeDirection.Right);
                                }
                                else
                                {
                                    SwipeGestureRecognizer(SwipeDirection.Left);
                                }
                            }
                            else
                            {
                                if (firstTouchPosition.y > lastTouchPosition.y)
                                {
                                    SwipeGestureRecognizer(SwipeDirection.Up);
                                }
                                else
                                {
                                    SwipeGestureRecognizer(SwipeDirection.Up);
                                }
                            }
                            Debug.Log("Swipe movement!");
                        }
                        else
                        {
                            PanGestureRecognizer(touchDelta);
                            Debug.Log("Panning movement!");
                        }
                    }
                    break; 
            }
        }


        private void CheckForPinchOrRotateGesture()
            {
                // get both touches 
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                var rotation = Vector2.Angle(touchOne.position, touchZero.position);
                if (rotation > rotationAngularOffset)
                {
                     RotationGestureRecognizer(rotation);
                    Debug.Log("Rotating gesture!");
                }
                // check for angular offSet 
                else
                {
                    PinchGestureRecognizer(deltaMagnitudeDiff);
                    Debug.Log("Pinching gesture!");
                }
            }


        #endregion

        #region private touch + methods 

        // let the action type and which UI / Game Object Element determine state 
        // mimicing the Swift Touch 
        private void TapGestureRecognizer()
        {

        }

        private void PinchGestureRecognizer(float pinchDifference)
        {

        }

        private void RotationGestureRecognizer(float angle)
        {

        }

        private void SwipeGestureRecognizer(SwipeDirection swipeDirection)
        {

        }

        private void PanGestureRecognizer(Vector3 panDelta)
        {

        }

        private void ScreenEdgePanGestureRecognizer()
        {

        }

        private void LongPressGestureRecognizer()
        {

        }

        #endregion 

    }
}