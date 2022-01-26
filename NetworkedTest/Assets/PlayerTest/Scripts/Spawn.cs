using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class Spawn : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnGUI()
    {
        // Remember using UnityEngine.Networking;
        //NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = 192.bla.bla.bla.ToString();

        GUILayout.BeginArea(new Rect(10, 10, 75, 75));

        // If we are not a server or a client
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // Show button to join as a host (server and client)
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();

            // Show button to join as a client
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        }

        GUILayout.EndArea();
    }
}
