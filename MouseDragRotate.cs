using UnityEngine;
using System.Collections;

public class MouseDragRotate : MonoBehaviour
{
    public GameObject cubeBlock;
    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    public static bool _isRotating;
    private Touch touch;
    
    

    void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
    }

    void Update()
    {
        if (!Input.touchSupported)
        {
            if (_isRotating)
            {
                // offset
                _mouseOffset = (Input.mousePosition - _mouseReference);

                // apply rotation
                _rotation.y = -(_mouseOffset.x) * _sensitivity;
                _rotation.x = -(_mouseOffset.y) * _sensitivity;

                // rotate
                //cubeBlock.transform.eulerAngles += _rotation;
                cubeBlock.transform.rotation = Quaternion.Euler(-_rotation.x, _rotation.y, 0f) * cubeBlock.transform.rotation;
                //cubeBlock.transform.rotation *= Quaternion.AngleAxis(_rotation.y, Vector3.up);
                //cubeBlock.transform.rotation *= Quaternion.AngleAxis(_rotation.x, Vector3.forward);
                // store mouse
                _mouseReference = Input.mousePosition;
            }
        }
        else if (Input.touchCount == 1 && !cubeScript.cubeDrag && _isRotating)
        {
            touch = Input.GetTouch(0);
            _rotation.y = -touch.deltaPosition.x * _sensitivity;
            _rotation.x = -touch.deltaPosition.y * _sensitivity;

            cubeBlock.transform.rotation = Quaternion.Euler(-_rotation.x, _rotation.y, 0f) * cubeBlock.transform.rotation;
        }
    }



    void OnMouseDown()
    {
        // rotating flag
        _isRotating = true;

        // store mouse
        _mouseReference = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // rotating flag
        _isRotating = false;
    }


}