using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{

    public Color normal;
    public Color hovering;
    public Color selected;

    private Rigidbody rb;

    public float grabDistanceRange;

    private float zCoord;
    private Vector3 zOffset; 

    public void Start()
    {
        normal = GetComponent<MeshRenderer>().material.color;
        rb = GetComponent<Rigidbody>();
    }


    // There is a depth limit to OnMouseDown
    // https://stackoverflow.com/questions/33229623/sometimes-onmousedown-method-in-unity-executes-sometimes-it-does-not

    public void OnMouseEnter()
    {
        Debug.Log("Entering object!");
    }

    public void OnMouseDown()
    {
        // only activate if you are at a certain distance 
        SetUpOffSet();
    }

    #region public Mouse eventhandlers
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
