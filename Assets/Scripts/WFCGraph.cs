using System.Collections.Generic;

public class WFCGraph 
{   
    Node currentNode;
    List<Node> elements;
    // Start is called before the first frame update
   class Node {
    int id;
    int entropy;
    bool collapsed;
    Edge a_Edge;
    Edge b_Edge;
    Edge c_Edge;

   }


   class Edge {
        
        List<string> options;

        Edge nextInternalEdge;

        Edge adjacentEdge;

        Node ownerNode;

   }
}
