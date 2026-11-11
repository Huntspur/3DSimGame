using UnityEngine;
using System;
using System.Collections.Generic;
[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject //ScriptableObject is a data container that allows you to store data in the Unity editor. It can be used to create assets that can be reused across multiple scenes or projects. In this case, it is used to store a list of objects that can be placed in the grid.
{
    public List<ObjectData> objectsData;
}

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string objectName { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField]
    public GameObject prefab { get; private set; }
}