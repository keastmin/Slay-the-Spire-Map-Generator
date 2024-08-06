using UnityEngine;

public class NodeReturn : MonoBehaviour
{
    [SerializeField]
    private NodeData normalNode;
    [SerializeField]
    private NodeData eventNode;
    [SerializeField]
    private NodeData eliteNode;
    [SerializeField]
    private NodeData restNode;
    [SerializeField]
    private NodeData merchantNode;
    [SerializeField]
    private NodeData tresureNode;
    [SerializeField]
    private NodeData bossNode;

    // Instance for probabilistic return of nodes to be used by MapGenerator.cs and Node.cs
    static public NodeReturn instance;

    void Awake()
    {
        instance = this;
    }

    // Method of returning node data probabilistically
    public NodeData GetRandomNodeData()
    {
        float num = Random.value;
        if      (num < 0.45f) return normalNode;
        else if (num < 0.67f) return eventNode;
        else if (num < 0.83f) return eliteNode;
        else if (num < 0.95f) return restNode;
        return merchantNode;
    }

    public NodeData GetTresureNodeData()
    {
        return tresureNode;
    }

    public NodeData GetRestNodeData()
    {
        return restNode;
    }

    public NodeData GetNormalNodeData()
    {
        return normalNode;
    }

    public NodeData GetBossNodeData()
    {
        return bossNode;
    }
}
