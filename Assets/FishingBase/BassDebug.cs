using UnityEngine;
using System.Collections;

public class BassDebug : MonoBehaviour {

    public Bass fishAi;
    //Debug Things
    public bool drawGizmo=false;
    void OnDrawGizmos () {
        #if UNITY_EDITOR
        if(!drawGizmo || fishAi==null)return;
        //Gizmos.DrawLine(transform.position,transform.TransformDirection(Vector3.forward)*10.0f);
        //Gizmos.DrawLine(transform.position,(moveAt - transform.position)*10.0f);
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[0]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[0]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[0]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[0]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[1]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[1]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[0],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[1]),new Vector3(fishAi._territory_Width[1],fishAi._territory_Height[1],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[1]),new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[0],fishAi._territory_Depth[1]));
        Gizmos.DrawLine(new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[0]),new Vector3(fishAi._territory_Width[0],fishAi._territory_Height[1],fishAi._territory_Depth[1]));
    #endif
    }

}
