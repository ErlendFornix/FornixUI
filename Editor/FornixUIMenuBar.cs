using UnityEngine;
using UnityEditor;

public class FornixUIMenuBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    [MenuItem("Fornix/UI/Spawn Button")]
    private static void SpawnButtonPrefab ()
    {
        Debug.Log("Button Spawned");
        GameObject uiButton = PrefabUtility.InstantiatePrefab(Resources.Load("Sphere")) as GameObject;
        Selection.activeGameObject = uiButton;
    }
}
