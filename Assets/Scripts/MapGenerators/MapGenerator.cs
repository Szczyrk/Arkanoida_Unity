using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arkanodia.MapGenerators
{
    public struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    // Room class representing a region of connected tiles
    public class Room : IComparable<Room>
    {
        public List<Coord> tiles; // All the tiles that make up this room
        public List<Coord> edgeTiles; // Tiles at the edge of the room (next to walls)
        public List<Room> connectedRooms; // Rooms connected by passages
        public int roomSize; // Number of tiles in this room
        public bool isAccessibleFromMainRoom; // Whether this room is accessible from the main room
        public bool isMainRoom; // Whether this is the main room

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY) // Only consider directly adjacent tiles
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        // Connects two rooms by adding them to each other's connected rooms list
        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        // Set room as accessible from the main room
        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        // Check if two rooms are already connected
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        // Compare rooms by size (for sorting purposes)
        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }

    public class MapGenerator
    {
        private int _width; // Width of the map
        private int _height; // Height of the map
        private string _seed; // Seed for random generation
        private string[] seeds = {"sas", "a2", "sa2", "sd4", "23", "43", "123", "34", "vc", "czd", "trw"};
        private int _randomFillPercent; // Percentage of tiles to be filled initially
        private int[,] _map; // The map represented as a 2D array
        private int _maxValue = 1; // Maximum tile value (e.g., wall)
        private int _minValue = 0; // Minimum tile value (e.g., empty space)

        public MapGenerator(int width, int height, int maxValue, int minValue)
        {
            _width = width;
            _height = height;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public int[,] GenerateMap(int level, int randomFillPercent)
        {
            // Initialize map and seed
            _map = new int[_width, _height];
            _seed = seeds[level];
            _randomFillPercent = randomFillPercent;

            Debug.Log($"Starting map generation for level {level} with random fill percent: {_randomFillPercent}");

            // Fill the map randomly and apply smoothing
            RandomFillMap();
            for (int i = 0; i < 5; i++)
            {
                SmoothMap();
                Debug.Log($"Map smoothing iteration {i + 1} completed.");
            }

            // Process the map to remove small regions
            ProcessMap();

            // Add borders around the map
            int borderSize = 1;
            int[,] borderedMap = new int[_width + borderSize * 2, _height + borderSize * 2];

            System.Random pseudoRandom = new System.Random(_seed.GetHashCode());
            for (int x = 0; x < borderedMap.GetLength(0); x++)
            {
                for (int y = 0; y < borderedMap.GetLength(1); y++)
                {
                    if (x >= borderSize && x < _width + borderSize && y >= borderSize && y < _height + borderSize)
                    {
                        borderedMap[x, y] = _map[x - borderSize, y - borderSize];
                    }
                    else
                    {
                        borderedMap[x, y] = pseudoRandom.Next(_minValue, _maxValue);
                    }
                }
            }

            Debug.Log("Map generation completed.");
            return borderedMap;
        }

        // Process map regions: eliminate small rooms and walls
        void ProcessMap()
        {
            List<List<Coord>> wallRegions = GetRegions(1); // Find all wall regions
            int wallThresholdSize = 50;

            System.Random pseudoRandom = new System.Random(_seed.GetHashCode());

            // Remove small wall regions
            foreach (List<Coord> wallRegion in wallRegions)
            {
                if (wallRegion.Count < wallThresholdSize)
                {
                    foreach (Coord tile in wallRegion)
                    {
                        _map[tile.tileX, tile.tileY] = pseudoRandom.Next(_minValue, _maxValue);
                    }
                    Debug.Log($"Removed small wall region of size {wallRegion.Count}");
                }
            }

            // Find all room regions (empty spaces)
            List<List<Coord>> roomRegions = GetRegions(0);
            int roomThresholdSize = 50;
            List<Room> survivingRooms = new List<Room>();

            // Remove small room regions
            foreach (List<Coord> roomRegion in roomRegions)
            {
                if (roomRegion.Count < roomThresholdSize)
                {
                    foreach (Coord tile in roomRegion)
                    {
                        _map[tile.tileX, tile.tileY] = pseudoRandom.Next(_minValue, _maxValue);
                    }
                    Debug.Log($"Removed small room region of size {roomRegion.Count}");
                }
                else
                {
                    survivingRooms.Add(new Room(roomRegion, _map));
                }
            }

            // Mark the largest room as the main room
            survivingRooms.Sort();
            if (survivingRooms.Count > 0)
            {
                survivingRooms[0].isMainRoom = true;
                survivingRooms[0].isAccessibleFromMainRoom = true;
                Debug.Log("Main room found and marked.");
            }

            // Connect rooms with passages
            ConnectClosestRooms(survivingRooms);
        }

        // Connects the closest rooms
        void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
        {
            List<Room> roomListA = new List<Room>();
            List<Room> roomListB = new List<Room>();

            if (forceAccessibilityFromMainRoom)
            {
                foreach (Room room in allRooms)
                {
                    if (room.isAccessibleFromMainRoom)
                    {
                        roomListB.Add(room);
                    }
                    else
                    {
                        roomListA.Add(room);
                    }
                }
            }
            else
            {
                roomListA = allRooms;
                roomListB = allRooms;
            }

            int bestDistance = 0;
            Coord bestTileA = new Coord();
            Coord bestTileB = new Coord();
            Room bestRoomA = new Room();
            Room bestRoomB = new Room();
            bool possibleConnectionFound = false;

            // Try to find the closest rooms to connect
            foreach (Room roomA in roomListA)
            {
                if (!forceAccessibilityFromMainRoom)
                {
                    possibleConnectionFound = false;
                    if (roomA.connectedRooms.Count > 0) continue;
                }

                foreach (Room roomB in roomListB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB)) continue;

                    for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                    {
                        for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                        {
                            Coord tileA = roomA.edgeTiles[tileIndexA];
                            Coord tileB = roomB.edgeTiles[tileIndexB];
                            int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                            if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestTileA = tileA;
                                bestTileB = tileB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }

                if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
                {
                    CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                    Debug.Log($"Passage created between room {bestRoomA.roomSize} and room {bestRoomB.roomSize}");
                }
            }

            if (possibleConnectionFound && forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
                ConnectClosestRooms(allRooms, true);
            }

            if (!forceAccessibilityFromMainRoom)
            {
                ConnectClosestRooms(allRooms, true);
            }
        }

        // Creates a passage between two rooms
        void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
        {
            Room.ConnectRooms(roomA, roomB);
            List<Coord> line = GetLine(tileA, tileB);
            foreach (Coord c in line)
            {
                DrawCircle(c, 5);
            }
            Debug.Log($"Passage drawn between rooms at ({tileA.tileX}, {tileA.tileY}) and ({tileB.tileX}, {tileB.tileY})");
        }

        // Draws a circle of empty space at a coordinate
        void DrawCircle(Coord c, int r)
        {
            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        int drawX = c.tileX + x;
                        int drawY = c.tileY + y;
                        if (IsInMapRange(drawX, drawY))
                        {
                            _map[drawX, drawY] = 0;
                        }
                    }
                }
            }
        }

        // Fills the map with random walls and empty spaces
        void RandomFillMap()
        {
            System.Random pseudoRandom = new System.Random(_seed.GetHashCode());
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                    {
                        _map[x, y] = 1; // Create border walls
                    }
                    else
                    {
                        _map[x, y] = (pseudoRandom.Next(0, 100) < _randomFillPercent) ? 1 : 0;
                    }
                }
            }
            Debug.Log("Map randomly filled.");
        }

        // Smooths the map by applying cellular automata rules
        void SmoothMap()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);

                    if (neighbourWallTiles > 4)
                    {
                        _map[x, y] = 1;
                    }
                    else if (neighbourWallTiles < 4)
                    {
                        _map[x, y] = 0;
                    }
                }
            }
            Debug.Log("Map smoothed.");
        }

        // Gets the number of surrounding walls for a specific tile
        int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (IsInMapRange(neighbourX, neighbourY))
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += _map[neighbourX, neighbourY];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }
            return wallCount;
        }

        // Check if a coordinate is within the map range
        bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public List<List<Coord>> GetRegions(int tileType)
        {
            List<List<Coord>> regions = new List<List<Coord>>();
            int[,] mapFlags = new int[_width, _height]; // Keeps track of visited tiles

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (mapFlags[x, y] == 0 && _map[x, y] == tileType)
                    {
                        List<Coord> newRegion = GetRegionTiles(x, y); // Get all tiles in this region
                        regions.Add(newRegion);

                        foreach (Coord tile in newRegion)
                        {
                            mapFlags[tile.tileX, tile.tileY] = 1; // Mark all tiles in this region as visited
                        }
                    }
                }
            }
            return regions;
        }

        // Get all tiles in a specific region starting from a given tile
        List<Coord> GetRegionTiles(int startX, int startY)
        {
            List<Coord> tiles = new List<Coord>();
            int[,] mapFlags = new int[_width, _height];
            int tileType = _map[startX, startY];

            Queue<Coord> queue = new Queue<Coord>();
            queue.Enqueue(new Coord(startX, startY));
            mapFlags[startX, startY] = 1;

            while (queue.Count > 0)
            {
                Coord tile = queue.Dequeue();
                tiles.Add(tile);

                // Check all 8 neighbors
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (IsInMapRange(x, y) && (x == tile.tileX || y == tile.tileY))
                        {
                            if (mapFlags[x, y] == 0 && _map[x, y] == tileType)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Coord(x, y));
                            }
                        }
                    }
                }
            }

            return tiles;
        }

        // Get a straight line of tiles between two coordinates
        public List<Coord> GetLine(Coord from, Coord to)
        {
            List<Coord> line = new List<Coord>();

            int x = from.tileX;
            int y = from.tileY;

            int dx = to.tileX - from.tileX;
            int dy = to.tileY - from.tileY;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Mathf.Abs(dx);
            int shortest = Mathf.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Mathf.Abs(dy);
                shortest = Mathf.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;
            for (int i = 0; i < longest; i++)
            {
                line.Add(new Coord(x, y));

                if (inverted)
                {
                    y += step;
                }
                else
                {
                    x += step;
                }

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted)
                    {
                        x += gradientStep;
                    }
                    else
                    {
                        y += gradientStep;
                    }
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }
    }
}
