using System.Collections.Generic;

public class WFCGraph
{
    Node currentNode;
    List<Node> elements;

    public WFCGraph(ref int[] triangleList, int resolution){
          elements = new List<Node>(triangleList.Length/3);

          // Data extraction from triangleList, and element initialization

          for (int i = 0; i < triangleList.Length/3; i++){
               elements[i] = new Node();
          }

          currentNode = elements[0];
    }
    
    public void RecalculateEntropy(){
          //Recalculate Entropy

    }

    public GetLowestEntropyElement(){

    }
    class Node
    {
        int id;
        int entropy;
        bool collapsed;
        Edge a_Edge;
        Edge b_Edge;
        Edge c_Edge;

        public Node()
        {

        }

    }


    class Edge
    {

        List<string> options;

        Edge nextInternalEdge;

        Edge adjacentEdge;

        Node ownerNode;

        public Edge()
        {

        }
    }
}
