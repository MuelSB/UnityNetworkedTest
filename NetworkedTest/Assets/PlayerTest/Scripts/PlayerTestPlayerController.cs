using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerTestPlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpawnDistance = 2.0f;
    [SerializeField] private int defaultHitPointsValue = 100;

    private Rigidbody rb;
    private PlayerControls playerControls;
    private Camera followCam;
    private TextMesh healthText;

    private NetworkVariable<int> healthPoints = new NetworkVariable<int>();

    [ServerRpc(RequireOwnership = false)]
    public void Damage_ServerRPC(int amount)
    {
        healthPoints.Value -= amount;
    }

    // Awake is called before start
    private void Awake()
    {
        SetupPlayerInput();

        rb = GetComponent<Rigidbody>();
        followCam = GetComponentInChildren<Camera>();
        healthText = GetComponentInChildren<TextMesh>();
        healthText.text = "Health: " + defaultHitPointsValue.ToString();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Disable cameras of other players
        if(!IsLocalPlayer)
        {
            followCam.gameObject.SetActive(false);
        }

        healthPoints.Value = defaultHitPointsValue;
    }

    private void OnEnable()
    {
        // Register callback for when the health points value is changed
        healthPoints.OnValueChanged += OnHealthPointsChanged;
    }

    private void OnDisable()
    {
        // Deregister callback for when the health points value is changed
        healthPoints.OnValueChanged -= OnHealthPointsChanged;
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
        playerControls.Player.Shoot.performed += Shoot_performed;
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        // Spawn a prefab on the server and spawn on all client
        if(IsLocalPlayer)
        {
            SpawnBullet_ServerRPC(transform.position);
        }
    }

    [ServerRpc]
    private void SpawnBullet_ServerRPC(Vector3 playerPosition)
    {
        var gameObject = Instantiate(bulletPrefab,
            playerPosition + (Vector3.forward * bulletSpawnDistance),
            Quaternion.identity);

        // Need to call spawn on the network object component to spawn on clients
        gameObject.GetComponent<NetworkObject>().Spawn(true);
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

    private void OnHealthPointsChanged(int oldHealthPoints, int newHealthPoints)
    {
        if (!IsClient) return;

        // Update the text to display the new health points value
        healthText.text = "Health: " + healthPoints.Value.ToString();
    }
}
