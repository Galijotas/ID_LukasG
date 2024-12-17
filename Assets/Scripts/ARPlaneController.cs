using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class ARPlaneController : MonoBehaviour
{
    ARPlaneManager _arPlaneManager;
    void Awake()
    {
        _arPlaneManager = GetComponent<ARPlaneManager>();
    }

    void OnEnable()
    {
        PlaceObjectOnPlane.OnPlacedObject += DisablePlaneDetection;
    }

    void OnDisable()
    {
        PlaceObjectOnPlane.OnPlacedObject -= DisablePlaneDetection;
    }

    void DisablePlaneDetection()
    {
        //planeDetectionMessage = "Disable Plane Detection and Hide Existing";
        SetAllPlanesActive(false);
        _arPlaneManager.enabled = false;
    }

    /// <summary>
    /// Iterates over all the existing planes and activates
    /// or deactivates their <c>GameObject</c>s'.
    /// </summary>
    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in _arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }


}
