using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class WFCGraph
{
    public Node currentNode;
    public Node[] elements;
    public Dictionary<EdgeId, int> edgeMatching;
    System.Random sampler;
    List<Node> toProcess;
    
    public WFCGraph(int[] triangleList, int resolution, System.Random sampler)
    {

        //elements = new Node[((triangleList.Length / 3) / (int)Math.Pow(4, resolution))];
        elements = new Node[((triangleList.Length / 3) )];
        toProcess = new List<Node>();

        // Data extraction from triangleList, and element initialization
        edgeMatching = new Dictionary<EdgeId, int>();

        Edge[] edges = new Edge[elements.Length * 3];

        int matchId;

        for (int i = 0; i < elements.Length; i++)
        {
            int id0 = i * 3;
            int id1 = i * 3 + 1;
            int id2 = i * 3 + 2;

            edges[id0] = new Edge();
            edges[id1] = new Edge();
            edges[id2] = new Edge();

            edges[id0].options = new List<string>() { "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB" };
            edges[id1].options = new List<string>() { "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB" };
            edges[id2].options = new List<string>() { "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB" };

            edges[id0].edgeId.a = triangleList[id0];
            edges[id0].edgeId.b = triangleList[id1];

            edges[id1].edgeId.a = triangleList[id1];
            edges[id1].edgeId.b = triangleList[id2];

            edges[id2].edgeId.a = triangleList[id2];
            edges[id2].edgeId.b = triangleList[id0];

            edgeMatching.Add(edges[id0].edgeId, id0);
            if (edgeMatching.TryGetValue(edges[id0].GetReversedEdgeId(), out matchId))
            {
                // Debug.Log("ALREADY MATCHED");
                edges[id0].adjacentEdge = edges[matchId];
                edges[matchId].adjacentEdge = edges[id0];
            }

            edgeMatching.Add(edges[id1].edgeId, id1);
            if (edgeMatching.TryGetValue(edges[id1].GetReversedEdgeId(), out matchId))
            {
                // Debug.Log("ALREADY MATCHED");
                edges[id1].adjacentEdge = edges[matchId];
                edges[matchId].adjacentEdge = edges[id1];
            }

            edgeMatching.Add(edges[id2].edgeId, id2);
            if (edgeMatching.TryGetValue(edges[id2].GetReversedEdgeId(), out matchId))
            {
                // Debug.Log("ALREADY MATCHED");
                edges[id2].adjacentEdge = edges[matchId];
                edges[matchId].adjacentEdge = edges[id2];
            }

            elements[i] = new Node(i, edges[id0], edges[id1], edges[id2], resolution);
        }

        this.sampler = sampler;

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
        if (lowestEntropyElements.Count > 0)
        {
            return lowestEntropyElements[sampler.Next(0, lowestEntropyElements.Count - 1)];
        }
        else
        {
            return -1;
        }

    }
    public StateInfo Step()
    {

        toProcess.Clear();
        int collapsingId = GetLowestEntropyElementId();
        StateInfo state = StateInfo.SUCCESFUL;
        if (collapsingId != -1)
        {
            Node collapsingNode = elements[collapsingId];
            collapsingNode.Collapse(sampler.Next(0, collapsingNode.entropy - 1));
            Debug.Log(collapsingNode.edges[0].options[0] + collapsingNode.edges[1].options[0] + collapsingNode.edges[2].options[0]);
            toProcess.Add(collapsingNode);
            int localLength;
            while (toProcess.Count > 0)
            {
                
                localLength = toProcess.Count;
                for (int i = 0; i < localLength; i++)
                {
                    state = Propagate(toProcess[0]);
                    if (state == StateInfo.ERROR)
                    {
                        return StateInfo.ERROR;
                    }
                }
            }
        }

        return state;





    }

    public void TestSubState()
    {
        int localLength = 0;
        if (toProcess.Count > 0)
        {
            localLength = toProcess.Count;
            for (int i = 0; i < localLength; i++)
            {
                Propagate(toProcess[0]);
            }
        }
    }

    private StateInfo Propagate(Node elementToProcess)
    {

        toProcess.RemoveAt(0);

        StateInfo localState = StateInfo.IN_PROGRESS;

        for (int i = 0; i < 3; i++)
        {
            // if (elementToProcess.edges[i].adjacentEdge.ownerNode.id != id)
            // {
            if (UpdateNeighbour(elementToProcess.edges[i]))
            {
                //Debug.Log("UPDATED");
                elementToProcess.edges[i].adjacentEdge.ownerNode.entropy = elementToProcess.edges[i].adjacentEdge.options.Count;
                toProcess.Add(elementToProcess.edges[i].adjacentEdge.ownerNode);
                //localState = StateInfo.IN_PROGRESS;

            }

            if (elementToProcess.edges[i].adjacentEdge.ownerNode.entropy == 0)
            {

                return StateInfo.ERROR;
            }
            // }

        }

        return localState;

    }

    private bool UpdateNeighbour(Edge edge)
    {
        int originalLength = edge.adjacentEdge.options.Count;
        int index = 0;
        bool optionCompatible;
        int debugCount = 0;
        while (index < edge.adjacentEdge.options.Count)
        {
            optionCompatible = false;
            debugCount = 0;
            foreach (string option in edge.options)
            {   
                char[] edgeOption = edge.adjacentEdge.options[index].ToCharArray();
                char[] adjacentReversedOption = option.ToCharArray();
                Array.Reverse(adjacentReversedOption);
                debugCount ++;
                
                //Debug.Log();
                Debug.Log(edgeOption[0] == adjacentReversedOption[0] && edgeOption[1] == adjacentReversedOption[1]);
                if (edgeOption[0] == adjacentReversedOption[0] && edgeOption[1] == adjacentReversedOption[1])
                {   
                    //Debug.Log("-------");
                    optionCompatible = true;
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

        // edge.adjacentEdge.options.Clear();
        // edge.adjacentEdge.options.Add("AB");

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
            sampler = new System.Random(seed);
        }
        else
        {
            sampler = new System.Random(sampler.Next());
        }


        foreach (Node n in elements)
        {
            //n.collapsed = false;
            n.edges[0].options = new List<string>() { "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB" };
            n.edges[1].options = new List<string>() { "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB" };
            n.edges[2].options = new List<string>() { "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB" };
            n.entropy = n.edges[0].options.Count;
        }
    }
    public class Node
    {
        public int id;
        public int entropy;
        // public bool collapsed;
        public Edge[] edges;
        public Node(int id, Edge a_Edge, Edge b_Edge, Edge c_Edge, int resolution)

        {
            this.id = id;
            //this.collapsed = false;
            this.entropy = a_Edge.options.Count;

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

    public class Edge
    {
        public EdgeId edgeId;
        public List<string> options;
        public Edge nextInternalEdge;
        public Edge adjacentEdge;
        public Node ownerNode;

        public EdgeId GetReversedEdgeId()
        {
            return new EdgeId(edgeId.b, edgeId.a);
        }


    }

    public struct EdgeId
    {
        public int a;

        public int b;

        public EdgeId(int a, int b)
        {
            this.a = a;

            this.b = b;
        }
    }

    public enum StateInfo
    {
        SUCCESFUL,
        IN_PROGRESS,
        ERROR

    }
}
