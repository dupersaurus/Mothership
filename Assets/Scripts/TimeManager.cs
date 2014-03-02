using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
	public delegate void cbWorldUpdate(float fDelta);

	/// <summary>
	/// Called on manager fixed update, with time elapsed according to the time scale
	/// </summary>
	public static event cbWorldUpdate OnWorldUpdate;

    private static TimeManager m_instance;

	/// <summary>
	/// Total time elapsed, in seconds
	/// </summary>
	private double m_totalTime = 0;

    /// <summary>
    /// Total time elapsed in the game, in seconds
    /// </summary>
    public static double ElapsedTime {
        get { return m_instance.m_totalTime; }
    }

	/// <summary>
	/// Multiplier to real time
	/// </summary>
	public float m_fTimeScale = 0;

    /// <summary>
    /// Multiplier to real time
    /// </summary>
    public static float TimeScale {
        get { return m_instance.m_fTimeScale; }
        set { m_instance.m_fTimeScale = value; }
    }

	// Use this for initialization
	void Awake () {
        m_instance = this;
	}

	void FixedUpdate () {
		float fDelta = Time.fixedDeltaTime * m_fTimeScale;

        if (fDelta == 0) {
            return;
        }
        
        m_totalTime += fDelta;
        OnWorldUpdate(fDelta);
	}

    public static void Pause() {
        TimeScale = 0;
    }
}

