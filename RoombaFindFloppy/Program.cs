using System;
using System.Collections;

namespace RoombaFindFloppy
{
    class Program
    {
        static void Main(string[] args)
        {
            Program myProgram = new Program();
            int columns = 6; //3;
            int rows = 6;   //3;
            int size = columns * rows;
            int[,] testFloppyCoord = { { 4, 2 }, { 0, 5 } };
            int[,] testWallCoord = { { 2, 1 }, { 2, 2 }, { 1, 4 }, { 3, 5 } };

            //simpler test case:
            //int[,] testFloppyCoord = { { 1, 2 }, { 1, 0 } };
            //int[,] testWallCoord = { { 0, 1 }, { 2, 0 } };

            //0,0     1,0🐰   2,0🚧
            //0,1🚧   1,1     2,1
            //0,2     1,2🐰   2,2

            //first, generate the Nodes involved in the grid
            Node[] allMyNodes = myProgram.SetupGridProblem(columns, rows, testFloppyCoord, testWallCoord);

            for (int i = 0; i < size; i++)
            {
                string thisItemCellState;
                if (allMyNodes[i].myCellType == Node.CellType.Path)
                {
                    thisItemCellState = "[ ] ";
                }
                else if (allMyNodes[i].myCellType == Node.CellType.Floppy)
                {
                    thisItemCellState = " 🐰 ";
                }
                else
                {
                    thisItemCellState = " 🚧 ";
                }
                Console.Write(thisItemCellState);
                if ((i + 1) % columns == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine("...finding floppy...");

            //this part computes the distances
            int[] distancesComputed = myProgram.FindShortestDistFromRoombaToFloppy(allMyNodes);


            for (int i = 0; i < size; i++)
            {
                string thisRow = distancesComputed[i] + " ";
                Console.Write(thisRow);
                if((i+1) % columns == 0)
                {
                    Console.WriteLine();
                }
            }
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

        Node[] SetupGridProblem(int cols, int rows, int[,] floppyCoords, int[,] wallCoords)
        {
            int size = rows * cols;
            Node[] allMyNodes = new Node[size];

            for (int i = 0; i < size; i++)
            {
                allMyNodes[i] = new Node(Node.CellType.Path, GenerateGridCoords(cols, rows, i), null, false, false, -1, -1);
                //Console.WriteLine(string.Join(",", allMyNodes[i].xyCoord));
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
                //Console.WriteLine(string.Join(",", allMyNodes[i].myCellType));
                allMyNodes[i].neighbors = AddNeighbors(allMyNodes, allMyNodes[i], i, cols, rows);
            }
            return allMyNodes;
        }

        int[] FindShortestDistFromRoombaToFloppy(Node[] myNodes)
        {
            int size = myNodes.Length;
            int[] gridComputed = new int[size];

            //try calculating just one
            //allMyNodes[0].distCount = distToGuard(allMyNodes[0]);

            //calculate the whole grid:
            for (int i = 0; i < myNodes.Length; i++)
            {
                myNodes[i].distCount = distToGuard(myNodes[i]);
                ResetData(myNodes);
                gridComputed[i] = myNodes[i].distCount;
            }

            return gridComputed;
            
        }
        int LevelOrder(Node root)
        {
            if(root == null)
            {
                return -1;
            }
            Queue Q = new Queue();
            Q.Enqueue(root);
            root.relativeLevel = 0;
            while(Q.Count != 0)
            {
                Node current = (Node)Q.Peek();
                Q.Dequeue();
                current.isVisited = true;
                if(current.myCellType == Node.CellType.Floppy)
                {
                    return current.relativeLevel;
                }
                foreach(Node neigh in current.neighbors)
                {
                    
                    if (neigh != null && !neigh.isVisited)
                    {
                        neigh.relativeLevel = current.relativeLevel + 1;
                        Q.Enqueue(neigh);
                        neigh.isVisited = true;
                    }
                }
            }
            return -1; //meaning, path does not exist
        }

        void ResetData(Node[] myNodes)
        {
            foreach(Node node in myNodes)
            {
                node.isVisited = false;
                node.relativeLevel = -1;
            }
        }
        int distToGuard(Node whichNode)
        {
            int findDistWithBFSTraversal = 100;
            if (whichNode.myCellType == Node.CellType.Wall)
            {
                return -1;
            }
            //base case
            if (whichNode.myCellType == Node.CellType.Floppy)
            {
                return 0;
            } else
            {
                findDistWithBFSTraversal = LevelOrder(whichNode);
                return findDistWithBFSTraversal;
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
            Path = 0,
            Floppy,
            Wall
        }
        public CellType myCellType;
        public int[] xyCoord = new int[2];
        public Node[] neighbors = new Node[4]; //Left,Right,Up,Down
        public bool calculationComplete;
        public bool isVisited;
        public int relativeLevel; 
        public int distCount;

        public Node(CellType myCellType, int[] xyCoord, Node[] neighbors, bool calculationComplete, bool isVisited, int relativeLevel, int distCount)
        {
            this.myCellType = myCellType;
            this.xyCoord = xyCoord;
            this.neighbors = neighbors;
            this.calculationComplete = calculationComplete;
            this.isVisited = isVisited;
            this.relativeLevel = relativeLevel;
            this.distCount = distCount;
        }
    }
}
