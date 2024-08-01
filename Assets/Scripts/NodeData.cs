using UnityEngine;

[CreateAssetMenu(fileName = "Node Data", menuName = "Scriptable Object/Node Data")]
public class NodeData : ScriptableObject
{
    public Sprite sprite;
    public NodeType type;
}
