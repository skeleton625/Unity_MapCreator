using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    /* 돌 오브젝트 프리팹 */
    [SerializeField]
    private GameObject[] Stones;
    /* 나무 오브젝트 프리팹 */
    [SerializeField]
    private GameObject[] Trees;
    /* 선택 블록 오브젝트 */
    [SerializeField]
    private GameObject settingBlock;
    /* 비선택 표시 */
    [SerializeField]
    private Material unSelected;
    /* 선택 표시 */
    [SerializeField]
    private Material Possible;
    [SerializeField]
    private Material imPossible;
    /* 선택 블록 한 행 개수 */
    [SerializeField]
    private int blockCount;
    /* 선택된 오브젝트 회전 속도 */
    [SerializeField]
    private float rotSpeed;

    /* 오브젝트 데이터들 */
    public static Dictionary<string, GameObject> treeDictionary;
    public static Dictionary<string, GameObject> stoneDictionary;

    /* 맵 제작 실행 여부 */
    private static bool isCrafting;
    /* 선택된 오브젝트 */
    private static GameObject selectedObject;
    /* 이전에 선택된 블록, 선택된 블록 */
    private GameObject preSelectedBlock, curSelectedBlock;
    /* Raycast Hit 정보 */
    private RaycastHit hitInfo;
    private bool[,] isSelected;

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
            string[] selectedNum;
            /* 마우스가 위치한 선택 블록 표시 */
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
                out hitInfo))
            {
                curSelectedBlock = hitInfo.transform.gameObject;
                selectedNum = curSelectedBlock.name.Split('_');
                preSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                if(isSelected[int.Parse(selectedNum[0]), int.Parse(selectedNum[1])])
                    curSelectedBlock.GetComponent<MeshRenderer>().material = imPossible;
                else
                    curSelectedBlock.GetComponent<MeshRenderer>().material = Possible;
                preSelectedBlock = curSelectedBlock;
            }
            /* R 키를 누를 경우, 선택한 오브젝트 회전 */
            if(Input.GetKey(KeyCode.R))
                selectedObject.transform.RotateAround(selectedObject.transform.position, Vector3.up, rotSpeed * Time.deltaTime);
            /* 왼쪽 마우스 버튼으로 선택한 오브젝트를 생성 */
            if (Input.GetMouseButtonDown(0))
            {
                selectedNum = curSelectedBlock.name.Split('_');
                if (!isSelected[int.Parse(selectedNum[0]), int.Parse(selectedNum[1])])
                {
                    isSelected[int.Parse(selectedNum[0]), int.Parse(selectedNum[1])] = true;
                    Instantiate(selectedObject, curSelectedBlock.transform.position, selectedObject.transform.rotation);
                    curSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                }
                else
                    Debug.Log("이미 생성되어 있는 자리");
            }
            /* 오브젝트 생성 종료 */
            else if (Input.GetMouseButton(1))
            {
                isCrafting = false;
            }
        }
    }

    /* 선택 블록 초기화 함수 */
    private void generateSettingBlock()
    {
        isSelected = new bool[blockCount, blockCount];

        for(int i = 0; i < blockCount; i++)
        {
            for(int j = 0; j < blockCount; j++)
            {
                Vector3 createPos = new Vector3(i * 4, 0, j * 4);
                GameObject clone = Instantiate(settingBlock, createPos, Quaternion.identity);
                clone.name = i.ToString() + '_' + j.ToString();
            }
        }
    }
}


