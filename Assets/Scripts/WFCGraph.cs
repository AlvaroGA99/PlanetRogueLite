using System.Collections.Generic;
using UnityEngine;
using PlanetProperties;
using System.Linq;
using UnityEngine.InputSystem.Interactions;
using System;

public class WFCGraph
{
    
    private PlanetTopography planetTopography;
    public Node currentNode;
    public Node[] elements;
    public Dictionary<EdgeId, int> edgeMatching;
    public readonly HashSet<string> compatibilityList  =  new HashSet<string>(){
       "AAAAAA","AABBAA","AACCAA","ABAABA","ABBBBA","ABCCBA","ACAACA","ACBBCA","ACCCCA",
       "BAAAAB","BABBAB","BACCAB","BBAABB","BBBBBB","BBCCBB","BCAACB","BCBBCB","BCCCCB",
       "CAAAAC","CABBAC","CACCAC","CBAABC","CBBBBC","CBCCBC","CCAACC","CCBBCC","CCCCCC"
    };
    System.Random sampler;
    List<Node> toProcess;
    StateInfo state;
    public List<int> lowestEntropyElementList;

    public WFCGraph(int[] triangleList, int resolution, System.Random sampler)
    {
        int dim = triangleList.Length / 3;
        elements = new Node[((dim))];
        toProcess = new List<Node>();
        Array vals = Enum.GetValues(typeof(PlanetTopography));
        planetTopography = (PlanetTopography)vals.GetValue(sampler.Next(vals.Length));
        Debug.Log(planetTopography);
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

            //edges[id0].options = new List<string>() { "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB" };
            //edges[id1].options = new List<string>() { "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB" };
            //edges[id2].options = new List<string>() { "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB" };

            edges[id0].ResetID0Option(planetTopography);
            edges[id1].ResetID1Option(planetTopography);
            edges[id2].ResetID2Option(planetTopography);
            
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
        ComputeLowestEntropyElementList();

    }

    public void ComputeLowestEntropyElementList(){
        lowestEntropyElementList = new List<int>();
        int minEntropy = 1000;
        for (int i = 0; i < elements.Length; i ++)
        {
            if (elements[i].entropy < minEntropy && elements[i].entropy > 1)
            {
                minEntropy = elements[i].entropy;
                lowestEntropyElementList.Clear();
                lowestEntropyElementList.Add(i);
            }
            else if (elements[i].entropy == minEntropy)
            {
                lowestEntropyElementList.Add(i);
            }


        }
    }

    private int GetLowestEntropyElementId()
    {
        //Returns the id of a random Node with the non-one(collasped) lowest entropy
        
        if (lowestEntropyElementList.Count > 0)
        {
            return lowestEntropyElementList[sampler.Next(0, lowestEntropyElementList.Count - 1)];
            //return sampler.Next(0, lowestEntropyElements.Count - 1);
        }
        else
        {
            return -1;
        }

    }

    public StateInfo Step()
    {
        //for(int i = 0; i < propagated.Length; i++){
          //  propagated[i] = false;
        //}
        toProcess.Clear();
        int collapsingId = GetLowestEntropyElementId();
        state = StateInfo.SUCCESFUL;
        if (collapsingId != -1)
        {   
            //lowestEntropyElementList.Remove(collapsingId);
            Node collapsingNode = elements[collapsingId];
            collapsingNode.Collapse(sampler.Next(0, collapsingNode.entropy - 1));
            toProcess.Add(collapsingNode);
            //propagated[collapsingId] = true;
            // for (int i = 0; i < 3; i++)
            // {   collapsingId = GetLowestEntropyElementId();
            //     if(!propagated[collapsingId]){
            //         collapsingNode.Collapse(sampler.Next(0, collapsingNode.entropy - 1));
            //         toProcess.Add(collapsingNode);
            //         propagated[collapsingId] = true;
            //     }
            // }

            int localLength;
            while (toProcess.Count > 0)
            {
                localLength = toProcess.Count;
                for (int i = 0; i < localLength; i++)
                {
                    state = Propagate(toProcess[0]);
                    if (state == StateInfo.ERROR)
                    {   
                        //toProcess.Clear();
                        //break;
                        return StateInfo.ERROR;
                    }
                    //yield return null;
                }
            }
        }
        return state;
    }

    private StateInfo Propagate(Node elementToProcess)
    {
        
        toProcess.RemoveAt(0);

        StateInfo state = StateInfo.IN_PROGRESS;

        for (int i = 0; i < 3; i++)
        {
            if (elementToProcess.edges[i].adjacentEdge.ownerNode.entropy  != 1 )
            {
            if (UpdateNeighbour(elementToProcess.edges[i]))
            {
                elementToProcess.edges[i].adjacentEdge.ownerNode.entropy = elementToProcess.edges[i].adjacentEdge.options.Count;
                //if (!propagated[elementToProcess.edges[i].adjacentEdge.ownerNode.id]){
                    toProcess.Add(elementToProcess.edges[i].adjacentEdge.ownerNode);
                  //  propagated[elementToProcess.edges[i].adjacentEdge.ownerNode.id] = true;
                //}
            }
            }
            //Debug.Log(elementToProcess.edges[0].adjacentEdge.ownerNode.entropy);
            if (elementToProcess.edges[0].adjacentEdge.ownerNode.entropy == 0)
            {
                //state = StateInfo.ERROR;
                //break;
                return StateInfo.ERROR;
            }
            
        }
        return state;

    }

    private bool UpdateNeighbour(Edge edge)
    {
        //if(propagatedFrom[edge.ownerNode.id] == edge.adjacentEdge.ownerNode.id){
          //  return false;
        //}
        int originalLength = edge.adjacentEdge.options.Count;
        int index = 0;
        bool optionCompatible;
        //int debugCount = 0;
        while (index < edge.adjacentEdge.options.Count)
        {
            optionCompatible = false;
            //debugCount = 0;
            foreach (string option in edge.options)
            {
                //char[] edgeOption = edge.adjacentEdge.options[index].ToCharArray();
                //char[] adjacentReversedOption = option.ToCharArray();
                //Array.Reverse(adjacentReversedOption);
                //debugCount++;

                //Debug.Log();
                //Debug.Log(edgeOption[0] == adjacentReversedOption[0] && edgeOption[1] == adjacentReversedOption[1]);
                //if (edgeOption[0] == adjacentReversedOption[0] && edgeOption[1] == adjacentReversedOption[1] && edgeOption[2] == adjacentReversedOption[2])
                if(compatibilityList.Contains(edge.adjacentEdge.options[index] + option) )
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
                 //edge.adjacentEdge.options.Remove()
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

        Array vals = Enum.GetValues(typeof(PlanetTopography));
        planetTopography = (PlanetTopography)vals.GetValue(sampler.Next(vals.Length));

        foreach (Node n in elements)
        {
            //n.collapsed = false;
            //n.edges[0].options = new List<string>() { "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB" };
            //n.edges[1].options = new List<string>() { "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB", "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB" };
            //n.edges[2].options = new List<string>() { "AA", "BA", "AA", "BA", "AB", "BB", "AB", "BB", "AA", "AB", "BA", "BB", "AA", "AB", "BA", "BB", "AA", "AA", "AB", "AB", "BA", "BA", "BB", "BB" };
            
             n.edges[0].ResetID0Option(planetTopography);
             n.edges[1].ResetID1Option(planetTopography);
             n.edges[2].ResetID2Option(planetTopography);

            n.entropy = n.edges[0].options.Count;

            n.tileVertices = new List<int> {n.edges[0].edgeId.a,n.edges[1].edgeId.a,n.edges[2].edgeId.a};
            n.tileTriangles = new List<int>();
        }
        ComputeLowestEntropyElementList();
    }

    public StateInfo CheckState(){
        return state;
    }

    public bool CheckTopography(Edge edge,int index){
            if(planetTopography == PlanetTopography.Laky ){
                //if(edge.options[index].Contains('C')){
                    return edge.nextInternalEdge.options[index].Contains('C') || edge.nextInternalEdge.nextInternalEdge.options[index].Contains('C');
                //}else{
                  //  return true;
                //}   
            }else if(planetTopography == PlanetTopography.Rocky ){
                //if(edge.options[index].Contains('A')){
                    return edge.nextInternalEdge.options[index].Contains('A') || edge.nextInternalEdge.nextInternalEdge.options[index].Contains('A');
                //}else{
                  //  return true;
                //}   
            }else{
                //if(edge.options[index].Contains('B')){
                    return edge.nextInternalEdge.options[index].Contains('B') || edge.nextInternalEdge.nextInternalEdge.options[index].Contains('B');
                //}else{
                  //  return true;
                //}   
            }
        }
    public class Node
    {
        public int id;
        public int entropy;
        public Edge[] edges;
        public Vector3 reference;
        public List<Vector3> meshVertices;
        public List<int> tileVertices;
        public List<int> tileTriangles;

        public Node(int id, Edge a_Edge, Edge b_Edge, Edge c_Edge, int resolution)

        {
            this.id = id;
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

            this.tileVertices = new List<int> {edges[0].edgeId.a,edges[1].edgeId.a,edges[2].edgeId.a};
            this.tileTriangles = new List<int>();
            this.meshVertices = new List<Vector3>();
            
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

        public void ResetID0Option(){
            options = new List<string>() {"AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA",
            "AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA",
            "AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB",
            "AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAC","AAC","AAC","AAC","AAC",
            "AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC",
            "AAC","AAC","AAC","AAC","AAC","AAC","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA",
            "ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA",
            "ABA","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB",
            "ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABC","ABC","ABC","ABC",
            "ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC",
            "ABC","ABC","ABC","ABC","ABC","ABC","ABC","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA",
            "ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA",
            "ACA","ACA","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB",
            "ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACC","ACC","ACC",
            "ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC",
            "ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA",
            "BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA",
            "BAA","BAA","BAA","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB",
            "BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAC","BAC",
            "BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC",
            "BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BBA","BBA","BBA","BBA","BBA","BBA","BBA",
            "BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA",
            "BBA","BBA","BBA","BBA","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB",
            "BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBC",
            "BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC",
            "BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BCA","BCA","BCA","BCA","BCA","BCA",
            "BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA",
            "BCA","BCA","BCA","BCA","BCA","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB",
            "BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB",
            "BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC",
            "BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","CAA","CAA","CAA","CAA","CAA",
            "CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA",
            "CAA","CAA","CAA","CAA","CAA","CAA","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB",
            "CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB",
            "CAB","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC",
            "CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CBA","CBA","CBA","CBA",
            "CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA",
            "CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB",
            "CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB",
            "CBB","CBB","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC",
            "CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CCA","CCA","CCA",
            "CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA",
            "CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB",
            "CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB",
            "CCB","CCB","CCB","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC",
            "CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC"};
        }

        public void ResetID0Option(PlanetTopography pt){
            switch(pt){
                case PlanetTopography.Laky:
                    options = new List<string>() {"AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA",
                    "AAA","AAA","AAA","AAA","AAA","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB",
                    "AAB","AAB","AAB","AAB","AAB","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC",
                    "AAC","AAC","AAC","AAC","AAC","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA",
                    "ABA","ABA","ABA","ABA","ABA","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB",
                    "ABB","ABB","ABB","ABB","ABB","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC",
                    "ABC","ABC","ABC","ABC","ABC","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA",
                    "ACA","ACA","ACA","ACA","ACA","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB",
                    "ACB","ACB","ACB","ACB","ACB","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC",
                    "ACC","ACC","ACC","ACC","ACC","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA",
                    "BAA","BAA","BAA","BAA","BAA","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB",
                    "BAB","BAB","BAB","BAB","BAB","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC",
                    "BAC","BAC","BAC","BAC","BAC","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA",
                    "BBA","BBA","BBA","BBA","BBA","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB",
                    "BBB","BBB","BBB","BBB","BBB","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC",
                    "BBC","BBC","BBC","BBC","BBC","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA",
                    "BCA","BCA","BCA","BCA","BCA","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB",
                    "BCB","BCB","BCB","BCB","BCB","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC",
                    "BCC","BCC","BCC","BCC","BCC","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA",
                    "CAA","CAA","CAA","CAA","CAA","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB",
                    "CAB","CAB","CAB","CAB","CAB","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC",
                    "CAC","CAC","CAC","CAC","CAC","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA",
                    "CBA","CBA","CBA","CBA","CBA","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB",
                    "CBB","CBB","CBB","CBB","CBB","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC",
                    "CBC","CBC","CBC","CBC","CBC","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA",
                    "CCA","CCA","CCA","CCA","CCA","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB",
                    "CCB","CCB","CCB","CCB","CCB","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC",
                    "CCC","CCC","CCC","CCC","CCC"};
                break;
                case PlanetTopography.Rocky:
                    options = new List<string>() {"AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC"};

                break;
                case PlanetTopography.Flat:
                    options = new List<string>() {"AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAA","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAB","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","AAC","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABA","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABB","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ABC","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACA","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACB","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","ACC","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAA","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAB","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BAC","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBA","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBB","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BBC","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCA","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCB","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","BCC","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAA","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAB","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CAC","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBA","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBB","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CBC","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCA","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCB","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC","CCC"};
                break;
            }
        }

        public void ResetID1Option(){
            options = new List<string>() {"AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC","ABA","ABA",
            "ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB","ACB","ACB","ACC","ACC","ACC",
            "BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA","BBA","BBB","BBB","BBB","BBC",
            "BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC","CAA","CAA","CAA","CAB","CAB",
            "CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA","CCA","CCA",
            "CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC","ABA",
            "ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB","ACB","ACB","ACC","ACC",
            "ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA","BBA","BBB","BBB","BBB",
            "BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC","CAA","CAA","CAA","CAB",
            "CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA","CCA",
            "CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC",
            "ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB","ACB","ACB","ACC",
            "ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA","BBA","BBB","BBB",
            "BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC","CAA","CAA","CAA",
            "CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA",
            "CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC",
            "AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB","ACB","ACB",
            "ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA","BBA","BBB",
            "BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC","CAA","CAA",
            "CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC",
            "CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC",
            "AAC","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB","ACB",
            "ACB","ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA","BBA",
            "BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC","CAA",
            "CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC",
            "CBC","CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB",
            "AAC","AAC","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA","ACB",
            "ACB","ACB","ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBA",
            "BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC","BCC",
            "CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC",
            "CBC","CBC","CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB","AAB",
            "AAB","AAC","AAC","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA","ACA",
            "ACB","ACB","ACB","ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA",
            "BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC","BCC",
            "BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB","CBB",
            "CBC","CBC","CBC","CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA","AAB",
            "AAB","AAB","AAC","AAC","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACA",
            "ACA","ACB","ACB","ACB","ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC",
            "BBA","BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB","BCC",
            "BCC","BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB","CBB",
            "CBB","CBC","CBC","CBC","CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAA",
            "AAB","AAB","AAB","AAC","AAC","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA",
            "ACA","ACA","ACB","ACB","ACB","ACC","ACC","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC",
            "BAC","BBA","BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCA","BCA","BCB","BCB","BCB",
            "BCC","BCC","BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBA","CBA","CBB",
            "CBB","CBB","CBC","CBC","CBC","CCA","CCA","CCA","CCB","CCB","CCB","CCC","CCC","CCC"};
        }

        public void ResetID1Option(PlanetTopography pt){
            switch(pt){
                case PlanetTopography.Laky:
                    options = new List<string>() {"AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABB","ABC","ACA","ACA","ACA","ACB",
                    "ACB","ACB","ACC","ACC","ACC","BAA","BAB","BAC","BBA","BBB","BBC","BCA","BCA","BCA","BCB",
                    "BCB","BCB","BCC","BCC","BCC","CAA","CAB","CAC","CBA","CBB","CBC","CCA","CCA","CCA","CCB",
                    "CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABB","ABC","ACA","ACA","ACA","ACB",
                    "ACB","ACB","ACC","ACC","ACC","BAA","BAB","BAC","BBA","BBB","BBC","BCA","BCA","BCA","BCB",
                    "BCB","BCB","BCC","BCC","BCC","CAA","CAB","CAC","CBA","CBB","CBC","CCA","CCA","CCA","CCB",
                    "CCB","CCB","CCC","CCC","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABA","ABB","ABB",
                    "ABC","ABC","ACA","ACB","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBA","BBB","BBB",
                    "BBC","BBC","BCA","BCB","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBA","CBB","CBB",
                    "CBC","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABB","ABC","ACA","ACA","ACA","ACB",
                    "ACB","ACB","ACC","ACC","ACC","BAA","BAB","BAC","BBA","BBB","BBC","BCA","BCA","BCA","BCB",
                    "BCB","BCB","BCC","BCC","BCC","CAA","CAB","CAC","CBA","CBB","CBC","CCA","CCA","CCA","CCB",
                    "CCB","CCB","CCC","CCC","CCC"};
                break;
                case PlanetTopography.Rocky:
                    options = new List<string>() {"AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC","ABA","ABB","ABC","ACA","ACB","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBB","BBC","BCA","BCB","BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBB","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC","ABA","ABB","ABC","ACA","ACB","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBB","BBC","BCA","BCB","BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBB","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAA","AAA","AAB","AAB","AAB","AAC","AAC","AAC","ABA","ABB","ABC","ACA","ACB","ACC","BAA","BAA","BAA","BAB","BAB","BAB","BAC","BAC","BAC","BBA","BBB","BBC","BCA","BCB","BCC","CAA","CAA","CAA","CAB","CAB","CAB","CAC","CAC","CAC","CBA","CBB","CBC","CCA","CCB","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABB","ABB","ABC","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAB","BAC","BBA","BBA","BBB","BBB","BBC","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAB","CAC","CBA","CBA","CBB","CBB","CBC","CBC","CCA","CCA","CCB","CCB","CCC","CCC"};

                break;
                case PlanetTopography.Flat:
                    options = new List<string>() {"AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACB","ACC","BAA","BAB","BAC","BBA","BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCB","BCC","CAA","CAB","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACB","ACC","BAA","BAB","BAC","BBA","BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCB","BCC","CAA","CAB","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC","AAA","AAB","AAC","ABA","ABA","ABA","ABB","ABB","ABB","ABC","ABC","ABC","ACA","ACB","ACC","BAA","BAB","BAC","BBA","BBA","BBA","BBB","BBB","BBB","BBC","BBC","BBC","BCA","BCB","BCC","CAA","CAB","CAC","CBA","CBA","CBA","CBB","CBB","CBB","CBC","CBC","CBC","CCA","CCB","CCC","AAA","AAA","AAB","AAB","AAC","AAC","ABA","ABB","ABC","ACA","ACA","ACB","ACB","ACC","ACC","BAA","BAA","BAB","BAB","BAC","BAC","BBA","BBB","BBC","BCA","BCA","BCB","BCB","BCC","BCC","CAA","CAA","CAB","CAB","CAC","CAC","CBA","CBB","CBC","CCA","CCA","CCB","CCB","CCC","CCC"};
                break;
            }
        }
        public void ResetID2Option(){
            options = new List<string>() {"AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA",
            "ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA",
            "AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA",
            "CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA",
            "BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA",
            "BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA",
            "ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA",
            "CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA",
            "CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA",
            "BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA",
            "ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA",
            "AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA",
            "CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA",
            "BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA",
            "BAA","BBA","BCA","CAA","CBA","CCA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA",
            "ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB",
            "CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB",
            "CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB",
            "BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB",
            "ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB",
            "AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB",
            "CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB",
            "BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB",
            "BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB",
            "ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB",
            "CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB",
            "CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB",
            "BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB",
            "ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB",
            "AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB",
            "CBB","CCB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAC","ABC","ACC","BAC","BBC",
            "BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC",
            "BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC",
            "ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC",
            "CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC",
            "CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC",
            "BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC",
            "ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC",
            "AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC",
            "CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC",
            "BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC",
            "BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC",
            "ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC",
            "CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC",
            "CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC",
            "BBC","BCC","CAC","CBC","CCC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC"};
        }
        
        public void ResetID2Option(PlanetTopography pt){
            switch(pt){
                case PlanetTopography.Laky:
                    options = new List<string>() {"AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","AAA","ABA","BAA","BBA","CAA","CBA","AAA","ABA","BAA","BBA",
                    "CAA","CBA","ACA","BCA","CCA","ACA","BCA","CCA","ACA","BCA","CCA","AAA","ABA","ACA","BAA",
                    "BBA","BCA","CAA","CBA","CCA","ACA","BCA","CCA","ACA","BCA","CCA","AAA","ABA","ACA","BAA",
                    "BBA","BCA","CAA","CBA","CCA","ACA","BCA","CCA","ACA","BCA","CCA","AAA","ABA","ACA","BAA",
                    "BBA","BCA","CAA","CBA","CCA","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","AAB","ABB","BAB","BBB","CAB","CBB","AAB","ABB","BAB","BBB",
                    "CAB","CBB","ACB","BCB","CCB","ACB","BCB","CCB","ACB","BCB","CCB","AAB","ABB","ACB","BAB",
                    "BBB","BCB","CAB","CBB","CCB","ACB","BCB","CCB","ACB","BCB","CCB","AAB","ABB","ACB","BAB",
                    "BBB","BCB","CAB","CBB","CCB","ACB","BCB","CCB","ACB","BCB","CCB","AAB","ABB","ACB","BAB",
                    "BBB","BCB","CAB","CBB","CCB","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","AAC","ABC","BAC","BBC","CAC","CBC","AAC","ABC","BAC","BBC",
                    "CAC","CBC","ACC","BCC","CCC","ACC","BCC","CCC","ACC","BCC","CCC","AAC","ABC","ACC","BAC",
                    "BBC","BCC","CAC","CBC","CCC","ACC","BCC","CCC","ACC","BCC","CCC","AAC","ABC","ACC","BAC",
                    "BBC","BCC","CAC","CBC","CCC","ACC","BCC","CCC","ACC","BCC","CCC","AAC","ABC","ACC","BAC",
                    "BBC","BCC","CAC","CBC","CCC"};
                break;
                case PlanetTopography.Rocky:
                    options = new List<string>() {"AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","BAA","CAA","AAA","BAA","CAA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","BAA","CAA","AAA","BAA","CAA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","AAA","BAA","CAA","AAA","BAA","CAA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAA","BAA","CAA","ABA","ACA","BBA","BCA","CBA","CCA","ABA","ACA","BBA","BCA","CBA","CCA","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","BAB","CAB","AAB","BAB","CAB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","BAB","CAB","AAB","BAB","CAB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","AAB","BAB","CAB","AAB","BAB","CAB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAB","BAB","CAB","ABB","ACB","BBB","BCB","CBB","CCB","ABB","ACB","BBB","BCB","CBB","CCB","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","BAC","CAC","AAC","BAC","CAC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","BAC","CAC","AAC","BAC","CAC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","AAC","BAC","CAC","AAC","BAC","CAC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC","AAC","BAC","CAC","ABC","ACC","BBC","BCC","CBC","CCC","ABC","ACC","BBC","BCC","CBC","CCC"};

                break;
                case PlanetTopography.Flat:
                    options = new List<string>() {"AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","ABA","BBA","CBA","ABA","BBA","CBA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","ABA","BBA","CBA","ABA","BBA","CBA","AAA","ABA","ACA","BAA","BBA","BCA","CAA","CBA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","AAA","ACA","BAA","BCA","CAA","CCA","ABA","BBA","CBA","AAA","ACA","BAA","BCA","CAA","CCA","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","ABB","BBB","CBB","ABB","BBB","CBB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","ABB","BBB","CBB","ABB","BBB","CBB","AAB","ABB","ACB","BAB","BBB","BCB","CAB","CBB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","AAB","ACB","BAB","BCB","CAB","CCB","ABB","BBB","CBB","AAB","ACB","BAB","BCB","CAB","CCB","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","ABC","BBC","CBC","ABC","BBC","CBC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","ABC","BBC","CBC","ABC","BBC","CBC","AAC","ABC","ACC","BAC","BBC","BCC","CAC","CBC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC","AAC","ACC","BAC","BCC","CAC","CCC","ABC","BBC","CBC","AAC","ACC","BAC","BCC","CAC","CCC"};
                break;
            }
        }
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
