using UnityEngine;
using System.Collections;

public enum BottomType{Mud,Sand,Concrete,Rock};
public enum WaterFlowType{Oki,Tikaba,Nagare,Liver};
public class PointParameters : PS_SingletonBehaviour<PointParameters> {

    public FogDensity FogOnMorning=FogDensity.NONE;
    public WaterFlowType waterFlow=WaterFlowType.Oki;

    public float depth=-3.0f;
    public BottomType Bottom=BottomType.Sand;
    public float baseWaterCleaness=0.1f;

    //どれだけ風を弱めるか 0.0 完全になくす　1.0もろに
    public Vector3 WindMiness=new Vector3();

}
