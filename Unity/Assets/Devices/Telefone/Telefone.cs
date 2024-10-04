using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class PhoneNmbr{

	public string callNumber;
    public AudioClip audioClip;
	public UnityEvent callEvent;
}

[RequireComponent(typeof(ArduinoController), typeof(AudioSource))]
[Title("v0.1", bold:false, horizontalLine:false, titleAlignment:TitleAlignments.Right)]
public class Telefone : MonoBehaviour
{

    ArduinoController controller;

    Dictionary<string, int> keyIndex = new Dictionary<string, int>()
    { {"1", 0}, {"2", 1}, {"3", 2}, {"4", 3}, {"5", 4}, {"6", 5}, {"7", 6}, {"8", 7}, {"9", 8}, {"*", 9}, {"0", 10}, {"#", 11} };

    
    [ReadOnly]
    public bool receiverUp = false;
    [ReadOnly]
    public bool ringing = false;
    [ReadOnly]
    public bool calling = false;
    
    [Title("Phone Book"), ListDrawerSettings(NumberOfItemsPerPage = 3)]
    public List<PhoneNmbr> phoneNumberList = new List<PhoneNmbr>();

    [Title("Dialling")]
    public DialTones dialTones;
    AudioSource audioSource;
    [ReadOnly]
    public string dialedNumber = "";
    [Range(.0f, 5f), Tooltip("Tempo até tentar chamar depois de se digitar um número correcto")]
    public float callDelay = 3f;
    [Range(.0f, 10f), Tooltip("Tempo até ir abaixo depois de se carregar num digito")]
    public float dialTimeout = 3f;
    private float callTimer;
    




    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<ArduinoController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ButtonUpdate();
        DialUpdate();
    }

    [TitleGroup("RingTone")]
    [ButtonGroup("RingTone/0"), Button("Ring")]
    public void RingToneA() { Ring("A"); }
    [ButtonGroup("RingTone/0"), Button("Zelda")]
    public void RingToneB() { Ring("B"); }
    [ButtonGroup("RingTone/0"), Button("Birthday")]
    public void RingToneC() { Ring("C"); }
    [ButtonGroup("RingTone/0"), Button("Random")]
    public void RingToneD() { Ring("D"); }

    void Ring(string tone)
    {
        if(!receiverUp)
            controller.SendMessage(tone);
    }



    [TitleGroup("Simulador")]
    [ButtonGroup("Simulador/0"), Button("ReceiverUp"), GUIColor("GetColor0"), DisableIf("receiverUp"), DisableInEditorMode]
    void _ReceiverUp()
    {
        receiverUp = true;
        
        SendMessage("ReceiverDown", SendMessageOptions.DontRequireReceiver);
        
    }

    [ButtonGroup("Simulador/0"), Button("ReceiverDown"), GUIColor("GetColor1"), EnableIf("receiverUp"), DisableInEditorMode]
    void _ReceiverDown()
    {
        receiverUp = false;
        calling = false;
        ResetDialling();
        
        SendMessage("ReceiverUp", SendMessageOptions.DontRequireReceiver);
        
    }
    
    [ButtonGroup("Simulador/1"), Button("1"), DisableInEditorMode, GUIColor("GetColor2")]
    void _Key1()
    {
        Dial("1");
        SendMessage("Key", '1', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/1"), Button("2"), DisableInEditorMode, GUIColor("GetColor3")]
    void _Key2()
    {
        Dial("2");
        SendMessage("Key", '2', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/1"), Button("3"), DisableInEditorMode, GUIColor("GetColor4")]
    void _Key3()
    {
        Dial("3");
        SendMessage("Key", '3', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/2"), Button("4"), DisableInEditorMode, GUIColor("GetColor5")]
    void _Key4()
    {
        Dial("4");
        SendMessage("Key", '4', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/2"), Button("5"), DisableInEditorMode, GUIColor("GetColor6")]
    void _Key5()
    {
        Dial("5");
        SendMessage("Key", '5', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/2"), Button("6"), DisableInEditorMode, GUIColor("GetColor7")]
    void _Key6()
    {
        Dial("6");
        SendMessage("Key", '6', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/3"), Button("7"), DisableInEditorMode, GUIColor("GetColor8")]
    void _Key7()
    {
        Dial("7");
        SendMessage("Key", '7', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/3"), Button("8"), DisableInEditorMode, GUIColor("GetColor9")]
    void _Key8()
    {
        Dial("8");
        SendMessage("Key", '8', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/3"), Button("9"), DisableInEditorMode, GUIColor("GetColor10")]
    void _Key9()
    {
        Dial("9");
        SendMessage("Key", '9', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/4"), Button("*"), DisableInEditorMode, GUIColor("GetColor11")]
    void _KeyStar()
    {
        Dial("*");
        SendMessage("Key", '*', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/4"), Button("0"), DisableInEditorMode, GUIColor("GetColor12")]
    void _Key0()
    {
        Dial("0");
        SendMessage("Key", '0', SendMessageOptions.DontRequireReceiver);
    }

    [ButtonGroup("Simulador/4"), Button("#"), DisableInEditorMode, GUIColor("GetColor13")]
    void _KeyHash()
    {
        Dial("#");
        SendMessage("Key", '#', SendMessageOptions.DontRequireReceiver);
    }


    void Dial(string s)
    {
        if(receiverUp)
        {
            // Toca o Tone de acordo com o DialTones Scriptable Object
            if(dialTones != null)
            {
                if (keyIndex.TryGetValue(s, out int k) && k <= dialTones.clips.Count)
                    audioSource.PlayOneShot(dialTones.clips[k]);
            }

            // Tenta chamar. Tempo para chamar e timeout é diferente!
            if(!calling)
            {
                dialedNumber += s;
                if(NumberExists(dialedNumber))
                    callTimer = callDelay;
                else
                    callTimer = dialTimeout;
            }
        }
    }

    void DialUpdate()
    {
        if(callTimer > 0)
        {
            callTimer -= Time.deltaTime;
            
            if(callTimer <= 0)
            {
                callTimer = 0;
                Call(dialedNumber);
            }
        }

    }

    void Call(string number)
    {
        calling = true;

        foreach(PhoneNmbr c in phoneNumberList)
        {
            if(c.callNumber == number)
            {
                c.callEvent.Invoke();
                if(c.audioClip != null)
                    audioSource.PlayOneShot(c.audioClip);
            }

            return;
        }

        //TODO: O que acontece quando o número chamado não existe?

    }

    void ResetDialling()
    {
        dialedNumber = "";
        callTimer = 0;
    }

    bool NumberExists(string number)
    {
        foreach(PhoneNmbr c in phoneNumberList)
        {
            if(c.callNumber == number)
                return true;
        }

        return false;
    }



    /* ------------ SIMULATOR STUFF ------------ */

    private float[] buttonTimers = new float[14];
    private static bool[] buttonPresses = new bool[14];


    void OnMessageArrived(string msg)
    {
        switch(msg.ToCharArray()[0])
        {
            case 'u':
                _ReceiverUp();
                buttonPresses[0] = true;
                buttonTimers[0] = .1f;
                break;

            case 'd':
                _ReceiverDown();
                buttonPresses[1] = true;
                buttonTimers[1] = .1f;
                break;

            case '1':
                _Key1();
                buttonPresses[2] = true;
                buttonTimers[2] = .1f;
                break;

            case '2':
                _Key2();
                buttonPresses[3] = true;
                buttonTimers[3] = .1f;
                break;

            case '3':
                _Key3();
                buttonPresses[4] = true;
                buttonTimers[4] = .1f;
                break;

            case '4':
                _Key4();
                buttonPresses[5] = true;
                buttonTimers[5] = .1f;
                break;

            case '5':
                _Key5();
                buttonPresses[6] = true;
                buttonTimers[6] = .1f;
                break;

            case '6':
                _Key6();
                buttonPresses[7] = true;
                buttonTimers[7] = .1f;
                break;

            case '7':
                _Key7();
                buttonPresses[8] = true;
                buttonTimers[8] = .1f;
                break;

            case '8':
                _Key8();
                buttonPresses[9] = true;
                buttonTimers[9] = .1f;
                break;

            case '9':
                _Key9();
                buttonPresses[10] = true;
                buttonTimers[10] = .1f;
                break;

            case '*':
                _KeyStar();
                buttonPresses[11] = true;
                buttonTimers[11] = .1f;
                break;

            case '0':
                _Key0();
                buttonPresses[12] = true;
                buttonTimers[12] = .1f;
                break;

            case '#':
                _KeyHash();
                buttonPresses[13] = true;
                buttonTimers[13] = .1f;
                break;
        }
    }

    void OnConnectionEvent(bool success)
    {
        if (success)
            Debug.Log("Connection established");
        //else
            //Debug.Log("Connection attempt failed or disconnection detected");
    }

    void ButtonUpdate()
    {
        for(int i = 0; i < 14; i++)
        {
            if(buttonTimers[i] > 0)
            {
                buttonTimers[i] = buttonTimers[i] - Time.deltaTime;
                if(buttonTimers[i] <= 0)
                {
                    buttonTimers[i] = 0;
                    buttonPresses[i] = false;
                }

                Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
            }
        }
    }


    private static Color GetColor0(){return buttonPresses[0] ? Color.green : Color.white;}
    private static Color GetColor1(){return buttonPresses[1] ? Color.green : Color.white;}
    private static Color GetColor2(){return buttonPresses[2] ? Color.green : Color.white;}
    private static Color GetColor3(){return buttonPresses[3] ? Color.green : Color.white;}
    private static Color GetColor4(){return buttonPresses[4] ? Color.green : Color.white;}
    private static Color GetColor5(){return buttonPresses[5] ? Color.green : Color.white;}
    private static Color GetColor6(){return buttonPresses[6] ? Color.green : Color.white;}
    private static Color GetColor7(){return buttonPresses[7] ? Color.green : Color.white;}
    private static Color GetColor8(){return buttonPresses[8] ? Color.green : Color.white;}
    private static Color GetColor9(){return buttonPresses[9] ? Color.green : Color.white;}
    private static Color GetColor10(){return buttonPresses[10] ? Color.green : Color.white;}
    private static Color GetColor11(){return buttonPresses[11] ? Color.green : Color.white;}
    private static Color GetColor12(){return buttonPresses[12]? Color.green : Color.white;}
    private static Color GetColor13(){return buttonPresses[13] ? Color.green : Color.white;}
}
