using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class CameraScript : MonoBehaviour
    {
        public TouchManager t;

        public void Start()
        {
            EnableEventSubscriptions();
        }

        public void OnDisable()
        {
            DisableEventSubscriptions();
        }

        // what I like about this method is that I can selectely set up event subscriptions here, rather than call the private methods 
        // the problem with calling the actual methods directly is that: 

        // remembering each of the names of each method 
        // rewriting the method each time

        // the big problem is that throughout the software development process 
        // we may not know which methods, game objects, or components that
        // we have to set up an event subscription to. 

        // WE JUST DONT KNOW 

        // WE JUST DON'T KNOW. 

       // if we do, it requires the cognitive load and extra step of writing down each method by name.

        // from a programmers UX perspective, why not have a set up categories of gestures to keep track of
        // and subscribe to each event based on the gesture type, rather than the event method. It feels more human to say, "Let's wait for a Gesture" rather than
        // "Subscribe to a Gesture" 

        private void EnableEventSubscriptions()
        {
            t.SubscribeToGestureEvent(GestureType.PanGesture, PanCamera);
            t.SubscribeToGestureEvent(GestureType.PinchGesture, ZoomCamera);
        }

        private void DisableEventSubscriptions()
        {
            t.UnsubscribeToGestureEvent(GestureType.PanGesture, PanCamera);
            t.UnsubscribeToGestureEvent(GestureType.PinchGesture, ZoomCamera);
        }

        public void PanCamera(TouchGesturePayload t)
        {

        }

        public void ZoomCamera(TouchGesturePayload t)
        {

        }
    }
}
