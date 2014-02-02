using UnityEngine;
using System.Collections;

/// <summary>Basic room types</summary>
public enum RoomModule {

    /// <summary>Stuff to command the ship with</summary>
    Command,

    /// <summary>Stuff to protect the ship with</summary>
    Security,

    /// <summary>Stuff to make the ship go</summary>
    Engineering,

    /// <summary>Stuff to put people in (and keep them happy)</summary>
    Habitation,

    /// <summary>Stuff to keep everyone alive (including food)</summary>
    LifeSupport,

    /// <summary>Stuff to get around the ship</summary>
    Corridor
}

/// <summary>
/// 
/// </summary>
public class RoomType {
    private int m_iId;
    private Color m_color;

    /// <summary>Minimum size, in cells, the room can be</summary>
    private Vector2 m_minSize;

    /// <summary>Maximum size, in cells, the room can be</summary>
    private Vector3 m_maxSize;

    /// <summary>Whether the room can be combined with adjacent rooms of the same type (false) or not (true)</summary>
    private bool m_bUnique;

    /// <summary>General room type</summary>
    private RoomModule m_moduleType;
}

public class RoomParams {

}