using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Planet : MonoBehaviour
{

    public Mesh m;

    public MeshFilter mF;

    private MeshCollider mC;

    public int res;

    public int seed;

    public GameObject testSphere;

    private float _planetIntegrity;
    //public Image PlanetIntegrity;

    //private WFCGraph tiles;

    private List<GameObject> gameobjectReferences;


    // Start is called before the first frame update
    void Start()
    {
        //print (new Vector3());

    }

    private void Awake()
    {
        gameobjectReferences = new List<GameObject>();
        mF = gameObject.AddComponent<MeshFilter>();
        m = mF.mesh;
        mC = gameObject.GetComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateVertexPositions(WFCGraph nodeGraph, Dictionary<String, List<float>> generationModuleWedgesValues, Dictionary<String, List<float>> generationModuleCentresValues)
    {
        Vector3[] vertices = new Vector3[m.vertexCount];

        Array.Copy(m.vertices, 0, vertices, 0, m.vertexCount);
        //print(nodeGraph.elements.Length);
        List<float> WedgeValues;
        List<float> CentreValues;
        foreach (WFCGraph.Node n in nodeGraph.elements)
        {   
            print(n.edges[0].options[0].Substring(0, 1) + n.edges[0].options[0].Substring(1, 1) + n.edges[1].options[0].Substring(0, 1) + n.edges[1].options[0].Substring(1, 1) + n.edges[2].options[0].Substring(0, 1) + n.edges[2].options[0].Substring(1, 1));
            String wedges = n.edges[0].options[0].Substring(0, 1) + n.edges[1].options[0].Substring(0, 1) + n.edges[2].options[0].Substring(0, 1);
            String centres = n.edges[0].options[0].Substring(1, 1) + n.edges[1].options[0].Substring(1, 1) + n.edges[2].options[0].Substring(1, 1);
            
            if (generationModuleWedgesValues.TryGetValue(wedges, out WedgeValues) && generationModuleCentresValues.TryGetValue(centres, out CentreValues) )
            {
                for (int i = 0; i < n.tileVertices.Count; i++)
                {
                    vertices[n.tileVertices[i]] = vertices[n.tileVertices[i]].normalized + vertices[n.tileVertices[i]].normalized * (WedgeValues[i] + CentreValues[i])/100;
                }
            }

        }
        m.vertices = vertices;
        m.RecalculateNormals();
        
        mC.sharedMesh = m;
    }

    public void GenerateSphereResolution(int resolution, WFCGraph tiles)
    {

        Dictionary<long, int> newVertexIndices = new Dictionary<long, int>();
        int newIndex = 0;
        int denom = 1;
        for (int r = 0; r < resolution; r++)
        {

            int originalLength = m.triangles.Length;

            int[] newTriangles = new int[m.triangles.Length * 4];
            Vector3[] newVertices = new Vector3[2 * m.vertexCount + (m.triangles.Length / 3) - 2];

            newIndex = m.vertexCount;

            Array.Copy(m.vertices, 0, newVertices, 0, m.vertexCount);

            for (int i = 0; i < originalLength; i += 3)
            {

                int i0 = m.triangles[i];
                int i1 = m.triangles[i + 1];
                int i2 = m.triangles[i + 2];

                int newIndex0 = GetNewVertexIndex(i0, i1, ref newVertexIndices, ref newVertices, ref newIndex);
                int newIndex1 = GetNewVertexIndex(i1, i2, ref newVertexIndices, ref newVertices, ref newIndex);
                int newIndex2 = GetNewVertexIndex(i2, i0, ref newVertexIndices, ref newVertices, ref newIndex);

                newTriangles[i * 4 + 9] = i0;
                newTriangles[i * 4 + 10] = newIndex0;
                newTriangles[i * 4 + 11] = newIndex2;

                newTriangles[i * 4 + 0] = newIndex0;
                newTriangles[i * 4 + 1] = i1;
                newTriangles[i * 4 + 2] = newIndex1;

                newTriangles[i * 4 + 3] = newIndex2;
                newTriangles[i * 4 + 4] = newIndex1;
                newTriangles[i * 4 + 5] = i2;

                newTriangles[i * 4 + 6] = newIndex0;
                newTriangles[i * 4 + 7] = newIndex1;
                newTriangles[i * 4 + 8] = newIndex2;



                tiles.elements[(i / 3) / denom].tileVertices.Add(newIndex0);
                tiles.elements[(i / 3) / denom].tileVertices.Add(newIndex1);
                tiles.elements[(i / 3) / denom].tileVertices.Add(newIndex2);

                if (r == resolution - 1)
                {
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex0);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(i1);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex1);

                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex2);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex1);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(i2);

                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex0);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex1);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex2);

                    tiles.elements[(i / 3) / denom].tileTriangles.Add(i0);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex0);
                    tiles.elements[(i / 3) / denom].tileTriangles.Add(newIndex2);
                }



            }
            denom *= 4;
            m.vertices = newVertices;
            m.triangles = newTriangles;
        }
        
        m.RecalculateNormals();
        
        mC.sharedMesh = m;
        //Debug.Log(tiles.elements[0].tileVertices.Count);
    }

    public void G(int resolution)
    {

        Dictionary<long, int> newVertexIndices = new Dictionary<long, int>();
        int newIndex = 0;
        for (int r = 0; r < resolution; r++)
        {

            int originalLength = m.triangles.Length;

            int[] newTriangles = new int[m.triangles.Length * 4];
            Vector3[] newVertices = new Vector3[2 * m.vertexCount + (m.triangles.Length / 3) - 2];

            newIndex = m.vertexCount;

            Array.Copy(m.vertices, 0, newVertices, 0, m.vertexCount);

            for (int i = 0; i < originalLength; i += 3)
            {

                int i0 = m.triangles[i];
                int i1 = m.triangles[i + 1];
                int i2 = m.triangles[i + 2];

                int newIndex0 = GetNewVertexIndex(i0, i1, ref newVertexIndices, ref newVertices, ref newIndex);
                int newIndex1 = GetNewVertexIndex(i1, i2, ref newVertexIndices, ref newVertices, ref newIndex);
                int newIndex2 = GetNewVertexIndex(i2, i0, ref newVertexIndices, ref newVertices, ref newIndex);

                newTriangles[i * 4 + 9] = i0;
                newTriangles[i * 4 + 10] = newIndex0;
                newTriangles[i * 4 + 11] = newIndex2;

                newTriangles[i * 4 + 0] = newIndex0;
                newTriangles[i * 4 + 1] = i1;
                newTriangles[i * 4 + 2] = newIndex1;

                newTriangles[i * 4 + 3] = newIndex2;
                newTriangles[i * 4 + 4] = newIndex1;
                newTriangles[i * 4 + 5] = i2;

                newTriangles[i * 4 + 6] = newIndex0;
                newTriangles[i * 4 + 7] = newIndex1;
                newTriangles[i * 4 + 8] = newIndex2;

            }
            m.vertices = newVertices;
            m.triangles = newTriangles;
        }
        
        m.RecalculateNormals();
        //m.RecalculateTangents();
        //Debug.Log(tiles.elements[0].tileVertices.Count);
    }
    private int GetNewVertexIndex(int i0, int i1, ref Dictionary<long, int> newVertexIndices, ref Vector3[] newVertices, ref int newIndex)
    {
        long edgeKey = EdgeKey(i0, i1);
        int vertexIndex;

        if (newVertexIndices.TryGetValue(edgeKey, out vertexIndex))
        {
            // Vertex already exists, return its index
            return vertexIndex;
        }

        Vector3 v0 = newVertices[i0];
        Vector3 v1 = newVertices[i1];
        Vector3 newVertex = ((v0 + v1) / 2f).normalized;
        //newVertex += newVertex;//*SampleNoiseHeight(newVertex);
        newVertices[newIndex] = newVertex;
        newVertexIndices[edgeKey] = newIndex;

        return newIndex++;
    }

    private long EdgeKey(int i0, int i1)
    {
        long minIndex = Mathf.Min(i0, i1);
        long maxIndex = Mathf.Max(i0, i1);
        long edgeIndex = minIndex;
        edgeIndex <<= 32;
        edgeIndex |= maxIndex;
        return edgeIndex;
    }


}
