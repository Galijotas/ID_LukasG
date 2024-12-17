using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectOnPlane : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private GameObject placementIndicator;

    private Pose _placementPose;
    private bool _placementPoseIsValid;
    private bool _isObjectPlaced;
    private ARRaycastManager _raycastManager;
    private ARPlaneManager _planeManager;
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    public static event Action OnPlacedObject;

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        placementIndicator.SetActive(false); // Ensure indicator is inactive initially  
    }

    private void Update()
    {
        if (_isObjectPlaced) return;

        UpdatePlacementPosition();

        if (_placementPoseIsValid)
        {
            UpdatePlacementIndicator();
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPosition()
    {
        var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (_raycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinPolygon))
        {
            _placementPoseIsValid = Hits.Count > 0;
            if (_placementPoseIsValid)
            {
                _placementPose = Hits[0].pose;
                var placedPlane = _planeManager.GetPlane(Hits[0].trackableId);
                if (placedPlane != null)
                {
                    placementIndicator.transform.rotation = placedPlane.transform.rotation;
                }
            }
        }
        else
        {
            _placementPoseIsValid = false;
        }
    }

    private void UpdatePlacementIndicator()
    {
        placementIndicator.SetActive(true);
        placementIndicator.transform.position = _placementPose.position;
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, _placementPose.position, placementIndicator.transform.rotation);
        OnPlacedObject?.Invoke();
        _isObjectPlaced = true;
        placementIndicator.SetActive(false);
    }
}