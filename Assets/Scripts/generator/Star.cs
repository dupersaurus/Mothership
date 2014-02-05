using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

    private const float RADIUS_TO_AU = 0.0046f;

    // From wiki
    private static float[] CLASS_WEIGHTS = { 0.00003f, 0.13f, 0.6f, 3, 7.6f, 12.1f, 76.45f};

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

    public Color[] m_spectralColors;

    /// <summary>If the star system is explorable</summary>
    private bool m_bInteresting;

    private SpectralClass m_spectralClass;
    private int m_iSeed;

    public int Seed {
        get { return m_iSeed; }
    }

    private Transform m_transform;
    private Material m_material;

    /// <summary>Radius of the star, in AUs</summary>
    private float m_fRadius;

    /// <summary>Radius of the star, in AUs</summary>
    private float Radius {
        get { return m_fRadius; }
    }

    /// <summary>
    /// NOT SEED SAFE Randomly sets the star up
    /// </summary>
    /// <param name="iSeed">The seed to use for the star and its solar system</param>
    public void Setup(int iSeed) {
        m_transform = transform;
        m_material = GetComponent<MeshRenderer>().material;

        Random.seed = iSeed;
        float fRandom = Random.Range(0f, 100f);
        int iStartLoop = Random.Range(0, 7);
        bool bContinue = true;

        for (int i = iStartLoop; bContinue || i != iStartLoop; i++) {
            if (i >= 7) {
                i = 0;
            }

            fRandom -= CLASS_WEIGHTS[i];

            if (fRandom <= 0) {
                m_spectralClass = (SpectralClass)i;
                break;
            }

            bContinue = false;
        }
        

        switch (m_spectralClass) {
            case SpectralClass.O:
                m_material.color = m_spectralColors[(int)SpectralClass.O];
                m_transform.localScale = new Vector3(0.2f, 0.2f, 1);
                m_fRadius = RADIUS_TO_AU * 6.6f;
                break;

            case SpectralClass.B:
                m_material.color = m_spectralColors[(int)SpectralClass.B];
                m_transform.localScale = new Vector3(0.15f, 0.15f, 1);
                m_fRadius = RADIUS_TO_AU * 4;
                break;

            case SpectralClass.A:
                m_material.color = m_spectralColors[(int)SpectralClass.A];
                m_transform.localScale = new Vector3(0.13f, 0.13f, 1);
                m_fRadius = RADIUS_TO_AU * 1.6f;
                break;

            case SpectralClass.F:
                m_material.color = m_spectralColors[(int)SpectralClass.F];
                m_transform.localScale = new Vector3(0.10f, 0.10f, 1);
                m_fRadius = RADIUS_TO_AU * 1.2f;
                break;

            case SpectralClass.G:
                m_material.color = m_spectralColors[(int)SpectralClass.G];
                m_transform.localScale = new Vector3(0.08f, 0.08f, 1);
                m_fRadius = RADIUS_TO_AU;
                break;

            case SpectralClass.K:
                m_material.color = m_spectralColors[(int)SpectralClass.K];
                m_transform.localScale = new Vector3(0.06f, 0.06f, 1);
                m_fRadius = RADIUS_TO_AU * 0.6f;
                break;

            case SpectralClass.M:
                m_material.color = m_spectralColors[(int)SpectralClass.M];
                m_transform.localScale = new Vector3(0.04f, 0.04f, 1);
                m_fRadius = RADIUS_TO_AU * 0.2f;
                break;
        }
    }

    /// <summary>
    /// Returns all bodies contained in this system
    /// </summary>
    /// <returns></returns>
    public SystemBody[] GetBodies() {
        SystemBody[] bodies = new SystemBody[4/*Random.Range(1, 7)*/];
        float fLastRadius = m_fRadius;

        Random.seed = m_iSeed;

        // Start inside and move out
        for (int i = 0; i < bodies.Length; i++) {

            // Basic distance from previous orbit, then extra random for this orbit
            fLastRadius += (Random.value * 1 + 1) + Random.value * 1;

            bodies[i] = new PlanetInfo(m_iSeed + i);
            bodies[i].MajorAxis = fLastRadius;
            bodies[i].MinorAxis = fLastRadius;
        }

        return bodies;
    }
}
