using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using Unity.XR.CoreUtils;

public class Dart : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private GameObject dirObj;
    public bool isForceOK = false;
    bool isDartRotating = false;
    bool isDartReadyToShoot = true;
    bool isDartHitOnBoard = false;


    XROrigin aRSession;
    GameObject ARCam;

    public Collider dartFrontCollider;

    // Start is called before the first frame update
    void Start()
    {
        aRSession = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<XROrigin>();
        ARCam = aRSession.transform.Find("XRCamera").gameObject;

        if (TryGetComponent(out Rigidbody rigid))
        {
            _rigidBody = rigid;

        }

        dirObj = GameObject.FindGameObjectWithTag("DartThrowPoint");
    }

    private void FixedUpdate()
    {
        if (isForceOK)
        {
            dartFrontCollider.enabled = true;
            StartCoroutine(InitDartDestroyVFX());

            GetComponent<Rigidbody>().isKinematic = false;

            isForceOK = false;
            isDartRotating = true;
        }

        //Add Force
        _rigidBody.AddForce(dirObj.transform.forward * (12f + 6f) * Time.deltaTime, ForceMode.VelocityChange);

        //Dart ready
        if (isDartReadyToShoot)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * 20f);
        }

        //Dart rotating
        if (isDartRotating)
        {
            isDartReadyToShoot = false;
            transform.Rotate(Vector3.forward * Time.deltaTime * 400f);
        }
    }

    IEnumerator InitDartDestroyVFX()
    {
        yield return new WaitForSeconds(5f);
        if (!isDartHitOnBoard)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dart_board"))
        {
            //Trigger viberaton
            Handheld.Vibrate();

            GetComponent<Rigidbody>().isKinematic = true;
            isDartRotating = false;

            //Dart hit the board
            isDartHitOnBoard = true;
        }
    }
}
