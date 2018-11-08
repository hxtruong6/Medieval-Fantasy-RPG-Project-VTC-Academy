using UnityEngine;

public class WyvernSkeletonSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] skeletonSpawnAreas;
    private int currentIndex;
    void Start()
    {
        currentIndex = -1;
        for (int i = 0; i < skeletonSpawnAreas.Length; i++)
        {
            skeletonSpawnAreas[i].SetActive(false);
        }
    }
    public void DisplaySkeletonSpawn()
    {
        if (currentIndex + 1 < skeletonSpawnAreas.Length)
        {
            skeletonSpawnAreas[++currentIndex].SetActive(true);
        }
    }

    public bool IsCurrentSkeletonGroupDie()
    {
        return skeletonSpawnAreas[currentIndex].activeSelf == false;
    }

}
