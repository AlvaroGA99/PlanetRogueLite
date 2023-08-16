using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Planet : MonoBehaviour
{

    private Mesh _m;

    private MeshFilter _mF;

    public int res;

    // Start is called before the first frame update
    void Start()
    {
      //print (new Vector3());
    }

    private void Awake()
    {
    
        _mF = gameObject.AddComponent<MeshFilter>();
        _m = _mF.mesh;
        InitializeBaseIcosahedron();
        GenerateSphereResolution(3);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        _m.vertices = vertices;
        _m.triangles = triangles;
        _m.RecalculateNormals();
    }

    private void GenerateSphereResolution(int resolution)
    {

        Dictionary<long, int> newVertexIndices = new Dictionary<long, int>();
        int newIndex = 0;

        for (int r = 0; r < resolution - 1; r++)
        {

            int originalLength = _m.triangles.Length;

            int[] newTriangles = new int[_m.triangles.Length * 4];
            Vector3[] newVertices = new Vector3[2*_m.vertexCount + (_m.triangles.Length/3) - 2];

            newIndex = _m.vertexCount;

            Array.Copy(_m.vertices, 0, newVertices, 0, _m.vertexCount);

            for (int i = 0; i < originalLength; i += 3)
            {

                int i0 = _m.triangles[i];
                int i1 = _m.triangles[i + 1];
                int i2 = _m.triangles[i + 2];

                int newIndex0 = GetNewVertexIndex(i0, i1,  ref newVertexIndices,  ref newVertices, ref newIndex);          
                int newIndex1 = GetNewVertexIndex(i1, i2, ref newVertexIndices,  ref newVertices, ref newIndex);
                int newIndex2 = GetNewVertexIndex(i2, i0, ref  newVertexIndices,  ref newVertices, ref newIndex);

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

            if(r == resolution - 1)
            {
            //    newVertices[0] += newVertices[0];//*SampleNoiseHeight( newVertices[0] );
            //     newVertices[1] += newVertices[1];//*SampleNoiseHeight( newVertices[1] );
            //     newVertices[2] += newVertices[2];//*SampleNoiseHeight( newVertices[2] );
            //     newVertices[3] += newVertices[3];//*SampleNoiseHeight( newVertices[3] );
            //     newVertices[4] += newVertices[4];//*SampleNoiseHeight( newVertices[4] );
            //     newVertices[5] += newVertices[5];//*SampleNoiseHeight( newVertices[5] );
            //     newVertices[6] += newVertices[6];//*SampleNoiseHeight( newVertices[6] );
            //     newVertices[7] += newVertices[7];//*SampleNoiseHeight( newVertices[7] );
            //     newVertices[8] += newVertices[8];//*SampleNoiseHeight( newVertices[8] );
            //     newVertices[9] += newVertices[9];//*SampleNoiseHeight( newVertices[9] );
            //     newVertices[10] +=  newVertices[10];//*SampleNoiseHeight( newVertices[10] );
            //     newVertices[11] +=  newVertices[11];//*SampleNoiseHeight( newVertices[11] );

            }

            _m.vertices = newVertices;
            _m.triangles = newTriangles;

        }

       
        // print(_m.vertices[0]);
        // print(_m.vertices[1]);
        // print(_m.vertices[2]);
        // print(_m.vertices[3]);
        // print(_m.vertices[4]);
        // print(_m.vertices[5]);
        // print(_m.vertices[6]);
        // print(_m.vertices[7]);
        // print(_m.vertices[8]);
        // print(_m.vertices[9]);
        // print(_m.vertices[10]);
        // print(_m.vertices[11]);

        _m.RecalculateNormals();
    }

    private float SampleNoiseHeight(Vector3 pointOnSphere)
    {

        float l1 = Mathf.PerlinNoise(UnityEngine.Random.Range(0f,0.2f),UnityEngine.Random.Range(0f,0.1f))/4f;
        float l2 = Mathf.PerlinNoise(UnityEngine.Random.Range(0f,0.1f),UnityEngine.Random.Range(0f,0.1f))/8f;
        float l3 = Mathf.PerlinNoise(UnityEngine.Random.Range(0f,0.1f),UnityEngine.Random.Range(0f,0.1f))/16f;
        float l4 = Mathf.PerlinNoise(UnityEngine.Random.Range(0f,0.1f),UnityEngine.Random.Range(0f,0.1f))/32f;

        return  l1 + l2 + l3 + l4;
    }
 
    private int GetNewVertexIndex(int i0, int i1, ref Dictionary<long, int> newVertexIndices,  ref Vector3[] newVertices, ref int newIndex)
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
