using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private int _movementSpeed = 10;
    [SerializeField] private int _rotationSpeed = 10;

    private float _pitch = 0;
    private float _yaw = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            _movementSpeed *= 2;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _movementSpeed /= 2;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            _yaw = _rotationSpeed * Input.GetAxis("Mouse X");
            _pitch = _rotationSpeed * -Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(_pitch, _yaw, 0.0f);
        }

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * _movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            transform.position += -transform.forward * _movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            transform.position += -transform.right * _movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * _movementSpeed * Time.deltaTime;
    }
}
