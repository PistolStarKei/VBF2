using UnityEngine;
using System.Collections;
using PathologicalGames;

public enum LureType_Hard{Minor,Crank,Shad,Popper,BuzBait,SpinerBait,BigBud,Vibrator,Koi};
public enum LureType_Soft{Shrinp,Zarigani,CarlyTail,ShadTail,Streght,Flog,Tube,Bug};
public enum RigType{Carolina,HeavyCarolina,Nosinker,RubberZig};
public enum LureMoveTypeOnDrag{Sink,Float};
public enum LureMoveTypeOnNoTention{Sink,Float,Stay};
[System.Serializable]
public class BuoyancyParams{
    public float buoyancyFactor = 1.0f;
    public float buoyancyOffset = 0.0f;
    public float underwaterMass = 1.0f;
    public float underwaterDrag = 4.0f;
    public float underwaterADrag = 1.0f;
    public bool keepAwake = false;
}
[System.Serializable]
public class LureParamsData{
    
    public float sinkingSpeed=0.5f;
    public float sinkingDepth=10.0f;
    //1-10まで、首振りの速さ
    public float kubiSpeed=0.5f;
    //1-10まで、首振りの細かさ
    public float kubiRotMax=30.0f;
    //前傾の角度
    public float kubiRotdownMax=30.0f;

    //1-10まで、どのくらいのしなりで進むか  一番抵抗あるディープクランクの0.4から3くらい
    public float DragTeikou=10.0f;



    public bool isShugyozai=false;
    public bool isReactionByteEnabled=false;

    public float appealScale_still=1.0f;
    public float appealScale_move=1.0f;
    public float appealScale_float=1.0f;
    public float appealScale_sinking=1.0f;
    public float appealScale_splash=1.0f;
    public float appealScale_reaction=1.0f;

    public Baits[] lookLike;
}

public class LureParams : MonoBehaviour {


    public LureParamsData lureParamsData;
    public BuoyancyParams buoParams;
    public bool keepAwake = false;
    public Vector3 spawnPosInLocal;


}
