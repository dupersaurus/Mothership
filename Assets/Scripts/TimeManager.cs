using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
	public delegate void cbWorldUpdate(float fDelta);

	/// <summary>
	/// Called on manager fixed update, with time elapsed according to the time scale
	/// </summary>
	public static event cbWorldUpdate OnWorldUpdate;

	/// <summary>
	/// Total time elapsed, in seconds
	/// </summary>
	private float m_totalTime = 0;

	/// <summary>
	/// Multiplier to real time
	/// </summary>
	private float m_fTimeScale = 1;

	// Use this for initialization
	void Start ()
	{

	}

	void FixedUpdate () {
		float fDelta = Time.fixedDeltaTime * m_fTimeScale;
	}
}

