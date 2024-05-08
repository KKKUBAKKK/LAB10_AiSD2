using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ASD.Graphs;

namespace ASD
{
    public class Lab10 : MarshalByRefObject
    {

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt>">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <returns>Informację czy istnieje droga przez labirynt oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>
        public (bool routeExists, int[] route) FindEscape(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold)
        {
            bool[] visited = new bool[labyrinth.VertexCount];
            visited[0] = true;
            (var routeExists, var route) =
                FindEscapeRec(labyrinth, startingTorches + roomTorches[0], roomTorches, debt - roomGold[0], roomGold, visited, 0);
            if (route == null)
                return (false, null);

            route.Reverse();
            return (true, route.ToArray());
        }

        public (bool routeExists, List<int> route) FindEscapeRec(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, 
            bool[] visited, int room)
        {
            
            if (room == labyrinth.VertexCount - 1)
            {
                if (debt <= 0)
                    return (true, new List<int>(){room});

                return (false, null);
            }
            else if (startingTorches <= 0)
            {
                return (false, null);
            }

            bool routeExists = false;
            List<int> route = null;
            foreach (var neighbor in labyrinth.OutNeighbors(room))
            {
                if (visited[neighbor])
                    continue;

                visited[neighbor] = true;
                (routeExists, route) =
                    FindEscapeRec(labyrinth, startingTorches + roomTorches[neighbor] - 1, roomTorches, debt - roomGold[neighbor], roomGold, visited, neighbor);
                visited[neighbor] = false;
                
                if (routeExists)
                    break;
            }
            if (routeExists)
                route.Add(room);
            
            return (routeExists, route);
        }

        /// <param name="labyrinth">Graf reprezentujący labirynt</param>
        /// <param name="startingTorches">Ilość pochodni z jaką startują bohaterowie</param>
        /// <param name="roomTorches">Ilość pochodni w poszczególnych pokojach</param>
        /// <param name="debt">Ilość złota jaką bohaterowie muszą zebrać</param>
        /// <param name="roomGold">Ilość złota w poszczególnych pokojach</param>
        /// <param name="dragonDelay">Opóźnienie z jakim wystartuje smok</param>
        /// <returns>Informację czy istnieje droga przez labirynt oraz tablicę reprezentującą kolejne wierzchołki na drodze. W przypadku, gdy zwracany jest false, wartość tego pola powinna być null.</returns>

        public (bool routeExists, int[] route) FindEscapeWithHeadstart(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, int dragonDelay)
        {
            (int next, int torches, int debt)[] visited = new(int next, int torches, int debt)[labyrinth.VertexCount];
            
            visited[0].next = -1;
            visited[0].torches = startingTorches + roomTorches[0];
            visited[0].debt = debt - roomGold[0];
            roomTorches[0] = 0;
            roomGold[0] = 0;

            int dragonRoom = -dragonDelay;
            if (dragonDelay != Int32.MaxValue)
                dragonRoom -= 1;

            bool[] destroyed = new bool[labyrinth.VertexCount];
            
            (var routeExists, var route) =
                FindEscapeWithHeadstartRec(labyrinth, roomTorches, roomGold, visited, 0, dragonRoom, destroyed);
            if (route == null)
                return (false, null);

            route.Reverse();
            return (true, route.ToArray());
            
            return (false, null);
        }
        
        public (bool routeExists, List<int> route) FindEscapeWithHeadstartRec(Graph labyrinth, int[] roomTorches, int[] roomGold, 
            (int next, int torches, int debt)[] visited, int room, int dragonRoom, bool[] destroyed)
        {
            if (room == labyrinth.VertexCount - 1 && visited[room].debt <= 0)
            {
                return (true, new List<int>(){room});
            }
            else if (visited[room].torches <= 0)
            {
                return (false, null);
            }

            if (dragonRoom < 0)
            {
                dragonRoom++;
                if (dragonRoom == 0)
                    destroyed[dragonRoom] = true;
            }
            else
            {
                dragonRoom = visited[dragonRoom].next;
                destroyed[dragonRoom] = true;
            }

            bool routeExists = false;
            List<int> route = new List<int>();
            foreach (var neighbor in labyrinth.OutNeighbors(room))
            {
                if (destroyed[neighbor])
                    continue;
                if (visited[room].torches - 1 + roomTorches[neighbor] <= visited[neighbor].torches &&
                    visited[room].debt - roomGold[neighbor] >= visited[neighbor].debt && neighbor != labyrinth.VertexCount - 1)
                    continue;

                visited[room].next = neighbor;
                (int, int, int) temp = visited[neighbor];
                visited[neighbor].debt = visited[room].debt - roomGold[neighbor];
                visited[neighbor].torches = visited[room].torches - 1 + roomTorches[neighbor];
                int g = roomGold[neighbor];
                int t = roomTorches[neighbor];
                roomGold[neighbor] = 0;
                roomTorches[neighbor] = 0;
                
                (routeExists, route) = FindEscapeWithHeadstartRec(labyrinth,
                    roomTorches, roomGold, visited, neighbor, dragonRoom, destroyed);

                if (routeExists)
                    break;
                
                visited[neighbor] = temp;
                roomGold[neighbor] = g;
                roomTorches[neighbor] = t;
            }

            if (dragonRoom >= 0)
                destroyed[dragonRoom] = false;

            if (!routeExists)
                return (false, null);
            
            route.Add(room);
            return (true, route);
        }
    }
}
