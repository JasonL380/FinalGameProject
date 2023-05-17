using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class RoomGeneration : MonoBehaviour
{
    //kinds of rooms
    public GameObject[] roomTypes;
    //which rooms its tried to generate
    private int[] tried;
    private int count;
    public LayerMask roomLayer;
    
    //Some magical values to transform the collider to isometric, do not change
    private Matrix4x4 transformMatrix = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1.41422f));

    public bool fits = false;

    private Grid grid;
    
    private void Start()
    {
        tried = new int[roomTypes.Length];
        roomLayer = LayerMask.GetMask("room");

        grid = FindObjectOfType<Grid>();
    }

    [Tooltip("the way into the next room through the door up right = 0, up left = 1, down left = 2, down right = 3")]
    public int facing;
    //has the room generated
    private bool roomGend;

    private void Update()
    {
        if (Application.isEditor)
        {
            
        }
        //fits = checkRoomFits(roomTypes[0], transform.position);
    }

    private void OnDrawGizmos()
    {
        
        /*if (this.enabled)
        {
            Vector2[] points = roomTypes[0].GetComponent<PolygonCollider2D>().points;

            Gizmos.color = fits ? Color.green : Color.red;
            Gizmos.DrawLine(points[0] + (Vector2) transform.position, points[1] + (Vector2) transform.position);
            Gizmos.DrawLine(points[1] + (Vector2) transform.position, points[2] + (Vector2) transform.position);
            Gizmos.DrawLine(points[2] + (Vector2) transform.position, points[3] + (Vector2) transform.position);
            Gizmos.DrawLine(points[3] + (Vector2) transform.position, points[0] + (Vector2) transform.position);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if no room has generated and the player collides
        if(!roomGend && collision.name.Equals("Player") )
        {
            //get a random room
            Shuffle(roomTypes);
            //randomly picks from the list until it's exhausted or it finds a matching one
            int i;
            for (i = 0; i < roomTypes.Length; ++i)
            {
                if (roomTypes[i].GetComponent<roomStats>().facing != facing)
                {
                    continue;
                }

                if (checkRoomFits(roomTypes[i], collision.transform.position))
                {
                    //make the room

                    GameObject roomClone = Instantiate(roomTypes[i]);
                    roomClone.transform.parent = grid.transform;
                    roomClone.SetActive(true);

                    //get the position of the door for centering it
                    Vector3Int doorPos = grid.WorldToCell(transform.position);
                    //some math to properly align the door
                    doorPos.x -= (int)(roomClone.transform.position.x) + (int)roomClone.GetComponent<roomStats>().doorPos.x;
                    doorPos.y -= (int)(roomClone.transform.position.y) + (int)roomClone.GetComponent<roomStats>().doorPos.y;
                    doorPos.z -= (int)(roomClone.transform.position.z);
                    Vector3 finalPos = grid.CellToWorld(doorPos);
                    //put the room in it's proper place
                    roomClone.transform.position = finalPos;
                    //don't let any other rooms generate
                    roomGend = true; 
                    break;
                }
            }
        }
    }

    void Shuffle (GameObject[] deck) {
        for (int i = 0; i < deck.Length; i++) {
            GameObject temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    
    bool checkRoomFits(GameObject room, Vector3 position)
    {
        Vector3 pos = position;
        pos.x = (float)Math.Round(position.x);
        pos.y = (float)Math.Round(position.y);

        //get the position of the door for centering it
        Vector3Int doorPos = grid.WorldToCell(pos);
        //some math to properly align the door
        doorPos.x -= (int)(room.transform.position.x) + (int)room.GetComponent<roomStats>().doorPos.x;
        doorPos.y -= (int)(room.transform.position.y) + (int)room.GetComponent<roomStats>().doorPos.y;
        doorPos.z -= (int)(room.transform.position.z);
        Vector2 finalPos = grid.CellToWorld(doorPos);
        
        PolygonCollider2D collider = room.GetComponent<PolygonCollider2D>();

        RaycastHit2D hit = new RaycastHit2D();
        
        for (int i = 0; i < collider.points.Length; ++i)
        {
            print(i);
            if (i + 1 == collider.points.Length)
            {
                hit = Physics2D.Linecast(collider.points[i] + finalPos, collider.points[0] + finalPos, LayerMask.GetMask("room"));
            }
            else
            {
                hit = Physics2D.Linecast(collider.points[i] + finalPos, collider.points[i + 1] + finalPos, LayerMask.GetMask("room"));
            }

            
            if (hit.collider != null)
            {
                return false;
                Debug.DrawLine(collider.points[i] + finalPos, i + 1 == collider.points.Length ? collider.points[0] + finalPos : collider.points[i + 1] + finalPos, Color.red);
            }
            else
            {
                Debug.DrawLine(collider.points[i] + finalPos, i + 1 == collider.points.Length ? collider.points[0] + finalPos : collider.points[i + 1] + finalPos, Color.green);
            }
            
        }

        return true;
        
        Quaternion rot = Quaternion.Euler(0, 0, 45);

        ContactFilter2D filter = new ContactFilter2D();
        
        filter.layerMask = LayerMask.NameToLayer("room");

        List<RaycastHit2D> collider2Ds = new List<RaycastHit2D>();

        collider.offset += (Vector2) finalPos;

        collider.Cast(new Vector2(1,1), filter, collider2Ds, distance: 0.1f);

        collider.offset -= (Vector2) finalPos;
        return collider2Ds.Count == 0;
    }
    
    Vector3 divideY(Vector3 vec)
    {
        return new Vector3(vec.x, vec.y * 0.5f, vec.z);
    }
}
