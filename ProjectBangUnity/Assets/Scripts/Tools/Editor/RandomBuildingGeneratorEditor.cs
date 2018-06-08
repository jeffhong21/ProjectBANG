namespace BuildingGenerator
{
    using UnityEngine;
    using UnityEditor;


    [CustomEditor(typeof(RandomBuildingGenerator))]
    public class RandomBuildingGeneratorEditor : Editor
    {

        RandomBuildingGenerator _target;


		private void OnEnable()
		{
            _target = target as RandomBuildingGenerator;
		}


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(12);
            if (GUILayout.Button("Generate Random Building"))
            {
                _target.Generate();
            }

            //GUILayout.Space(12);
            //if (GUILayout.Button("Pull Assets from Resource Manager"))
            //{
            //    _target.GetDataFromResourceManager(_target.buildingType);
            //}
        }
	}
}
