using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    /* 맵 크기 */
    [SerializeField]
    private int terrainSize;
    /* 선택된 오브젝트 회전 속도 */
    [SerializeField]
    private float rotSpeed;

    /* 패널 실행 여부 */
    public static bool isPanelOn;
    /* 오브젝트 버튼 클릭 여부 */
    private static bool isObjectSelected;
    /* 데이터 작성 버튼 클릭 여부 */
    public static bool isWriteStart;
    public static bool isReset;
    /* 버튼을 통해 선택된 오브젝트의 타입, 이름 */
    private static string selectedType;
    private static string selectedName;

    /* 오브젝트 데이터들 */
    private Dictionary<string, GameObject> treeDictionary;
    private Dictionary<string, GameObject> stoneDictionary;
    /* 해당 칸에 대한 정보 배열들 */
    private int[,] isSelected;
    private GameObject[,] createdObject;
    /* 맵 제작 오브젝트 번호 */
    private int craftingNum;
    /* 선택된 오브젝트 */
    private GameObject selectedObject;
    /* 선택 블록 개수 */
    private int blockCount;

    /* 이전에 선택된 블록, 선택된 블록 */
    private GameObject preSelectedBlock, curSelectedBlock;
    private Terrain theTerrain;
    /* Raycast Hit 정보 */
    private RaycastHit hitInfo;

    /* 사용할 오브젝트들을 메모리에 저장해 둠 */
    void Start()
    {
        /* 나무, 돌 오브젝트 사전 초기화 */
        treeDictionary = new Dictionary<string, GameObject>();
        stoneDictionary = new Dictionary<string, GameObject>();
        theTerrain = GetComponent<Terrain>();

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
        curSelectedBlock = settingBlock;

        /* 맵 데이터 초기화 */
        blockCount = terrainSize / 4;
        theTerrain.terrainData = generateTerrain(
            theTerrain.terrainData,
            0, 0,
            terrainSize, terrainSize,
            4);
        generateSettingBlock();
    }

    void Update()
    {
        generateObject();
        createObjectToPoint();
        writeText();
        resetMap();
    }

    public static void setObject(string _type, string _name)
    {
        selectedType = _type;
        selectedName = _name;
        isObjectSelected = true;
    }

    private void resetMap()
    {
        if(isReset)
        {
            theTerrain.terrainData = generateTerrain(
                theTerrain.terrainData,
                0, 0,
                terrainSize, terrainSize,
                4);
            for (int i = 0; i < blockCount; i++)
            {
                for (int j = 0; j < blockCount; j++)
                {
                    if (createdObject[i, j] != null)
                    {
                        Destroy(createdObject[i, j]);
                        createdObject[i, j] = null;
                    }
                }
            }
            craftingNum = 0;
            preSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
            selectedObject.SetActive(false);
            isReset = false;
        }
    }

    /* 오브젝트의 생성을 시작할 수 있도록 생성할 오브젝트 정의 함수 */
    private void generateObject()
    {
        if (isObjectSelected)
        {
            selectedObject.SetActive(false);
            switch (selectedType)
            {
                case "Tree":
                    selectedObject = treeDictionary[selectedName];
                    craftingNum = int.Parse(selectedName.Split('-')[1]);
                    break;
                case "Stone":
                    selectedObject = stoneDictionary[selectedName];
                    craftingNum = int.Parse(selectedName.Split('-')[1]);
                    break;
                case "Water":
                    craftingNum = -1;
                    break;
            }
            selectedObject.SetActive(true);
            isObjectSelected = false;
        }
    }

    private void writeText()
    {
        if(isWriteStart)
        {
            StreamWriter dataselectedType = new StreamWriter("map_data_type.txt", false);
            StreamWriter data_rot = new StreamWriter("map_data_rot.txt", false);

            string input_type;
            string input_rot;

            for (int i = 0; i < terrainSize; i++)
            {
                input_type = "";
                input_rot = "";
                for (int j = 0; j < terrainSize - 1; j++)
                {
                    if(i % 4 == 0 && j % 4 == 0)
                    {
                        int blockX = i / 4, blockZ = j / 4;
                        input_type += isSelected[blockX, blockZ] + ", ";
                        if (isSelected[blockX, blockZ] > 0)
                            input_rot += createdObject[blockX, blockZ].transform.rotation.y + ", ";
                        else
                            input_rot += "0, ";
                    }
                    else
                    {
                        input_type += "0, ";
                        input_rot += "0, ";
                    }
                }
                if (i % 4 == 0)
                {
                    int blockX = i / 4;
                    input_type += isSelected[blockX, blockCount - 1];
                    if (isSelected[blockX, blockCount - 1] > 0)
                        input_rot += createdObject[blockX, blockCount - 1].transform.rotation.y;
                    else
                        input_rot += "0";
                }
                else
                {
                    input_type += '0';
                    input_rot += '0';
                }
                dataselectedType.WriteLine(input_type);
                data_rot.WriteLine(input_rot);
            }
            dataselectedType.Close();
            data_rot.Close();

            isWriteStart = false;
            Debug.Log("작성 끝");
        }
    }

    private void ReadText()
    {
        string[] text = File.ReadAllLines(@"tree_data.txt");
        int i, j;
        foreach (string show in text)
        {
            string[] sd = show.Split(':', ' ');
            i = int.Parse(sd[5]);
            j = int.Parse(sd[9]);
        }
    }

    /* 마우스를 통해 원하는 자리에 오브젝트를 생성하는 함수 */
    private void createObjectToPoint()
    {
        
        if (craftingNum != 0 && !isPanelOn)
        {
            string[] selectedNum = curSelectedBlock.name.Split('_');
            int x = int.Parse(selectedNum[0]);
            int z = int.Parse(selectedNum[1]);

            /* 마우스가 위치한 선택 블록 표시 */
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
                out hitInfo))
            {
                curSelectedBlock = hitInfo.transform.gameObject;
                /* 이전 선택 블록 투명화 */
                preSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                /* 해당 위치에 오브젝트가 있을 경우 빨간색을 표시 */
                if (isSelected[x, z] != 0)
                    curSelectedBlock.GetComponent<MeshRenderer>().material = imPossible;
                /* 그렇지 않을 경우 연두색을 표시 */
                else
                    curSelectedBlock.GetComponent<MeshRenderer>().material = Possible;
                preSelectedBlock = curSelectedBlock;
            }

            /* W 키를 누를 경우, 선택한 오브젝트 회전 */
            if(Input.GetKey(KeyCode.W) && craftingNum != -1)
                selectedObject.transform.RotateAround(
                    selectedObject.transform.position, 
                    Vector3.up, 
                    rotSpeed * Time.deltaTime);
            /* R키를 누를 경우, 오브젝트 생성 종료 */
            else if (Input.GetKey(KeyCode.R))
            {
                craftingNum = 0;
                preSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                selectedObject.SetActive(false);
            }

            /* 왼쪽 마우스 버튼으로 선택한 오브젝트를 생성 */
            if (Input.GetMouseButtonDown(0) && isSelected[x, z] == 0)
            {
                /* 물 이외의 오브젝트일 경우 */
                if(craftingNum != -1)
                {   
                    createdObject[x,z] = Instantiate(
                                            selectedObject, 
                                            curSelectedBlock.transform.position, 
                                            selectedObject.transform.rotation);
                    curSelectedBlock.GetComponent<MeshRenderer>().material = unSelected;
                }
                /* 물 오브젝트일 경우 */
                else
                {
                    createdObject[x, z] = settingBlock;
                    theTerrain.terrainData = 
                        generateTerrain(theTerrain.terrainData, x*4, z*4, 4, 4, 0);
                }
                isSelected[x, z] = craftingNum;
            }
            /* 마우스 오른쪽 키를 누를 경우, 오브젝트 삭제 */
            else if (Input.GetMouseButton(1) && isSelected[x, z] != 0)
            {
                if (createdObject[x, z].name == "0_0")
                    theTerrain.terrainData = 
                        generateTerrain(theTerrain.terrainData, x * 4, z * 4, 4, 4, 4);
                else
                    Destroy(createdObject[x, z]);   
                isSelected[x, z] = 0;
                createdObject[x, z] = null;
            }
        }
    }

    /* 특정 위치의 Terrain 깊이 설정 함수 */
    private TerrainData generateTerrain(TerrainData terrainData, int startX, int startZ, int endX, int endZ, int depth)
    {
        terrainData.SetHeights(startX, startZ, GenerateHeights(endX, endZ, depth));
        return terrainData;
    }

    /* 특정 영역에 높이를 설정해주는 함수 */
    private float[,] GenerateHeights(int endX, int endZ, float depth)
    {
        float[,] heights = new float[endX, endZ];
        for (int x = 0; x < endX; x++)
        {
            for (int y = 0; y < endZ; y++)
                heights[x, y] = depth;
        }
        return heights;
    }

    /* 선택 블록 초기화 함수 */
    private void generateSettingBlock()
    {
        isSelected = new int[blockCount, blockCount];
        createdObject = new GameObject[blockCount, blockCount];

        for(int i = 0; i < blockCount; i++)
        {
            for(int j = 0; j < blockCount; j++)
            {
                Vector3 createPos = new Vector3(i * 4+2, 4, j * 4+2);
                GameObject clone = Instantiate(settingBlock, createPos, Quaternion.identity);
                clone.name = i.ToString() + '_' + j.ToString();
            }
        }
    }
}


