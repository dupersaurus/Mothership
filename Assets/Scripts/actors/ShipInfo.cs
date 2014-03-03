using System;
using System.Collections.Generic;

public class ShipInfo {
    /// <summary>Amount of fuel carried.</summary>
    private float m_fFuelLoad;

    public float FuelLoad {
        get { return m_fFuelLoad; }
    }

    /// <summary>
    /// Fuel usage while in FTL, units/sec
    /// </summary>
    public float FTLFuelUseage {
        get { return 1; }
    }

    /// <summary>
    /// Time that the ship can stay in continuous FTL, in game seconds
    /// </summary>
    /// <value>The FTL time.</value>
    public float FTLTime {
        get { return 15552000; /* 180 days */}
    }

    /// <summary>
    /// FTL speed, ly/sec (game)
    /// </summary>
    /// <value>The FTL speed.</value>
    public float FTLSpeed {
        get { return 0.0001f; }
    }

    public ShipInfo() {
        m_fFuelLoad = 400 / FTLSpeed;
    }

    /// <summary>
    /// Use an amount of fuel
    /// </summary>
    /// <param name="fAmount"></param>
    public void UseFuel(float fAmount) {
        m_fFuelLoad -= fAmount;
    }
}