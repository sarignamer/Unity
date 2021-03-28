using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerStrategy : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private bool isIsometric = false;

    [Space(1f)]
    [Header("Movement")]
    [SerializeField]
    private float normalSpeed = .1f;
    [SerializeField]
    private float fastSpeed = .4f;
    [SerializeField]
    private float movementSpeed = .1f;
    [SerializeField]
    private float movementTime = 5f;

    [Space(1f)]
    [Header("Rotation")]
    [SerializeField]
    private float rotationAmount = .1f;
    [SerializeField]
    private float mouseRotationAmount = 5f;
    [SerializeField]
    private float rotationTime = 5f;

    [Space(1f)]
    [Header("Zoom")]
    [SerializeField]
    [Range(-5f, 5f)]
    private float zoomAmount = 1f;
    [SerializeField]
    private float zoomTime = 5f;

    [Space(2f)]
    [Header("Debug Info")]
    [SerializeField]
    private Vector3 newPosition;
    [SerializeField]
    private Quaternion newRotation;
    [SerializeField]
    private Vector3 newZoom;
    [SerializeField]
    private Vector3 dragStartPosition;
    [SerializeField]
    private Vector3 dragCurrentPosition;
    [SerializeField]
    private Vector3 rotateStartPosition;
    [SerializeField]
    private Vector3 rotateCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        camera.transform.SetParent(this.transform);
        camera.transform.localPosition = new Vector3(0, 50, -50);
        camera.transform.localRotation = Quaternion.Euler(45, 0, 0);

        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, movementTime * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationTime * Time.deltaTime);
        camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, newZoom, zoomTime * Time.deltaTime);
    }

    private void HandleMouseInput()
    {
        //Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += new Vector3(0, -zoomAmount, zoomAmount) * Input.mouseScrollDelta.y;
        }

        //Movement
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Utils.MouseUtils.GetMousePosition());

            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = camera.ScreenPointToRay(Utils.MouseUtils.GetMousePosition());

            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
            }

            newPosition = transform.position + dragStartPosition - dragCurrentPosition;
        }

        //Rotation
        if (Input.GetMouseButtonDown(1))
        {
            rotateStartPosition = Utils.MouseUtils.GetMousePosition();
        }

        if (Input.GetMouseButton(1))
        {
            rotateCurrentPosition = Utils.MouseUtils.GetMousePosition();

            Vector3 diff = rotateStartPosition - rotateCurrentPosition;

            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-diff.x / mouseRotationAmount));
        }
    }

    private void ChangeCameraMode()
    {
        if (isIsometric)
        {
            camera.fieldOfView = 10f;
            camera.transform.localPosition = new Vector3(0, 250, -250);
            newZoom = camera.transform.localPosition;
            camera.transform.localRotation = Quaternion.Euler(30, 0, 0);
        }
        else
        {
            camera.fieldOfView = 60f;
            camera.transform.localPosition = new Vector3(0, 50, -50);
            newZoom = camera.transform.localPosition;
            camera.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            isIsometric = !isIsometric;
            ChangeCameraMode();
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R))
        {
            newZoom += new Vector3(0, -zoomAmount, zoomAmount);
        }

        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= new Vector3(0, -zoomAmount, zoomAmount);
        }
    }
}
