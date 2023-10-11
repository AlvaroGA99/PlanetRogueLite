using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlanetGenerator : MonoBehaviour
{
    private Planet[] _orbits;

    public Planet planetPrefab;

    [SerializeField]
    private PlayerController _player;

    private WFCGraph tiles;

    void Awake()
    {
        _orbits = new Planet[7];
        _orbits[6] = Instantiate(planetPrefab, new Vector3(0,0,0) , Quaternion.identity);
        float angle = 0.0f;
        for (int i = 0; i < _orbits.Length - 1; i++)
        {
            angle = UnityEngine.Random.Range(0.66f * Mathf.PI, Mathf.PI / 3);
//            print(angle);
            _orbits[i] = Instantiate(planetPrefab, transform.position + new Vector3(250 * (i + 1) * Mathf.Cos(angle), 0, -250 * (i + 1) * Mathf.Sin(angle)), Quaternion.identity);
            _player._SphereT = _orbits[6].transform;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Mesh initialMesh = GenerateBaseIcosahedronAndGraphResolutionMesh(1);
        tiles = new WFCGraph(initialMesh.triangles, 0, new System.Random());
        WFCGraph.StateInfo state;
        
        for (int i = 0; i <_orbits.Length ; i++)
        {   
            
            _orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
            _orbits[i].m = _orbits[i].mF.mesh;
            _orbits[i].GenerateSphereResolution(1);
            state = tiles.Step();
        //print(state);
        while (state != WFCGraph.StateInfo.SUCCESFUL)
        {
            if (state == WFCGraph.StateInfo.ERROR)
            {   
                //state = tiles.Rollback();
                //sprint(state);
                if(state == WFCGraph.StateInfo.ERROR){
                    tiles.Reset(-1);
                }     
            }
            state = tiles.Step();
        }
            _orbits[i].UpdateVertexPositions(tiles);
            tiles.Reset(-1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Mesh GenerateBaseIcosahedronAndGraphResolutionMesh(int resolution)
    {

        Mesh m = new Mesh();

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

            if (r == resolution - 1)
            {
                newVertices[0] = newVertices[0].normalized;
                newVertices[1] = newVertices[1].normalized;
                newVertices[2] = newVertices[2].normalized;
                newVertices[3] = newVertices[3].normalized;
                newVertices[4] = newVertices[4].normalized;
                newVertices[5] = newVertices[5].normalized;
                newVertices[6] = newVertices[6].normalized;
                newVertices[7] = newVertices[7].normalized;
                newVertices[8] = newVertices[8].normalized;
                newVertices[9] = newVertices[9].normalized;
                newVertices[10] = newVertices[10].normalized;
                newVertices[11] = newVertices[11].normalized;

            }

            m.vertices = newVertices;
            m.triangles = newTriangles;

        }

        m.RecalculateNormals();
        return m;

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
