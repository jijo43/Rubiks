using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifyParent : MonoBehaviour
{
    RaycastHit hit;
    public Color rayColor = Color.red;
    public Faces position;
    
    public GameController controller;

    public enum Faces
    {
        front,back, top, right, bottom,left
    }
    private void Awake()
    {
        controller = FindFirstObjectByType<GameController>();
    }
    void Update()
    {
        
        Vector3 rayDirection = -transform.forward;
        Debug.DrawRay(transform.position, rayDirection * 5f, rayColor);

        if (Physics.Raycast(transform.position, rayDirection, out hit,5f) && controller.hitCastRay)
        {
            switch (hit.collider.tag)
            {
                case "Top":
                   // Debug.Log("Top face");
                    position = (Faces.top);
                    break;
                case "Bottom":
                    //Debug.Log("Bottom face");
                    position = (Faces.bottom);
                    break;
                case "Left":
                   // Debug.Log("Left face");
                    position = (Faces.left);
                    break;
                case "Right":
                    //Debug.Log("Right face");
                    position = (Faces.right);
                    break;
                case "Front":
                   // Debug.Log("Front face");
                    position = (Faces.front);
                    break;
                case "Back":
                    //Debug.Log("Back face");
                    position = (Faces.back);
                    break;
                default:
                    Debug.Log(gameObject.name + " detected " + hit.collider.tag);
                    break;
            }
           
        }
        
    }
}