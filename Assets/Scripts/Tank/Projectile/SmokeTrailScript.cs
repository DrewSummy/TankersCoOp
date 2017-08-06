using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailScript : MonoBehaviour {

    // Detach this gameObject from the projectile and Destroy it.
    public void removeSmokeTrail(Vector3 velocity)
    {
        StartCoroutine(removeSmokeTrailHelper(velocity));
    }
    public IEnumerator removeSmokeTrailHelper(Vector3 velocity)
    {
        GetComponent<ParticleSystem>().Stop();
        transform.SetParent(null);
        GetComponent<Rigidbody>().velocity = velocity;
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}