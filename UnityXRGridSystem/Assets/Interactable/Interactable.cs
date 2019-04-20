using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{

    #region Color Types To Indicate Status 
    public Color normal;
    public Color hovering;
    public Color selected;
    #endregion 

    public float grabDistanceRange;


    public enum InteractableClass
    {
        Table,
        World
    }

    public InteractableClass typeOfInteractable;


    private Interactable interactablePair; 
    private float zCoord;
    private Vector3 zOffset; 

    public void Start()
    {
        normal = GetComponent<MeshRenderer>().material.color;
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

    }


    public void EnableEventSubscribers()
    {



    }

    public void DisableEventSubscribers()
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


}
