using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using Unity.VisualScripting;

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

        currentNode = elements[sampler.Next(0, elements.Length - 1)];




    }

    public WFCGraph(int[] triangleList, int resolution, int seed)
    {
        elements = new Node[((triangleList.Length / 3) / (int)Math.Pow(4, resolution))];

        toProcess = new List<Node>();

        // Data extraction from triangleList, and element initialization
        Dictionary<int, int> edgeMatching = new Dictionary<int, int>();
        Edge[] edges = new Edge[elements.Length * 3];

        for (int i = 0; i < elements.Length; i++)
        {
            edges[i*3].edgeId.a = triangleList[i*3];
            edges[i*3].edgeId.b = triangleList[i*3 + 1];

            edges[i*3 + 1].edgeId.a = triangleList[i*3 + 1];
            edges[i*3 + 1].edgeId.b = triangleList[i*3 + 2];

            edges[i*3 + 2].edgeId.a = triangleList[i*3 + 2];
            edges[i*3 + 2].edgeId.b = triangleList[i*3];


            // edges[i*3].adjacentEdge = edges[*3 + ];
            // edges[i*3 + 1].adjacentEdge = edges[*3 + ];
            // edges[i*3 + 2].adjacentEdge = edges[*3 + ];

            elements[i] = new Node(i, edges[3 * i], edges[3 * i + 1], edges[3 * i + 2], resolution);
        }

        sampler = new Random(seed);

        currentNode = elements[sampler.Next(0, elements.Length - 1)];


    }



    private int GetLowestEntropyElementId()
    {
        // Returns the id of a random Node with the non-one(collasped) lowest entropy
        List<int> lowestEntropyElements = new List<int>();
        int minEntropy = 64;
        foreach (Node n in elements)
        {
            if (n.entropy < minEntropy && n.entropy > 1)
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



    public bool Step()
    {

        toProcess.Clear();
        Node collapsingNode = elements[GetLowestEntropyElementId()];
        collapsingNode.Collapse(sampler.Next(0, collapsingNode.entropy - 1));
        toProcess.Add(collapsingNode);
        int localLength = 0;
        while (toProcess.Count > 0)
        {
            localLength = toProcess.Count;
            for (int i = 0; i < localLength; i++)
            {
                if (!Propagate(toProcess[i], i))
                {
                    return false;
                }
            }
        }

        return true;

    }

    private bool Propagate(Node elementToProcess, int index)
    {

        toProcess.RemoveAt(0);

        for (int i = 0; i < 3; i++)
        {
            if (i != index)
            {
                if (UpdateNeighbour(elementToProcess.edges[i]))
                {
                    elementToProcess.edges[i].adjacentEdge.ownerNode.entropy = elementToProcess.edges[i].adjacentEdge.options.Count;
                    toProcess.Add(elementToProcess.edges[i].adjacentEdge.ownerNode);
                }
                if (elementToProcess.edges[i].adjacentEdge.ownerNode.entropy == 0)
                {
                    return false;
                }
            }
        }

        return true;

    }

    private bool UpdateNeighbour(Edge edge)
    {
        int originalLength = edge.adjacentEdge.options.Count;
        int index = 0;
        bool optionCompatible;
        while (index < edge.adjacentEdge.options.Count)
        {
            optionCompatible = true;
            foreach (string option in edge.options)
            {
                char[] reversedOption = option.ToCharArray();
                Array.Reverse(reversedOption);
                if (!edge.adjacentEdge.options[index].Equals(reversedOption))
                {
                    optionCompatible = false;
                    break;
                }
            }
            if (!optionCompatible)
            {
                edge.adjacentEdge.options.RemoveAt(index);
                edge.adjacentEdge.nextInternalEdge.options.RemoveAt(index);
                edge.adjacentEdge.nextInternalEdge.nextInternalEdge.options.RemoveAt(index);
            }
            else
            {
                index++;
            }
        }
        if (originalLength != edge.adjacentEdge.options.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset(int seed)
    {

        if (seed >= 0)
        {
            sampler = new Random(seed);
        }
        else
        {
            sampler = new Random();
        }


        foreach (Node n in elements)
        {
            n.entropy = 8;
            //n.collapsed = false;
            n.edges[0].options = new List<string>() { "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB" };
            n.edges[1].options = new List<string>() { "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB" };
            n.edges[2].options = new List<string>() { "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB" };
        }
    }
    class Node
    {
        public int id;
        public int entropy;
        // public bool collapsed;
        public Edge[] edges;
        public Node(int id, Edge a_Edge, Edge b_Edge, Edge c_Edge, int resolution)

        {
            this.id = id;
            //this.collapsed = false;
            this.entropy = 8;

            this.edges = new Edge[3];

            this.edges[0] = a_Edge;
            this.edges[1] = b_Edge;
            this.edges[2] = c_Edge;

            this.edges[0].nextInternalEdge = this.edges[1];
            this.edges[1].nextInternalEdge = this.edges[2];
            this.edges[2].nextInternalEdge = this.edges[0];

            for (int i = 0; i < 3; i++)
            {
                this.edges[i].ownerNode = this;
            }

            if (resolution == 0)
            {

            }
            else
            {

            }
        }

        public void Collapse(int option)
        {
            entropy = 1;

            string savedOption;

            for (int i = 0; i < 3; i++)
            {
                savedOption = edges[i].options[option];
                edges[i].options.Clear();
                edges[i].options.Add(savedOption);
            }

        }

    }


    class Edge
    {
        public EdgeId edgeId;
        public List<string> options;
        public Edge nextInternalEdge;
        public Edge adjacentEdge;
        public Node ownerNode;

        public EdgeId GetReversedEdgeId(){
            return new EdgeId(edgeId.b,edgeId.a);
        }


    }

    struct EdgeId {
        public int a;

        public int b;

        public EdgeId(int a, int b){
            this.a = a;

            this.b = b;
        }
    }
}
