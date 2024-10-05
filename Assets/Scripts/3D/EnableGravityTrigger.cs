using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGravityTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Cube cube;
        
        if(other.gameObject.TryGetComponent<Cube>(out cube))
        {
            if(!cube.UseGravity)
            {
                cube.EnableGravity();
            }
        }
    }
}
