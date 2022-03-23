using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Threading;

public class NetworkManager : MonoBehaviour
{
    public string LocalIPAddress;
    public int ListeningPort;
    public int SendingPort;
    Thread listener;
    static Queue pQueue = Queue.Synchronized(new Queue()); //this is the message queue, it is thread safe
    static UdpClient udp;
    private IPEndPoint endPoint;

    public GameObject canvas;
    public GameObject map;
    public GameObject xButtons;
    public GameObject oButtons;

    public GameManager gameManager; //drag this on the inspector

    private void Update()
    {
        //in the main thread, read the message and update the game manager
        lock (pQueue.SyncRoot)
        {
            if (pQueue.Count > 0)
            {
                object o = pQueue.Dequeue(); //Take the olders message out of the queue
                gameManager.GotNetworkMessage((string)o); //Send it to the game manager
            }
        }
    }

    private void OnDestroy()
    {
        EndUDP();
    }

    public void StartUDP()
    {
        endPoint = new IPEndPoint(IPAddress.Any, ListeningPort); //this line will listen to all IP addresses in the network
        //endPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress), ListeningPort); //this line will listen to a specific IP address
        udp = new UdpClient(endPoint);
        Debug.Log("Listening for Data...");
        listener = new Thread(new ThreadStart(MessageHandler));
        listener.IsBackground = true;
        listener.Start();
    }

    void MessageHandler()
    {
        Byte[] data = new byte[0];
        while (true)
        {
            try
            {
                //Did we get a new message?
                data = udp.Receive(ref endPoint);
            }
            catch (Exception err)
            {
                //If there's a problem
                Debug.Log("Communication error, recieve data error " + err);
                udp.Close();
                return;
            }
            //Treat the new message
            string msg = Encoding.ASCII.GetString(data);
            Debug.Log("UDP incoming " + msg);
            pQueue.Enqueue(msg);
        }
    }

    private void EndUDP()
    {
        if (udp != null)
        {
            udp.Close();
        }
        if (listener != null)
        {
            listener.Abort();
        }
    }

    public void SendMessage(string message)
    {
        UdpClient send_client = new UdpClient();
        IPEndPoint send_endPoint = new IPEndPoint(IPAddress.Parse(LocalIPAddress), SendingPort);
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        send_client.Send(bytes, bytes.Length, send_endPoint);
        send_client.Close();
        Debug.Log("Sent message: " + message);
    }

    public void PlayerOne()
    {
        LocalIPAddress = "127.0.0.1";
        ListeningPort = 40001;
        SendingPort = 40000;
        canvas.SetActive(false);
        map.SetActive(true);
        StartUDP();
        gameManager.playerOne = true;
        gameManager.Pside = "playerOne";
        oButtons.SetActive(false);
    }

    public void PlayerTwo()
    {
        LocalIPAddress = "127.0.0.1";
        ListeningPort = 40000;
        SendingPort = 40001;
        canvas.SetActive(false);
        map.SetActive(true);
        StartUDP();
        gameManager.playerTwo = true;
        gameManager.Pside = "playerTwo";
        xButtons.SetActive(false);
    }

}
