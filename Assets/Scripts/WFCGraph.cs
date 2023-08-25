using System.Collections.Generic;
using System;
using UnityEngine.Profiling;

public class WFCGraph
{
    Node currentNode;
    Node[] elements;
    Random sampler;

    List<Node> toProcess;

    public WFCGraph(int[] triangleList, int resolution)
    {
        elements = new Node[((triangleList.Length / 3) / (int)Math.Pow(4, resolution))];

        toProcess = new List<Node>();

        // Data extraction from triangleList, and element initialization

        Edge[] edges = new Edge[elements.Length * 3];

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = new Node(i, edges[3 * i], edges[3 * i + 1], edges[3 * i + 2], resolution);
        }

        sampler = new Random();

        currentNode = elements[sampler.Next(0,elements.Length - 1)];

        


    }

        public WFCGraph(int[] triangleList, int resolution, int seed) 
    {
        elements = new Node[((triangleList.Length / 3) / (int)Math.Pow(4, resolution))];

        toProcess = new List<Node>();

        // Data extraction from triangleList, and element initialization

        Edge[] edges = new Edge[elements.Length * 3];

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = new Node(i, edges[3 * i], edges[3 * i + 1], edges[3 * i + 2], resolution);
        }

        sampler = new Random(seed);

        currentNode = elements[sampler.Next(0,elements.Length - 1)];

   
    }

    public void RecalculateEntropy()
    {
        //Recalculate Entropy
        foreach (Node n in elements)
        {
            n.entropy = n.a_Edge.options.Count;
        }
    }

    public int GetLowestEntropyElementId()
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

        return lowestEntropyElements[sampler.Next(0, lowestEntropyElements.Count - 1)];
    }

    public void Reset(int seed){

        if (seed >= 0){
        sampler = new Random(seed);    
        }else{
            sampler = new Random();
        }
        

        foreach (Node n in elements)
        {
            n.entropy = 5;
            n.collapsed = false;
            n.a_Edge.options = new List<string>() {"AA","AA","AB","AB","BA","BA","BB","BB"};
            n.b_Edge.options = new List<string>() {"AA","AB","BA","BB","AA","AB","BA","BB"};
            n.c_Edge.options = new List<string>() {"AA", "BA", "AA", "BA", "AB", "BB", "AB" ,"BB"};
        }
    }
    class Node
    {
        public int id;
        public int entropy;
        public bool collapsed;
        public Edge a_Edge;
        public Edge b_Edge;
        public Edge c_Edge;
        public Node(int id, Edge a_Edge, Edge b_Edge, Edge c_Edge, int resolution)

        {
            this.id = id;
            this.collapsed = false;
            this.entropy = 5;

            this.a_Edge = a_Edge;
            this.b_Edge = b_Edge;
            this.c_Edge = c_Edge;

            this.a_Edge.ownerNode = this;
            this.b_Edge.ownerNode = this;
            this.c_Edge.ownerNode = this;

            if (resolution == 0)
            {
                switch (id)
                {
                    
                }
            }
            else
            {
                
            }
        }

    }


    class Edge
    {

        public List<string> options;

        public Edge nextInternalEdge;

        public Edge adjacentEdge;

        public Node ownerNode;

    }
}
