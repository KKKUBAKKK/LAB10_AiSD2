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
            int[] visited = new int[labyrinth.VertexCount];
            
            visited[0] = 1;
            (var routeExists, var route) =
                FindEscapeWithHeadstartRec(labyrinth, startingTorches + roomTorches[0], roomTorches, debt - roomGold[0], roomGold, dragonDelay, visited, 0);
            if (route == null)
                return (false, null);

            route.Reverse();
            return (true, route.ToArray());
            
            return (false, null);
        }
        
        public (bool routeExists, List<int> route) FindEscapeWithHeadstartRec(Graph labyrinth, int startingTorches, int[] roomTorches, int debt, int[] roomGold, 
            int dragonDelay, int[] visited, int room)
        {
            if (room == labyrinth.VertexCount - 1)
            {
                if (debt <= 0)
                    return (true, new List<int>(){room});

                return (false, null); // moze chodzi o to, ze mozna przejsc przez koniec dalej?
                // wyszlo na to samo :(
                // bool end = true;
                // foreach (var n in labyrinth.OutNeighbors(room))
                // {
                //     if (visited[room] == 0)
                //         end = false;
                // }
                //
                // if (end)
                //     return (false, null);
            }
            else if (startingTorches <= 0)
            {
                return (false, null);
            }
            // TODO: trzeba jakos zapisac wiercholki, ktore odwiedzil smok, bo teraz mozna je odwiedzic, (i je odwiedzam :( )
            bool routeExists = false;
            List<int> route = null;
            foreach (var neighbor in labyrinth.OutNeighbors(room))
            {
                // if (visited[neighbor] != 0 && visited[neighbor] < visited[room] - dragonDelay) // TODO: przydaloby sie stare visited[room]
                //     continue;
                
                if (visited[neighbor] != 0 && visited[room] - visited[neighbor] > dragonDelay)
                    continue;
                
                if (visited[neighbor] != 0)
                {
                    int oldDelay = dragonDelay;
                    dragonDelay -= visited[room] - visited[neighbor];
                    int temp = visited[neighbor];// TODO: moze nie robic tego, tylko zachowywac stare numery?
                    visited[neighbor] = visited[room] + 1;
                    (routeExists, route) =
                        FindEscapeWithHeadstartRec(labyrinth, startingTorches - 1, roomTorches,
                            debt, roomGold, dragonDelay, visited, neighbor);
                    dragonDelay = oldDelay;
                    visited[neighbor] = temp;
                }
                else
                {
                    visited[neighbor] = visited[room] + 1;
                    (routeExists, route) =
                        FindEscapeWithHeadstartRec(labyrinth, startingTorches + roomTorches[neighbor] - 1, roomTorches,
                            debt - roomGold[neighbor], roomGold, dragonDelay, visited, neighbor);
                    visited[neighbor] = 0;
                }
                
                if (routeExists)
                    break;
            }
            if (routeExists)
                route.Add(room);
            
            return (routeExists, route);
        }
    }
}
