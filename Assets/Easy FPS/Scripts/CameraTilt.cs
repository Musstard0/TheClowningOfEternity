using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    //Editor variables, you can customize these
    public float _tiltAmount = 5;
    public KeyCode _leftBtn = KeyCode.A; //A is default
    public KeyCode _rightBtn = KeyCode.D; //D is default

    // Update is called once per frame
    void Update()
    {

        // If _leftBtn key is hit, rotate Z axis of camera by _tiltAmount
        if (Input.GetKeyDown(_leftBtn))
        {
            this.transform.Rotate(0, 0, _tiltAmount);
        }
        else if (Input.GetKeyUp(_leftBtn))
        {
            this.transform.Rotate(0, 0, -_tiltAmount);
        }

        // Same as above, but inverted values
        if (Input.GetKeyDown(_rightBtn))
        {
            this.transform.Rotate(0, 0, -_tiltAmount);
        }
        else if (Input.GetKeyUp(_rightBtn))
        {
            this.transform.Rotate(0, 0, _tiltAmount);
        }
    }
}
