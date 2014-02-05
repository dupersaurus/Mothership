using UnityEngine;
using System.Collections;

public class SolarSystemBody : MonoBehaviour {

    protected Transform m_transform;
    protected Material m_material;

    public virtual void Setup(SystemBody body) {
        m_transform = transform;
        m_material = GetComponent<MeshRenderer>().material;
    }
}
