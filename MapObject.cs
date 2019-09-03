using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Stones;
    [SerializeField]
    private GameObject[] Trees;
    [SerializeField]
    private GameObject settingBlock;
    [SerializeField]
    private int blockCount;
    [SerializeField]
    private Material unSelected;
    [SerializeField]
    private Material Selected;
    /* 오브젝트 데이터들 */
    public static Dictionary<string, GameObject> treeDictionary;
    public static Dictionary<string, GameObject> stoneDictionary;
    private static bool isCrafting;
    private static GameObject selectedObject;
    private GameObject preSelectedBlock, curSelectedBlock;
    private RaycastHit hitInfo;

    /* 사용할 오브젝트들을 메모리에 저장해 둠 */
    void Start()
    {
        /* 나무, 돌 오브젝트 사전 초기화 */
        treeDictionary = new Dictionary<string, GameObject>();
        stoneDictionary = new Dictionary<string, GameObject>();
        string inputObject;
        for(int i = 0; i < Trees.Length; i++)
        {
            inputObject = Trees[i].name;
            treeDictionary.Add(inputObject, Trees[i]);
        }
        for (int i = 0; i < Stones.Length; i++)
        {
            inputObject = Stones[i].name;
            stoneDictionary.Add(inputObject,Stones[i]);
        }

        /* 오브젝트를 둘 수 있는 블록 오브젝트 초기화 */
        preSelectedBlock = settingBlock;
        selectedObject = settingBlock;
        generateSettingBlock();
    }

    void Update()
    {
        createObjectToPoint();        
    }

    /* 오브젝트의 생성을 시작할 수 있도록 생성할 오브젝트 정의 함수 */
    public static void generateObject(string _type, string _objectName)
    {
        /* */
        //if(selectedObject != null)
        selectedObject.SetActive(false);
        switch (_type)
        {
            case "Tree":
                selectedObject = treeDictionary[_objectName];
                break;
            case "Stone":
                selectedObject = stoneDictionary[_objectName];
                break;
            case "Water":
                break;
        }
        selectedObject.SetActive(true);
        isCrafting = true;
    }

    /* 마우스를 통해 원하는 자리에 오브젝트를 생성하는 함수 */
    private void createObjectToPoint()
    {
        
        if (isCrafting)
        {
            /* 마우스가 위치한 선택 블록 표시 */
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
                out hitInfo))
            {
                curSelectedBlock = hitInfo.transform.gameObject;
                preSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                curSelectedBlock.GetComponent<MeshRenderer>().material = Selected;
                preSelectedBlock = curSelectedBlock;
            }
            /* 왼쪽 마우스 버튼으로 선택한 오브젝트를 생성 */
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(selectedObject, curSelectedBlock.transform.position, Quaternion.identity);
                isCrafting = false;
                curSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
            }
            else if (Input.GetMouseButtonDown(1))
            {

            }
        }
    }

    /* 선택 블록 초기화 함수 */
    private void generateSettingBlock()
    {
        for(int i = 0; i < blockCount; i++)
        {
            for(int j = 0; j < blockCount; j++)
            {
                Vector3 createPos = new Vector3(i * 4, 0, j * 4);
                Instantiate(settingBlock, createPos, Quaternion.identity);
            }
        }
    }
}


