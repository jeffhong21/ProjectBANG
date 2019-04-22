using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode, System.Serializable]
public class ParticleToolC : MonoBehaviour {

	public ManipulatorObjectC manipulatorObject;
	

	public new ParticleSystem particleSystem;				
	public ParticleSystem.Particle[] particles;

	public List<ManipulatorObjectC> manipulators = new List<ManipulatorObjectC>();


	public Vector3[] pVelocity;
	public float[] pStartSize;
	public Vector3[] pPosition;
	public Color[] pStartColor;

	// ---------- Internal  -------------

	private int maxParticles;
	public int affectedParticles;										/// All the particles affected by the manipulator
	private ParticleSystem.MainModule psMainModule;						/// ParticleSystem main module

	// ---------- For Debugging -------------
	public ParticleDebuggerC _particleDebuggerC;

	public int manipulatorInstance{
		get{
			return _manipulatorInstance;
		}
		set{
			if (value < 0 ){
				_manipulatorInstance = 0;
			}
			else if (value > manipulators.Count - 1 && manipulators.Count != 0){
				//Debug.Log("Manipulator index: " +  _manipulatorInstance + " is out of range.\nSetting value index to " + (manipulators.Count - 1 ));
				_manipulatorInstance = manipulators.Count - 1;
			}
			else{
				_manipulatorInstance = value;
			}
		}
	}
	private int _manipulatorInstance = 0;

	public int _howManyParticles;



	// -----------------------


	public void Start(){
		

		particleSystem = GetComponent<ParticleSystem>();
		maxParticles = particleSystem.main.maxParticles;
		if (particles == null || particles.Length < maxParticles){
			particles = new ParticleSystem.Particle[maxParticles];
		}
		
	}

	public void CreateManipulator(){

		manipulatorObject = new ManipulatorObjectC();
		manipulatorObject.CreateManipulatorObject(new Vector3(0,0,5), Vector3.zero, particleSystem);
		
		manipulators.Add(manipulatorObject);
	}

	public bool ActiveManipulator(){
		if(manipulators.Count > 0){
			return true;
		}
		return false;
	}

	public ParticleSystem ParticleSystem(){
		return particleSystem = GetComponent<ParticleSystem>();
	}

	public void Particles(){
		maxParticles = particleSystem.main.maxParticles;
		if (particles == null || particles.Length < maxParticles){
			particles = new ParticleSystem.Particle[maxParticles];
		}
	}

	public void GetParticleProperties(){

		affectedParticles = particleSystem.GetParticles(particles);
		Debug.Log(affectedParticles);

		for (int i = 0; i < affectedParticles; i ++){
			
			pVelocity = new Vector3[affectedParticles];
			pStartSize = new float[affectedParticles];
			pPosition = new Vector3[affectedParticles];
			pStartColor = new Color[affectedParticles];

			pVelocity[i] = particles[i].velocity;
			pStartSize[i] = particles[i].startSize;
			pPosition[i] = particles[i].position;
			pStartColor[i] = particles[i].startColor;

			Debug.Log(particles[i] + " starting size:  " + pStartSize[i] + "\n" + 
					  particles[i] + " position:  " + pPosition[i]);

			Debug.Log(particles[i] + " velocity:  " + pVelocity[i] + "\n" + 
					  particles[i] + " starting color:  " + pStartColor[i]);
		}
		//particleSystem.SetParticles(particles, affectedParticles);

	}




	void LateUpdate(){
		Vector3 manipulatorPosition;
		//Vector3 particlePosition;
		Color startColor;
		

		if (ActiveManipulator()){
			manipulatorObject = manipulators[manipulatorInstance];
			manipulatorPosition = manipulatorObject.transform.position;

			particleSystem.GetParticles(particles);
			for (int i = 0; i < particles.Length; i ++)
			{
				startColor = particles[i].startColor;
				//particlePosition = particles[i].position;  // particlePosition = GetParticlePosition(particles[i]);
				if (manipulatorObject.IsParticleInRange(particles[i], manipulatorObject.transform, manipulatorObject.shape)){
					//manipulatorObject.particlesInRange.Add(particles[i]);
					particles[i].velocity = GetForce(particles[i], manipulatorObject, manipulatorObject.type);
					particles[i].startColor = Color.red;
				}
			}

			//  Do calculations on all particles affected.  Once particles die, need to recalculate cause once particle dies, it can't find the particle.
			// for (int j = 0; j < manipulatorObject.particlesInRange.Count; j ++){
			// 	particles[j].startColor = Color.red;
			// }
			
			particleSystem.SetParticles(particles, particles.Length);
			
		}

	}




	public Vector3 GetForce(ParticleSystem.Particle particle, ManipulatorObjectC manipulator, MANIPULATORTYPE type){

		Vector3 manipulatorPosition = manipulator.transform.position;


		Vector3 force = Vector3.zero;
		Vector3 particlePosition = particle.position;
		float manipulatorDistance = Vector3.Distance(manipulatorPosition, particlePosition) / manipulator.smoothDistance;
		float manipulatorStrength = manipulator.strength * manipulator.size ;
		float mStrengthModifier = manipulatorStrength/manipulatorDistance;
		float time = Time.deltaTime * ( mStrengthModifier / manipulator.smoothStrength);

		switch(type){
			case MANIPULATORTYPE.Attraction:
				force = Vector3.Lerp(particle.velocity, (manipulatorPosition - particlePosition) * mStrengthModifier, time );
				break;
			case MANIPULATORTYPE.Repellent:
				force = Vector3.Lerp(particle.velocity, (particlePosition - manipulatorPosition) * mStrengthModifier, time );
				break;
			case MANIPULATORTYPE.VortexAttraction:
				force = Vector3.Lerp(particle.velocity, ( (manipulatorPosition - particlePosition) * mStrengthModifier - Vector3.Cross(transform.forward, manipulatorPosition - particlePosition )  * mStrengthModifier ), 
										 (Time.deltaTime * mStrengthModifier) / manipulator.smoothStrength );
				break;
		}
		return force;
	}



	void OnDestroy()
	{
		//DeleteManipulator(manipulators);
		for (int i = 0; i < manipulators.Count; i++){
			if (manipulators[i].transform != null){
				if (Application.isEditor){
					Object.DestroyImmediate(manipulators[i].transform.gameObject);
					
				}
				else{
					Object.Destroy(manipulators[i].transform.gameObject);
				}
			}
		}
	}

	public void DeleteManipulator(ManipulatorObjectC manipulatorObject){

		if (Application.isEditor){
			manipulators.Remove(manipulatorObject);
			Object.DestroyImmediate(manipulatorObject.transform.gameObject);
		}
		else{
			manipulators.Remove(manipulatorObject);
			Object.Destroy(manipulatorObject.transform.gameObject);
		}
	}


	// public string DebugManipulatorObject(int instance){

	// 	Transform _transform = manipulatorObject.transform;
	// 	MANIPULATORTYPE _type = manipulatorObject.type;
	// 	MANIPULATORSHAPE _shape = manipulatorObject.shape;
	// 	float _size = manipulatorObject.size;
	// 	Bounds _bounds = manipulatorObject.bounds;
	// 	float _strength = manipulatorObject.strength;
	// 	float _smoothStrength = manipulatorObject.smoothStrength;
	// 	float _smoothDistance = manipulatorObject.smoothDistance;

	// 	//Debug.Log(manipulators[instance].transform);

	// 	string manipulatorInfo = ("Manipulator:  " +  manipulators[instance].transform + "\n" +
	// 								"Position:  " +  manipulators[instance].transform.localPosition + "\n" +
	// 								"Rotation:  " +  manipulators[instance].transform.localRotation + "\n" +
	// 								"Type:  " +  manipulators[instance].type + "\n" +
	// 								"Size:  " +  manipulators[instance].size + "\n" +
	// 								"Strength:  " +  manipulators[instance].strength + "\n");
	// 	return manipulatorInfo;
	// }


}




public enum MANIPULATORTYPE{
	Attraction,
	Repellent,
	VortexAttraction,
	None
}

public enum MANIPULATORSHAPE{
	Sphere,
	Box
}

public enum MANIPULATORPROPERTY{
	Color,
	Size,
	Vortex,
	None
}
