using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractablePayload
{

}
namespace UnityEngine.XR.ARFoundation
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {

        #region Color Types To Indicate Status 
        public Color normal;
        public Color hovering;
        public Color selected;
        #endregion

        public float grabDistanceRange;
        public TouchManager touchManager;

        public enum InteractableClass
        {
            Table,
            World
        }

        public InteractableClass typeOfInteractable;


        private Interactable interactablePair;
        private float zCoord;
        private Vector3 zOffset;

        #region public Interactable events  // for VR only

        // use these events to send messages to methods / other components which are connected to this interactable
        // to call specific methods 

        public delegate void OnInteracted(InteractablePayload g);

        // these account for both mouse plus touch events 

        public event OnInteracted GridTouched;

        public event OnInteracted OnObjectHover;

        public event OnInteracted OnObjectNearTouched;
        public event OnInteracted OnObjectTouched;
        public event OnInteracted OnObjectUnTouched;

        // for ARKit -> you're limited to the touch, tap, and drag actions
        // you can't differentiate by button input

        public event OnInteracted OnObjectGrabbed;
        public event OnInteracted OnObjectUnGrabbed;

        public event OnInteracted OnObjectSnapped;
        public event OnInteracted OnObjectUnSnapped;

        // pain point: I don't want to bind all objects to these events
        // since this leads to a situation where

        // i.e. Interactable a is a public Interactable for 100+ game objects.
        // but that Interactable is very unique

        // you could get around this by keeping the Interactable undefined until
        // there is a collision with another gameObject 

        // i.e. OnTriggerEnter() -> set the currentObjectInteractable equal to this Interactable
        // set up event subscription 

        // if the object is snapping to the controller
        // set the controller's current Interactable to equal this.
        // do the appropriate event subscriptions on the Controller

        #endregion


        public void Start()
        {
            normal = GetComponent<MeshRenderer>().material.color;

            EnableSubscriptions();
        }

        public void OnDisable()
        {
            DisableSubscriptions();
        }

        private void EnableSubscriptions()
        {
            subscribeToTouchEvents(GestureType.PinchGesture, rescaleObject);
            subscribeToTouchEvents(GestureType.RotateGesture, rotateObject);
        }

        private void DisableSubscriptions()
        {
            unsubscribeToTouchEvents(GestureType.PinchGesture, rescaleObject);
            unsubscribeToTouchEvents(GestureType.RotateGesture, rotateObject);
        }


        // There is a depth limit to OnMouseDown
        // https://stackoverflow.com/questions/33229623/sometimes-onmousedown-method-in-unity-executes-sometimes-it-does-not


        #region public Mouse eventhandlers
        public void OnMouseEnter()
        {
            Debug.Log("Entering object!");
        }

        public void OnMouseDown()
        {
            // only activate if you are at a certain distance 
            SetUpOffSet();
        }

        public void OnMouseDrag()
        {
            Debug.Log("Selecting object");
            onObjectGrab();
        }

        //public void OnMouseUp()
        //{
        //    Debug.Log("Letting go of object");
        //    onObjectUngrab();
        //}

        public void OnMouseOver()
        {
            Debug.Log("Hovering object");
            onHoverObject();

        }

        public void OnMouseExit()
        {
            Debug.Log("The mouse is away!");
            onObjectDeselected();
        }
        #endregion

        #region InteractableTableModifier 

        public void setInteractableLink(Interactable interactableToLink)
        {

        }

        #endregion

        #region Public Interactable Links Methods 

        public void SetInteractableClass(InteractableClass type)
        {
            typeOfInteractable = type;
        }

        // enable these event subscribers when a new Interactable is made.
        // this type of methods are SPECIFIC to this game 

        // are we merely subscribing two Interactables together?
        // this method should just be enabling subscriptions among interactables 
        public void EnableEventSubscribersBetweenInteractables()
        {



        }

        // disable these event subscribers when a Interactable is deleted.
        // this method should just be disabling subscriptions among interactables 
        public void DisableEventSubscribersBetweenInteractables()
        {



        }

        #endregion

        #region private Object modifiers

        private void SetUpOffSet()
        {
            zCoord = Camera.main.WorldToScreenPoint(transform.position).z;
            zOffset = this.transform.position - getMouseWorldPos();
        }

        private Vector3 getMouseWorldPos()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = zCoord;

            return Camera.main.ScreenToWorldPoint(mousePos);

        }

        private void onObjectGrab()
        {
            transform.position = getMouseWorldPos() + zOffset;
        }

        private void onObjectUngrab()
        {
            GetComponent<MeshRenderer>().material.color = hovering;

        }

        private void onHoverObject()
        {
            GetComponent<MeshRenderer>().material.color = hovering;

        }

        private void onObjectDeselected()
        {
            GetComponent<MeshRenderer>().material.color = normal;
        }

        #endregion

        #region gesture callbackMethods
        public void rescaleObject(TouchGesturePayload t)
        {
            Debug.Log("Interactable rescaling");
            transform.localScale = new Vector3(transform.localScale.x+t.touchDelta,transform.localScale.y+t.touchDelta, transform.localScale.z+t.touchDelta);
        }

        public void rotateObject(TouchGesturePayload t)
        {
            Debug.Log("Interactable rotating");
            transform.Rotate(0f,t.touchDelta,0f);
        }
        #endregion 

        #region ManagingEventSubscriptions
        protected void subscribeToTouchEvents(GestureType gestureType, TouchManager.OnGestureRecognized callbackMethod)
        {
            touchManager.SubscribeToGestureEvent(gestureType, callbackMethod);
        }

        protected void unsubscribeToTouchEvents(GestureType gestureType, TouchManager.OnGestureRecognized callbackMethod)
        {
            touchManager.UnsubscribeToGestureEvent(gestureType, callbackMethod);
        }
        #endregion

    }
}