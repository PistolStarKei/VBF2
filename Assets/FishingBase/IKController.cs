using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
public class IKController : MonoBehaviour {
	public FullBodyBipedIK ik;

	public Transform leftHandEffecter;
	public Transform leftArmBendTarget;
	public Transform rightHandEffecter;
	public Transform rightArmBendTarget;
	public Transform leftFootEffecter;
	public Transform rigthtFootEffecter;
	public Transform[] effecters_boat;
	public Transform[] effecters_cast;

	public void SwitchIKTarget(bool isBoat){

		if(isBoat){
			ik.solver.leftHandEffector.target=effecters_boat[0];
			ik.solver.leftArmChain.bendConstraint.bendGoal=effecters_boat[1];
			ik.solver.rightHandEffector.target=effecters_boat[2];
			ik.solver.rightArmChain.bendConstraint.bendGoal=effecters_boat[3];
			ik.solver.leftFootEffector.target=effecters_boat[4];
			ik.solver.rightFootEffector.target=effecters_boat[5];
		}else{
			ik.solver.leftHandEffector.target=effecters_cast[0];
			ik.solver.leftArmChain.bendConstraint.bendGoal=effecters_cast[1];
			ik.solver.rightHandEffector.target=effecters_cast[2];
			ik.solver.rightArmChain.bendConstraint.bendGoal=effecters_cast[3];
			ik.solver.leftFootEffector.target=effecters_cast[4];
			ik.solver.rightFootEffector.target=effecters_cast[5];
		}

	}
}
