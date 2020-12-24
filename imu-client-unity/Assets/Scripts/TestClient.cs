using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestClient : MonoBehaviour
{
    private const string ServerIp = "127.0.0.1";
    private const int Port = 5005;

    // Start is called before the first frame update
    private void Start()
    {
        //---data to send to the server---
        var textToSend = DateTime.Now.ToString();
        
        Debug.Log($"Connecting to: {ServerIp}:{Port}");

        //---create a TCPClient object at the IP and port no.---
        var client = new TcpClient(ServerIp, Port);
        var nwStream = client.GetStream();
        var bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

        //---send the text---
        Debug.Log("Sending : " + textToSend);
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);

        //---read back the text---
        var bytesToRead = new byte[client.ReceiveBufferSize];
        var bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        Debug.Log("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
        //client.Close();
    }
}
