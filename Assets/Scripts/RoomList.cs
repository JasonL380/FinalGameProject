// Name: Jason Leech
// Date: 05/17/2023
// Desc:

using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class RoomList : MonoBehaviour
    {
        public GameObject[] rooms;

        public int roomCount;

        public int maxRooms;

        public bool generated = false;

        public void FixedUpdate()
        {
            generated = false;
        }
    }
}