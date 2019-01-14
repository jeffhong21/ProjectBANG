using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class AnimationImportSettingsEditor : Editor
{


    private static ModelImporterAnimationType m_AnimationType = ModelImporterAnimationType.Human;
    private static Avatar m_SourceAvatar;

    private static bool loop = true;
    private static bool loopTime = true;
    private static bool loopPose = false;
    //  Enables root motion be baked into the movement of the bones. Disable to make root motion be stored as root motion.
    private static bool lockRootRotation = false;
    private static bool lockRootHeightY = true;
    private static bool lockRootPositionXZ = false;
    //  Keeps the vertical position as it is authored in the source file.
    private static bool keepOriginalOrientation = true;
    private static bool keepOriginalPositionY = true;
    private static bool keepOriginalPositionXZ = true;
    //  Keeps the feet aligned with the root transform position.
    private static bool heightFromFeet = false;




    [MenuItem("Tools/Animation Import Settings")]
    public static void ProcessAnimationClipNames()
    {
        var settings = new AnimationImportSettingsEditor();
        var selection = Selection.gameObjects;

        for (int i = 0; i < selection.Length; i++)
        {
            var asset = selection[i];
            //Debug.Log(asset.name);
            ProcessModel(asset);

        }
    }

    public static void ProcessModel(GameObject root)
    {
        string path = AssetDatabase.GetAssetPath(root);
        ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

        if (!modelImporter){
            Debug.Log("No Model Importer");
            return;
        }

        //ProcessModelRig(modelImporter);
        ProcessAnimationNames(modelImporter, root);
    }


    public static void ProcessModelRig(ModelImporter modelImporter)
    {
        if (m_SourceAvatar == null){
            Debug.Log("No Avatar");
            return;
        }

        modelImporter.animationType = m_AnimationType;
        modelImporter.sourceAvatar = m_SourceAvatar;
    }



    public static void ProcessAnimationNames(ModelImporter modelImporter, GameObject root)
    {
        //  Grab all model clips.
        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
        //  Grab the first clip.
        ModelImporterClipAnimation clipAnimation = clipAnimations[0];


        clipAnimation.name = root.name;
        clipAnimation.loop = loop;
        clipAnimation.loopTime = loopTime;
        clipAnimation.loopPose = loopPose;

        clipAnimation.lockRootRotation = lockRootRotation;
        clipAnimation.lockRootHeightY = lockRootHeightY;
        clipAnimation.lockRootPositionXZ = lockRootPositionXZ;

        clipAnimation.keepOriginalOrientation = keepOriginalOrientation;
        clipAnimation.keepOriginalPositionY = keepOriginalPositionY;
        clipAnimation.keepOriginalPositionXZ = keepOriginalPositionXZ;

        if( heightFromFeet){
            clipAnimation.keepOriginalPositionY = false;
            clipAnimation.heightFromFeet = heightFromFeet;
        } 

        //Debug.LogFormat("\"{0}\" {1}", clipAnimation.name, clipAnimation.loop ? "does loop" : "does NOT loop");
        modelImporter.clipAnimations = clipAnimations;
        // Save
        modelImporter.SaveAndReimport();
    }





    static ModelImporterClipAnimation GetModelImporterClip(ModelImporter mi)
    {
        ModelImporterClipAnimation clip = null;
        if (mi.clipAnimations.Length == 0)
        {
            //if the animation was never manually changed and saved, we get here. Check defaultClipAnimations
            if (mi.defaultClipAnimations.Length > 0)
            {
                clip = mi.defaultClipAnimations[0];
            }
            else
            {
                Debug.LogError("GetModelImporterClip can't find clip information");
            }
        }
        else
        {
            clip = mi.clipAnimations[0];
        }
        return clip;
    }
}