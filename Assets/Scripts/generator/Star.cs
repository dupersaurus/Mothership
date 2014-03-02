using UnityEngine;
using System.Collections;

public class StarData {
    public const float RADIUS_TO_AU = 0.0046f;

    // From wiki
    public static float[] CLASS_WEIGHTS = { 0.00003f, 0.13f, 0.6f, 3, 7.6f, 12.1f, 76.45f };

    // Harvard classificaiton
    public enum SpectralClass {
        O,
        B,
        A,
        F,
        G,
        K,
        M,
    }

    /// <summary>If the star system is explorable</summary>
    private bool m_bInteresting = false;

    public SpectralClass m_spectralClass;
    private int m_iSeed;

    public int Seed {
        get { return m_iSeed; }
    }

    private string m_sId;

    /// <summary>The object's astrometric identification</summary>
    public string Identification {
        get { return m_sId; }
    }

    public SpectralClass Class {
        get { return m_spectralClass; }
    }

    public string Type {
        get { return m_spectralClass.ToString() + " Main Sequence"; }
    }

    public bool IsInteresting {
        get { return m_bInteresting; }
    }

    public StarData(SectorGenerator sector, int iSeed) {
        m_iSeed = iSeed;
        m_sId = string.Format("S {0} {1} x{2}", sector.StellarGridCoords.x.ToString("+#;-#"), sector.StellarGridCoords.y.ToString("+#;-#"), iSeed.ToString("X6"));

        Random.seed = iSeed;
        float fRandom = Random.Range(0f, 100f);
        int iStartLoop = Random.Range(0, 7);
        bool bContinue = true;

        for (int i = iStartLoop; bContinue || i != iStartLoop; i++) {
            if (i >= 7) {
                i = 0;
            }

            fRandom -= StarData.CLASS_WEIGHTS[i];

            if (fRandom <= 0) {
                m_spectralClass = (StarData.SpectralClass)i;
                break;
            }

            bContinue = false;
        }

        m_bInteresting = -fRandom <= 1; //Random.value > 0.01f;
    }
}

public class Star : MonoBehaviour {

    private StarData m_info;

    public Color[] m_spectralColors;

    public int Seed {
        get { return m_info.Seed; }
    }

    private Transform m_transform;
    private Material m_material;

    /// <summary>Radius of the star, in AUs</summary>
    private float m_fRadius;

    /// <summary>Radius of the star, in AUs</summary>
    public float Radius {
        get { return m_fRadius; }
    }

    public StarData Data {
        get { return m_info; }
    }

    /// <summary>
    /// NOT SEED SAFE Randomly sets the star up
    /// </summary>
    /// <param name="iSeed">The seed to use for the star and its solar system</param>
    public void Setup(SectorGenerator parent, int iSeed) {
        m_info = new StarData(parent, iSeed);

        Setup(m_info);
    }

    /// <summary>
    /// Sets the star up visually. Seed safe.
    /// </summary>
    /// <param name="data"></param>
    public void Setup(StarData data) {
        m_transform = transform;
        m_material = GetComponent<MeshRenderer>().material;

        switch (data.Class) {
            case StarData.SpectralClass.O:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.O];
                m_transform.localScale = new Vector3(0.2f, 0.2f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 6.6f;
                break;

            case StarData.SpectralClass.B:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.B];
                m_transform.localScale = new Vector3(0.15f, 0.15f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 4;
                break;

            case StarData.SpectralClass.A:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.A];
                m_transform.localScale = new Vector3(0.13f, 0.13f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 1.6f;
                break;

            case StarData.SpectralClass.F:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.F];
                m_transform.localScale = new Vector3(0.10f, 0.10f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 1.2f;
                break;

            case StarData.SpectralClass.G:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.G];
                m_transform.localScale = new Vector3(0.08f, 0.08f, 1);
                m_fRadius = StarData.RADIUS_TO_AU;
                break;

            case StarData.SpectralClass.K:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.K];
                m_transform.localScale = new Vector3(0.06f, 0.06f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 0.6f;
                break;

            case StarData.SpectralClass.M:
                m_material.color = m_spectralColors[(int)StarData.SpectralClass.M];
                m_transform.localScale = new Vector3(0.04f, 0.04f, 1);
                m_fRadius = StarData.RADIUS_TO_AU * 0.2f;
                break;
        } 
        
        /*if (data.IsInteresting) {
            m_material.color = Color.magenta;
            m_transform.localScale = new Vector3(0.3f, 0.3f, 1);
        }*/
    }

    /// <summary>
    /// NOT SEED SAFE Returns all bodies contained in this system
    /// </summary>
    /// <returns></returns>
    public SystemBody[] GetBodies() {
        SystemBody[] bodies = new SystemBody[Random.Range(0, 7)];
        float fLastRadius = m_fRadius;

        Random.seed = Seed;

        // Start inside and move out
        for (int i = 0; i < bodies.Length; i++) {

            // Basic distance from previous orbit, then extra random for this orbit
            fLastRadius += (Random.value * 2 + 1) + Random.value * 1;

            bodies[i] = new PlanetInfo(Seed + i);
            bodies[i].MajorAxis = fLastRadius * SolarSystem.UNIT_TO_AU;
            bodies[i].MinorAxis = fLastRadius * SolarSystem.UNIT_TO_AU;
        }

        return bodies;
    }

    public Vector3 LocalPosition {
        get { return m_transform.localPosition; }
    }

    public Vector3 Position {
        get { return m_transform.position; }
    }

    public bool IsInteresting {
        get { return m_info.IsInteresting; }
    }
}
