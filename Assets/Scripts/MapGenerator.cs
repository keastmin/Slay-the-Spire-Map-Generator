using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public Transform mapImage;
    public Button button;
    public Button bossButton;
    public RawImage lineDot;

    public int col = 7;
    public int row = 15;
    public float paddingX = 80f;
    public float paddingY = 120f;

    private Vector2[,] positionGrid;
    private Vector2 bossRoomPos;
    private Node[,] nodeGrid;
    private HashSet<int>[,] paths;  // Next Path X Index

    void Awake()
    {
        positionGrid = new Vector2[col, row];
        nodeGrid = new Node[col, row];
        paths = new HashSet<int>[col, row - 1];
        for(int i = 0; i < col; i++)
        {
            for(int j = 0; j < row - 1; j++)
            {
                paths[i, j] = new HashSet<int>();
            }
        }
    }

    void Start()
    {
        InitPositionGrid();
        MakePath();
        ConnectPathLine();
        CreateNode();
        CompileNode();
        ConnectBossRoom();
    }

    private void InitPositionGrid()
    {
        RectTransform rectTransform = mapImage.GetComponent<RectTransform>();
        float width = rectTransform.rect.width - paddingX * 2;
        float height = rectTransform.rect.height - paddingY * 2 - 100;
        float spacingX = width / (col + 1);
        float spacingY = height / (row + 1);
        float startX = paddingX + spacingX;
        float startY = paddingY + spacingY;

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                float posX = startX + x * spacingX;
                float posY = startY + y * spacingY;

                // 랜덤하게 위치 조정
                posX += UnityEngine.Random.Range(-10.0f, 10.1f);
                posY += UnityEngine.Random.Range(-10.0f, 10.1f);
                positionGrid[x, y] = new Vector2(posX, posY);
            }
        }

        bossRoomPos = new Vector2(width / 2 + spacingX, startY + row * spacingY);
    }

    private void MakePath()
    {
        int firstStartIdx = 0;

        for(int i = 0; i < 6; i++)
        {
            // Select Start Index
            int selectIdx = UnityEngine.Random.Range(0, col);

            if(i == 0)
            {
                firstStartIdx = selectIdx;
            }
            else if(i == 1)
            {
                while(firstStartIdx == selectIdx)
                {
                    selectIdx = UnityEngine.Random.Range(0, col);
                }
            }

            // Start select next path index
            for(int floor = 0; floor < row - 1; floor++)
            {
                List<int> possibleIdx = new List<int>();

                if(selectIdx > 0)
                {
                    if(CheckPathCross(floor, selectIdx - 1, selectIdx))
                    {
                        possibleIdx.Add(selectIdx - 1);
                    }
                }
                possibleIdx.Add(selectIdx);
                if(selectIdx < col - 1)
                {
                    if(CheckPathCross(floor, selectIdx + 1, selectIdx))
                    {
                        possibleIdx.Add(selectIdx + 1);
                    }
                }

                int prevIdx = selectIdx;
                selectIdx = possibleIdx[UnityEngine.Random.Range(0, possibleIdx.Count)];
                paths[prevIdx, floor].Add(selectIdx);
            }
        }
    }

    bool CheckPathCross(int checkY, int checkX, int nextIndex)
    {
        foreach(int idx in paths[checkX, checkY])
        {
            if(idx == nextIndex)
            {
                return false;
            }
        }
        return true;
    }

    void ConnectPathLine()
    {
        HashSet<int> endIdx = new HashSet<int>();
        for(int y = 0; y < row - 1; y++)
        {
            for(int x = 0; x < col; x++)
            {
                foreach(int nextX in paths[x, y])
                {
                    DrawLine(positionGrid[x, y], positionGrid[nextX, y + 1]);
                    if (y + 1 == row - 1) endIdx.Add(nextX);
                }
            }
        }
        foreach(int idx in endIdx)
        {
            DrawLine(positionGrid[idx, row - 1], bossRoomPos);
        }
    }

    void DrawLine(Vector2 currV, Vector2 targV)
    {
        float dotSpacing = 12.0f;
        Vector2 direction = (targV - currV).normalized;
        currV += direction * 17;
        targV -= direction * 13;
        float distance = Vector2.Distance(currV, targV);
        float angle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;

        int dotCount = Mathf.FloorToInt(distance / dotSpacing);
        for(int i = 0; i <= dotCount; i++)
        {
            RawImage rawImage = Instantiate(lineDot, mapImage);
            RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currV + direction * (dotSpacing * i);
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            rectTransform.localScale = Vector2.one;
        }
    }

    void CreateNode()
    {
        Queue<Tuple<int, int>> nodeQ = new Queue<Tuple<int, int>>();
        bool[,] visited = new bool[col, row];

        for (int i = 0; i < col; i++)
        {
            if (paths[i, 0].Count > 0)
            {
                nodeQ.Enqueue(new Tuple<int, int>(i, 0));
                visited[i, 0] = true;
                InstantiateButton(i, 0);
                nodeGrid[i, 0].selectState = true;
            }
        }

        while (nodeQ.Count > 0)
        {
            Tuple<int, int> currT = nodeQ.Dequeue();
            int x = currT.Item1;
            int y = currT.Item2;

            if (y == row - 1) continue;

            foreach (int next in paths[x, y])
            {
                if (!visited[next, y + 1])
                {
                    InstantiateButton(next, y + 1);
                    nodeQ.Enqueue(new Tuple<int, int>(next, y + 1));
                    visited[next, y + 1] = true;
                }
                nodeGrid[next, y + 1].prevNodes.Add(nodeGrid[x, y]);
                nodeGrid[x, y].nextNodes.Add(nodeGrid[next, y + 1]);
            }
        }

        for (int i = 0; i < row; i++)
        {
            List<Node> nodes = new List<Node>();
            for (int j = 0; j < col; j++)
            {
                if (nodeGrid[j, i] != null)
                {
                    nodes.Add(nodeGrid[j, i]);
                }
            }

            for (int k = 0; k < nodes.Count; k++)
            {
                for (int u = 0; u < nodes.Count; u++)
                {
                    if (k != u)
                    {
                        nodes[k].floorNodes.Add(nodes[u]);
                    }
                }
            }
        }
    }

    void InstantiateButton(int x, int y)
    {
        Button node = Instantiate(button, mapImage);
        RectTransform nodeRect = node.GetComponent<RectTransform>();
        nodeGrid[x, y] = node.GetComponent<Node>();
        nodeGrid[x, y].NodeSet();
        nodeRect.anchoredPosition = positionGrid[x, y];
        nodeRect.localScale = Vector2.one;
    }

    void CompileNode()
    {
        Rule1();

        List<int> startNodes = new List<int>();
        for(int i = 0; i < col; i++)
        {
            if (nodeGrid[i, 0] != null)
            {
                startNodes.Add(i);
            }
        }

        foreach (int idx in startNodes)
        {
            Queue<Tuple<int, int>> nodeQ = new Queue<Tuple<int, int>>();
            nodeQ.Enqueue(new Tuple<int, int>(idx, 0));

            while (nodeQ.Count > 0)
            {
                HashSet<NodeType> changeTypeHash = new HashSet<NodeType>();
                Tuple<int, int> nodePos = nodeQ.Dequeue();
                int x = nodePos.Item1;
                int y = nodePos.Item2;

                if(y < row - 1)
                {
                    foreach(int next in paths[x, y])
                    {
                        nodeQ.Enqueue(new Tuple<int, int>(next, y + 1));
                    }
                }

                if (y == 0 || y == 8 || y == 14) continue;

                if(CheckRule(x, y))
                {
                    Rule2_Compliance(y, changeTypeHash);
                    Rule3_Compliance(x, y, changeTypeHash);
                    Rule4_Compliance(x, y, changeTypeHash);

                    if (changeTypeHash.Count >= 5)
                    {
                        Debug.Log("Map Reset");
                        ChangeAllNodeType();
                        CompileNode();
                        return;
                    }
                    ChangeNode(x, y, changeTypeHash);
                }
            }
        }
    }

    // 1 floor : normal, 9 floor : tresure, 15 floor : rest
    void Rule1()
    {
        int[] floors = { 0, 8, 14 };
        NodeData[] datas =
        {
            NodeReturn.instance.GetNormalNodeData(),
            NodeReturn.instance.GetTresureNodeData(),
            NodeReturn.instance.GetRestNodeData()
        };

        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if (nodeGrid[j, floors[i]] != null)
                {
                    nodeGrid[j, floors[i]].NodeSet(datas[i]);
                }
            }
        }
    }

    bool CheckRule(int x, int y)
    {
        if (Rule2_Check(x, y)) return true;
        if (Rule3_Check(x, y)) return true;
        if (Rule4_Check(x, y)) return true;
        return false;
    }

    // Elite and Rest Sites can’t be assigned below the 6th Floor.
    bool Rule2_Check(int x, int y)
    {
        NodeType type = nodeGrid[x, y].nodeType;
        if (y < 5 && (type == NodeType.Elite || type == NodeType.Rest)) return true;
        return false;
    }

    bool Rule3_Check(int x, int y)
    {
        NodeType currType = nodeGrid[x, y].nodeType;
        if(currType == NodeType.Elite || currType == NodeType.Rest || currType == NodeType.Merchant)
        {
            foreach(Node prev in nodeGrid[x, y].prevNodes)
            {
                if (prev.nodeType == currType) return true;
            }
            foreach(Node next in nodeGrid[x, y].nextNodes)
            {
                if (next.nodeType == currType) return true;
            }
        }
        return false;
    }

    bool Rule4_Check(int x, int y)
    {
        foreach (Node prev in nodeGrid[x, y].prevNodes)
        {
            if (prev.nextNodes.Count > 1)
            {
                HashSet<NodeType> typeHash = new HashSet<NodeType>();
                foreach (Node next in prev.nextNodes)
                {
                    typeHash.Add(next.nodeType);
                }
                if (typeHash.Count == 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Rule2_Compliance(int y, HashSet<NodeType> hash)
    {
        if(y < 5)
        {
            hash.Add(NodeType.Elite);
            hash.Add(NodeType.Rest);
        }
    }

    void Rule3_Compliance(int x, int y, HashSet<NodeType> hash)
    {
        foreach(Node prev in nodeGrid[x, y].prevNodes)
        {
            if (prev.nodeType == NodeType.Elite || prev.nodeType == NodeType.Rest || prev.nodeType == NodeType.Merchant)
                hash.Add(prev.nodeType);
        }
        foreach (Node next in nodeGrid[x, y].nextNodes)
        {
            if (next.nodeType == NodeType.Elite || next.nodeType == NodeType.Rest || next.nodeType == NodeType.Merchant)
                hash.Add(next.nodeType);
        }
    }

    void Rule4_Compliance(int x, int y, HashSet<NodeType> hash)
    {
        foreach(Node prev in nodeGrid[x, y].prevNodes)
        {
            if(prev.nextNodes.Count > 1)
            {
                HashSet<NodeType> typeHash = new HashSet<NodeType>();
                foreach(Node next in prev.nextNodes)
                {
                    if (next != nodeGrid[x, y]) typeHash.Add(next.nodeType);
                }
                if(typeHash.Count == 1) hash.Add(typeHash.First());
            }
        }
    }

    void ChangeNode(int x, int y, HashSet<NodeType> hash)
    {
        Debug.Log("변경 = " + x + " " + y);
        while (hash.Contains(nodeGrid[x, y].nodeType))
        {
            nodeGrid[x, y].NodeSet();
        }
    }

    void ChangeAllNodeType()
    {
        for(int x = 0; x < col; x++)
        {
            for(int y = 0; y < row; y++)
            {
                if (nodeGrid[x, y] != null)
                {
                    nodeGrid[x, y].NodeSet();
                }
            }
        }
    }

    void ConnectBossRoom()
    {
        Button boss = Instantiate(bossButton, mapImage);
        RectTransform nodeRect = boss.GetComponent<RectTransform>();
        Node bossNode = boss.GetComponent<Node>();
        nodeRect.anchoredPosition = bossRoomPos;
        nodeRect.localScale = Vector2.one;
        for (int i = 0; i < col; i++)
        {
            if (nodeGrid[i, row - 1] != null)
            {
                nodeGrid[i, row - 1].nextNodes.Add(bossNode);
            }
        }
    }
}

