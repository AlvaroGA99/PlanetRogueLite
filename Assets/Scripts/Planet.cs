using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetProperties;

public class Planet : MonoBehaviour
{
    public Mesh m;
    public MeshFilter mF;
    public GameObject PlanetIntegrity;
    public int res;
    public int seed;
    public float mass;
    public bool isInSpawnState;
    public DestructionMesh _destructionMeshPrefab;
    private PlanetLayerElement highLayerElement;
    private PlanetLayerElement mediumLayerElement;
    private PlanetLayerElement fluidLayerElement;
    private MeshCollider mC;
    private List<DestructionMeshData> _destructionMeshes;
    private float _planetIntegrity;
    
    // Start is called before the first frame update
    void Start()
    {
    }
    
    public void Init(System.Random sampler){
        Array vals = Enum.GetValues(typeof(PlanetLayerElement));
        highLayerElement = (PlanetLayerElement)vals.GetValue(sampler.Next(vals.Length));
        mediumLayerElement = (PlanetLayerElement)vals.GetValue(sampler.Next(vals.Length));
        fluidLayerElement = (PlanetLayerElement)vals.GetValue(sampler.Next(vals.Length));
    }
    private void Awake()
    {   isInSpawnState = true;
        Projectile.OnDestroyEnemy += ReducePlanetIntegrity;
        _planetIntegrity = 0.2f;
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
                    n.meshVertices.Add(n.meshVertices[i] - n.meshVertices[i].normalized*5.5f/100);
                    n.meshVertices[i + originalLength] -= n.reference;
                    n.meshVertices[i] -= n.reference;
                 }
            }
            meshVertexMatching.Clear();

            List<int> trianglesForCompleteMesh = new List<int>{60,61,46,62,52,61,60,62,61,51,62,60,63,64,52,65,49,64,63,65,64,53,65,63,62,63,52,66,53,63,62,66,63,51,66,62,67,66,51,68,53,66,67,68,66,48,68,67,69,70,49,71,55,70,69,71,70,54,71,69,72,73,55,74,47,73,72,74,73,56,74,72,71,72,55,75,56,72,71,75,72,54,75,71,76,75,54,77,56,75,76,77,75,50,77,76,65,69,49,78,54,69,65,78,69,53,78,65,79,76,54,80,50,76,79,80,76,57,80,79,78,79,54,81,57,79,78,81,79,53,81,78,68,81,53,82,57,81,68,82,81,48,82,68,83,82,48,84,57,82,83,84,82,58,84,83,85,80,57,86,50,80,85,86,80,59,86,85,84,85,57,87,59,85,84,87,85,58,87,84,88,87,58,89,59,87,88,89,87,45,89,88,0,45,88,43,0,88,43,88,58,13,43,58,13,58,83,38,13,83,38,83,48,3,38,48,3,48,67,22,3,67,22,67,51,6,22,51,6,51,60,15,6,60,15,60,46,1,15,46,1,46,61,16,1,61,16,61,52,7,16,52,7,52,64,19,7,64,19,64,49,4,19,49,4,49,70,25,4,70,25,70,55,10,25,55,10,55,73,28,10,73,28,73,47,2,28,47,2,47,74,29,2,74,29,74,56,11,29,56,11,56,77,32,11,77,32,77,50,5,32,50,5,50,86,41,5,86,41,86,59,14,41,59,14,59,89,44,14,89,44,89,45,0,44,45};
            n.tileTriangles.AddRange(trianglesForCompleteMesh);
            Mesh aux = new Mesh();
            aux.vertices = n.meshVertices.ToArray();
            aux.triangles = n.tileTriangles.ToArray();
            aux.RecalculateNormals();
            _destructionMeshes.Add(new DestructionMeshData(aux,n.reference));

        }
        m.vertices = vertices;
        m.RecalculateNormals();
        
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
        //mC.sharedMesh = m;
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
        isInSpawnState = false;
        foreach(DestructionMeshData d in _destructionMeshes){
            DestructionMesh aux =  Instantiate(_destructionMeshPrefab, transform.position, Quaternion.identity);
            aux.Init(d.reference,d.mesh);
        }
        
        this.gameObject.SetActive(false);
    }

    private void UpdatePlanetIntegrity(){
        PlanetIntegrity.transform.localScale = new Vector3(_planetIntegrity,1,1);
    }

    public void SetColliders(){
        mC.sharedMesh = m;
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
