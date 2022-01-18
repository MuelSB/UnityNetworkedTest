using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float fireForceStrength = 50.0f;
    [SerializeField] private int bulletDamage = 10;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        FireBullet();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckDestroyBullet_ServerRPC();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger_ServerRPC(other);
    }

    [ServerRpc]
    private void FireBullet()
    {
        rb.AddForce(Vector3.forward * fireForceStrength);
    }

    [ServerRpc]
    private void CheckDestroyBullet_ServerRPC()
    {
        if (transform.position.z >= 1000.0f)
            Destroy_ServerRPC();
    }

    [ServerRpc]
    private void Destroy_ServerRPC()
    {
        // Netcode automatically destroys objects on clients that are destroyed on the server
        Destroy(gameObject);
    }

    [ServerRpc]
    private void OnTrigger_ServerRPC(Collider other)
    {
        // Test if the hit object was a player
        var playerTestController = other.gameObject.GetComponent<PlayerTestPlayerController>();

        if (playerTestController)
        {
            playerTestController.Damage_ServerRPC(bulletDamage);
        }

        Destroy_ServerRPC();
    }
}
