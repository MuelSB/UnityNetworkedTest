using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerTestPlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Rigidbody rb;
    private PlayerControls playerControls;
    private Camera followCam;
    private TextMesh healthText;

    private int healthPoints = 100;

    // Awake is called before start
    private void Awake()
    {
        SetupPlayerInput();

        rb = GetComponent<Rigidbody>();
        followCam = GetComponentInChildren<Camera>();
        healthText = GetComponentInChildren<TextMesh>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Disable cameras of other players
        if(!IsLocalPlayer)
        {
            followCam.gameObject.SetActive(false);
        }

        // Set health value
        healthText.text = "Health: " + healthPoints.ToString();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    // LateUpdate is called after all update functions have been called
    private void LateUpdate()
    {
    }

    // FixedUpdate is called once per physics update
    private void FixedUpdate()
    {
        if(IsLocalPlayer)
        {
            var inputVector = playerControls.Player.Locomotion.ReadValue<Vector2>();
            rb.AddForce(new Vector3(inputVector.x, 0.0f, inputVector.y) * speed);
        }
    }

    // SetupPlayerInput is called during Awake
    private void SetupPlayerInput()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();

        playerControls.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
    }

    private void OnGUI()
    {
        // Print connection type label
        if (NetworkManager.Singleton.IsHost) GUILayout.Label("Host (Server/Client)");
        else if (NetworkManager.Singleton.IsClient) GUILayout.Label("Client");
        else if (NetworkManager.Singleton.IsServer) GUILayout.Label("Server");
    }
}
