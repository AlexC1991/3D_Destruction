using System;
using UnityEngine;

public class DestroyBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject,0.1f);
        }
    }
}
