using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.IO;
using UnityEngine.InputSystem.Controls;
using System.Threading.Tasks;

public class PlanetGenerator : MonoBehaviour
{
    public Planet[] _orbits;

    public Planet planetPrefab;

    //private Planet _currentPlanet;

    [SerializeField]
    private PlayerController _player;
    private WFCGraph tiles;

    public bool isFinishedLoading;

    Dictionary<String, List<float>> generationModuleWedgesValues;
    Dictionary<String, List<float>> generationModuleCentresValues;

    System.Random sampler;
    int seed;
    Mesh initialMesh;


    void Awake()
    {   
        seed = (int)UnityEngine.Random.value;
        sampler = new System.Random(seed);
        _orbits = new Planet[7];
        //_orbits[6] = Instantiate(planetPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        float angle = 0.0f;
        for (int i = 0; i < _orbits.Length; i++)
        {
            angle = UnityEngine.Random.Range(0, 2*Mathf.PI );
            //            print(angle);
            _orbits[i] = Instantiate(planetPrefab, transform.position + -transform.forward * 250 + new Vector3(250 * (i + 1) * Mathf.Cos(angle), 0, -250 * (i + 1) * Mathf.Sin(angle)), Quaternion.identity);
            _orbits[i].transform.SetParent(transform);
        }
        transform.localScale = Vector3.zero;
        //_currentPlanet = _orbits[6];
        //_player._SphereT = _currentPlanet.transform;
    }
    // Start is called before the first frame update
    async void Start()
    {
    
        generationModuleWedgesValues = new Dictionary<string, List<float>>();
        generationModuleCentresValues = new Dictionary<string, List<float>>();

        generationModuleWedgesValues.Add("AAA", GenerateWedgesValuesList("AAA"));
        generationModuleCentresValues.Add("AAA", GenerateCentresValuesList("AAA"));

        generationModuleWedgesValues.Add("AAB", GenerateWedgesValuesList("AAB"));
        generationModuleCentresValues.Add("AAB", GenerateCentresValuesList("AAB"));

        generationModuleWedgesValues.Add("AAC", GenerateWedgesValuesList("AAC"));
        generationModuleCentresValues.Add("AAC", GenerateCentresValuesList("AAC"));

        generationModuleWedgesValues.Add("ABA", GenerateWedgesValuesList("ABA"));
        generationModuleCentresValues.Add("ABA", GenerateCentresValuesList("ABA"));

        generationModuleWedgesValues.Add("ABB", GenerateWedgesValuesList("ABB"));
        generationModuleCentresValues.Add("ABB", GenerateCentresValuesList("ABB"));

        generationModuleWedgesValues.Add("ABC", GenerateWedgesValuesList("ABC"));
        generationModuleCentresValues.Add("ABC", GenerateCentresValuesList("ABC"));

        generationModuleWedgesValues.Add("ACA", GenerateWedgesValuesList("ACA"));
        generationModuleCentresValues.Add("ACA", GenerateCentresValuesList("ACA"));

        generationModuleWedgesValues.Add("ACB", GenerateWedgesValuesList("ACB"));
        generationModuleCentresValues.Add("ACB", GenerateCentresValuesList("ACB"));

        generationModuleWedgesValues.Add("ACC", GenerateWedgesValuesList("ACC"));
        generationModuleCentresValues.Add("ACC", GenerateCentresValuesList("ACC"));

        generationModuleWedgesValues.Add("BAA", GenerateWedgesValuesList("BAA"));
        generationModuleCentresValues.Add("BAA", GenerateCentresValuesList("BAA"));

        generationModuleWedgesValues.Add("BAB", GenerateWedgesValuesList("BAB"));
        generationModuleCentresValues.Add("BAB", GenerateCentresValuesList("BAB"));

        generationModuleWedgesValues.Add("BAC", GenerateWedgesValuesList("BAC"));
        generationModuleCentresValues.Add("BAC", GenerateCentresValuesList("BAC"));

        generationModuleWedgesValues.Add("BBA", GenerateWedgesValuesList("BBA"));
        generationModuleCentresValues.Add("BBA", GenerateCentresValuesList("BBA"));

        generationModuleWedgesValues.Add("BBB", GenerateWedgesValuesList("BBB"));
        generationModuleCentresValues.Add("BBB", GenerateCentresValuesList("BBB"));

        generationModuleWedgesValues.Add("BBC", GenerateWedgesValuesList("BBC"));
        generationModuleCentresValues.Add("BBC", GenerateCentresValuesList("BBC"));

        generationModuleWedgesValues.Add("BCA", GenerateWedgesValuesList("BCA"));
        generationModuleCentresValues.Add("BCA", GenerateCentresValuesList("BCA"));

        generationModuleWedgesValues.Add("BCB", GenerateWedgesValuesList("BCB"));
        generationModuleCentresValues.Add("BCB", GenerateCentresValuesList("BCB"));

        generationModuleWedgesValues.Add("BCC", GenerateWedgesValuesList("BCC"));
        generationModuleCentresValues.Add("BCC", GenerateCentresValuesList("BCC"));

        generationModuleWedgesValues.Add("CAA", GenerateWedgesValuesList("CAA"));
        generationModuleCentresValues.Add("CAA", GenerateCentresValuesList("CAA"));

        generationModuleWedgesValues.Add("CAB", GenerateWedgesValuesList("CAB"));
        generationModuleCentresValues.Add("CAB", GenerateCentresValuesList("CAB"));

        generationModuleWedgesValues.Add("CAC", GenerateWedgesValuesList("CAC"));
        generationModuleCentresValues.Add("CAC", GenerateCentresValuesList("CAC"));

        generationModuleWedgesValues.Add("CBA", GenerateWedgesValuesList("CBA"));
        generationModuleCentresValues.Add("CBA", GenerateCentresValuesList("CBA"));

        generationModuleWedgesValues.Add("CBB", GenerateWedgesValuesList("CBB"));
        generationModuleCentresValues.Add("CBB", GenerateCentresValuesList("CBB"));

        generationModuleWedgesValues.Add("CBC", GenerateWedgesValuesList("CBC"));
        generationModuleCentresValues.Add("CBC", GenerateCentresValuesList("CBC"));

        generationModuleWedgesValues.Add("CCA", GenerateWedgesValuesList("CCA"));
        generationModuleCentresValues.Add("CCA", GenerateCentresValuesList("CCA"));

        generationModuleWedgesValues.Add("CCB", GenerateWedgesValuesList("CCB"));
        generationModuleCentresValues.Add("CCB", GenerateCentresValuesList("CCB"));

        generationModuleWedgesValues.Add("CCC", GenerateWedgesValuesList("CCC"));
        generationModuleCentresValues.Add("CCC", GenerateCentresValuesList("CCC"));

        initialMesh = GenerateBaseIcosahedronAndGraphResolutionMesh(2);
        tiles = new WFCGraph(initialMesh.triangles, 0, sampler);

        for (int i = 0; i < _orbits.Length; i++)
        {
            _orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
            _orbits[i].m = _orbits[i].mF.mesh;
            _orbits[i].mass = 10000;  
            }
        for (int i = 0; i < _orbits.Length; i++)
        {
            await LoadLevel();

            _orbits[i].GenerateSphereResolution(3, tiles);
            _orbits[i].UpdateVertexPositions(tiles, generationModuleWedgesValues, generationModuleCentresValues);
            tiles.Reset(-1);
        }
        isFinishedLoading = true;
        // WFCGraph.StateInfo state;


        // for (int i = 0; i < _orbits.Length; i++)
        // {

        //     _orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
        //     _orbits[i].m = _orbits[i].mF.mesh;

        //     state = tiles.Step();
        //     while (state != WFCGraph.StateInfo.SUCCESFUL)
        //     {
        //         if (state == WFCGraph.StateInfo.ERROR)
        //         {   //si tamaño de lista de entropias == 0
        //             //if(tiles.lowestEntropyElementList.Count == 0){
        //             //print("RESET");
        //             tiles.Reset(-1);
        //             //}else{
        //             //tiles.RestoreState();
        //             //}
        //             //si no 
        //         }
        //         else
        //         {
        //             //tiles.SaveState();
        //             tiles.ComputeLowestEntropyElementList();
        //         }

        //         state = tiles.Step();
        //     }

        //     _orbits[i].GenerateSphereResolution(3, tiles);
        //     _orbits[i].UpdateVertexPositions(tiles, generationModuleWedgesValues, generationModuleCentresValues);
        //     tiles.Reset(-1);
        // }
        
        //await LoadLevel();

        //StartCoroutine(FramedLoad());
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

            m.vertices = newVertices;
            m.triangles = newTriangles;

        }

        //m.RecalculateNormals();
        //m.RecalculateTangents();
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

    private List<float> GenerateWedgesValuesList(String key)
    {
        List<float> readValues = new List<float>();
        String line;
        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/GenerationModules/Wedges/" + key + ".txt");
            line = sr.ReadLine();
            readValues.Add(float.Parse(line));
            while (line != null)
            {
                line = sr.ReadLine();
                readValues.Add(float.Parse(line));
            }
            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        return readValues;

    }

    private List<float> GenerateCentresValuesList(String key)
    {
        List<float> readValues = new List<float>();
        String line;
        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/GenerationModules/Centres/" + key + ".txt");
            line = sr.ReadLine();
            readValues.Add(float.Parse(line));
            while (line != null)
            {
                line = sr.ReadLine();
                readValues.Add(float.Parse(line));
            }
            sr.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        return readValues;

    }

    // public Transform GetPlanetTransform(){
    //     return _currentPlanet.transform;
    // }

    // public bool IsInSpawnState(){
    //     return _currentPlanet.isInSpawnState;
    // }

    IEnumerator FramedLoad()
    {
        
        Dictionary<String, List<float>> generationModuleWedgesValues = new Dictionary<string, List<float>>();
        Dictionary<String, List<float>> generationModuleCentresValues = new Dictionary<string, List<float>>();

        generationModuleWedgesValues.Add("AAA", GenerateWedgesValuesList("AAA"));
        generationModuleCentresValues.Add("AAA", GenerateCentresValuesList("AAA"));

        generationModuleWedgesValues.Add("AAB", GenerateWedgesValuesList("AAB"));
        generationModuleCentresValues.Add("AAB", GenerateCentresValuesList("AAB"));

        generationModuleWedgesValues.Add("AAC", GenerateWedgesValuesList("AAC"));
        generationModuleCentresValues.Add("AAC", GenerateCentresValuesList("AAC"));

        generationModuleWedgesValues.Add("ABA", GenerateWedgesValuesList("ABA"));
        generationModuleCentresValues.Add("ABA", GenerateCentresValuesList("ABA"));

        generationModuleWedgesValues.Add("ABB", GenerateWedgesValuesList("ABB"));
        generationModuleCentresValues.Add("ABB", GenerateCentresValuesList("ABB"));

        generationModuleWedgesValues.Add("ABC", GenerateWedgesValuesList("ABC"));
        generationModuleCentresValues.Add("ABC", GenerateCentresValuesList("ABC"));

        generationModuleWedgesValues.Add("ACA", GenerateWedgesValuesList("ACA"));
        generationModuleCentresValues.Add("ACA", GenerateCentresValuesList("ACA"));

        generationModuleWedgesValues.Add("ACB", GenerateWedgesValuesList("ACB"));
        generationModuleCentresValues.Add("ACB", GenerateCentresValuesList("ACB"));

        generationModuleWedgesValues.Add("ACC", GenerateWedgesValuesList("ACC"));
        generationModuleCentresValues.Add("ACC", GenerateCentresValuesList("ACC"));

        generationModuleWedgesValues.Add("BAA", GenerateWedgesValuesList("BAA"));
        generationModuleCentresValues.Add("BAA", GenerateCentresValuesList("BAA"));

        generationModuleWedgesValues.Add("BAB", GenerateWedgesValuesList("BAB"));
        generationModuleCentresValues.Add("BAB", GenerateCentresValuesList("BAB"));

        generationModuleWedgesValues.Add("BAC", GenerateWedgesValuesList("BAC"));
        generationModuleCentresValues.Add("BAC", GenerateCentresValuesList("BAC"));

        generationModuleWedgesValues.Add("BBA", GenerateWedgesValuesList("BBA"));
        generationModuleCentresValues.Add("BBA", GenerateCentresValuesList("BBA"));

        generationModuleWedgesValues.Add("BBB", GenerateWedgesValuesList("BBB"));
        generationModuleCentresValues.Add("BBB", GenerateCentresValuesList("BBB"));

        generationModuleWedgesValues.Add("BBC", GenerateWedgesValuesList("BBC"));
        generationModuleCentresValues.Add("BBC", GenerateCentresValuesList("BBC"));

        generationModuleWedgesValues.Add("BCA", GenerateWedgesValuesList("BCA"));
        generationModuleCentresValues.Add("BCA", GenerateCentresValuesList("BCA"));

        generationModuleWedgesValues.Add("BCB", GenerateWedgesValuesList("BCB"));
        generationModuleCentresValues.Add("BCB", GenerateCentresValuesList("BCB"));

        generationModuleWedgesValues.Add("BCC", GenerateWedgesValuesList("BCC"));
        generationModuleCentresValues.Add("BCC", GenerateCentresValuesList("BCC"));

        generationModuleWedgesValues.Add("CAA", GenerateWedgesValuesList("CAA"));
        generationModuleCentresValues.Add("CAA", GenerateCentresValuesList("CAA"));

        generationModuleWedgesValues.Add("CAB", GenerateWedgesValuesList("CAB"));
        generationModuleCentresValues.Add("CAB", GenerateCentresValuesList("CAB"));

        generationModuleWedgesValues.Add("CAC", GenerateWedgesValuesList("CAC"));
        generationModuleCentresValues.Add("CAC", GenerateCentresValuesList("CAC"));

        generationModuleWedgesValues.Add("CBA", GenerateWedgesValuesList("CBA"));
        generationModuleCentresValues.Add("CBA", GenerateCentresValuesList("CBA"));

        generationModuleWedgesValues.Add("CBB", GenerateWedgesValuesList("CBB"));
        generationModuleCentresValues.Add("CBB", GenerateCentresValuesList("CBB"));

        generationModuleWedgesValues.Add("CBC", GenerateWedgesValuesList("CBC"));
        generationModuleCentresValues.Add("CBC", GenerateCentresValuesList("CBC"));

        generationModuleWedgesValues.Add("CCA", GenerateWedgesValuesList("CCA"));
        generationModuleCentresValues.Add("CCA", GenerateCentresValuesList("CCA"));

        generationModuleWedgesValues.Add("CCB", GenerateWedgesValuesList("CCB"));
        generationModuleCentresValues.Add("CCB", GenerateCentresValuesList("CCB"));

        generationModuleWedgesValues.Add("CCC", GenerateWedgesValuesList("CCC"));
        generationModuleCentresValues.Add("CCC", GenerateCentresValuesList("CCC"));
       Mesh initialMesh = GenerateBaseIcosahedronAndGraphResolutionMesh(2);
       tiles = new WFCGraph(initialMesh.triangles, 0, new System.Random());

        //WFCGraph.StateInfo state;

        for (int i = 0; i < _orbits.Length; i++)
        {

            _orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
            _orbits[i].m = _orbits[i].mF.mesh;

        }

        for (int i = 0; i < _orbits.Length; i++)
        {

            //_orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
            //_orbits[i].m = _orbits[i].mF.mesh;

            tiles.Step();
            yield return null;
            while (tiles.CheckState() != WFCGraph.StateInfo.SUCCESFUL)
            {
                if (tiles.CheckState() == WFCGraph.StateInfo.ERROR)
                {   //si tamaño de lista de entropias == 0
                    //if(tiles.lowestEntropyElementList.Count == 0){
                    //print("RESET");
                    tiles.Reset(-1);
                    //}else{
                    //tiles.RestoreState();
                    //}
                    //si no 
                }
                else
                {
                    //tiles.SaveState();
                    tiles.ComputeLowestEntropyElementList();
                }

                 tiles.Step();
                yield return null;
            }

            //_orbits[i].GenerateSphereResolution(3, tiles);
            //_orbits[i].UpdateVertexPositions(tiles, generationModuleWedgesValues, generationModuleCentresValues);
            tiles.Reset(-1);
            yield return null;
        }
    }

    private async Task LoadLevel(){
        await Task.Run(() => {

        WFCGraph.StateInfo state;


        //for (int i = 0; i < _orbits.Length; i++)
        //{

            //_orbits[i].mF.mesh = Mesh.Instantiate(initialMesh);
            //_orbits[i].m = _orbits[i].mF.mesh;

            state = tiles.Step();
            while (state != WFCGraph.StateInfo.SUCCESFUL)
            {
                //print("dgfgdgdgf");
                if (state == WFCGraph.StateInfo.ERROR)
                {   //si tamaño de lista de entropias == 0
                    //if(tiles.lowestEntropyElementList.Count == 0){
                    //print("RESET");
                    tiles.Reset(-1);
                    //}else{
                    //tiles.RestoreState();
                    //}
                    //si no 
                }
                else
                {
                    //tiles.SaveState();
                    tiles.ComputeLowestEntropyElementList();
                }

                state = tiles.Step();
            }

           
        //} 
        
    });

    }

    public async void ReloadLevel(){
        
           for (int i = 0; i < _orbits.Length; i++)
        {
            await LoadLevel();
            _orbits[i].GenerateSphereResolution(3, tiles);
            _orbits[i].UpdateVertexPositions(tiles, generationModuleWedgesValues, generationModuleCentresValues);
            tiles.Reset(-1);
        }
        isFinishedLoading = true;
    }
}
