//--------------------------------------------------------------------------------.
// 作成者：藤澤 幸輝.
//--------------------------------------------------------------------------------.
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

/// <summary>
/// エネミーの基底クラス.
/// </summary>
public class EnemyBase : MonoBehaviour
{
    public GameObject target;

    [SerializeField]
    public SensorSystem.VisionSensor visionSensor = new SensorSystem.VisionSensor
    {
        innerRadius = 0.0f,
        outerRadius = 10.0f,
        topOffset = 2.0f,
        bottomOffset = -1.0f,
        degree = 90.0f,        
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color line_color = visionSensor.isHit(transform.position, transform.forward, target.transform.position) ? Color.red : Color.gray;
        visionSensor.drawDebug(transform.position, transform.forward, line_color);
    }

    
    void Test()
    {

    }
}
