using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceRotation : MonoBehaviour
{
    [SerializeField] Transform frontFace;
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
           
            frontFace.rotation = new Quaternion(0, 0, -90f,0);  
        }
    }
}
