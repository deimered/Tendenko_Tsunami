using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Waves : MonoBehaviour
{
    [Min(1)]
    public int Dimension = 10;
    [Min(1)]
    public float UVScale = 1;

    public MeshFilter meshFilter;
    public Mesh mesh;

    [Serializable]
    public struct Octave
    {
        //Velocidade de variação da onda;
        public Vector2 speed;
        //Scale para o PerlinNoise;
        public Vector2 scale;
        //Altura da onda;
        public float height;
        //Fazer as ondas percorrerem um eixo ou dois ao mesmo tempo;
        public bool alternate;

        public Vector2 startPos;
    }

    public Octave[] wateroctaves;
    public Octave[] tsunamiOctaves;

    //Tsunami transition
    [SerializeField]
    private bool tsunamiFlag;
    [SerializeField]
    private float heigthTreshould = 0.3f;
    [SerializeField]
    private float tsunamiStartTime;
    private int T = 0;

    [SerializeField]
    private int tsunamiExtraHeight;

    [SerializeField]
    private float waterSpeedFactor = 0.01f;
    [SerializeField]
    private float tsunamiSpeedFactor = 0.03f;
    private Vector3 waterSpeed;
    private Vector3 tsunamiSpeed;

    private float prevH = float.MinValue;

    public Transform FloatPoint;
    public event EventHandler OnReachPoint;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh
        {
            name = this.gameObject.name
        };

        mesh.vertices = GenerateVerts();
        mesh.triangles = GenerateTries();
        mesh.uv = GenerateUVs();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;


        foreach (Octave octave in wateroctaves)
            waterSpeed += new Vector3(octave.speed.x, 0, octave.speed.y);

        foreach (Octave octave in tsunamiOctaves)
            tsunamiSpeed += new Vector3(octave.speed.x, 0, octave.speed.y);

        waterSpeed *= waterSpeedFactor;
        tsunamiSpeed *= tsunamiSpeedFactor;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVerts();
        if (FloatPoint != null && GetHeight(FloatPoint.position) >= FloatPoint.position.y)
            OnReachPoint?.Invoke(this, EventArgs.Empty);
    }

    private Vector3[] GenerateVerts()
    {
        //Vector3[] verts = new Vector3[(Dimension + 1) * (Dimension + 1)];
        Vector3[] verts = new Vector3[((Dimension + 1) * (Dimension + 1)) * 2];

        //vertices igualmente distribuidos
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
            {
                verts[x * (Dimension + 1) + z] = new Vector3(x, 0, z);
                verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, 0, z);
            }

        return verts;
    }

    private int[] GenerateTries()
    {
        int[] tries = new int[mesh.vertices.Length * 6];
        //int[] tries = new int[(mesh.vertices.Length * 6) * 2];

        //Dois triangulos num Tile
        for (int x = 0; x < Dimension; x++)
            for (int z = 0; z < Dimension; z++)
            {


                tries[(x * (Dimension + 1) + z) * 6] = x * (Dimension + 1) + z;
                tries[(x * (Dimension + 1) + z) * 6 + 1] = (x + 1) * (Dimension + 1) + z + 1;
                tries[(x * (Dimension + 1) + z) * 6 + 2] = (x + 1) * (Dimension + 1) + z;
                tries[(x * (Dimension + 1) + z) * 6 + 3] = x * (Dimension + 1) + z;
                tries[(x * (Dimension + 1) + z) * 6 + 4] = x * (Dimension + 1) + z + 1;
                tries[(x * (Dimension + 1) + z) * 6 + 5] = (x + 1) * (Dimension + 1) + z + 1;


                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6] = (x + Dimension) * (Dimension + 1) + (z + Dimension) + 1;
                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6 + 1] = (x + Dimension + 1) * (Dimension + 1) + (z + Dimension) + 1;
                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6 + 2] = (x + Dimension + 1) * (Dimension + 1) + (z + Dimension) + 2;
                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6 + 3] = (x + Dimension) * (Dimension + 1) + (z + Dimension) + 1;
                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6 + 4] = (x + Dimension + 1) * (Dimension + 1) + (z + Dimension) + 2;
                tries[((x + Dimension) * (Dimension + 1) + (z + Dimension) + 1) * 6 + 5] = (x + Dimension) * (Dimension + 1) + (z + Dimension) + 2;

                
            }
        return tries;
    }

    private Vector2[] GenerateUVs()
    {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];

        //vertices igualmente distribuidos
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
            {

                Vector2 v = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[x * (Dimension + 1) + z] = new Vector2(v.x <= 1 ? v.x : 2 - v.x, v.y <= 1 ? v.y : 2 - v.y);
                uvs[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector2(v.x <= 1 ? v.x : 2 - v.x, v.y <= 1 ? v.y : 2 - v.y);
            }

        return uvs;
    }

    private void UpdateVerts()
    {
        Vector3[] verts = mesh.vertices;

        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
            {

                if (!tsunamiFlag)
                {
                    verts[x * (Dimension + 1) + z] = new Vector3(x, WaterVerts(verts, x, z), z);
                    verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, WaterVerts(verts, x, z), z);
                }

                else
                {
                    float h = TsunamiVerts(verts, x, z);
                    //T - Indicador do indice em z onde o tsunami se encontra.
                    //heigthTreshould - Tamanho minimo necessário para passar da água calma para o tsunami.
                    //prevH - Tamanho da onda no indice z anterior para ajudar numa transição mais suave para o tsunami.
                    if (T == z && h > heigthTreshould && ((prevH <= h + 0.01 && prevH >= h - 0.01) || prevH == float.MinValue))
                    {
                        T++;
                        prevH = h;
                    }

                    //Suavização da transição do mar calmo para o tsunami.
                    //Devido à velocidade do tsunami poderá ser removido para aumentar perfomance.
                    /*if (z == T + 1)
                    {
                        verts[x * (Dimension + 1) + z] = new Vector3(x, 3 * (h + tsunamiExtraHeight) / 4 + WaterVerts(verts, x, z) / 4, z);
                        verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, 3 * (h + tsunamiExtraHeight) / 4 + WaterVerts(verts, x, z) / 4, z);
                    }

                    else if (z == T + 2)
                    {
                        verts[x * (Dimension + 1) + z] = new Vector3(x, (h + tsunamiExtraHeight + WaterVerts(verts, x, z)) / 2, z);
                        verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, (h + tsunamiExtraHeight + WaterVerts(verts, x, z)) / 2, z);
                    }

                    else if (z == T + 3)
                    {
                        verts[x * (Dimension + 1) + z] = new Vector3(x, (h + tsunamiExtraHeight) / 4 + 3 * WaterVerts(verts, x, z) / 4, z);
                        verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, (h + tsunamiExtraHeight) / 4 + 3 * WaterVerts(verts, x, z) / 4, z);
                    }

                    else
                    {
                        verts[x * (Dimension + 1) + z] = new Vector3(x, T >= z ? h + tsunamiExtraHeight : WaterVerts(verts, x, z), z);
                        verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, T >= z ? h + tsunamiExtraHeight : WaterVerts(verts, x, z), z);
                    }*/

                    verts[x * (Dimension + 1) + z] = new Vector3(x, T >= z ? h + tsunamiExtraHeight : WaterVerts(verts, x, z), z);
                    verts[(x + Dimension) * (Dimension + 1) + (z + Dimension) + 1] = new Vector3(x, T >= z ? h + tsunamiExtraHeight : WaterVerts(verts, x, z), z);

                }
            }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }


    public float WaterVerts(Vector3[] verts, int x, int z)
    {
        float y = 0;
        float perl;

        for (int i = 0; i < wateroctaves.Length; i++)
        {
            if (wateroctaves[i].alternate)
            {
                perl = Mathf.PerlinNoise((Utils.ConvertScale(Dimension, wateroctaves[i].scale.x, x + transform.position.x)),
                                         (Utils.ConvertScale(Dimension, wateroctaves[i].scale.y, z + transform.position.z))) * 2 * Mathf.PI;
                y += Mathf.Cos(perl + wateroctaves[i].speed.magnitude * Time.time) * wateroctaves[i].height;
            }
            else
            {
                perl = Mathf.PerlinNoise(((x + transform.position.x) * wateroctaves[i].scale.x + Time.time * wateroctaves[i].speed.x * -1) / Dimension,
                                         ((z + transform.position.z) * wateroctaves[i].scale.y + Time.time * wateroctaves[i].speed.y * -1) / Dimension) - 0.5f; //Perlin Noise da valores entre 0 e 1,
                                                                                                                                                                // o -0.5f é para colocar os valores entre 0.5 e -0.5 com o centro no 0.
                y += perl * wateroctaves[i].height;
            }
        }

        return y;
    }

    public float TsunamiVerts(Vector3[] verts, int x, int z)
    {
        float y = 0;
        float perl;

        for (int i = 0; i < tsunamiOctaves.Length; i++)
        {
            if (tsunamiOctaves[i].alternate)
            {
                perl = Mathf.PerlinNoise((Utils.ConvertScale(Dimension, tsunamiOctaves[i].scale.x, x + transform.position.x)),
                                         (Utils.ConvertScale(Dimension, tsunamiOctaves[i].scale.y, z + transform.position.z))) * 2 * Mathf.PI;
                y += Mathf.Cos(perl + tsunamiOctaves[i].speed.magnitude * Time.time) * tsunamiOctaves[i].height;
            }
            else
            {
                perl = Mathf.PerlinNoise(((x + transform.position.x + tsunamiOctaves[i].startPos.x) * tsunamiOctaves[i].scale.x + (tsunamiStartTime + Time.time) * tsunamiOctaves[i].speed.x * -1) / Dimension,
                                         ((z + transform.position.z + tsunamiOctaves[i].startPos.y) * tsunamiOctaves[i].scale.y + (tsunamiStartTime + Time.time) * tsunamiOctaves[i].speed.y * -1) / Dimension) - 0.5f; //Perlin Noise da valores entre 0 e 1,
                                                                                                                                                                                         // o -0.5f é para colocar os valores entre 0.5 e -0.5 com o centro no 0.
                                                                                                                                                                                         // O -1 é para fazer as ondas seguirem a direção da velocidade.
                y += perl * tsunamiOctaves[i].height;
            }
        }
        return y;
    }

    public float GetHeight(Vector3 position)
    {
        //Fator de escala e posição no espaço local
        Vector3 scale = new Vector3(1 / this.transform.lossyScale.x, 0, 1 / this.transform.lossyScale.z);
        Vector3 localPos = Vector3.Scale((position - transform.position), scale);

        //Pontos à volta do position
        Vector3 p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        Vector3 p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        Vector3 p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        Vector3 p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        //Ajustar os pontos se ficarem fora dos limites
        p1.x = Mathf.Clamp(p1.x, 0, Dimension);
        p1.z = Mathf.Clamp(p1.z, 0, Dimension);
        p2.x = Mathf.Clamp(p2.x, 0, Dimension);
        p2.z = Mathf.Clamp(p2.z, 0, Dimension);
        p3.x = Mathf.Clamp(p3.x, 0, Dimension);
        p3.z = Mathf.Clamp(p3.z, 0, Dimension);
        p4.x = Mathf.Clamp(p4.x, 0, Dimension);
        p4.z = Mathf.Clamp(p4.z, 0, Dimension);

        //Obter a máxima distância entre os pontos e usá-la para calcular max - dist
        float max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos),
                              Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);

        float dist = (max - Vector3.Distance(p1, localPos))
                   + (max - Vector3.Distance(p2, localPos))
                   + (max - Vector3.Distance(p3, localPos))
                   + (max - Vector3.Distance(p3, localPos) + Mathf.Epsilon);

        //Soma da altura com pesos
        float height = mesh.vertices[(int)p1.x * (Dimension + 1) + (int)p1.z].y * (max - Vector3.Distance(p1, localPos))
                     + mesh.vertices[(int)p2.x * (Dimension + 1) + (int)p2.z].y * (max - Vector3.Distance(p2, localPos))
                     + mesh.vertices[(int)p3.x * (Dimension + 1) + (int)p3.z].y * (max - Vector3.Distance(p3, localPos))
                     + mesh.vertices[(int)p4.x * (Dimension + 1) + (int)p4.z].y * (max - Vector3.Distance(p4, localPos));

        //Escala
        return height * this.transform.lossyScale.y / dist + transform.position.y;
    }


    public Vector3 GetTsunamiSpeed(Vector3 position)
    {
        Vector3 scale = new Vector3(1 / this.transform.lossyScale.x, 0, 1 / this.transform.lossyScale.y);
        Vector3 localPos = Vector3.Scale((position - transform.position), scale);

        if (Mathf.Clamp(localPos.z, 0, Dimension) < T)
            return tsunamiSpeed;
        return waterSpeed;
    }


    public void ActivateTsunami()
    {
        tsunamiFlag = true;
    }
}
