using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManipulatorObjectC : MonoBehaviour {

	// public ManipulatorCalculationsC manipulatorCalculations;
	public ParticleToolC particleTool;

	public Transform transform;										/// The manipulator.
	public MANIPULATORTYPE type;									/// Is the manipulator attraction, repellent, etc.
	public MANIPULATORSHAPE shape;									///	Is the manipulator a sphere, box, etc.
	public float size;												/// Size of the manipulator if it is a sphere
	public Bounds bounds;											/// Size of the manipulator if it is a box
	public float strength;											/// Strength of the manipulator									
	[RangeAttribute(0f, 1f)] public float smoothStrength;			/// Smoothing effect of manipulator.
	[RangeAttribute(0f, 1f)] public float smoothDistance;

	//public ParticleToolC[] particlesInRange;
	public List<ParticleSystem.Particle> particlesInRange = new List<ParticleSystem.Particle>();
	//public ParticleSystem.Particle[] m_Particles;

    public int manipulatorID;



	public ManipulatorObjectC(){
		size = 5f;
		strength = 1f;
		smoothStrength = 0.5f;
		smoothDistance = 0.5f;
	} 


	public Transform CreateManipulatorObject(Vector3 position, Vector3 rotation, ParticleSystem particleSystem){
		transform = new GameObject("ManipulatorObject").transform;
		bounds = new Bounds(Vector3.zero, new Vector3(size, size, size));
		manipulatorID = transform.GetInstanceID();
		transform.name = transform.name + "_" + manipulatorID;
		transform.parent = particleSystem.transform;
		transform.localPosition = position;
		//transform.localRotation = Quaternion.Euler(rotation);  //Quaternion.identity;
		transform.localRotation = Quaternion.identity;

		return transform;
	}

	public Vector3 GetManipulatorUp(){
		return Quaternion.Euler(transform.localRotation.eulerAngles) * transform.up;
	}


	public Vector3 GetManipulatorPosition(Transform transform){
		if (transform.parent == null){
			return transform.TransformPoint(transform.position);
		}
		return transform.TransformPoint(transform.localPosition);
	}


	public bool IsManipulatorAlive(){
		if (transform == null){
			return false;
		}
		return true;
	}

	public bool IsParticleInRange(ParticleSystem.Particle particle, Transform transform, MANIPULATORSHAPE shape){
		switch(shape)
		{
			case MANIPULATORSHAPE.Sphere:
				float distanceSqr = Vector3.SqrMagnitude(transform.position - particle.position);
				if (distanceSqr <= size * size){
					return true;
				}
				break;
			case MANIPULATORSHAPE.Box:
				if (bounds.Contains( particle.position - transform.position)){
					return true;
				}
				break;
		}
		return false;
	}



	// void Update(){
	// 	Vector3 manipulatorPosition;
	// 	Vector3 particlePosition;
	// 	//ParticleSystem.Particle particle;
	// 	Color startColor;


	// 	// if (IsManipulatorAlive() )
	// 	// {

	// 	manipulatorPosition = transform.position;

	// 	particleTool.particleSystem.GetParticles(particleTool.particles);
	// 	for (int i = 0; i < particleTool.particles.Length; i ++)
	// 	{
	// 		startColor = particleTool.particles[i].startColor;
	// 		particlePosition = particleTool.particles[i].position;  // particlePosition = GetParticlePosition(particleTool.particles[i]);
	// 		if (IsParticleInRange(particleTool.particles[i], transform, shape)){
	// 			Debug.Log(particleTool.particles[i] + " is in range");
	// 			//particlesInRange.Add(particleTool.particles[i]);
	// 			//particleTool.particles[i].velocity = GetForce(particles[i], manipulatorPosition, type);
	// 			particleTool.particles[i].startColor = Color.red;
	// 		}
	// 		else{
	// 			Debug.Log(particleTool.particles[i] + " NOT IN RANGE");
	// 		}
	// 		// if (!IsParticleInRange(particle, transform, shape)){
	// 		// 	particle.startColor = startColor;
	// 		// }
	// 	}
		
	// 	particleTool.particleSystem.SetParticles(particleTool.particles, particleTool.particles.Length);
	// }

}

