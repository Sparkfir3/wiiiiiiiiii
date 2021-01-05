using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NextSection), true)]
public class NextSectionEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        NextSection next = target as NextSection;

        if(GUILayout.Button("Jump Camera")) {
            next.JumpCamera();
        }
        if(GUILayout.Button("Debug Target Position")) {
            next.PrintTargetCameraPos();
        }
    }

}
