using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    public const float TILE_SIZE = 1;
    public const float HALF_TILE_SIZE = 0.5f;

    public static TilePlacer Ship;

    public enum RoomConflictType {

        /// <summary>No conflict</summary>
        Valid,

        /// <summary>Placement rejected</summary>
        Invalid,

        /// <summary>Can be placed and combined with another room</summary>
        Combinable
    }

    public Vector2 m_minGridSize = Vector2.one;

    /// <summary>How big the tile is, in cells</summary>
    public Vector2 m_gridSize = Vector2.one;

    /// <summary>Id of the room's type</summary>
    private int m_iRoomType;

    /// <summary>Id of the room's type</summary>
    public int TypeId {
        get { return m_iRoomType; }
    }

    /// <summary>If the tile can be moved or not</summary>
    public bool m_bLocked;

    public Color m_color = Color.white;

    protected Transform m_transform;

    private List<GameObject> m_cells;

    private Vector3 m_dragOffset = Vector3.zero;

    /// <summary>All in cell coords, in terms of distance from the origin</summary>
    protected Rect m_cellBounds;

    /// <summary>The bounds of the room for collision purposes, in world coords</summary>
    protected Rect m_collisionBounds;

    /// <summary>Prefab to use to create cells</summary>
    protected Object m_cellPrefab;

	// Use this for initialization
	void Awake() {
        m_transform = transform;
	}

    void Start() {
        
    }

    /// <summary>
    /// Initializes the room on creation
    /// </summary>
    /// <param name="position"></param>
    /// <param name="iTypeIndex"></param>
    public void Create(Vector2 position, int iTypeIndex) {
        m_iRoomType = iTypeIndex;
        GameObject cell;
        BoxCollider box;

        // Spawn cells
        for (int i = 0; i < m_gridSize.x * m_gridSize.y; i++) {
            cell = CreateCell(new Vector3(i % m_gridSize.x, -Mathf.Floor(i / m_gridSize.x), 0));

            box = cell.AddComponent<BoxCollider>();
            box.size = new Vector3(TILE_SIZE, TILE_SIZE, 1);
            box.center = Vector3.zero;
        }

        /*BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(m_gridSize.x, m_gridSize.y, 1);
        box.center = new Vector3((m_gridSize.x - 1) * 0.5f, (m_gridSize.y - 1) * -0.5f, 0);*/

        m_cellBounds = new Rect(0, 0, 0, 0);
        //m_cellBounds = new Rect(m_transform.position.x - HALF_TILE_SIZE, m_transform.position.y + HALF_TILE_SIZE, m_gridSize.x + HALF_TILE_SIZE, m_gridSize.y + HALF_TILE_SIZE);

        MoveTo(position.x, position.y);
    }

    public bool IsLocked {
        get { return m_bLocked; }
    }

    /// <summary>
    /// Returns if the tile accepts a raycast hit
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public bool HitTest(RaycastHit hit) {
        // Tile's (0,0) cell is the top-left corner
        m_dragOffset = hit.point - m_transform.position;
        m_dragOffset.x = Mathf.Round(Mathf.Abs(m_dragOffset.x));
        m_dragOffset.y = -Mathf.Round(Mathf.Abs(m_dragOffset.y));
        m_dragOffset.z = 0;

        return true;
        //return !m_bLocked;
    }

    /// <summary>
    /// Moves the tile to a given x,y position.The tile will center itself over that position, based on where it's origin is.
    /// </summary>
    /// <param name="position">The center of the grid cell occupied by the tile's origin</param>
    public void MoveTo(float fX, float fY) {
        transform.position = new Vector3(fX, fY, 0) - m_dragOffset;
    }

    /// <summary>
    /// Expands the tile bounds to a given world position, snapped to the grid
    /// </summary>
    /// <param name="pos">World position, snapped to the grid</param>
    public void ExpandTo(Vector2 pos) {
        
        // Mouse position relative to the origin
        Vector2 drag = pos - (Vector2)m_transform.position;

        // Rectange defined by the mouse and the origin, the new room size
        Rect newBounds = new Rect();

        if (drag.x < 0) {
            newBounds.xMin = drag.x;
            newBounds.xMax = 0;
        } else {
            newBounds.xMin = 0;
            newBounds.xMax = drag.x;
        }

        if (drag.y < 0) {
            newBounds.yMin = drag.y;
            newBounds.yMax = 0;
        } else {
            newBounds.yMin = 0;
            newBounds.yMax = drag.y;
        }

        // Scale single cell to fit the rectangle...
        Transform cell = m_cells[0].transform;

        cell.localPosition = newBounds.center;
        cell.localScale = new Vector3(newBounds.width + 1f, newBounds.height + 1f, 1);

        m_cellBounds = newBounds;
        Debug.Log("Cell >> " + m_cellBounds);

        // Check valid
        newBounds.center += ((Vector2)transform.position - new Vector2(HALF_TILE_SIZE, HALF_TILE_SIZE));
        newBounds.width += TILE_SIZE;
        newBounds.height += TILE_SIZE;
        m_collisionBounds = newBounds;

        Debug.Log("Collision >> " + m_collisionBounds + ", Cell >> " + m_cellBounds);

        RoomConflictType conflict = Ship.CheckPlacement(this, m_collisionBounds);

        if (conflict == RoomConflictType.Valid) {
            cell.GetComponent<MeshRenderer>().material.color = GetRoomColor();
        } else {
            cell.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        /*BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.center = cell.localPosition;
        box.size = cell.localScale;*/

        // ...OR fill in and trim the existing tiles to fit the new rectangle

        /* // Possibly better way, if more complicated
        Vector2 newBounds = pos - (Vector2)m_transform.position;
        bool bTrim = false;

        // Expand right
        if (newBounds.x > 0) {
            if (newBounds.x > m_cellBounds.xMax) {
                for (int i = 0; i < m_cellBounds.height + 1; i++) {
                    CreateCell(new Vector3((m_cellBounds.xMax + 1) * TILE_SIZE, m_cellBounds.yMax - i * TILE_SIZE, 0));
                }

                m_cellBounds.xMax++;
            }
        } else if (newBounds.x < 0) {
            bTrim = true;
            m_cellBounds.xMax--;
        }

        // Expand down
        if (newBounds.y < 0) {
            if (newBounds.y < m_cellBounds.yMin) {
                for (int i = 0; i < m_cellBounds.width + 1; i++) {
                    CreateCell(new Vector3(m_cellBounds.xMin + i * TILE_SIZE, (m_cellBounds.yMin - 1) * TILE_SIZE, 0));
                }

                m_cellBounds.yMin--;
            }
        } else if (newBounds.y > 0) {
            bTrim = true;
            m_cellBounds.yMin++;
        }*/

        // Y
        /*if (diff.y > 0) {

        } else if (diff.y < 0) {
            bTrim = true;
        }

        // Bottom corner
        if (diff.x > 0 && diff.y > 0) {

        }*/
    }

    private GameObject CreateCell(Vector3 pos) {
        if (m_cellPrefab == null) {
            m_cellPrefab = Resources.Load("Room Cell");
        }

        if (m_cells == null) {
            m_cells = new List<GameObject>();
        }

        GameObject cell = Instantiate(m_cellPrefab) as GameObject;
        cell.transform.parent = m_transform;
        cell.transform.localPosition = pos;

        cell.GetComponent<MeshRenderer>().material.color = GetRoomColor();

        m_cells.Add(cell);
        return cell;
    }

    protected override Color GetRoomColor() {
        Random.seed = m_iRoomType;
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

    public Rect GetCellBounds() {
        return m_cellBounds;
    }

    /// <summary>
    /// Returns if this room collides with a given rect
    /// </summary>
    /// <param name="rect">Rectangle to check collision with, in world coords</param>
    /// <returns></returns>
    public RoomConflictType CheckCollision(Room room, Rect rect) {
        rect.center -= (Vector2)m_transform.localPosition;

        if (m_cellBounds.Overlaps(rect)) {
            return RoomConflictType.Invalid;
        } else {
            return RoomConflictType.Valid;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(m_collisionBounds.center, new Vector3(m_collisionBounds.width, m_collisionBounds.height, 1));
    }
}
