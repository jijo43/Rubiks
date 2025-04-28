using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool hitCastRay;
    
    [Header("References to parent face objects")]
    public Transform leftTransform;
    public Transform rightTransform;
    public Transform frontTransform;
    public Transform backTransform;
    public Transform topTransform;
    public Transform bottomTransform;
}
