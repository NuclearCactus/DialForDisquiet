using UnityEngine;

public class PhoneInput : MonoBehaviour
{
    public string number = "";
    public void Key(char c)
    {
        number+=c;
        if(number.EndsWith("100"))
        {
            Debug.Log("Ring");
        }
        else if(number.EndsWith("101"))
        {
            Debug.Log("Friend1");
        }
        else if(number.EndsWith("102"))
        {
            Debug.Log("Friend2");
        }
    }
}
