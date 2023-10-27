using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    // public void Init(){
    //     tiles = new WFCGraph(mF.mesh.triangles, 0, new System.Random());
    //     WFCGraph.StateInfo state;
    //     state = tiles.Step();
    //     print(state);
    //     while (state != WFCGraph.StateInfo.SUCCESFUL)
    //     {
    //         if (state == WFCGraph.StateInfo.ERROR)
    //         {
    //             tiles.Reset(-1);
    //         }
    //         state = tiles.Step();
    //     }

    //     UpdateVertexPositions();
    // }

    private void Awake()
    {
        gameobjectReferences = new List<GameObject>();
        mF = gameObject.AddComponent<MeshFilter>();
        m = mF.mesh;
        mC = gameObject.GetComponent<MeshCollider>();
        //InitializeBaseIcosahedron();
        //GenerateSphereResolution(2);


        //Debug.Log(m.triangles.Length/3);
        //GenerateSphereResolution(2);


        //print(succesful);
        //TestEdgesUpdate();
        //GenerateSphereResolution(5);

    }

    // Update is called once per frame
    void Update()
    {
        // if (Keyboard.current.jKey.wasReleasedThisFrame)
        // {
        //     foreach (var item in gameobjectReferences)
        //     {
        //         Destroy(item);
        //     }
        //     gameobjectReferences.Clear();
        //     tiles.Step();
        //     TestEdgesUpdate();

        // }
        // if (Keyboard.current.rKey.wasReleasedThisFrame)
        // {
        //     foreach (var item in gameobjectReferences)
        //     {
        //         Destroy(item);
        //     }
        //     gameobjectReferences.Clear();
        //     tiles.Reset(-1);
        //     TestEdgesUpdate();
        // }
        // if (Keyboard.current.sKey.wasReleasedThisFrame)
        // {
        //     foreach (var item in gameobjectReferences)
        //     {
        //         Destroy(item);
        //     }
        //     gameobjectReferences.Clear();
        //     tiles.TestSubState();
        //     TestEdgesUpdate();
        // }
    }

    private void InitializeBaseIcosahedron()
    {
        int radius = 1;

        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f,  t,  0f).normalized * radius,
            new Vector3( 1f,  t,  0f).normalized * radius,
            new Vector3(-1f, -t,  0f).normalized * radius,
            new Vector3( 1f, -t,  0f).normalized * radius,
            new Vector3( 0f, -1f,  t).normalized * radius,
            new Vector3( 0f,  1f,  t).normalized * radius,
            new Vector3( 0f, -1f, -t).normalized * radius,
            new Vector3( 0f,  1f, -t).normalized * radius,
            new Vector3( t,  0f, -1f).normalized * radius,
            new Vector3( t,  0f,  1f).normalized * radius,
            new Vector3(-t,  0f, -1f).normalized * radius,
            new Vector3(-t,  0f,  1f).normalized * radius
        };

        int[] triangles = new int[]
        {
            0, 11, 5,
            0, 5, 1,
            0, 1, 7,
            0, 7, 10,
            0, 10, 11,
            1, 5, 9,
            5, 11, 4,
            11, 10, 2,
            10, 7, 6,
            7, 1, 8,
            3, 9, 4,
            3, 4, 2,
            3, 2, 6,
            3, 6, 8,
            3, 8, 9,
            4, 9, 5,
            2, 4, 11,
            6, 2, 10,
            8, 6, 7,
            9, 8, 1
        };

        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
    }

    public void UpdateVertexPositions(WFCGraph nodeGraph, Dictionary<String, List<float>> generationModuleValues)
    {
        Vector3[] vertices = new Vector3[m.vertexCount];

        Array.Copy(m.vertices, 0, vertices, 0, m.vertexCount);
        //print(nodeGraph.elements.Length);
        List<float> values;
        foreach (WFCGraph.Node n in nodeGraph.elements)
        {
            String a = n.edges[0].options[0].Substring(0, 1) + n.edges[1].options[0].Substring(0, 1) + n.edges[2].options[0].Substring(0, 1);
            if (generationModuleValues.TryGetValue(a, out values))
            {
                for (int i = 0; i < n.tileVertices.Count; i++)
                {
                    vertices[n.tileVertices[i]] = vertices[n.tileVertices[i]].normalized + vertices[n.tileVertices[i]].normalized * values[i]/3;
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
            //print(originalLength/3);
            // if (r == resolution - 1)
            // {

            //     newVertices[0] = newVertices[0].normalized;
            //     newVertices[1] = newVertices[1].normalized;
            //     newVertices[2] = newVertices[2].normalized;
            //     newVertices[3] = newVertices[3].normalized;
            //     newVertices[4] = newVertices[4].normalized;
            //     newVertices[5] = newVertices[5].normalized;
            //     newVertices[6] = newVertices[6].normalized;
            //     newVertices[7] = newVertices[7].normalized;
            //     newVertices[8] = newVertices[8].normalized;
            //     newVertices[9] = newVertices[9].normalized;
            //     newVertices[10] = newVertices[10].normalized;
            //     newVertices[11] = newVertices[11].normalized;

            // }

            denom *= 4;
            m.vertices = newVertices;
            m.triangles = newTriangles;

        }

        m.RecalculateNormals();
        

        Debug.Log(tiles.elements[0].tileVertices.Count);
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
