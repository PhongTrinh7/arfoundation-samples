using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhongBuildingBlock : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Touch");
        if (collision.collider.CompareTag("SnapPoint"))
        {
            Destroy(collision.gameObject);
        }
    }
}
