using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetProperties;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class Planet : MonoBehaviour
{
    public Mesh m;
    public MeshFilter mF;
    public GameObject PlanetIntegrity;
    public PlanetProperties.PlanetLayerElement fluidPropertie;
    public float mass;
    public bool isInSpawnState;

    private MeshCollider mC;
    private float _planetIntegrity;
    private Material _material;
    private Material _fluidMaterial;

    void Start()
    {
        
    }
    
    private void Awake()
    {   
        _material = GetComponent<MeshRenderer>().material;
        _fluidMaterial = transform.GetChild(0).gameObject.GetComponentInChildren<MeshRenderer>().material;
        isInSpawnState = true;
        _planetIntegrity = 0.2f;
        PlanetIntegrity = GameObject.Find("PlanetIntegrity");
        mF = gameObject.AddComponent<MeshFilter>();
        m = mF.mesh;
        mC = gameObject.GetComponent<MeshCollider>();

    }

    void Update()
    {

    }

    public void UpdateVertexPositions(WFCGraph nodeGraph, Dictionary<String, List<float>> generationModuleWedgesValues, Dictionary<String, List<float>> generationModuleCentresValues)
    {   
        Vector3[] vertices = new Vector3[m.vertexCount];
        Dictionary<int, int> meshVertexMatching = new Dictionary<int, int>();
        Array.Copy(m.vertices, 0, vertices, 0, m.vertexCount);
        List<float> WedgeValues;
        List<float> CentreValues;
        foreach (WFCGraph.Node n in nodeGraph.elements)
        {   
            print(n.edges[0].options.Count);
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
        
    }

    public Task<(int[] t, Vector3[] v)> GenerateSphereResolution(int resolution, WFCGraph tiles)
    {
        Vector3[] originalVertices = m.vertices;
        int[] originalTriangles = m.triangles;
        return Task<(int[] tr,Vector3[] vr)>.Run(() => {
        Dictionary<long, int> newVertexIndices = new Dictionary<long, int>();

        int newIndex = 0;
        int denom = 1;

        for (int r = 0; r < resolution; r++)
        {

            int originalLength = originalTriangles.Length;

            int[] newTriangles = new int[originalTriangles.Length * 4];
            Vector3[] newVertices = new Vector3[2 * originalVertices.Length + (originalTriangles.Length / 3) - 2];

            newIndex = originalVertices.Length;

            Array.Copy(originalVertices, 0, newVertices, 0,originalVertices.Length);

            for (int i = 0; i < originalLength; i += 3)
            {

                int i0 = originalTriangles[i];
                int i1 =originalTriangles[i + 1];
                int i2 = originalTriangles[i + 2];

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

            }
            denom *= 4;
            originalVertices = newVertices;
            originalTriangles = newTriangles;
        }
        return (originalTriangles,originalVertices);
        });
    }

    public void SetMesh(int[] triangles,Vector3[] vertices){
        m.vertices = vertices;
        m.triangles = triangles;
    }
    private int GetNewVertexIndex(int i0, int i1, ref Dictionary<long, int> newVertexIndices, ref Vector3[] newVertices, ref int newIndex)
    {
        long edgeKey = EdgeKey(i0, i1);
        int vertexIndex;

        if (newVertexIndices.TryGetValue(edgeKey, out vertexIndex))
        {
            return vertexIndex;
        }

        Vector3 v0 = newVertices[i0];
        Vector3 v1 = newVertices[i1];
        Vector3 newVertex = ((v0 + v1) / 2f).normalized;
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

    private void UpdatePlanetIntegrity(){
        PlanetIntegrity.transform.localScale = new Vector3(_planetIntegrity,1,1);
    }

    public void SetColliders(){
        mC.sharedMesh = m;
    }

    public void SetHighLayerTexture(Texture2D color,Texture2D normal){
         _material.SetTexture("_HighLayer",color);
        _material.SetTexture("_HighLayerNormal",normal);
    }

    public void SetMediumLayerTexture(Texture2D color,Texture2D normal){
        _material.SetTexture("_LowLayer",color);
        _material.SetTexture("_LowLayerNormal",normal);
    }

    public void SetFluidLayerTexture(Color32 color,PlanetLayerElement fp){
        _fluidMaterial.SetColor("_BaseColor",color);
        _fluidMaterial.SetColor("_EmissionColor",color);
        fluidPropertie = fp;

    }

      public void SetFluidMat(Material mat, PlanetLayerElement fp){
        MeshRenderer mr = transform.GetChild(0).gameObject.GetComponentInChildren<MeshRenderer>();
        mr.material = mat;
        fluidPropertie = fp;
    }

}
