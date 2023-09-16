using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{

    public float airDrag = 1;
    public float waterDrag = 10;
    //public bool attachToSurface = false;
    public Transform[] FloatPoints;

    private Rigidbody rb;
    public Waves waves;

    private float waterLine;
    private Vector3[] waterlinePoints;

    private Vector3 centerOffset;
    private Vector3 vectorRotation;
    private Vector3 targetUp;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //testar com true
        rb.useGravity = false;


        waterlinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
            waterlinePoints[i] = FloatPoints[i].position;
        centerOffset = Utils.GetCenter(waterlinePoints) - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (waves != null)
        {
            //default water surface
            float newWaterLine = 0f;
            Vector3[] points = new Vector3[FloatPoints.Length];

            //set WaterLinePoints and WaterLine
            for (int i = 0; i < FloatPoints.Length; i++)
            {
                //height
                waterlinePoints[i] = FloatPoints[i].position;
                waterlinePoints[i].y = waves.GetHeight(FloatPoints[i].position);
                newWaterLine += waterlinePoints[i].y / FloatPoints.Length;
                points[i] = waterlinePoints[i].y > FloatPoints[i].position.y ? waterlinePoints[i] : FloatPoints[i].position;
            }

            float waterLineDelta = newWaterLine - waterLine;
            waterLine = newWaterLine;


            //gravidade
            Vector3 gravity = Physics.gravity;
            rb.drag = airDrag;
            if (waterLine > Center.y)
            {
                rb.drag = waterDrag;
                //go up
                gravity = -Physics.gravity + 0.9f * waterLineDelta * Vector3.up;

                rb.AddForce(waves.GetTsunamiSpeed(Center), ForceMode.VelocityChange);
            }
            rb.AddForce(gravity * Mathf.Clamp(Mathf.Abs(waterLine - Center.y), 0, 1));

            //Obter a normal do plano dos points
            targetUp = Utils.GetNormal(points);

            //Rotação do objeto flutuante.
            targetUp = Vector3.SmoothDamp(transform.up, targetUp, ref vectorRotation, 0.2f); // o vectorRotation guarda a informação para iterações posteriores.
            rb.rotation = Quaternion.FromToRotation(transform.up, targetUp) * rb.rotation;
        }
    }

    public Vector3 Center
    {
        get { return transform.position + centerOffset; }
    }

    /*private void OnDrawGizmos()
    {
        if (FloatPoints == null)
            return;

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null)
                continue;

            if (waves != null && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(waterlinePoints[i], Vector3.one * 0.3f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, waterLine, Center.z), Vector3.one * 1f);
        }
    }*/
}
