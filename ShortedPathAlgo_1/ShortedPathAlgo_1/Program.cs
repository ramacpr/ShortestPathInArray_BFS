using System;
using System.Collections.Generic;

namespace ShortedPathAlgo1
{
    public class Node
    {
        public int row { get; set; }
        public int column { get; set;  }
        public Node(int r, int c, int val)
        {
            row = r;
            column = c;
            Value = val; 
        }

        Dictionary<Node, int> AllParents = null;
        List<Node> ClosestParents = null; 
        public int Value { get; set; }

        public Dictionary<Node, int> GetAllParents()
        {
            return AllParents; 
        }

        public void AddParent(Node parentNode, int weight)
        {
            if (AllParents == null)
                AllParents = new Dictionary<Node, int>();

            if(AllParents.ContainsKey(parentNode) == true)
            {
                // replace the existing weight with new weight if and only if 
                // the new weight is smaller. Coz, we are interested in the 
                // shorted distance problem. 
                if (weight > AllParents[parentNode])
                    return;
            }
            AllParents[parentNode] = weight;
        }

        public List<Node> GetClosestParents()
        {
            if (AllParents == null)
                return null; 

            // the closest parents will be computed only once per node
            if(ClosestParents == null)
            {
                ClosestParents = new List<Node>();
                int leastDistance = -1;
                // find the minimum of all values 
                foreach (int val in AllParents.Values)
                {
                    if (leastDistance == -1)
                    {
                        leastDistance = val;
                        continue;
                    }

                    if (val < leastDistance)
                        leastDistance = val; 
                }

                foreach (KeyValuePair<Node, int> kv in AllParents)
                    if (kv.Value == leastDistance)
                        ClosestParents.Add(kv.Key);
            }
            return ClosestParents; 
        }
    }
    class Program
    {
        static Node[,] AllNodes;
        static int MaxRows;
        static int MaxCols; 

        static void Main()
        {
            ShortestPath(); 
        }

        static void ShortestPath()
        {              
            Console.WriteLine("Enter Valid Rows"); 
            while (int.TryParse(Console.ReadLine(), out MaxRows) == false) ;
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.WriteLine("Enter Valid Columns");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            while (int.TryParse(Console.ReadLine(), out MaxCols) == false) ;

            int[] colData = new int[MaxCols];

            AllNodes = new Node[MaxRows,MaxCols];

            for(int i = 0; i < MaxRows; i ++)
            {
                Console.WriteLine("Enter " + i.ToString() + " row's data:");
                int[] inputs = Array.ConvertAll(Console.ReadLine().Trim().Split(' '), x => int.Parse(x));
                for(int j = 0; j < MaxCols; j++)                
                    AllNodes[i, j] = new Node(i, j, inputs[j]);
            }

            DiscoverPaths(0, 0, 1);

            // take the final node from user 
            bool end = false;
            int[] input;
            List<Node> path = new List<Node>();
            while (end == false)
            {
                Console.WriteLine("\nEnter final row and column to find the shortes path or just enter to exit!");
                input = Array.ConvertAll(Console.ReadLine().Trim().Split(" "), x => int.Parse(x));
                if (input.Length == 2)
                {
                    if (input[0] > MaxRows || input[1] > MaxCols)
                    {
                        Console.WriteLine("Invalid row or column index!");
                        continue;
                    }                    

                    path.Add(AllNodes[input[0], input[1]]);
                    PrintShortestPath(AllNodes[input[0], input[1]], path);

                    path.Clear();
                }
                else
                    end = true;
            }

            // 
            

        }

        static void DiscoverPaths(int row, int col, int weight)
        {
            if (row < 0 || col < 0)
                return;
            if (row >= MaxRows && col >= MaxCols)
                return;

            Node thidNode = AllNodes[row, col];
            // if the node is invalid (not present) or if the parent have already been analysed, skip
            if (thidNode == null)
                return;

            List<Node> adjNodes = new List<Node>(); 

            //int[] adjRow = new int[3] { -1, -1, -1 };
            //int[] adjCol = new int[3] { -1, -1, -1 };

            if (col + 1 < MaxCols)
            {
                AllNodes[row, col + 1].AddParent(thidNode, weight);
                adjNodes.Add(AllNodes[row, col + 1]);
                //adjRow[0] = row; adjCol[0] = col + 1;
            }
            if(row + 1 < MaxRows)
            {
                AllNodes[row + 1, col].AddParent(thidNode, weight);
                adjNodes.Add(AllNodes[row + 1, col]);
            }
            if(col + 1 < MaxCols && row + 1 < MaxRows)
            {
                AllNodes[row + 1, col + 1].AddParent(thidNode, weight);
                adjNodes.Add(AllNodes[row + 1, col + 1]);
            }


            foreach(Node node in adjNodes)
            {
                DiscoverPaths(node.row, node.column, weight + 1);
            }
        }

        static void PrintShortestPath(Node finalNode, List<Node> path)
        {
            if (finalNode == null || path == null)
                return;

            List<Node> closestParents = finalNode.GetClosestParents(); 
            if(closestParents == null)
            {
                Console.WriteLine("");
                Console.Write("Path Length = " + path.Count.ToString());
                foreach (Node node in path)
                    Console.Write(" [" + node.row.ToString() + "," + node.column.ToString() + "] ");
                return; 
            }
            
            foreach(Node parent in closestParents)
            {
                path.Add(parent);
                PrintShortestPath(parent, path);
                path.Remove(parent);
            }
        }

    }
}
