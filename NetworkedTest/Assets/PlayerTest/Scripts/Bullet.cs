using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float fireForceStrength = 50.0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb.AddForce(Vector3.forward * fireForceStrength);
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.z >= 1000.0f)
            Destroy_ServerRPC();
    }

    private void Destroy_ServerRPC()
    {
        // Netcode automatically destroys objects on clients that are destroyed on the server
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy_ServerRPC();
    }
}
