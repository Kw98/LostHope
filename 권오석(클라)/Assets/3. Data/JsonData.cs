using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonData : Singleton<JsonData>
{
    [System.Serializable]
    public class MonsterJsonData
    {
        public int power;
        public float atkdelay;
        public float speed;
    }

    [System.Serializable]
    public class MonsterJson
    {
        public List<MonsterJsonData> monster;
    }

    [SerializeField] private TextAsset monsterJson;

    public MonsterJson mj;

    // Start is called before the first frame update
    void Start()
    {
        mj = JsonUtility.FromJson<MonsterJson>(monsterJson.text);
    }
}
