using UnityEngine;
using System.Collections;

public class SystemBody {
	public float MajorAxis;
	public float MinorAxis;
}

public class Star : MonoBehaviour {

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

    private SpectralClass m_spectralClass;
    private int m_iSeed;

    private Transform m_transform;
    private Material m_material;

	public int Seed {
		get { return m_iSeed; }
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
                break;

            case SpectralClass.B:
                m_material.color = m_spectralColors[(int)SpectralClass.B];
                m_transform.localScale = new Vector3(0.15f, 0.15f, 1);
                break;

            case SpectralClass.A:
                m_material.color = m_spectralColors[(int)SpectralClass.A];
                m_transform.localScale = new Vector3(0.10f, 0.10f, 1);
                break;

            case SpectralClass.F:
                m_material.color = m_spectralColors[(int)SpectralClass.F];
                m_transform.localScale = new Vector3(0.07f, 0.07f, 1);
                break;

            case SpectralClass.G:
                m_material.color = m_spectralColors[(int)SpectralClass.G];
                m_transform.localScale = new Vector3(0.05f, 0.05f, 1);
                break;

            case SpectralClass.K:
                m_material.color = m_spectralColors[(int)SpectralClass.K];
                m_transform.localScale = new Vector3(0.03f, 0.03f, 1);
                break;

            case SpectralClass.M:
                m_material.color = m_spectralColors[(int)SpectralClass.M];
                m_transform.localScale = new Vector3(0.02f, 0.02f, 1);
                break;
        }
    }
}
