using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSender : MonoBehaviour {

    public Projectile parent;

    private void OnCollisionEnter(Collision collision)
    {
        parent.projectileCollision();
        Debug.Log("hmasdfm");
    }
}
