//--------------------------------------------------------------------------------.
// 作成者：藤澤 幸輝.
//--------------------------------------------------------------------------------.
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// エネミーのセンサー関連.
/// </summary>
public class SensorSystem
{
    /// <summary>
    /// 視覚センサー.
    /// </summary>
    [System.Serializable]
    public class VisionSensor
    {        
        private const int SEGMENTS = 20; // 扇形の分割数                    

        public float innerRadius;   // 内半径.
        public float outerRadius;   // 外半径.
        public float topOffset;     // 上方向オフセット.
        public float bottomOffset;  // 下方向オフセット.
        public float degree;        // 扇の角度.

        /// <summary>
        /// 範囲内にターゲットがいるか判定.
        /// </summary>
        /// <param name="origin">判定開始位置</param>
        /// <param name="forward">前方向</param>
        /// <param name="target">ターゲット位置</param>
        /// <returns></returns>
        public bool isHit(Vector3 origin, Vector3 forward, Vector3 target)
        {
            Vector3 to_target = target - origin;

            // 上下チェック.
            if (to_target.y > topOffset || to_target.y < bottomOffset)
            {
                return false;
            }

            // XZ平面での距離.
            Vector3 to_target_xz = new Vector3(to_target.x, 0, to_target.z);
            float dist = to_target_xz.magnitude;
            if (dist < innerRadius || dist > outerRadius)
            {
                return false;
            }

            // 扇角チェック.
            Vector3 forwardXZ = new Vector3(forward.x, 0, forward.z).normalized;
            Vector3 dirXZ = to_target_xz.normalized;
            float angle = Vector3.Angle(forwardXZ, dirXZ);

            return angle <= degree * 0.5f;
        }

        /// <summary>
        /// デバッグ表示.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="forward"></param>        
        public void drawDebug(Vector3 origin, Vector3 forward, Color lineColor)
        {
            float half_angle = degree * 0.5f;

            Vector3 prev_top_outer = Vector3.zero;
            Vector3 prev_top_inner = Vector3.zero;
            Vector3 prev_bottom_outer = Vector3.zero;
            Vector3 prev_bottom_inner = Vector3.zero;

            Vector3 first_top_outer = Vector3.zero;
            Vector3 first_top_inner = Vector3.zero;
            Vector3 first_bottom_outer = Vector3.zero;
            Vector3 first_bottom_inner = Vector3.zero;

            for (int i = 0; i <= SEGMENTS; i++)
            {
                float angle = -half_angle + i * (degree / SEGMENTS);
                Quaternion rot = Quaternion.Euler(0, angle, 0);
                Vector3 dir = rot * forward;

                Vector3 top_outer = origin + dir * outerRadius + Vector3.up * topOffset;
                Vector3 top_inner = origin + dir * innerRadius + Vector3.up * topOffset;
                Vector3 bottom_outer = origin + dir * outerRadius + Vector3.up * bottomOffset;
                Vector3 bottom_inner = origin + dir * innerRadius + Vector3.up * bottomOffset;

                if (i == 0)
                {
                    first_top_outer = top_outer;
                    first_top_inner = top_inner;
                    first_bottom_outer = bottom_outer;
                    first_bottom_inner = bottom_inner;
                }
                else
                {
                    // 外周と内周を結ぶ
                    Debug.DrawLine(prev_top_outer, top_outer, lineColor);
                    Debug.DrawLine(prev_top_inner, top_inner, lineColor);
                    Debug.DrawLine(prev_bottom_outer, bottom_outer, lineColor);
                    Debug.DrawLine(prev_bottom_inner, bottom_inner, lineColor);
                }

                // 上面と下面を縦に結ぶ
                Debug.DrawLine(top_outer, bottom_outer, lineColor);
                Debug.DrawLine(top_inner, bottom_inner, lineColor);

                // 放射状の線
                Debug.DrawLine(top_inner, top_outer, lineColor);       // 上面 内→外
                Debug.DrawLine(bottom_inner, bottom_outer, lineColor); // 底面 内→外

                prev_top_outer = top_outer;
                prev_top_inner = top_inner;
                prev_bottom_outer = bottom_outer;
                prev_bottom_inner = bottom_inner;
            }
        }
    }
}
