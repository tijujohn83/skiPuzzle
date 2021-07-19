using System;
using System.Collections.Generic;
using System.Linq;

//http://geeks.redmart.com/2015/01/07/skiing-in-singapore-a-coding-diversion/
namespace SnakeAndLadders
{
    class SingaporeSki
    {
        public int Size { get; private set; }

        public LinkedList<SkiData>[]  AdjacencyList;
        public SingaporeSki(int size)
        {
            Size = size;

            AdjacencyList = new LinkedList<SkiData>[size*size +1];
            for (int i = 1; i <= size*size; i++)
            {
                AdjacencyList[i] = new LinkedList<SkiData>();
            }
        }

        public void AddEdgeAtBegin(SkiData startVertex, SkiData endVertex)
        {
            AdjacencyList[startVertex.Value].AddFirst(endVertex);
        }
        public void CreateAdjacencyList(int[][] input)
        {
            int c = 1;
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    SkiData startVertex = new SkiData(c);

                    if (i - 1 >= 0 && input[i-1][j] < input[i][j] ) //GetUpVertex
                    {
                        SkiData endVetex = new SkiData(c-Size);
                        AddEdgeAtBegin(startVertex, endVetex);
                    }
                    if (i + 1 < Size && input[i + 1][j] < input[i][j]) //GetDownVertex
                    {
                        SkiData endVetex = new SkiData(c+Size);
                        AddEdgeAtBegin(startVertex, endVetex);
                    }
                    if (j - 1 >= 0  && input[i][j-1] < input[i][j] ) //GetLeftVertex
                    {
                        SkiData endVetex = new SkiData(c-1);
                        AddEdgeAtBegin(startVertex, endVetex);
                    }
                    if (j + 1 < Size && input[i][j+1] < input[i][j]) //GetRightVertex
                    {
                        SkiData endVetex = new SkiData(c+1);
                        AddEdgeAtBegin(startVertex, endVetex);
                    }
                    c++;
                }
            }
        }
      

        public void DFS(int[] level ,bool[] visited,int[] input)
        {
            for (int i = 1; i <= Size*Size; i++)
            {
                  Queue<int> queue = new Queue<int>();
                if (AdjacencyList[i] != null)
                {
                    level[i] = Math.Max(1, level[i]); //setting Level =1;
                    queue.Enqueue(i); //enqueue root
                    int depth = 1;
                    level[i] = DepthFirstSearch(i, visited, level[i],level);
                }
            }
            int maxlevels = level.ToList().Max();
            List<int> nodes = new List<int>();
            for (int i = 1; i <= Size * Size; i++)
            {
                if (level[i] == maxlevels)
                    nodes.Add(i);
            }
            int max = 0;
            foreach (var node in nodes)
            {
                int startnode = node;;
                int endValue = Traversepath(level, node, maxlevels, input);
                max = Math.Max(max, input[startnode] - endValue);
            }

        }


        public int Traversepath(int[] levels, int node, int level,int[] input)
        {
            int min = Int32.MaxValue;

            foreach (var next in GetAllNextVertices(node,levels))
            {
                int c = Traversepath(levels, next, level - 1, input);
                min = Math.Min(c, min);
            }
            return Math.Min(min,input[node]);
        }

        private IEnumerable<int> GetAllNextVertices(int node,int[] levels)
        {
            int currentlevel = levels[node];
            //GetTop
            if ((node > Size) && (levels[node - Size] == currentlevel - 1))
                yield return node - Size;
            //GetDown
            if ((node <= (Size*Size -Size)) && (levels[node + Size] == currentlevel - 1))
                yield return node+Size;
            //GetLeft
            if ((node%Size != 1) && (levels[node-1] == currentlevel - 1))
                yield return node-1;
            //GetRight
            if ((node % Size != 0) && (levels[node + 1] == currentlevel - 1))
                yield return node+1;
        }

        public int DepthFirstSearch(int node, bool[] visited, int level,int[] levels)
        {
            int max = 1;
            if (!(visited[node]))
            {
                foreach (var list in AdjacencyList[node])
                {
                    if (!(visited[list.Value]))
                    {
                        int clevel = 1 + DepthFirstSearch(list.Value, visited, level, levels);
                        max = Math.Max(max, clevel);
                    }
                    else
                    {
                        max = Math.Max(levels[list.Value] + level, max);
                    }
                    visited[list.Value] = true;
                }
             
                levels[node] = Math.Max(levels[node], max);
                visited[node] = true;
            }
            return levels[node];

        }

        public class SkiData
        {
            public int Value;



            public SkiData(int value)
            {
                Value = value;

            }
            
        }
    }
}
