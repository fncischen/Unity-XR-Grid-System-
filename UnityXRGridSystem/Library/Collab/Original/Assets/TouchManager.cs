﻿using System.Collections;
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
        public Vector2 panMovement;
        public Touch touch;
        public Touch secondTouch; 
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

        private Vector3 touchPosWorldPoint;

        private int fingerId;

        // minimum distance for a swipe to be registered 
        public float swipeRange;

        // minimum angle to register 
        public float minRotationGestureOffset; 
        public float rotationAngularOffset;
        public float rotationSpeedModifier; 

        // min deltaPinchMagnitude to register
        public float minDeltaPinchOffset;
        private Quaternion rotationY;
        private bool isPinching;

        private bool isRotated;
        private Vector2 startRotationVector;
        private Vector2 currentRotationVector; 

        private enum SwipeDirection
        {
            Left,
            Right,
            Up,
            Down
        }

        public Interactable currentInteractable;
        public Plane objPlane;  

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


        public TouchGesturePayload CreateTapTouchGesturePayload(Interactable i, Touch touched)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.touch = touched;
            return t; 
        }

        public TouchGesturePayload CreatePinchTouchGesturePayload(Interactable i, float touchDelta, Touch t1, Touch t2)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.touchDelta = touchDelta;
            t.touch = t1;
            t.secondTouch = t2;
            return t;
        }

        public TouchGesturePayload CreateRotationTouchGesturePayload(Interactable i, Vector2 angle, Touch t1, Touch t2)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.rotation = angle;
            t.touch = t1;
            t.secondTouch = t2;
            return t;
        }

        public TouchGesturePayload CreatePanTouchGesturePayload(Interactable i, Vector2 panMovement, Touch touched)
        {
            TouchGesturePayload t = new TouchGesturePayload();
            t.interactable = i;
            t.panMovement = panMovement;
            t.touch = touched; 
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
            if (Input.touchCount > 0)
            {
                switch (Input.touchCount)
                {

                    case 1:
                        //CheckForRotateOrTapGestures();
                        CheckForTapPanOrSwipeGestures();
                        break;
                    case 2:
                        CheckForPinchOrRotateGesture();
                        break;

                }
                    // https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
            }
            else {
                isPinching = false;
                isRotated = false; 
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
                prevTouchPos = touch.position;
               
                Ray touchRay = generateTouchRay(touch);
                RaycastHit hit;

                if (Physics.Raycast(touchRay.origin, touchRay.direction, out hit))
                {
                    currentInteractable = hit.collider.GetComponent<Interactable>();
                    Ray touchedRay = Camera.main.ScreenPointToRay(touch.position);

                    objPlane = new Plane(Camera.main.transform.forward * -1, currentInteractable.gameObject.transform.position);
                    float rayDistance;

                    objPlane.Raycast(touchedRay, out rayDistance);
                    touchPosWorldPoint = currentInteractable.gameObject.transform.position - touchedRay.GetPoint(rayDistance);

                    
                }

                    break;
            case TouchPhase.Ended:
                //if (touch.tapCount == 1)
                //{
                //    TapGestureRecognizer(touch);
                //    Debug.Log("Single Tap Counted");
                //}
                if (currentInteractable)
                {
                    recentTouchPos = Vector3.zero;
                    prevTouchPos = Vector3.zero;
                    currentInteractable = null;
                    Debug.Log("End swipe or panning movement");
                }
                break;
            case TouchPhase.Moved:
                if (touch.fingerId == fingerId)
                {
                    recentTouchPos = touch.position;

                    Vector3 touchDelta = recentTouchPos - prevTouchPos;

                    //if (Mathf.Abs(recentTouchPos.x - prevTouchPos.x) > swipeRange || Mathf.Abs(recentTouchPos.y - prevTouchPos.y) > swipeRange)
                    //{
                    //    // https://forum.unity.com/threads/simple-swipe-and-tap-mobile-input.376160/ 
                    //    if (Mathf.Abs(recentTouchPos.x - prevTouchPos.x) > Mathf.Abs(recentTouchPos.y - prevTouchPos.y))
                    //    {
                    //        if (recentTouchPos.x > prevTouchPos.x)
                    //        {
                    //            SwipeGestureRecognizer(SwipeDirection.Right, touchDelta);
                    //        }
                    //        else
                    //        {
                    //            SwipeGestureRecognizer(SwipeDirection.Left, touchDelta);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (recentTouchPos.y > prevTouchPos.y)
                    //        {
                    //            SwipeGestureRecognizer(SwipeDirection.Up, touchDelta);
                    //        }
                    //        else
                    //        {
                    //            SwipeGestureRecognizer(SwipeDirection.Up, touchDelta);
                    //        }
                    //    }
                    //    Debug.Log("Swipe movement!");
                    //}
                    if (currentInteractable)
                    {
                        PanGestureRecognizer(touchDelta, touch);
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

            // https://forum.unity.com/threads/rotation-gesture.87308/
            if (!isRotated | !isPinching)
            {

                startRotationVector = touchOne.position - touchZero.position;
                isRotated = startRotationVector.sqrMagnitude > minRotationGestureOffset * minRotationGestureOffset;

                isPinching = Vector2.Distance(touchOne.position, touchZero.position) > minDeltaPinchOffset;
            }
            else
            {
                currentRotationVector = touchOne.position - touchZero.position;
                float rotation = Vector2.Angle(currentRotationVector, startRotationVector);
                Debug.Log("Rotation: " + rotation);
                if (rotation > rotationAngularOffset)
                {
                    RotationGestureRecognizer(rotation, touchZero, touchOne);
                    Debug.Log("Rotating gesture!");
                }

                if (isPinching)
                {
                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    Debug.Log("Delta Magnitude Difference: " + deltaMagnitudeDiff);

                    // check for angular offSet 

                    // add pinch gesture deltaMagnitudeDiff minOffSet 
                    if (Mathf.Abs(deltaMagnitudeDiff) > minDeltaPinchOffset)
                    {
                        PinchGestureRecognizer(deltaMagnitudeDiff, touchZero, touchOne);
                        Debug.Log("Pinching gesture!");
                    }
                }

                
            }
    }


    #endregion

    #region private touch + methods 

    // let the action type and which UI / Game Object Element determine state 
    // mimicing the Swift Touch 
    private void TapGestureRecognizer(Touch t)
    {
            Interactable k;

            if (TouchHasHitInteractable(t,out k))
            {
                TouchGesturePayload g = CreateTapTouchGesturePayload(k,t);
                OnTapGesture?.Invoke(g);
            }
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
                currentInteractable = k;
                currentInteractable.transform.localScale = new Vector3(transform.localScale.x + pinchDifference, transform.localScale.y + pinchDifference, transform.localScale.z + pinchDifference);
                
                // question: what if this event listener is set simultaneously to all interactables? 
                
                //TouchGesturePayload g = CreatePinchTouchGesturePayload(k, pinchDifference, t1,t2);
                //OnPinchGesture?.Invoke(g);
            }
    }

    // use one finger to rotate 

    private void RotationGestureRecognizer(float angle, Touch t1, Touch t2)
    {
        Interactable k;

        if (TouchHasHitInteractable(t1,t2, out k))
        {
                currentInteractable = k;
                rotationY = Quaternion.Euler(0f, angle * rotationSpeedModifier, 0f);
                Debug.Log(rotationY);
                currentInteractable.transform.rotation *= rotationY;
            //TouchGesturePayload g = CreateTouchGesturePayload(k, angle, t1,t2);
            //OnPinchGesture?.Invoke(g);
        }
    }


    private void SwipeGestureRecognizer(SwipeDirection swipeDirection, Vector3 touchDelta)
    {

    }

    // use two gestures to pan 

    private void PanGestureRecognizer(Vector2 panDelta, Touch t1)
    {
      
                // touch pos offset
            Ray touchedRay = Camera.main.ScreenPointToRay(t1.position);

            float rayDistance; 

            if (objPlane.Raycast(touchedRay, out rayDistance))
            {
                    currentInteractable.transform.position = touchedRay.GetPoint(rayDistance) + touchPosWorldPoint;
                    Debug.Log("Object is moving");

            }

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

        Ray generateTouchRay(Touch t)
        {
            Vector3 touchPosFar = new Vector3(t.position.x, t.position.y, Camera.main.farClipPlane);
            Vector3 touchPosNear = new Vector3(t.position.x, t.position.y, Camera.main.nearClipPlane);

            Vector3 touchPosF = Camera.main.ScreenToWorldPoint(touchPosFar);
            Vector3 touchPosN = Camera.main.ScreenToWorldPoint(touchPosNear);

            Ray r = new Ray(touchPosN, touchPosF - touchPosN);
            return r; 
        }

        private bool TouchHasHitInteractable(Touch t1, out Interactable k)
        {
            Ray rayTouch1 = generateTouchRay(t1);

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
        
        Ray rayTouch1 = generateTouchRay(t1);
        Ray rayTouch2 = generateTouchRay(t2);

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