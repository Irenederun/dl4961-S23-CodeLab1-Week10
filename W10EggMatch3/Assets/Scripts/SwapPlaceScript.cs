using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwapPlaceScript : MonoBehaviour
{
    Vector3 objectPos;

    public GameObject highlightCircle;      //init the highlight circle around the gameObject
    public static bool hasSwapped;

    void Start()
    {
        //default: the highlight circle is not visible
        highlightCircle.SetActive(false);
        hasSwapped = false;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Clicked!");
            
            //when the mouse is over the gameObject and left key is clicked, show the highlight circle
            highlightCircle.SetActive(true);
            
            //if we haven't assigned a value for Transform object 1 yet
            if (GameManager.instance.object1 == null)
            {
               // Debug.Log("Object 1 clicked!");
                //the transform of the current gameObject is assigned to object1
                GameManager.instance.object1 = gameObject.transform;
            }
            
            //if object 1 is already assigned but object 2 is empty
            //and the current gameObject's transform is not the same as object 1
            //(basically avoid clicking on one gameObject twice)
            else if(!GameManager.instance.object2  && GameManager.instance.object1 != transform)
            {
                //Debug.Log("Object 2 clicked!");
                //the transform of the current gameObject is assigned to object2
                GameManager.instance.object2 = gameObject.transform;
                swap();
                GameManager.instance.ConnectThree(true);
                
                //have to specify which highlight to turn off
                GameManager.instance.object1.GetComponent<SwapPlaceScript>().highlightCircle.SetActive(false);
                GameManager.instance.object2.GetComponent<SwapPlaceScript>().highlightCircle.SetActive(false);
                
                //reset object 1 and object 2 to null
                GameManager.instance.object1 = null;
                GameManager.instance.object2 = null;
            }
        }
    }

    private void swap()
    {
        //swap value in grid
        //get the two objects' position and assign them to the Vector2 vars
        Vector2 obj1Pos = GameManager.instance.object1.position;
        Vector2 obj2Pos = GameManager.instance.object2.position;

        //get obj1 and obj2's grid values
        int obj1Val = GameManager.instance.grid[(int)obj1Pos.x, (int)obj1Pos.y];
        int obj2Val = GameManager.instance.grid[(int)obj2Pos.x, (int)obj2Pos.y];
        
        //swap the grid values
        GameManager.instance.grid[(int)obj1Pos.x, (int)obj1Pos.y] = obj2Val;
        GameManager.instance.grid[(int)obj2Pos.x, (int)obj2Pos.y] = obj1Val;

        //swap the positions
        GameManager.instance.object2.position = obj1Pos;
        GameManager.instance.object1.position = obj2Pos;
    }

    void Update()
    {
        //if object1 and object2 from GameManager are both true
        if (GameManager.instance.Swapped(GameManager.instance.object1, GameManager.instance.object2))
        {
            //disable the highlight circle gameObject
            highlightCircle.SetActive(false);
        }
    }
}
