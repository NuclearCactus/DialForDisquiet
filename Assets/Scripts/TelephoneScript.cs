using System;
using UnityEngine;

public class TelephoneScript : MonoBehaviour
{

    public static event Action onTeleport;
    public bool pickup;
    PlayerMovement player;
    // Invoked when a line of data is received from the serial device.
    void OnMessageArrived(string msg)
    {
        Debug.Log(msg);
        msg = msg.Trim();
        if(msg == "up"){
            pickup = true;
        }
        else
        {
            pickup = false;
        }
        
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        
    }

}
