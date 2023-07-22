using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGraph 
{   
    Node currentNode;
    List<Node> elements;
    // Start is called before the first frame update
   class Node {
    
    int id;
    int entropy;
    bool collapsed;
    Edge a;
    Edge b;
    Edge c;
   }


   class Edge {
        List<string> options;

   }
}
