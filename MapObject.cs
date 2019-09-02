using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField]
    private GameObject treeButton;
    [SerializeField]
    private GameObject stoneButton;

    /* 오브젝트 데이터들 */
    public static Dictionary<string, GameObject> treeDictionary;
    public static Dictionary<string, GameObject> stoneDictionary;
    
    /* 사용할 오브젝트들을 메모리에 저장해 둠 */
    void Start()
    {
        treeDictionary = new Dictionary<string, GameObject>();
        stoneDictionary = new Dictionary<string, GameObject>();

        string inputObject;
        for(int i = 0; i < treeButton.transform.childCount; i++)
        {
            inputObject = treeButton.transform.GetChild(i).name;
            treeDictionary.Add(inputObject, GameObject.Find(inputObject.Split('_')[1]));
        }
        for (int i = 0; i < stoneButton.transform.childCount; i++)
        {
            inputObject = stoneButton.transform.GetChild(i).name;
            stoneDictionary.Add(inputObject, GameObject.Find(inputObject.Split('_')[1]));
        }
    }
}
