using System;
using UnityEngine;

public class ExclusionScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BystanderScript script = other.GetComponentInParent<BystanderScript>();
        if (script != null)
        {
            script.Excluded();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BystanderScript script = other.GetComponentInParent<BystanderScript>();
        if (script != null)
        {
            script.Included();
        }
    }
}
