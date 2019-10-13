using System;
using System.Collections;

namespace RoombaFindFloppy
{
    class Program
    {
        static void Main(string[] args)
        {
            Program myProgram = new Program();
            int columns = 5; //3;
            int rows = 5;   //3;
            int size = columns * rows;
            int[,] testGrid = new int[size, 2];
            for(int i = 0; i < testGrid.GetLength(0); i++)
            {
                testGrid[i,0] = myProgram.GenerateGridCoords(columns, rows, i)[0];
                testGrid[i, 1] = myProgram.GenerateGridCoords(columns, rows, i)[1];

            }

            int[,] testFloppyCoord = { { 4, 2 }, { 0, 5 } };
            int[,] testWallCoord = { { 2, 1 }, { 2, 2 }, { 1, 4 }, { 3, 5 } };

            //test case 1:
            //int[,] testFloppyCoord = { { 1, 2 }, { 1, 0 } };
            //int[,] testWallCoord = { { 0, 1 }, { 2, 0 } };

            //0,0     1,0🐰   2,0🚧
            //0,1🚧   1,1     2,1
            //0,2     1,2🐰   2,2

            //test case 2:
            //int[,] testFloppyCoord = { { 4, 2 }, { 0, 5 } };
            //int[,] testWallCoord = { { 2, 1 }, { 2, 2 }, { 1, 4 }, { 3, 5 } };

            int[,] ShortDistTest = myProgram.FindShortestDistFromRoombaToFloppy(columns, rows, testFloppyCoord, testWallCoord);
            
        }

        int[] GenerateGridCoords(int cols, int rows, int flattenedIndex)
        {
            int[] gridCoordinates = new int[2];
            int x = flattenedIndex % cols;
            int y = (flattenedIndex / cols) % rows;
            gridCoordinates[0] = x;
            gridCoordinates[1] = y;
            return gridCoordinates;
        }

        int[,] FindShortestDistFromRoombaToFloppy(int cols, int rows, int[,] floppyCoords, int[,] wallCoords)
        {
            int size = rows * cols;
            int[,] gridComputed = new int[size, 2];
            Node[] allMyNodes = new Node[size];
            //populate the nodes
            for(int i = 0; i < size; i++)
            {
                allMyNodes[i] = new Node(Node.CellType.Undefined, GenerateGridCoords(cols,rows,i), null, false, false, -1);
                Console.WriteLine(string.Join(",", allMyNodes[i].xyCoord));
                for (int j = 0; j < floppyCoords.GetLength(0); j++)
                {
                    if (allMyNodes[i].xyCoord[0] == floppyCoords[j, 0] && allMyNodes[i].xyCoord[1] == floppyCoords[j, 1])
                    {
                        allMyNodes[i].myCellType = Node.CellType.Floppy;
                    }
                }

                for (int j = 0; j < wallCoords.GetLength(0); j++)
                {
                    if (allMyNodes[i].xyCoord[0] == wallCoords[j, 0] && allMyNodes[i].xyCoord[1] == wallCoords[j, 1])
                    {
                        allMyNodes[i].myCellType = Node.CellType.Wall;
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                Console.WriteLine(string.Join(",", allMyNodes[i].xyCoord));

                allMyNodes[i].neighbors = AddNeighbors(allMyNodes, allMyNodes[i], i, cols, rows);

            }

            //for(int i =0; i < size; i++)
            //{
            //   allMyNodes[i].distCount = distToGuard(allMyNodes[i]);
            //   Console.WriteLine("dist: " + allMyNodes[i].distCount);

            //}

            //try calculating just one
            allMyNodes[0].distCount = distToGuard(allMyNodes[0]);
            Console.WriteLine(allMyNodes[0].distCount);
            //for (int i = 0; i < rows; i++)
            //{
            //    for (int j = 0; j < rows; j++)
            //    {
            //        string thisRow = "" + allMyNodes[i].distCount;
            //    }
            //    Console.WriteLine()
            //}


            return gridComputed;
            
        }
        Queue LevelOrder(Node root)
        {
            if(root == null)
            {
                return null;
            }
            Queue Q = new Queue();
            Q.Enqueue(root);
            while(Q.Count != 0)
            {
                Node current = (Node)Q.Peek();
                foreach(Node neigh in current.neighbors)
                {
                    if (neigh != null && !neigh.isVisited)
                    {
                        Q.Enqueue(neigh);
                        neigh.isVisited = true;
                    }
                }
                //Q.Dequeue();
            }
            return Q;
        }
        int distToGuard(Node whichNode)
        {
            int distTracker = 0;
            int potentialWinner = 100;
            if (whichNode.myCellType == Node.CellType.Wall)
            {
                return -1;
            }
            //pre-base case
            if (whichNode.myCellType == Node.CellType.Floppy)
            {
                return 0;
            } else
            {

                //base case
                foreach (Node neigh in whichNode.neighbors)
                {
                    if (neigh != null && !neigh.isVisited)
                    {
                        if (neigh.myCellType == Node.CellType.Floppy)
                        {
                            distTracker = 1;
                            return 1;
                        }
                    }
                }

                //traversal
                //queue method
                Queue myQueue = LevelOrder(whichNode);
                while (myQueue.Count != 0)
                {
                    Node current = myQueue.Dequeue();

                    distTracker = 1 + distToGuard(current);
                    Console.WriteLine(distTracker);
                    if (distTracker < potentialWinner)
                    {
                        potentialWinner = distTracker;
                    }
                }

                //foreach (Node neigh in whichNode.neighbors)
                //{
                //    if (neigh != null && !neigh.isVisited)
                //    {
                //        neigh.isVisited = true;
                //        if (neigh.distCount == -1)
                //        {
                //            Console.WriteLine(1 + distToGuard(neigh));
                //            return 1 + distToGuard(neigh);
                //        }
                //        //else
                //        //{
                //        //    return 1 + neigh.distCount;
                //        //}
                //        //}
                //    }

                //    int storedDist;
                //    if (neigh != null && !neigh.isVisited)
                //    {
                //        neigh.isVisited = true;
                //        storedDist = 1 + distToGuard(neigh);
                //    }

                //}
                //return distToGuard(whichNode.neighbors[0])
                return potentialWinner;
                //return -1 ;
            }

        }

        Node[] AddNeighbors(Node[] allAvailableNodes, Node selfNode, int index, int width, int height)
        {
            if (selfNode.myCellType == Node.CellType.Wall)
            {
                return null;
            }
            Node[] NeighborTracking = new Node[4];
            if (selfNode.xyCoord[0] != 0)
            {
                if (allAvailableNodes[index -1].myCellType != Node.CellType.Wall)
                {
                    NeighborTracking[0] = allAvailableNodes[index - 1];
                }
            }
            if(selfNode.xyCoord[0] != width - 1)
            {
                if (allAvailableNodes[index + 1].myCellType != Node.CellType.Wall)
                {
                    NeighborTracking[1] = allAvailableNodes[index + 1];
                }

            }
            if (selfNode.xyCoord[1] != 0)
            {
                if (allAvailableNodes[index - width].myCellType != Node.CellType.Wall)
                {
                    NeighborTracking[2] = allAvailableNodes[index - width];
                }
            }
            if(selfNode.xyCoord[1] != height - 1)
            {
                if(allAvailableNodes[index + width].myCellType != Node.CellType.Wall)
                {
                    NeighborTracking[3] = allAvailableNodes[index + width];
                }
            }
            return NeighborTracking;
        }

    }
    class Node
    {
        public enum CellType
        {
            Undefined = 0,
            Path,
            Floppy,
            Wall
        }
        public CellType myCellType;
        public int[] xyCoord = new int[2];
        public Node[] neighbors = new Node[4]; //Left,Right,Up,Down
        public bool calculationComplete;
        public bool isVisited;
        public int distCount;

        public Node(CellType myCellType, int[] xyCoord, Node[] neighbors, bool calculationComplete, bool isVisited, int distCount)
        {
            this.myCellType = myCellType;
            this.xyCoord = xyCoord;
            this.neighbors = neighbors;
            this.calculationComplete = calculationComplete;
            this.isVisited = isVisited;
            this.distCount = distCount;
        }
    }
}
