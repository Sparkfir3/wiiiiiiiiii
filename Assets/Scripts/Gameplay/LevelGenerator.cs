using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

#pragma warning disable 0649 // Disable "Field is never assigned" warning for SerializeField

    [SerializeField] private List<GameObject> sectionList = new List<GameObject>();
    private int sectionCount = 0;

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
        sectionCount++;

        GameObject section = Instantiate(GetNextSection(), nextSectionSpawnPos.position, Quaternion.identity);
        if(sectionCount != 1)
            section.GetComponentInChildren<NextSection>().OnFirstTrigger.AddListener(SpawnNextSection);

        nextSectionSpawnPos.position += new Vector3(0f, 0f, 14f);
    }

    private GameObject GetNextSection() {
        return sectionList[Random.Range(0, sectionList.Count)];
    }

    // -----------------------------------------------------------------------------------------------------------

    private void OnDrawGizmos() {
        if(displaySectionSpawnPos && nextSectionSpawnPos) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(nextSectionSpawnPos.position, 0.25f);
        }
    }

}
