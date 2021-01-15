using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractableBase), true)]
public class BlockEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        InteractableBase block = target as InteractableBase;

        if(GUILayout.Button("Enable Bound Gizmos")) {
            block.SetBoundGizmos(true);
            Debug.Log("Enabled bound gizmos");
        }
        if(GUILayout.Button("Disable Bound Gizmos")) {
            block.SetBoundGizmos(false);
            Debug.Log("Disabled bound gizmos");
        }
    }

}
