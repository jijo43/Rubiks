using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePiecePosition : MonoBehaviour
{
    public List<IdentifyParent.Faces> faces = new List<IdentifyParent.Faces>();
    public string positionDescription;
    public GameController controller;
   
    private void Awake()
    {
        controller = FindFirstObjectByType<GameController>();
    }
    private void Update()
    {
        if (controller.hitCastRay)
        {
            IdentifyPosition();
         
        }
    }
    public void IdentifyPosition()
    {
        // Clear previous data
        faces.Clear();

        // Get all child quads with IdentifyParent component
        IdentifyParent[] childQuads = GetComponentsInChildren<IdentifyParent>();

        // Add each face to our list (prevent duplicates)
        foreach (IdentifyParent quad in childQuads)
        {
            if (!faces.Contains(quad.position))
            {
                faces.Add(quad.position);
            }
        }

        // Generate position description
        positionDescription = GeneratePositionDescription();

        // Output for debugging
        Debug.Log($"Cube piece: {gameObject.name} is at position: {positionDescription}");
    }

    private string GeneratePositionDescription()
    {
        if (faces.Count == 0)
            return "Unknown";

        // Create a list of strings to represent the faces
        List<string> faceStrings = new List<string>();
        foreach (IdentifyParent.Faces face in faces)
        {
            faceStrings.Add(face.ToString());
        }

        // Sort for consistent naming
        faceStrings.Sort();

        // Join with hyphens
        return string.Join("-", faceStrings);
    }
    public void AssignRightFaces()
    {
        if (controller.rightTransform == null)
        {
            Debug.LogWarning("Right Transform is not assigned!");
            return;
        }

        if (faces.Contains(IdentifyParent.Faces.right))
        {
            transform.SetParent(controller.rightTransform);
        }
    }

    //  NEW FUNCTION: Assign all Left faces
    public void AssignLeftFaces()
    {
        if (controller.leftTransform == null)
        {
            Debug.LogWarning("Left Transform is not assigned!");
            return;
        }

        if(faces.Contains(IdentifyParent.Faces.left))
        {
            transform.SetParent(controller.leftTransform);
        }
       
    }
    public void AssignFrontFaces()
    {
        if (controller.leftTransform == null)
        {
            Debug.LogWarning("Left Transform is not assigned!");
            return;
        }

        if (faces.Contains(IdentifyParent.Faces.front))
        {
            transform.SetParent(controller.frontTransform);
        }

    }
    public void AssignBackFaces()
    {
        if (controller.leftTransform == null)
        {
            Debug.LogWarning("Left Transform is not assigned!");
            return;
        }

        if (faces.Contains(IdentifyParent.Faces.back))
        {
            transform.SetParent(controller.backTransform);
        }

    }
    public void AssignTopFaces()
    {
        if (controller.leftTransform == null)
        {
            Debug.LogWarning("Left Transform is not assigned!");
            return;
        }

        if (faces.Contains(IdentifyParent.Faces.top))
        {
            transform.SetParent(controller.topTransform);
        }

    }
    public void AssignBottomFaces()
    {
        if (controller.leftTransform == null)
        {
            Debug.LogWarning("Left Transform is not assigned!");
            return;
        }

        if (faces.Contains(IdentifyParent.Faces.bottom))
        {
            transform.SetParent(controller.bottomTransform);
        }

    }
}