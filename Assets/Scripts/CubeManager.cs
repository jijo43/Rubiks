using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameController controller;
    public GameObject mainCube; // The top Cube object
    public List<GameObject> cubePieces = new List<GameObject>();

    
    [Header("References to parent face objects")]
    public Transform frontFace;
    public Transform backFace;
    public Transform leftFace;
    public Transform rightFace;

    
    [Header("Rotation settings")]
    public float rotationDuration = 0.5f;
    private bool isRotating = false;

    public bool identifyPositions = false;

    [System.Serializable]
    public class CubePieceInfo
    {
        public GameObject piece;
        public List<IdentifyParent.Faces> faces = new List<IdentifyParent.Faces>();
        public string positionDescription;
        public IdentifyParent.Faces primaryFace; 
    }

    public List<CubePieceInfo> pieceInfos = new List<CubePieceInfo>();
    public CubePiecePosition[] allPieces;
    private void Awake()
    {
        controller = FindFirstObjectByType<GameController>();
  
         allPieces = FindObjectsByType<CubePiecePosition>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }
    void Start()
    {
        // Find all cube pieces
        FindAllCubePieces();

        // Set default face references if not assigned
        if (!frontFace) frontFace = mainCube.transform.Find("FrontFace");
        if (!backFace) backFace = mainCube.transform.Find("BackFace");
        if (!leftFace) leftFace = mainCube.transform.Find("FrontFace/Left");
        if (!rightFace) rightFace = mainCube.transform.Find("FrontFace/Right");
    }

    void Update()
    {
        if (identifyPositions && !isRotating)
        {
            IdentifyAllPiecePositions();
            ReparentCubePieces();
            identifyPositions = false;
        }

      
    }

    public void FindAllCubePieces()
    {
        cubePieces.Clear();

  
        Transform[] frontSections = new Transform[] {
            frontFace?.Find("Left/Top"),
            frontFace?.Find("Left/Bottom"),
            frontFace?.Find("Right/Top"),
            frontFace?.Find("Right/Bottom")
        };

     
        Transform[] backSections = new Transform[] {
            backFace?.Find("Left/Top"),
            backFace?.Find("Left/Bottom"),
            backFace?.Find("Right/Top"),
            backFace?.Find("Right/Bottom")
        };

        // Combine all sections
        List<Transform> allSections = new List<Transform>();
        allSections.AddRange(frontSections);
        allSections.AddRange(backSections);

        // Add cube pieces from all sections
        foreach (Transform section in allSections)
        {
            if (section == null) continue;

            foreach (Transform child in section)
            {
                if (child.name.Contains("Cube") && !child.name.Contains("NOTRequired"))
                {
                    cubePieces.Add(child.gameObject);
                }
            }
        }

        Debug.Log($"Found {cubePieces.Count} cube pieces");
    }

    public void IdentifyAllPiecePositions()
    {
        // Clear previous data
        pieceInfos.Clear();

        // Finding GameController
      //  GameController controller = FindFirstObjectByType<GameController>();
       // controller.hitCastRay = true;

        // Process each cube piece
        foreach (GameObject piece in cubePieces)
        {
            CubePieceInfo info = new CubePieceInfo();
            info.piece = piece;

            
            IdentifyParent[] quads = piece.GetComponentsInChildren<IdentifyParent>();

          
            foreach (IdentifyParent quad in quads)
            {
                if (!info.faces.Contains(quad.position))
                {
                    info.faces.Add(quad.position);
                }
            }

        
            DeterminePrimaryFace(info);

            info.positionDescription = GeneratePositionDescription(info.faces);

            // Adding to our list
            pieceInfos.Add(info);

       
            Debug.Log($"Cube piece: {piece.name} is at position: {info.positionDescription}, primary face: {info.primaryFace}");
        }

  
       // controller.hitCastRay = false;
    }

    private void DeterminePrimaryFace(CubePieceInfo info)
    {
    
        if (info.faces.Contains(IdentifyParent.Faces.right))
            info.primaryFace = IdentifyParent.Faces.right;
        else if (info.faces.Contains(IdentifyParent.Faces.left))
            info.primaryFace = IdentifyParent.Faces.left;
        else if (info.faces.Contains(IdentifyParent.Faces.front))
            info.primaryFace = IdentifyParent.Faces.front;
        else if (info.faces.Contains(IdentifyParent.Faces.back))
            info.primaryFace = IdentifyParent.Faces.back;
        else if (info.faces.Contains(IdentifyParent.Faces.top))
            info.primaryFace = IdentifyParent.Faces.top;
        else if (info.faces.Contains(IdentifyParent.Faces.bottom))
            info.primaryFace = IdentifyParent.Faces.bottom;
        else
            info.primaryFace = IdentifyParent.Faces.front; 
    }

    private string GeneratePositionDescription(List<IdentifyParent.Faces> faces)
    {
        if (faces.Count == 0)
            return "Unknown";
        List<string> faceStrings = new List<string>();
        foreach (IdentifyParent.Faces face in faces)
        {
            faceStrings.Add(face.ToString());
        }

        faceStrings.Sort();
        return string.Join("-", faceStrings);
    }

    public void ReparentCubePieces()
    {
        foreach (CubePieceInfo info in pieceInfos)
        {
            Transform newParent = null;

           
            switch (info.primaryFace)
            {
                case IdentifyParent.Faces.right:
                    newParent = rightFace;
                    break;
                case IdentifyParent.Faces.left:
                    newParent = leftFace;
                    break;
                case IdentifyParent.Faces.front:
                    newParent = frontFace;
                    break;
                case IdentifyParent.Faces.back:
                    newParent = backFace;
                    break;
                default:
                    
                    if (info.faces.Contains(IdentifyParent.Faces.left))
                        newParent = leftFace;
                    else if (info.faces.Contains(IdentifyParent.Faces.right))
                        newParent = rightFace;
                    else
                        newParent = frontFace; 
                    break;
            }

            if (newParent != null)
            {
                
                Vector3 worldPos = info.piece.transform.position;
                Quaternion worldRot = info.piece.transform.rotation;
                Vector3 worldScale = info.piece.transform.localScale;

               
                info.piece.transform.SetParent(newParent);

                
                info.piece.transform.position = worldPos;
                info.piece.transform.rotation = worldRot;
                info.piece.transform.localScale = worldScale;

               
            }
        }
    }


    public void IdentifyPositions()
    {
        identifyPositions = true;
    }
    #region Front Face Movement
   
    public void RotateFaceRight()
    {
        // Call AssignLeftFaces on each found component
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignFrontFaces();
                Debug.Log("leftup");
            }
        }
        if (isRotating) return;
        controller.hitCastRay = true;

        // Take the current position
        Vector3 currentPos = frontFace.localPosition; 
        float x = -0.5f;
        float y = 1.5f;
       Vector3 positionIncrement = new Vector3();
        if (currentPos.x == -0.5f)
        {
            positionIncrement = new Vector3(y, -x, 0f); 
        }else if(currentPos.x == 1f)
        {
            positionIncrement = new Vector3(-x, -y, 0f); 
        }else if(currentPos.x == 1.5f)
        {
            positionIncrement = new Vector3(-y, x, 0f); 
        }
        else
        {
            positionIncrement = new Vector3(x, y, 0f); // Only change X and Y, keep Z same
        }

        // Calculate target position
        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(frontFace,targetPos, new Vector3(0, 0, -90)));
    }
    public void RotateFaceLeft()
    {
        // Call AssignLeftFaces on each found component
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignFrontFaces();
                Debug.Log("leftup");
            }
        }
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = frontFace.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == -0.5f)
        {
            positionIncrement = new Vector3(-x, -y, 0f);
        }
        else if (currentPos.x == 1.5f)
        {
            positionIncrement = new Vector3(x, y, 0f);
        }
        else if (currentPos.x == 1f)
        {
            positionIncrement = new Vector3(-y, x, 0f);
        }
        else
        {
            positionIncrement = new Vector3(y, -x, 0f);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(frontFace, targetPos, new Vector3(0, 0, 90)));
    }
#endregion

    #region Back Movement
    public void RotateBackRight()
    {
       
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignBackFaces();
                Debug.Log("leftup");
            }
        }
        if (isRotating) return;
        controller.hitCastRay = true;

        
        Vector3 currentPos = backFace.localPosition; 
        float x = -0.5f;
        float y = 1.5f;
       Vector3 positionIncrement = new Vector3();
        if (currentPos.x == -0.5f)
        {
            positionIncrement = new Vector3(y, -x, 0f); 
        }else if(currentPos.x == 1f)
        {
            positionIncrement = new Vector3(-x, -y, 0f); 
        }else if(currentPos.x == 1.5f)
        {
            positionIncrement = new Vector3(-y, x, 0f);
        }
        else
        {
            positionIncrement = new Vector3(x, y, 0f); // Only change X and Y, keep Z same
        }

        // Calculate target position
        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(backFace,targetPos, new Vector3(0, 0, -90)));
    }

    
   
    public void RotateBackLeft()
    {
        
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignBackFaces();
                Debug.Log("backfaces");
            }
        }
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = backFace.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == -0.5f)
        {
            positionIncrement = new Vector3(-x, -y, 0f);
        }
        else if (currentPos.x == 1.5f)
        {
            positionIncrement = new Vector3(x, y, 0f);
        }
        else if (currentPos.x == 1f)
        {
            positionIncrement = new Vector3(-y, x, 0f);
        }
        else
        {
            positionIncrement = new Vector3(y, -x, 0f);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(backFace, targetPos, new Vector3(0, 0, 90)));
    }
    #endregion


    #region Left Movement
    public void LeftUp()
    {
      
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignLeftFaces();
                Debug.Log("leftup");
            }
        }
        RotateLeftUp();
    }
    public void LeftDown()
    {
       
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignLeftFaces();
                Debug.Log("leftup");
            }
        }
        RotateLeftDown();
    }
    public void RotateLeftDown()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.leftTransform.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.y == 0.5f)
        {
            positionIncrement = new Vector3(0, x, y);
        }
        else if (currentPos.y == 1.5f)
        {
            positionIncrement = new Vector3(0, -x,-y);
        }
        else if (currentPos.y == 2f)
        {
            positionIncrement = new Vector3(0, -y, x);
        }
        else
        {
            positionIncrement = new Vector3(0, y, -x);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.leftTransform, targetPos, new Vector3(-90, 0, 0)));
    }
    public void RotateLeftUp()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.leftTransform.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.y == 0.5f)
        {
            positionIncrement = new Vector3(0, y, -x);
        }
        else if (currentPos.y == 1.5f)
        {
            positionIncrement = new Vector3(0, -y, x);
        }
        else if (currentPos.y == 2f)
        {
            positionIncrement = new Vector3(0, x, y);
        }
        else
        {
            positionIncrement = new Vector3(0, -x, -y);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.leftTransform, targetPos, new Vector3(90, 0, 0)));
    }
    #endregion

    #region Right Movement
    public void RotateRightUp()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.rightTransform.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.y == 0.5f)
        {
            positionIncrement = new Vector3(0, y, -x);
        }
        else if (currentPos.y == 1.5f)
        {
            positionIncrement = new Vector3(0, -y, x);
        }
        else if (currentPos.y == 2f)
        {
            positionIncrement = new Vector3(0, x, y);
        }
        else
        {
            positionIncrement = new Vector3(0, -x, -y);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.rightTransform, targetPos, new Vector3(90, 0, 0)));
    }
    public void RotateRightDown()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.rightTransform.localPosition;
        float x = -0.5f;
        float y = 1.5f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.y == 0.5f)
        {
            positionIncrement = new Vector3(0, x, y);
        }
        else if (currentPos.y == 1.5f)
        {
            positionIncrement = new Vector3(0, -x, -y);
        }
        else if (currentPos.y == 2f)
        {
            positionIncrement = new Vector3(0, -y, x);
        }
        else
        {
            positionIncrement = new Vector3(0, y, -x);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.rightTransform, targetPos, new Vector3(-90, 0, 0)));
    }
    public void RightUp()
    {
        
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignRightFaces();
                
            }
        }
        RotateRightUp();
    }
    public void RightDown()
    {
      
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignRightFaces();
                
            }
        }
        RotateRightDown();
    }
    #endregion

    #region TopMovement
    public void TopLeft()
    {
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignTopFaces();

            }
        }
        RotateTopLeft();
    }
    public void TopRight()
    {
        
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignTopFaces();

            }
        }
        RotateTopRight();
    }
    public void RotateTopLeft()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.topTransform.localPosition;
        float x = -1f;
        float y = 1f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == 0f && currentPos.z ==-1f)
        {
            positionIncrement = new Vector3(0, 0, y);
        }
        else if(currentPos.x == 1f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(x, 0, 0);
        }
        else if (currentPos.x == 1f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(0, 0, x);
        }
        else if (currentPos.x == 0f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(y, 0, 0);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.topTransform, targetPos, new Vector3(0, 90, 0)));
    }
    public void RotateTopRight()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.topTransform.localPosition;
        float x = -1f;
        float y = 1f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == 0f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(y, 0, 0);
        }
        else if (currentPos.x == 1f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(0, 0, y);
        }
        else if (currentPos.x == 1f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(x, 0, 0);
        }
        else if (currentPos.x == 0f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(0, 0, x);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.topTransform, targetPos, new Vector3(0, -90, 0)));
    }
    #endregion

    #region BottomMovement
    public void BottomLeft()
    {
        // Call AssignBottomFaces on each found component
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignBottomFaces();

            }
        }
        RotateBottomLeft();
    }
    public void BottomRight()
    {
        // Call AssignBottomFaces on each found component
        foreach (CubePiecePosition piece in allPieces)
        {
            if (piece != null)
            {
                piece.AssignBottomFaces();

            }
        }
        RotateBottomRight();
    }
    public void RotateBottomLeft()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.bottomTransform.localPosition;
        float x = -1f;
        float y = 1f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == 0f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(0, 0, y);
        }
        else if (currentPos.x == 1f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(x, 0, 0);
        }
        else if (currentPos.x == 1f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(0, 0, x);
        }
        else if (currentPos.x == 0f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(y, 0, 0);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.bottomTransform, targetPos, new Vector3(0, 90, 0)));
    }
    public void RotateBottomRight()
    {
        if (isRotating) return;
        controller.hitCastRay = true;

        Vector3 currentPos = controller.bottomTransform.localPosition;
        float x = -1f;
        float y = 1f;
        Vector3 positionIncrement = new Vector3();

        if (currentPos.x == 0f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(y, 0, 0);
        }
        else if (currentPos.x == 1f && currentPos.z == -1f)
        {
            positionIncrement = new Vector3(0, 0, y);
        }
        else if (currentPos.x == 1f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(x, 0, 0);
        }
        else if (currentPos.x == 0f && currentPos.z == 0f)
        {
            positionIncrement = new Vector3(0, 0, x);
        }

        Vector3 targetPos = currentPos + positionIncrement;
        StartCoroutine(RotateFace(controller.bottomTransform, targetPos, new Vector3(0, -90, 0)));
    }
    #endregion

    // Rotation coroutine
    private IEnumerator RotateFace(Transform face, Vector3 targetPosition, Vector3 rotationAmount)
    {
        isRotating = true;

        Quaternion startRotation = face.localRotation;
        Quaternion targetRotation = face.localRotation * Quaternion.Euler(rotationAmount);
        Vector3 startPosition = face.localPosition;
        Vector3 endPosition = targetPosition;
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            face.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationDuration);
            face.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

       
        face.localRotation = targetRotation;
        face.localPosition = endPosition;


     
        yield return new WaitForSeconds(0.1f);

        // After rotation, we need to re-identify positions
        IdentifyPositions();

        isRotating = false;
    }
}