using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePlacer : MonoBehaviour {

    public Vector2 m_tileSize = Vector2.one;
    public UIPopupList m_types;

    private Camera m_camera;
    private Transform m_transform;

    private Room m_dragging;
    private Vector2 m_lastMousePos = Vector2.zero;

    private Dictionary<int, List<Room>> m_roomMaster;

    private bool m_bCreateMode = true;

    void Awake() {
        m_camera = camera;
        m_transform = transform;

        m_roomMaster = new Dictionary<int, List<Room>>();
        Room.Ship = this;
    }

    void Update() {
        RaycastHit hit;
        Vector3 mousePos = Input.mousePosition;
        Ray ray = m_camera.ScreenPointToRay(mousePos);
        Room tile;
        bool bFound = false;

        if (Input.GetMouseButtonDown(0) && UICamera.hoveredObject == null) {

            // Create mode: create new tile at mouse point
            if (m_bCreateMode && Physics.Raycast(ray, out hit, 10)) {
                tile = hit.transform.parent.GetComponent<Room>();

                if (tile != null) {
                    bFound = true;
                }
            }

            if (!bFound) {
                m_dragging = CreateNewRoom(ray.origin, m_types.items.IndexOf(m_types.selection));
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            m_dragging = null;
        }

        Vector2 mouseGrid = SnapWorldToGrid(ray.origin);

        if (Input.GetMouseButton(0) && m_dragging != null && mouseGrid != m_lastMousePos) {
            m_dragging.ExpandTo(mouseGrid);
        }

        m_lastMousePos = mouseGrid;
    }

    /// <summary>
    /// Create a new room at a given position with a given type
    /// </summary>
    /// <param name="iId"></param>
    private Room CreateNewRoom(Vector3 position, int iId) {
        Room room = (Instantiate(Resources.Load("Room")) as GameObject).GetComponent<Room>();

        if (room == null) {
            return null;
        }

        room.Create(SnapWorldToGrid(position), iId);

        if (!m_roomMaster.ContainsKey(iId)) {
            m_roomMaster[iId] = new List<Room>();
        }

        m_roomMaster[iId].Add(room);
        return room;
    }

    /// <summary>
    /// Snaps a world position to the grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector2 SnapWorldToGrid(Vector3 pos) {
        return new Vector2(Mathf.Round(pos.x / m_tileSize.x), Mathf.Round(pos.y / m_tileSize.y));
    }

    /// <summary>
    /// Checks for valid placement
    /// </summary>
    /// <param name="room">The proposed room</param>
    /// <param name="bounds">The proposed bounds of the room, in world space</param>
    /// <returns>If valid</returns>
    public Room.RoomConflictType CheckPlacement(Room room, Rect bounds) {
        Room.RoomConflictType result = Room.RoomConflictType.Valid;
        Room.RoomConflictType temp;

        // Overlap pass
        foreach (KeyValuePair<int, List<Room>> kvp in m_roomMaster) {
            for (int i = 0; i < kvp.Value.Count; i++) {
                if (room == kvp.Value[i]) {
                    continue;
                }

                temp = kvp.Value[i].CheckCollision(room, bounds);

                if (temp == Room.RoomConflictType.Invalid) {
                    return Room.RoomConflictType.Invalid;
                } else if (temp != Room.RoomConflictType.Valid) {
                    result = temp;
                }
            }
        }

        return result;
    }
}
