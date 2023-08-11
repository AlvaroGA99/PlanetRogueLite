using System.Collections.Generic;
using System;
public class WFCGraph
{
    Node currentNode;
    List<Node> elements;

    public WFCGraph(ref int[] triangleList, int resolution)
    {
        elements = new List<Node>((triangleList.Length / 3)/(int)Math.Pow(4,resolution));

        // Data extraction from triangleList, and element initialization

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i] = new Node();
        }

        currentNode = elements[0];
    }

    public void RecalculateEntropy()
    {
        //Recalculate Entropy
        foreach (Node n in elements)
        {
            n.entropy = n.a_Edge.options.Count;
        }
    }

    public int GetLowestEntropyElementId(Random sampler)
    {   
        // Returns the id of a random Node with the non-zero lowest entropy
        List<int> lowestEntropyElements = new List<int>();
        int minEntropy = 64;
        foreach (Node n in elements)
        {
            if (n.entropy < minEntropy && n.entropy > 0)
            {
                minEntropy = n.entropy;
                lowestEntropyElements.Clear();
                lowestEntropyElements.Add(n.id);
            }
            else if (n.entropy == minEntropy)
            {
                lowestEntropyElements.Add(n.id);
            }


        }

        return lowestEntropyElements[sampler.Next(0, lowestEntropyElements.Count)];
    }
    class Node
    {
        public int id;
        public int entropy;
        bool collapsed;
        public Edge a_Edge;
        public Edge b_Edge;
        public Edge c_Edge;

        public Node()
        {

        }

    }


    class Edge
    {

        public List<string> options;

        Edge nextInternalEdge;

        Edge adjacentEdge;

        Node ownerNode;

        public Edge()
        {

        }
    }
}
