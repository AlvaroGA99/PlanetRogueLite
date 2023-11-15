using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Planet : MonoBehaviour
{
    public Mesh m;
    public MeshFilter mF;
    private MeshCollider mC;
    public DestructionMesh _destructionMeshPrefab;
    private List<DestructionMeshData> _destructionMeshes;
    public int res;
    public int seed;
    private float _planetIntegrity;
    public GameObject PlanetIntegrity;

    //private WFCGraph tiles;

    //private List<GameObject> gameobjectReferences;


    // Start is called before the first frame update
    void Start()
    {
        //print (new Vector3());

    }

    private void Awake()
    {   
        Projectile.OnDestroyEnemy += ReducePlanetIntegrity;
        _planetIntegrity = 0.1f;
        PlanetIntegrity = GameObject.Find("PlanetIntegrity");
        //gameobjectReferences = new List<GameObject>();
        mF = gameObject.AddComponent<MeshFilter>();
        m = mF.mesh;
        mC = gameObject.GetComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateVertexPositions(WFCGraph nodeGraph, Dictionary<String, List<float>> generationModuleWedgesValues, Dictionary<String, List<float>> generationModuleCentresValues)
    {   _destructionMeshes = new List<DestructionMeshData>();
        Vector3[] vertices = new Vector3[m.vertexCount];
        Dictionary<int, int> meshVertexMatching = new Dictionary<int, int>();
        Array.Copy(m.vertices, 0, vertices, 0, m.vertexCount);
        //print(nodeGraph.elements.Length);
        List<float> WedgeValues;
        List<float> CentreValues;
        foreach (WFCGraph.Node n in nodeGraph.elements)
        {   
//            print(n.edges[0].options[0].Substring(0, 1) + n.edges[0].options[0].Substring(1, 1) + n.edges[1].options[0].Substring(0, 1) + n.edges[1].options[0].Substring(1, 1) + n.edges[2].options[0].Substring(0, 1) + n.edges[2].options[0].Substring(1, 1));
            String wedges = n.edges[0].options[0].Substring(0, 1) + n.edges[1].options[0].Substring(0, 1) + n.edges[2].options[0].Substring(0, 1);
            String centres = n.edges[0].options[0].Substring(1, 1) + n.edges[1].options[0].Substring(1, 1) + n.edges[2].options[0].Substring(1, 1);

            if (generationModuleWedgesValues.TryGetValue(wedges, out WedgeValues) && generationModuleCentresValues.TryGetValue(centres, out CentreValues) )
            {
                for (int i = 0; i < n.tileVertices.Count; i++)
                {
                    vertices[n.tileVertices[i]] = vertices[n.tileVertices[i]].normalized + vertices[n.tileVertices[i]].normalized * (WedgeValues[i] + CentreValues[i])/100;
                    if(meshVertexMatching.TryAdd(n.tileVertices[i],n.meshVertices.Count)){
                        n.meshVertices.Add(vertices[n.tileVertices[i]]);
                        n.reference += vertices[n.tileVertices[i]];
                    }
                }
                 n.reference/= n.meshVertices.Count;

                 for(int i = 0; i < n.tileTriangles.Count; i++){
                    n.tileTriangles[i] = meshVertexMatching[n.tileTriangles[i]];
                 }
                 int originalLength = n.meshVertices.Count;
                 for(int i = 0; i < originalLength; i++){
                    n.meshVertices.Add(n.meshVertices[i] - n.meshVertices[i].normalized*0.5f);
                    n.meshVertices[i] -= n.reference;
                 }
            }
            meshVertexMatching.Clear();

            List<int> trianglesForCompleteMesh = new List<int>{15,16,1,17,7,16,15,17,16,6,17,15,18,19,7,20,4,19,18,20,19,8,20,18,17,18,7,21,8,18,17,21,18,6,21,17,22,21,6,23,8,21,22,23,21,3,23,22,24,25,4,26,10,25,24,26,25,9,26,24,27,28,10,29,2,28,27,29,28,11,29,27,26,27,10,30,11,27,26,30,27,9,30,26,31,30,9,32,11,30,31,32,30,5,32,31,20,24,4,33,9,24,20,33,24,8,33,20,34,31,9,35,5,31,34,35,31,12,35,34,33,34,9,36,12,34,33,36,34,8,36,33,23,36,8,37,12,36,23,37,36,3,37,23,38,37,3,39,12,37,38,39,37,13,39,38,40,35,12,41,5,35,40,41,35,14,41,40,39,40,12,42,14,40,39,42,40,13,42,39,43,42,13,44,14,42,43,44,42,0,44,43,0,45,65,20,0,65,20,65,81,36,20,81,36,81,84,39,36,84,39,84,88,43,39,88,43,88,53,8,43,53,8,53,58,13,8,58,13,58,82,37,13,82,37,82,63,18,37,63,18,63,68,23,18,68,23,68,83,38,23,83,38,83,48,3,38,48,3,48,66,21,3,66,21,66,62,17,21,62,17,62,67,22,17,67,22,67,51,6,22,51,6,51,60,15,6,60,15,60,46,1,15,46,1,46,49,4,1,49,4,49,52,7,4,52,7,52,55,10,7,55,10,55,61,16,10,61,16,61,64,19,16,64,19,64,70,25,19,70,25,70,73,28,25,73,28,73,47,2,28,47,2,47,69,24,2,69,24,69,79,34,24,79,34,79,85,40,34,85,40,85,89,44,40,89,44,89,54,9,44,54,9,54,59,14,9,59,14,59,80,35,14,80,35,80,71,26,35,71,26,71,76,31,26,76,31,76,86,41,31,86,41,86,50,5,41,50,5,50,75,30,5,75,30,75,72,27,30,72,27,72,77,32,27,77,32,77,56,11,32,56,11,56,74,29,11,74,29,74,45,0,29,45};
            n.tileTriangles.AddRange(trianglesForCompleteMesh);
            Mesh aux = new Mesh();
            aux.vertices = n.meshVertices.ToArray();
            aux.triangles = n.tileTriangles.ToArray();
            aux.RecalculateNormals();
            _destructionMeshes.Add(new DestructionMeshData(aux,n.reference));

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

    private void ReducePlanetIntegrity(){
        _planetIntegrity -= 0.1f;
        UpdatePlanetIntegrity();
        if(_planetIntegrity <= 0){
            FracturePlanet();
        }
    }

    private void FracturePlanet(){
        foreach(DestructionMeshData d in _destructionMeshes){
            DestructionMesh aux =  Instantiate(_destructionMeshPrefab, transform.position, Quaternion.identity);
            aux.Init(d.reference,d.mesh);
        }
        
        this.gameObject.SetActive(false);
    }

    private void UpdatePlanetIntegrity(){
        PlanetIntegrity.transform.localScale = new Vector3(_planetIntegrity,1,1);
    }

    private class DestructionMeshData{
        public Mesh mesh;
        public Vector3 reference;

        public DestructionMeshData(Mesh mesh, Vector3 reference){
            this.mesh = mesh;
            this.reference = reference;
        }

    }
}
