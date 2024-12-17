using System.Collections;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DartController : MonoBehaviour
{
    public GameObject DartPrefab;
    public Transform DartThrowPoint;
    XROrigin aRSession;
    GameObject ARCam;
    private GameObject DartTemp;
    private Rigidbody rb;
    Transform DartBoardObj;
    private bool _isDartBoardSearched;
    private float _distanceFromDartBoard = 0f;
    public TMP_Text text_distance;

    void Start()
    {
        aRSession = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<XROrigin>();
        ARCam = aRSession.transform.Find("XRCamera").gameObject;
    }

    void OnEnable()
    {
        PlaceObjectOnPlane.OnPlacedObject += DartsInit;
    }

    void OnDisable()
    {
        PlaceObjectOnPlane.OnPlacedObject -= DartsInit;
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("dart"))
                {
                    //Disable back touch Collider from dart 
                    raycastHit.collider.enabled = false;
                    DartTemp.transform.parent = aRSession.transform;

                    var currentDartScript = DartTemp.GetComponent<Dart>();
                    currentDartScript.isForceOK = true;

                    // Load next dart
                    DartsInit();
                }
            }
        }

        if (_isDartBoardSearched)
        {
            _distanceFromDartBoard = Vector3.Distance(DartBoardObj.position, ARCam.transform.position);
            text_distance.text = _distanceFromDartBoard.ToString().Substring(0, 3);
        }
    }

    void DartsInit()
    {
        DartBoardObj = GameObject.FindWithTag("dart_board").transform;
        if (DartBoardObj)
        {
            _isDartBoardSearched = true;
        }

        StartCoroutine(WaitAndSpawnDart());
    }

    public IEnumerator WaitAndSpawnDart()
    {
        yield return new WaitForSeconds(0.1f);
        DartTemp = Instantiate(DartPrefab, DartThrowPoint.position, ARCam.transform.localRotation);
        DartTemp.transform.parent = ARCam.transform;
        rb = DartTemp.GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}