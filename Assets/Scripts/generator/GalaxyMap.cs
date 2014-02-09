using UnityEngine;
using System.Collections;
using Mothership.UI;

public class GalaxySectorData {

    public int X;
    public int Y;

    /// <summary>[0,1] base density of the sector. Approximately corresponds to dust, yellowish color</summary>
    public float BaseDensity;

    private float m_fBandingDensity;

    /// <summary>[0,1] density from arm bands. Approximately corresponds to stars, bluish color</summary>
    public float BandingDensity {
        get { return Mathf.Clamp01(m_fBandingDensity); }
        set { m_fBandingDensity = value; }
    }

    public float GetSectorDensity() {
        //return (BaseDensity + BandingDensity) * 0.5f;
        
        // Get a middle range
        return (((BaseDensity + BandingDensity) - 2) + 3) * 0.125f;
    }

    public Color GetSectorColor() {
        return new Color(BaseDensity * 0.55f + m_fBandingDensity * 0.05f, BaseDensity * 0.55f + m_fBandingDensity * 0.05f, Mathf.Clamp(BaseDensity * 0.4f + m_fBandingDensity * 0.4f, 0, 0.4f));
    }
}

public class GalaxyMap : MonoBehaviour {

    public Camera m_camera;

    /// <summary>Number of sectors on either side of the 0-axis</summary>
    public int m_sectorResolution = 25;

    /// <summary>How big each sector is visually</summary>
    public float m_sectorDensity = 0.25f;

    private GalaxySectorData[,] m_sectors;

    private static GalaxyMap m_instance;
    private Transform m_transform;
    private Object m_prefab;

    public AnimationCurve m_baseDensityFalloff;

    void Start() {
        m_instance = this;

        float fDistance;
        float fBaseDensity;

        m_transform = transform;

        Random.seed = 1;

        // Base density pass
        GalaxySectorData data;
        m_sectors = new GalaxySectorData[m_sectorResolution * 2 + 1, m_sectorResolution * 2 + 1];

        for (int i = -m_sectorResolution; i <= m_sectorResolution; i++) {
            for (int j = -m_sectorResolution; j <= m_sectorResolution; j++) {
                fDistance = new Vector2(i * m_sectorDensity, j * m_sectorDensity).sqrMagnitude;
                fBaseDensity = m_baseDensityFalloff.Evaluate(fDistance / Mathf.Pow(m_sectorResolution * m_sectorDensity, 2));

                data = new GalaxySectorData();
                data.X = i;
                data.Y = j;
                data.BaseDensity = fBaseDensity;
                
                m_sectors[i + m_sectorResolution, j + m_sectorResolution] = data;
            }
        }

        // Bar
        

        // Spirals
        DrawSpiral(0.4f, 0.001f, 0.5f, 0.2f, 0.45f, 0, 0);
        DrawSpiral(0.6f, 0.001f, 0.5f, 0.23f, 0.43f, Mathf.PI, 0);
        DrawSpiral(0.3f, 0.001f, 0.5f, 0.25f, 0.4f, Mathf.PI / 4, (Mathf.PI / 4) * 7, (Mathf.PI / 4) * 10);
        DrawSpiral(0.3f, 0.001f, 0.5f, 0.25f, 0.45f, (Mathf.PI / 4) * 7, (Mathf.PI / 4) * 7, (Mathf.PI / 4) * 9);

        StartCoroutine(DrawChunk(-m_sectorResolution));

        UI.SetMode(UI.Mode.Galaxy);
    }

    /// <summary>
    /// Draw a spiral band
    /// </summary>
    /// <param name="fStrength">The strength of the band. To each sector hit by the calculation, this value is added to BandingDensity</param>
    /// <param name="fParallelFalloff">How much fStrength falls off for each step of the calculation.</param>
    /// <param name="fPerpFalloff">For each step of the calculation, sectors around the one hit will have fStrength * fPerpFalloff added to BandingDensity</param>
    /// <param name="fA">r=ae*(b*theta)</param>
    /// <param name="fB">r=ae*(b*theta)</param>
    /// <param name="fRotate">Amount to shift the arm by, in radians from 0</param>
    /// <param name="fThetaStart">Angle to start drawing the band at</param>
    /// <param name="fThetaEnd">OPTIONAL Angle to stop drawing the band at</param>
    private void DrawSpiral(float fStrength, float fParallelFalloff, float fPerpFalloff, float fA, float fB, float fRotate, float fThetaStart, float fThetaEnd = -1) {
        if (fThetaEnd < 0) {
            fThetaEnd = Mathf.PI * 4;
        }
        
        LineRenderer line = (LineRenderer)renderer;
        float fRadius;
        Vector3 point;
        int iVertex = 0;
        int iSize = (int)((fThetaEnd - fThetaStart) / 0.1f);
        int iX, iY;

        if (line != null) {
            line.SetVertexCount(iSize);
        }

        for (float fTheta = fThetaStart; iVertex < iSize; fTheta += 0.1f, iVertex++) {
            fRadius = Mathf.Pow(2.718f, fTheta * fB) * fA;
            point = Quaternion.Euler(0, 0, (fTheta + fRotate) * Mathf.Rad2Deg) * (Vector3.right * fRadius);

            if (line != null) {
                line.SetPosition(iVertex, point);
            }

            // apply to sector
            iX = Mathf.FloorToInt(point.x / m_sectorDensity) + m_sectorResolution;
            iY = Mathf.FloorToInt(point.y / m_sectorDensity) + m_sectorResolution;

            // Out of bounds
            try {
                m_sectors[iX, iY].BandingDensity = fStrength;
            } catch {
                break;
            }

            for (int i = -1; i <= 1; i++) {
                if (iX + i < 0 || iX + i >= m_sectors.GetLength(0) - 1) {
                    continue;
                }

                for (int j = -1; j <= 1; j++) {
                    if (iY + j < 0 || iY + j >= m_sectors.GetLength(1) - 1) {
                        continue;
                    }

                    m_sectors[iX + i, iY + j].BandingDensity += (fStrength * fPerpFalloff);
                }
            }
        }
    }

    /// <summary>
    /// Draw a chunk of the map
    /// </summary>
    /// <param name="iRow"></param>
    /// <returns></returns>
    private IEnumerator DrawChunk(int iRow) {
        if (m_prefab == null) {
            m_prefab = Resources.Load("Galaxy Sector");
        }

        GameObject spawn;
        GameObject me = gameObject;
        Transform spawnTrans;
        Material material;

        for (int iCol = -m_sectorResolution; iCol <= m_sectorResolution; iCol++) {
            spawn = Instantiate(m_prefab) as GameObject;
            spawn.name = "Sector " + iCol + ", " + iRow;

            spawnTrans = spawn.transform;
            spawnTrans.parent = m_transform;
            spawnTrans.localScale = new Vector3(m_sectorDensity, m_sectorDensity, 1);
            spawnTrans.localPosition = new Vector3(iCol * m_sectorDensity, iRow * m_sectorDensity, 0);

            material = spawnTrans.GetComponentInChildren<MeshRenderer>().material;
            material.color = m_sectors[iCol + m_sectorResolution, iRow + m_sectorResolution].GetSectorColor();
        }

        yield return new WaitForSeconds(0.005f);

        if (iRow < m_sectorResolution) {
            StartCoroutine(DrawChunk(iRow + 1));
        }
    }

    private void OnMouseDown() {
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = m_camera.ScreenPointToRay(mousePos);
        int iX, iY;

        if (Physics.Raycast(ray, out hit, 11)) {
            mousePos = m_transform.InverseTransformPoint(hit.point);
            iX = Mathf.RoundToInt(mousePos.x / m_sectorDensity);
            iY = Mathf.RoundToInt(mousePos.y / m_sectorDensity);

            Debug.Log(m_sectors[iX + m_sectorResolution, iY + m_sectorResolution].GetSectorDensity() + " (" + m_sectors[iX + m_sectorResolution, iY + m_sectorResolution].BaseDensity + ", " + m_sectors[iX + m_sectorResolution, iY + m_sectorResolution].BandingDensity + ")");

            gameObject.SetActive(false);

            Vector3 cameraTo = Galaxy.SetupSectors(iX, iY);
            cameraTo.z = -10;
            SectorCamera.MoveCameraTo(cameraTo, 0, null);
        }
    }

    public static void ReturnToMap() {
        m_instance._ReturnToMap();
    }

    private void _ReturnToMap() {
        UI.SetMode(UI.Mode.Galaxy);
        gameObject.SetActive(true);
    }

    public static GalaxySectorData GetSectorData(int iX, int iY) {
        return m_instance._GetSectorData(iX, iY);
    }

    private GalaxySectorData _GetSectorData(int iX, int iY) {
        iX += m_sectorResolution;
        iY += m_sectorResolution;

        if (iX < 0 || iX >= m_sectors.GetLength(0) || iY < 0 || iY >= m_sectors.GetLength(1)) {
            return null;
        } else {
            return m_sectors[iX, iY];
        }
    }

    public bool IsActive {
        get { return gameObject.activeSelf; }
    }
}
