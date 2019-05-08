using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SpatialTracking;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.ARFoundation
{

    public struct TouchGesturePayload
    {
        public Interactable interactable;
        public float touchDelta;
        public Vector2 rotation;
    }

    public enum GestureType
    {
        TapGesture,
        PinchGesture,
        RotateGesture,
        PanGesture,
        SwipeGesture
    }

    public class TouchManager : MonoBehaviour
    {
        #region public & private variables
        public ARSessionOrigin session;

        private Vector3 recentTouchPos;
        private Vector3 prevTouchPos;

        private int fingerId;

        // minimum distance for a swipe to be registered 
        public float swipeRange;

        // minimum angle to register 
        public float rotationAngularOffset;

        // min deltaPinchMagnitude to register
        public float minDeltaPinchOffset;

        private enum SwipeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        #endregion

        #region public delegates and gesture types

        public delegate void OnGestureRecognized(TouchGesturePayload g);
        public event OnGestureRecognized OnTapGesture;
        public event OnGestureRecognized OnPinchGesture;
        public event OnGestureRecognized OnRotateGesture;
        public event OnGestureRecognized OnPanGesture;
        public event OnGestureRecognized OnSwipeGesture;

        #endregion 

        #region public configuration methods

        public void SetUpEventSubscriptions()
        {

        }


        public TouchGesturePayload CreateTouchGesturePayload(Interactable i, float touchDelta)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.touchDelta = touchDelta;
            return t;
        }

        public TouchGesturePayload CreateTouchGesturePayload(Interactable i, Vector2 angle)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.rotation = angle;
            return t;
        }

        #endregion

        #region public methods 

        public void Start()
        {
            SetUpEventSubscriptions();
        }

        public void Update()
        {
            manageTouches();
        }

        /// <summary>
        /// Checks each gesture behaviour ----> depending on the
        /// UI element / GameObject, send an action to that GameObject/Component,
        /// and let the object take care of the object. 
        /// </summary>

        public virtual void manageTouches()
        {
            switch (Input.touchCount)
            {

                case 1:
                    CheckForPanOrScaleGestures();
                    break;
                case 2:
                    CheckForRotateOrTapGestures();
                    break;

                    // https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
            }
        }
        #endregion 

        #region touchRecognizers

        // https://stackoverflow.com/questions/9898627/what-is-the-difference-between-pan-and-swipe-in-ios

        // 
        private void CheckForPanOrScaleGestures()
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // get average position between T1 and T2. 
            Vector2 averagePrevTouchPos = (touchOnePrevPos - touchZeroPrevPos) / 2;
            Vector2 averageTouchPos = (touchOne.position - touchZero.position) / 2;

            Vector2 touchSwipeDelta = averageTouchPos - averagePrevTouchPos; 


            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // check either if the object is panning -> ( math: see if the delta difference magnitude diff is 0);
            // check either if the object is scaling -> ( see if the delta is above pinch offset); 

            if (deltaMagnitudeDiff > minDeltaPinchOffset)
            {
                PinchGestureRecognizer(deltaMagnitudeDiff, touchZero, touchOne);
                Debug.Log("Pinching gesture!");
            }
            else if (touchSwipeDelta.magnitude > 0)
            {
                PanGestureRecognizer(touchSwipeDelta);
            }

        }

        private void CheckForRotateOrTapGestures()
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    fingerId = touch.fingerId;
                    prevTouchPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    if (touch.tapCount == 1)
                    {
                        TapGestureRecognizer();
                        Debug.Log("Single Tap Counted");
                    }
                    else
                    {
                        recentTouchPos = Vector3.zero;
                        prevTouchPos = Vector3.zero;
                        Debug.Log("End rotation");
                    }
                    break;
                case TouchPhase.Moved:
                    if (touch.fingerId == fingerId)
                    {
                        recentTouchPos = touch.position;

                        Vector2 touchDelta = recentTouchPos - prevTouchPos;

                        // use the Vec2 touchDelta to do a Quanterion rotation, based on 
                        // touchDelta data 
                        RotationGestureRecognizer(touchDelta,touch);

                    }
                    break;
            }
        }

    //private void CheckForTapPanOrSwipeGestures()
    //{
    //    Touch touch = Input.GetTouch(0);

    //    switch (touch.phase)
    //    {
    //        case TouchPhase.Began:
    //            fingerId = touch.fingerId;
    //            prevTouchPos = touch.position;
    //            break;
    //        case TouchPhase.Ended:
    //            if (touch.tapCount == 1)
    //            {
    //                TapGestureRecognizer();
    //                Debug.Log("Single Tap Counted");
    //            }
    //            else
    //            {
    //                recentTouchPos = Vector3.zero;
    //                prevTouchPos = Vector3.zero;
    //                Debug.Log("End swipe or panning movement");
    //            }
    //            break;
    //        case TouchPhase.Moved:
    //            if (touch.fingerId == fingerId)
    //            {
    //                recentTouchPos = touch.position;

    //                Vector3 touchDelta = recentTouchPos - prevTouchPos;

    //                if (Mathf.Abs(recentTouchPos.x - prevTouchPos.x) > swipeRange || Mathf.Abs(recentTouchPos.y - prevTouchPos.y) > swipeRange)
    //                {
    //                    // https://forum.unity.com/threads/simple-swipe-and-tap-mobile-input.376160/ 
    //                    if (Mathf.Abs(recentTouchPos.x - prevTouchPos.x) > Mathf.Abs(recentTouchPos.y - prevTouchPos.y))
    //                    {
    //                        if (recentTouchPos.x > prevTouchPos.x)
    //                        {
    //                            SwipeGestureRecognizer(SwipeDirection.Right, touchDelta);
    //                        }
    //                        else
    //                        {
    //                            SwipeGestureRecognizer(SwipeDirection.Left, touchDelta);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (recentTouchPos.y > prevTouchPos.y)
    //                        {
    //                            SwipeGestureRecognizer(SwipeDirection.Up, touchDelta);
    //                        }
    //                        else
    //                        {
    //                            SwipeGestureRecognizer(SwipeDirection.Up, touchDelta);
    //                        }
    //                    }
    //                    Debug.Log("Swipe movement!");
    //                }
    //                else
    //                {
    //                    PanGestureRecognizer(touchDelta);
    //                    Debug.Log("Panning movement!");
    //                }
    //            }
    //            break;
    //    }
    //}


    //private void CheckForPinchOrRotateGesture()
    //{
    //    // get both touches 
    //    Touch touchZero = Input.GetTouch(0);
    //    Touch touchOne = Input.GetTouch(1);

    //    // Find the position in the previous frame of each touch.
    //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    //    // Find the magnitude of the vector (the distance) between the touches in each frame.
    //    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

    //    // Find the difference in the distances between each frame.
    //    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

    //    var rotation = Vector2.Angle(touchOne.position, touchZero.position);


    //    Debug.Log("Rotation: " + rotation);
    //    if (rotation > rotationAngularOffset)
    //    {
    //        // RotationGestureRecognizer(rotation, touchZero, touchOne);
    //        Debug.Log("Rotating gesture!");
    //    }
    //    // check for angular offSet 

    //    // add pinch gesture deltaMagnitudeDiff minOffSet 
    //    else if (deltaMagnitudeDiff > minDeltaPinchOffset)
    //    {
    //        PinchGestureRecognizer(deltaMagnitudeDiff, touchZero, touchOne);
    //        Debug.Log("Pinching gesture!");
    //    }
    //}


    #endregion

    #region private touch + methods 

    // let the action type and which UI / Game Object Element determine state 
    // mimicing the Swift Touch 
    private void TapGestureRecognizer()
    {

    }

    private void LongPressGestureRecognizer()
    {

    }

    // good for zooming in / zooming out 

    private void PinchGestureRecognizer(float pinchDifference, Touch t1, Touch t2)
    {
        Interactable k;

        if (TouchHasHitInteractable(t1, t2, out k))
        {
            TouchGesturePayload g = CreateTouchGesturePayload(k, pinchDifference);
            OnPinchGesture?.Invoke(g);
        }
    }

    // use one finger to rotate 

    private void RotationGestureRecognizer(Vector2 angle, Touch t)
    {
        Interactable k;

        if (TouchHasHitInteractable(t, out k))
        {
            TouchGesturePayload g = CreateTouchGesturePayload(k, angle);
            OnPinchGesture?.Invoke(g);
        }
    }


    private void SwipeGestureRecognizer(SwipeDirection swipeDirection, Vector3 touchDelta)
    {

    }

    // use two fingers to pan 

    private void PanGestureRecognizer(Vector2 panDelta)
    {

    }

    private void ScreenEdgePanGestureRecognizer(SwipeDirection swipeDirection)
    {

    }

    #endregion

    #region public event subscription manager methods
    // a public API that allows me to set up event subscriptions from outside this method, as long as the delegate is OnGestureRecognized()

    public virtual void SubscribeToGestureEvent(GestureType gestureType, OnGestureRecognized callbackMethod)
    {
        ManageGestureEvents(gestureType, true, callbackMethod);
    }

    public virtual void UnsubscribeToGestureEvent(GestureType gestureType, OnGestureRecognized callbackMethod)
    {
        ManageGestureEvents(gestureType, false, callbackMethod);
    }


    protected virtual void ManageGestureEvents(GestureType gestureType, bool state, OnGestureRecognized callBackMethod)
    {
        switch (gestureType)
        {
            case GestureType.PanGesture:
                ManagePanGesture(state, callBackMethod);
                break;
            case GestureType.PinchGesture:
                ManagePinchGesture(state, callBackMethod);
                break;
            case GestureType.RotateGesture:
                ManageRotateGesture(state, callBackMethod);
                break;
            case GestureType.SwipeGesture:
                ManageSwipeGesture(state, callBackMethod);
                break;
            case GestureType.TapGesture:
                ManageTapGesture(state, callBackMethod);
                break;
        }
    }
    #endregion

    #region private gesture subscription methods

    protected virtual void ManagePanGesture(bool register, OnGestureRecognized callBackMethod)
    {
        if (register)
        {
            OnPanGesture += callBackMethod;
        }
        else
        {
            OnPanGesture -= callBackMethod;
        }
    }

    protected virtual void ManagePinchGesture(bool register, OnGestureRecognized callBackMethod)
    {
        if (register)
        {
            OnPinchGesture += callBackMethod;
        }
        else
        {
            OnPinchGesture -= callBackMethod;
        }
    }

    protected virtual void ManageRotateGesture(bool register, OnGestureRecognized callBackMethod)
    {
        if (register)
        {
            OnRotateGesture += callBackMethod;
        }
        else
        {
            OnRotateGesture -= callBackMethod;
        }
    }

    protected virtual void ManageSwipeGesture(bool register, OnGestureRecognized callBackMethod)
    {
        if (register)
        {
            OnSwipeGesture += callBackMethod;
        }
        else
        {
            OnSwipeGesture -= callBackMethod;
        }
    }

    protected virtual void ManageTapGesture(bool register, OnGestureRecognized callBackMethod)
    {
        if (register)
        {
            OnTapGesture += callBackMethod;
        }
        else
        {
            OnTapGesture -= callBackMethod;
        }
    }


        #endregion

        #region private helper methods 

        private bool TouchHasHitInteractable(Touch t1, out Interactable k)
        {
            Ray rayTouch1 = Camera.main.ScreenPointToRay(t1.position);

            RaycastHit hitted;

            bool hitInteractable = Physics.Raycast(rayTouch1, out hitted) && hitted.collider.GetComponent<Interactable>();

            if (hitInteractable)
            {
                k = hitted.collider.GetComponent<Interactable>();
                return true;
            }
            else
            {
                k = null;
                return false;
            }
        }

        private bool TouchHasHitInteractable(Touch t1, Touch t2, out Interactable k)
        {
        Ray rayTouch1 = Camera.main.ScreenPointToRay(t1.position);
        Ray rayTouch2 = Camera.main.ScreenPointToRay(t2.position);

        RaycastHit hitted;
        RaycastHit hitted2;

        bool hitInteractable = Physics.Raycast(rayTouch1, out hitted) && hitted.collider.GetComponent<Interactable>();
        bool hitInteractableTwo = Physics.Raycast(rayTouch2, out hitted2) && hitted2.collider.GetComponent<Interactable>();

        bool sameInteractable = (hitted.collider.GetComponent<Interactable>() == hitted2.collider.GetComponent<Interactable>());

        if (hitInteractable && hitInteractableTwo && sameInteractable)
        {
            k = hitted.collider.GetComponent<Interactable>();
            return true;
        }
        else
        {
            k = null;
            return false;
        }
    }

    #endregion
    }
}