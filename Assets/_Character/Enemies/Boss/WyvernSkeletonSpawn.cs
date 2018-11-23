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
    public bool IsOverLoad()
    {
        return currentIndex + 1 >= skeletonSpawnAreas.Length;
    }

    public void DisplaySkeletonSpawn()
    {
        currentIndex++;
        if (currentIndex < skeletonSpawnAreas.Length)
        {
            skeletonSpawnAreas[currentIndex].SetActive(true);
        }
    }

    public bool IsCurrentSkeletonGroupDie()
    {
        if (currentIndex >= 0 && skeletonSpawnAreas[currentIndex])
        {
            skeletonSpawnAreas[currentIndex].GetComponent<SkeletonArea>().AutoFindDestrouy();
        }
        //Debug.Log("Curr " + currentIndex + " is " + skeletonSpawnAreas[currentIndex].activeInHierarchy);
        return currentIndex == -1 || skeletonSpawnAreas[currentIndex] == null;
    }

}
