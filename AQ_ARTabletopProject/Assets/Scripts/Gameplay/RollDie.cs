using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollDie : MonoBehaviour
{
    public delegate void DieRolled(int pRoll);
    public static DieRolled OnDieRolled;

    public static Vector2 DieTossValues;

    private Camera _mainCam;
    private Rigidbody _bodyRB;
    private List<Transform> _dieSides = new List<Transform>();

    private Text _uiText;
    private Text _text2;
    private bool _canShowText;

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

    void Start()
    {
        _mainCam = Camera.main;
        _bodyRB = transform.Find("Body").GetComponent<Rigidbody>();
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("Side"))
            {
                _dieSides.Add(child.transform);
            }
        }

        if (GameObject.Find("UI Text"))
        {
            _uiText = GameObject.Find("UI Text").GetComponent<Text>();
            _uiText.text = "";
        }
    }

    void FixedUpdate()
    {
        if (_uiText)
        {
            if (_bodyRB.velocity.magnitude == 0f)
            {
                if (_canShowText && _uiText.text == "")
                {
                    foreach (Transform side in _dieSides)
                    {
                        if (side.transform.up == Vector3.up)
                        {
                            _uiText.text = string.Format("You rolled a {0}", side.name.Substring(4));
                            _canShowText = false;
                        }
                    }
                }
            }
        }

        HandleTouch();
        HandleClick();

        if (Input.GetKeyDown(KeyCode.T))
        {
            rollRandomDie();
        }
    }

    private void HandleTouch()
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

            _uiText.text = "";
            _canShowText = true;
        }
    }

    private void HandleClick()
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

            if (_uiText)
            {
                _uiText.text = "";
                _canShowText = true;
            }
        }
    }

    void rollRandomDie()
    {
        _bodyRB.transform.position += Vector3.up * 5 * 0.005f;

        int x = UnityEngine.Random.Range(-1, 2);
        while (x == 0)
            x = UnityEngine.Random.Range(-1, 2);

        int z = UnityEngine.Random.Range(-1, 2);
        while (z == 0)
            z = UnityEngine.Random.Range(-1, 2);

        _bodyRB.AddForce(new Vector3(x, 0, z) * _sideForce + Vector3.up * _upwardsForce);
        _bodyRB.AddTorque(new Vector3(x, 0, z) * _rotationTorque);

        DieTossValues = new Vector2(x, z);
    }

    void rollWithValues(int pX, int pZ)
    {
        //_bodyRB.transform.position += Vector3.up * 5;

        _bodyRB.AddForce(new Vector3(pX, 0, pZ) * _sideForce + Vector3.up * _upwardsForce);
        _bodyRB.AddTorque(new Vector3(pX, 0, pZ) * _rotationTorque);
    }

    public IEnumerator RollRandom()
    {
        rollRandomDie();
        yield return new WaitUntil(() => _bodyRB.velocity.magnitude > 0f);

        yield return new WaitWhile(() => _bodyRB.velocity.magnitude > 0f);
        OnDieRolled(numberRolled());
    }

    public IEnumerator RollWithValues(int pX, int pZ)
    {
        rollWithValues(pX, pZ);
        yield return new WaitUntil(() => _bodyRB.velocity.magnitude > 0f);

        yield return new WaitWhile(() => _bodyRB.velocity.magnitude > 0f);
        OnDieRolled(numberRolled());
    }

    int numberRolled()
    {
        foreach (Transform side in _dieSides)
        {
            float dot = Vector3.Dot(side.transform.up, Vector3.up);
            if (dot >= 0.9f)
                return Int16.Parse(side.name.Substring(4));
        }

        return 0;
    }
}
