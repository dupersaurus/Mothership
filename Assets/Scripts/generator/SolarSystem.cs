using UnityEngine;
using System.Collections;

public abstract class SystemBody {
    public float MajorAxis;
    public float MinorAxis;
    protected int Seed;

    public SystemBody(int iSeed) {
        Seed = iSeed;
    }

    /// <summary>
    /// Create a GameObject according to the body's type
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public abstract SolarSystemBody Create(Transform parent);
}

public class SolarSystem : MonoBehaviour {
    /// <summary>Unity units per AU in solar system space</summary>
    public const float UNIT_PER_AU = 5f;
    
    private Transform m_transform;
    private SystemShip m_ship;

    public void Setup(Star star, ShipInfo player) {
        m_transform = transform;
        SystemBody[] bodies = star.GetBodies();
        float fSpawnRadius = 6;

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].Create(m_transform);

            Debug.Log("Major >> " + bodies[i].MajorAxis / UNIT_PER_AU);
            if (bodies[i].MajorAxis / UNIT_PER_AU > fSpawnRadius) {
                fSpawnRadius = bodies[i].MajorAxis / UNIT_PER_AU + 2;
            }
        }

        // Create the star
        GameObject starGO = Instantiate(Resources.Load("Star")) as GameObject;
        starGO.transform.parent = m_transform;
        starGO.GetComponent<Star>().Setup(star.Data);
        starGO.transform.localPosition = Vector3.zero;
        starGO.transform.localScale = new Vector3(star.Radius * UNIT_PER_AU * 100, star.Radius * UNIT_PER_AU * 100, 1);

        // Create the ship
        Debug.Log("Spawn radius >> " + fSpawnRadius);
        m_ship = NGUITools.AddChild(gameObject, Resources.Load("System Ship") as GameObject).GetComponent<SystemShip>();
        m_ship.Setup(player, fSpawnRadius);
    }
}
