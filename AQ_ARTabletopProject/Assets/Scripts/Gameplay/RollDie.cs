using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RollDie : MonoBehaviour
{
    public static event Action<int> onDieRolled;

    public static Vector2 DieTossValues;

    private Camera _mainCam;
    private Rigidbody _bodyRB;
    private List<Transform> _dieSides = new List<Transform>();

    //Android
    private Touch _currentTouch;
    private Vector3 _touchDelta;
    private Vector3 _touchDirection;
    private Vector3 _lastTouchPos;
    private bool _touchedThis;

    //Editor
    private Vector3 _mouseDelta;
    private Vector3 _mouseDirection;
    private Vector3 _lastMousePos;
    private bool _clickedThis;

    [SerializeField]
    private float _upwardsForce;
    [SerializeField]
    private float _sideForce;
    [SerializeField]
    private float _rotationTorque;

    private void Start()
    {
        _mainCam = Camera.main;
        _bodyRB = transform.Find("Body").GetComponent<Rigidbody>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            if (child.name.Contains("Side"))
                _dieSides.Add(child.transform);
    }

    private void FixedUpdate()
    {
        //Uncomment to enable dragging
        //handleTouch();
        //handleClick();
    }

    private void handleTouch()
    {
        if (Input.touchCount == 0) return;

        _currentTouch = Input.GetTouch(0);
        Vector3 touchToWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(_currentTouch.position.x, _currentTouch.position.y, _mainCam.WorldToScreenPoint(_bodyRB.gameObject.transform.position).z));

        if (_currentTouch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_currentTouch.position);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == _bodyRB.transform.gameObject)
                {
                    Vector3 pos = _bodyRB.transform.position;
                    transform.position = pos;

                    _bodyRB.transform.localPosition = Vector3.zero;
                    _bodyRB.freezeRotation = true;
                    _bodyRB.freezeRotation = false;
                    _bodyRB.velocity = Vector3.zero;
                    _bodyRB.useGravity = false;

                    _touchDelta = touchToWorldPos - _bodyRB.gameObject.transform.position;
                    _lastTouchPos = touchToWorldPos;

                    _touchedThis = true;
                }
            }
        }

        if (!_touchedThis) return;

        if (_currentTouch.phase == TouchPhase.Moved || _currentTouch.phase == TouchPhase.Stationary)
        {
            transform.position = (Vector3.Lerp(transform.position, new Vector3(touchToWorldPos.x - _touchDelta.x, 2, touchToWorldPos.z - _touchDelta.z), Time.deltaTime * 20));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero +
                                                                                     (new Vector3(Mathf.Clamp(_touchDirection.z, -1f, 1f), 0, -Mathf.Clamp(_touchDirection.x, -1f, 1f)) * 50)),
                                                                                      Time.deltaTime * 20);
            _touchDirection = touchToWorldPos - _lastTouchPos;
            _lastTouchPos = touchToWorldPos;
            _touchDirection.y = 0;
        }

        if (_currentTouch.phase == TouchPhase.Ended)
        {
            _bodyRB.useGravity = true;
            _bodyRB.AddForce(new Vector3(Mathf.Clamp(_touchDirection.x, -2f, 2f), 0, Mathf.Clamp(_touchDirection.z, -2f, 2f)) * 1000 + Vector3.up * 250);
            _bodyRB.AddTorque(new Vector3(_touchDirection.z, 0, -_touchDirection.x) * 1000);

            transform.rotation = Quaternion.identity;

            _touchedThis = false;
        }
    }

    private void handleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == _bodyRB.transform.gameObject)
                {
                    Vector3 mouseToWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCam.WorldToScreenPoint(_bodyRB.gameObject.transform.position).z));
                    Vector3 pos = _bodyRB.transform.position;
                    transform.position = pos;

                    _bodyRB.transform.localPosition = Vector3.zero;
                    _bodyRB.freezeRotation = true;
                    _bodyRB.freezeRotation = false;
                    _bodyRB.velocity = Vector3.zero;
                    _bodyRB.useGravity = false;

                    _mouseDelta = mouseToWorldPos - _bodyRB.gameObject.transform.position;
                    _lastMousePos = mouseToWorldPos;

                    _clickedThis = true;
                }
            }
        }

        if (!_clickedThis) return;

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseToWorldPos = _mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCam.WorldToScreenPoint(_bodyRB.gameObject.transform.position).z));

            transform.position = (Vector3.Lerp(transform.position, new Vector3(mouseToWorldPos.x - _mouseDelta.x, 2, mouseToWorldPos.z - _mouseDelta.z), Time.deltaTime * 20));
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero +
                                                                                     (new Vector3(Mathf.Clamp(_mouseDirection.z, -1f, 1f), 0, -Mathf.Clamp(_mouseDirection.x, -1f, 1f)) * 50)),
                                                                                      Time.deltaTime * 20);
            _mouseDirection = mouseToWorldPos - _lastMousePos;
            _lastMousePos = mouseToWorldPos;
            _mouseDirection.y = 0;
        }
        else
        {
            _bodyRB.useGravity = true;
            _bodyRB.AddForce(new Vector3(Mathf.Clamp(_mouseDirection.x, -2f, 2f), 0, Mathf.Clamp(_mouseDirection.z, -2f, 2f)) * 2000 + Vector3.up * 500);
            _bodyRB.AddTorque(new Vector3(_mouseDirection.z, 0, -_mouseDirection.x) * 1000);

            transform.rotation = Quaternion.identity;

            _clickedThis = false;
        }
    }

    private void rollRandomDie()
    {
        _bodyRB.transform.position += Vector3.up * 5;

        float x = UnityEngine.Random.Range(-1.0f, 1.0f);
        float z = UnityEngine.Random.Range(-1.0f, 1.0f);

        _bodyRB.AddForce(new Vector3(x, 0, z).normalized * _sideForce + Vector3.up * _upwardsForce);
        _bodyRB.AddTorque(new Vector3(x, 1, z).normalized * _rotationTorque);

        DieTossValues = new Vector2(x, z);
    }

    private void rollWithValues(int pX, int pZ)
    {
        _bodyRB.transform.position += Vector3.up * 5;

        _bodyRB.AddForce(new Vector3(pX, 0, pZ).normalized * _sideForce + Vector3.up * _upwardsForce);
        _bodyRB.AddTorque(new Vector3(pX, 1, pZ).normalized * _rotationTorque);
    }

    public IEnumerator RollRandom()
    {
        rollRandomDie();
        yield return new WaitUntil(() => _bodyRB.velocity.magnitude > 0f);

        yield return new WaitWhile(() => _bodyRB.velocity.magnitude > 0f);
        onDieRolled?.Invoke(numberRolled());
    }

    public IEnumerator RollWithValues(int pX, int pZ)
    {
        rollWithValues(pX, pZ);
        yield return new WaitUntil(() => _bodyRB.velocity.magnitude > 0f);

        yield return new WaitWhile(() => _bodyRB.velocity.magnitude > 0f);
        onDieRolled?.Invoke(numberRolled());
    }

    private int numberRolled()
    {
        Dictionary<Transform, float> sideDots = new Dictionary<Transform, float>();

        foreach (Transform side in _dieSides)
        {
            float dot = Vector3.Dot(side.transform.up, Vector3.up);
            sideDots.Add(side, dot);
        }

        List<KeyValuePair<Transform, float>> tempList = sideDots.ToList();
        tempList.Sort((pX, pY) => pX.Value.CompareTo(pY.Value));
        tempList.Reverse();

        return Int16.Parse(tempList[0].Key.name.Substring(4));
    }
}
