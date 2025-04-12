using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the Grid postion to where the mouse pointer is pointing to 
/// </summary>
public class GridCellPosition : MonoBehaviour
{
    // Reference to Text field to display the unit's position
    [SerializeField] Text UITextElement;

    // Update is called once per frame
    void Update()
    {
        //Creating Ray from camera to where mouse is pointing
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;
            //if ray hit the cube then get it's tranform postion and display on the text field
            if (hoveredObject != null)
            {
                Vector3 pos = hoveredObject.GetComponent<Transform>().position;
                UITextElement.text = pos.ToString();
            }
        }
        
    }
}
