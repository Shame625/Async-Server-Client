using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System;

public class UIMnanager : MonoBehaviour {

    public GameObject textInfo;
    public GameObject connectButton;
    public GameObject submitButton;
    public GameObject usernameField;

    NetworkManager Instance = NetworkManager.Instance;

    public void ConnectToServer()
    {
        bool connected = Instance.LoopConnect();
        if(connected)
        {
            connectButton.SetActive(false);
            usernameField.SetActive(true);
            submitButton.SetActive(true);

            textInfo.GetComponent<Text>().text = "Connected!";
        }
    }

    public void SubmitUserName()
    {
        string name = usernameField.GetComponent<InputField>().text;

        byte[] buffer = Encoding.ASCII.GetBytes(name);

        int len = 4 + buffer.Length;

        byte[] lenBytes = BitConverter.GetBytes((UInt16)len);
        Debug.Log(len);
        byte[] data = new byte[len];

        data[0] = 0;
        data[1] = 0;
        data[2] = lenBytes[0];
        data[3] = lenBytes[1];
        for(int i = 0; i < buffer.Length; i++)
        {
            data[4+i] = buffer[i];
        }

        Instance.SendPacket(data);
    }
}
