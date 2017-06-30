using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class LineScriptTest :  PS_SingletonBehaviour<LineScriptTest> {

    public float lineTention=0.0f;
    public float lineMeter=0.0f;

    public bool isLineHasTention(){
        if(lineTention>0.0f)return true;
        return false;
    }
    public void UpdateLineProperties(float lineTentionA,float lineMeterA){
        lineTention = lineTentionA;lineMeter = lineMeterA;
        //if(LureController.Instance!=null) HUD_LureParams.Instance. UpdateLineGUI(lineMeter,- LureController.Instance.gameObject.transform.position.y);
    }

    bool isCreated=false;
    public void ShowLine(){
        if(!isCreated){
            CreateLine (lineColor,1);
            isCreated=true;
        }else{
            myLine.active=true;
        }
    }
    public void HideLine(){
        if(myLine!=null)myLine.active=false;
    }
    public void DestroyLine(){

        VectorLine.Destroy (ref myLine);
        Destroy(gameObject);

    }

    public Color lineColor=Color.white;
    public float lineSize=0.5f;

    public delegate double Function(double x, double a, double v, double b);
    public Transform LurePosition;//end
    Vector3 P1 = new Vector3(0.0f, 0.0f, 0.0f);//start point
    Vector3 P2 = new Vector3(10.0f, 10.0f, 10.0f);//end point
    Vector3 PD1 = new Vector3(10.0f, 10.0f, 10.0f);//end point
    Vector3 PD2 = new Vector3(10.0f, 10.0f, 10.0f);//end point
    public  float Length = 20.0F;//length of line
    int N = 20;//points num between points

    internal bool previousSolved = false;
    internal Vector3 prevP1 = new Vector3(0.0f, 0.0f, 0.0f);
    internal Vector3 prevP2 = new Vector3(10.0f, 10.0f, 10.0f);
    internal float prevL = 20.0f;
    private VectorLine myLine ;
    private Vector3[] points=new Vector3[2];
    public Material lineMat;


    void CreateLine(Color lineColor,int lineSize){



        points=new Vector3[2]{ RodControllerTest.Instance.rodTip.position,LurePosition.position};
        myLine = VectorLine.SetLine3D(lineColor,points);
        myLine.joins=Joins.Fill;
        myLine.material=lineMat;
        SetLineWidth (lineSize,lineColor);
        /*VectorLine.SetLineParameters (lineColor, null, lineSize, 1, 
        1, LineType.Continuous, Joins.Fill);
        myLine.Resize(N);*/
        myLine.Resize(N);
        if(myLine==null)Debug.Log("CreateLine null");
        this.UpdateEndPoints();
    }


    public void SetLineWidth(float width,Color col){
        myLine.lineWidth = lineSize;
        myLine.material.color=col;


    }
    public float lineSlack=0.0f;
    float distance=0.0f;
    public bool isFukeMode=false;
    public float fukeSpeed=0.2f;
    public float timeSinceFuke=2.0f;
    public float fukeTime=1.0f;
    bool isFukeTimeMode=false;
    bool isInWaterAdding=false;
    public void InvokeFuke(bool isOn,bool isTimeMode,bool isInWaterAdding){
        this.isFukeTimeMode=isTimeMode;
        if(isInWaterAdding){
            timeSinceFuke=0.0f;
        }
        this.isInWaterAdding=isInWaterAdding;
        if(isOn){
            timeSinceFuke=0.0f;
            isFukeMode=true;

        }else{
            isFukeMode=false;

        }

    }
    void  Update()
    {

        if (!isCreated)
            return;
        this.UpdateEndPoints();

        if ((P2 - P1).magnitude <= 0.0f)
        {
            return;
        }else{
            distance=(P2 - P1).magnitude;

           

            lineSlack=distance-Length;
            UpdateLineProperties(( PD2-PD1).magnitude-Length,Length < distance ? distance : Length);



        }
        if (P1 != prevP1 || P2 != prevP2 || Length != prevL)
        {
            //if changes have been made
            prevP1 = P1;
            prevP2 = P2;
            prevL = Length;
        }
        else
        {
            // no changes to update
            return;
        }
        //LineRenderer lineRenderer = GetComponent<LineRenderer>();
        // Catenary function
        Catenary.Function f = delegate(double x0, double a0, double v0, double b0)
        {
            return a0 * Math.Cosh((x0 + v0) / a0) + b0;
        };
        float w = 1.0f;// / W;
        float l = Length * w;
        // Before rotation
        float x1i = P1.x * w;
        float x2i = P2.x * w;
        float y1i = P1.y * w;
        float y2i = P2.y * w;
        float z1i = P1.z * w;
        float z2i = P2.z * w;
        // Rotating P2
        P2 = new Vector3(P1.x + (new Vector3(P2.x, 0.0f, P2.z) - new Vector3(P1.x, 0.0f, P1.z)).magnitude, P2.y, 0.0f);
        // After rotation
        float x1 = P1.x * w;
        float x2 = P2.x * w;
        float y1 = P1.y * w;
        float y2 = P2.y * w;
        double a = 0; 
        double v = 0;
        double b = 0;
        bool solved = false;
        if (Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) < l)
        {   
            solved = this.Solve(x1,  y1, x2,  y2, l, out a, out v, out b);
            if (solved) 
            {
                double dx = N <= 1 ? 0 : ((x2 - x1) / (N - 1));
                double dxi = N <= 1 ? 0 : ((x2i - x1i) / (N - 1));
                double dzi = N <= 1 ? 0 : ((z2i - z1i) / (N - 1));
                double prevY = y1;
                for (int i = 0; i < N; i++)
                {
                    double x = x1 + ((double) i) * dx;
                    double xi = x1i + ((double) i) * dxi;
                    double zi = z1i + ((double) i) * dzi;
                    double y =  f(x, a, v, b);
                    if (
                        double.IsNaN(y) ||
                        double.IsInfinity(y) ||
                        Math.Abs(y - prevY) > l
                    )
                    {
                        solved = false;
                        break;
                    }
                    Vector3 pos = new Vector3((float) xi / w, (float) y / w, (float) zi / w);

                    SetPosition(i, pos);
                    prevY = y;
                }
            }
            if (!solved && previousSolved)
            {
                Debug.Log("Catenary could not be solved. Distance between end-points is aparently too short comparing to configured catenary length.");
            }
        }
        if (!solved)
        {
            double dxi = N <= 1 ? 0 : ((x2i - x1i) / (N - 1));
            double dyi = N <= 1 ? 0 : ((y2i - y1i) / (N - 1));
            double dzi = N <= 1 ? 0 : ((z2i - z1i) / (N - 1));
            for (int i = 0; i < N; i++)
            {
                double xi = x1i + ((double) i) * dxi;
                double yi = y1i + ((double) i) * dyi;
                double zi = z1i + ((double) i) * dzi;
                Vector3 pos = new Vector3((float) xi / w, (float) yi / w, (float) zi / w);
                SetPosition(i, pos);
            }
        }
        previousSolved = solved;


    }


    private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion quat)
    {
        return quat * (point - pivot) + pivot;
    }

    void SetPosition(int num,Vector3 pos){

        myLine.points3[num]=pos;
        myLine.Draw3D();
    }

    private void UpdateEndPoints()
    {
        if(LurePosition==null)return;
        P1 = RodControllerTest.Instance.rodTip.position;
        P2 = LurePosition.position;
        if (P2.x < P1.x)
        {
            Vector3 tP2 = P2;
            P2 = P1;
            P1 = tP2;
        }

        PD1= RodControllerTest.Instance.dummyRodTrans.position;
        PD2=LurePosition.position;
        if (PD2.x < PD1.x)
        {
            Vector3 tPD2 = PD2;
            PD2 = PD1;
            PD1 = tPD2;
        }
    }

    private static bool Solve(float x1, float y1, float x2, float y2, float L, out double a, out double v, out double b)
    {
        a = 0;
        v = 0;
        b = 0;
        double d = x2 - x1;
        double h = y2 - y1;
        NewtonRaphson.Function f = delegate(double a0)
        {
            return 2.0 * a0 * Math.Sinh(d / (2.0 * a0)) - Math.Sqrt((L * L) - (h * h));
        };
        NewtonRaphson.Function df = delegate(double a0)
        {
            return 2.0 * Math.Sinh(d / (2.0 * a0)) - d * Math.Cosh(d / (2.0 * a0)) / a0;
        };
        double newA = NewtonRaphson.FindRoot(0.01 * d, 0.00001, 100, f, df);
        if (double.IsNaN(newA) || double.IsInfinity(newA))
        {
            return false;
        }
        else
        {
            a = newA;
            double k = 0.5 * (a * Math.Log((L + h) / (L - h)) - d);
            v = k - x1;
            b = y1 - a * Math.Cosh(k / a);
            return true;
        }
    }


    public void SetLineDamageToZero(){
        SetLineDamage(1.0f);
    }
    public UISlider linePowerMeter;
    public void SetLineDamage(float damage){
        TackleParams.Instance.tParams.Params_lineDamage+=damage;
        linePowerMeter.value=(TackleParams.Instance.tParams.Params_lineLife-TackleParams.Instance.tParams.Params_lineDamage);


    }
}
