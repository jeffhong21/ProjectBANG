using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDebuggerC : ParticleToolC {

	//public int manipulatorInstance = ParticleToolC.manipulatorInstance;

	public string DebugManipulatorObject(ManipulatorObjectC manipulatorObject){

		Transform _transform = manipulatorObject.transform;
		MANIPULATORTYPE _type = manipulatorObject.type;
		MANIPULATORSHAPE _shape = manipulatorObject.shape;
		float _size = manipulatorObject.size;
		Bounds _bounds = manipulatorObject.bounds;
		float _strength = manipulatorObject.strength;
		float _smoothStrength = manipulatorObject.smoothStrength;
		float _smoothDistance = manipulatorObject.smoothDistance;

		string manipulatorInfo = ("Manipulator:  " +  manipulatorObject.transform + "\n" +
									"Position:  " +  manipulatorObject.transform.localPosition + "\n" +
									"Rotation:  " +  manipulatorObject.transform.localRotation + "\n" +
									"Type:  " +  manipulatorObject.type + "\n" +
									"Size:  " +  manipulatorObject.size + "\n" +
									"Strength:  " +  manipulatorObject.strength + "\n");
		return manipulatorInfo;
	}




}
