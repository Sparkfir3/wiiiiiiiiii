﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [SerializeField] private List<GameObject> sectionList = new List<GameObject>();

    [Header("Debug")]
    [SerializeField] private Transform nextSectionSpawnPos;
    [SerializeField] private bool displaySectionSpawnPos;

    private void Start() {
        InitializeLevel();
    }

    // -----------------------------------------------------------------------------------------------------------

    public void InitializeLevel(int sectionCount = 3) {
        for(int i = 0; i < sectionCount; i++)
            SpawnNextSection();
    }

    public void SpawnNextSection() {
        Instantiate(GetNextSection(), nextSectionSpawnPos.position, Quaternion.identity);
        nextSectionSpawnPos.position += new Vector3(0f, 0f, 14f);
    }

    private GameObject GetNextSection() {
        return sectionList[Random.Range(0, sectionList.Count - 1)];
    }

    // -----------------------------------------------------------------------------------------------------------

    private void OnDrawGizmos() {
        if(displaySectionSpawnPos && nextSectionSpawnPos) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(nextSectionSpawnPos.position, 0.25f);
        }
    }

}
