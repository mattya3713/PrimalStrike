//--------------------------------------------------------------------------------.
// �쐬�ҁF���V �K�P.
//--------------------------------------------------------------------------------.
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// �G�l�~�[�̃Z���T�[�֘A.
/// </summary>
public class SensorSystem
{
    /// <summary>
    /// ���o�Z���T�[.
    /// </summary>
    [System.Serializable]
    public class VisionSensor
    {        
        private const int SEGMENTS = 20; // ��`�̕�����                    

        public float innerRadius;   // �����a.
        public float outerRadius;   // �O���a.
        public float topOffset;     // ������I�t�Z�b�g.
        public float bottomOffset;  // �������I�t�Z�b�g.
        public float degree;        // ��̊p�x.

        /// <summary>
        /// �͈͓��Ƀ^�[�Q�b�g�����邩����.
        /// </summary>
        /// <param name="origin">����J�n�ʒu</param>
        /// <param name="forward">�O����</param>
        /// <param name="target">�^�[�Q�b�g�ʒu</param>
        /// <returns></returns>
        public bool isHit(Vector3 origin, Vector3 forward, Vector3 target)
        {
            Vector3 to_target = target - origin;

            // �㉺�`�F�b�N.
            if (to_target.y > topOffset || to_target.y < bottomOffset)
            {
                return false;
            }

            // XZ���ʂł̋���.
            Vector3 to_target_xz = new Vector3(to_target.x, 0, to_target.z);
            float dist = to_target_xz.magnitude;
            if (dist < innerRadius || dist > outerRadius)
            {
                return false;
            }

            // ��p�`�F�b�N.
            Vector3 forwardXZ = new Vector3(forward.x, 0, forward.z).normalized;
            Vector3 dirXZ = to_target_xz.normalized;
            float angle = Vector3.Angle(forwardXZ, dirXZ);

            return angle <= degree * 0.5f;
        }

        /// <summary>
        /// �f�o�b�O�\��.
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
                    // �O���Ɠ���������
                    Debug.DrawLine(prev_top_outer, top_outer, lineColor);
                    Debug.DrawLine(prev_top_inner, top_inner, lineColor);
                    Debug.DrawLine(prev_bottom_outer, bottom_outer, lineColor);
                    Debug.DrawLine(prev_bottom_inner, bottom_inner, lineColor);
                }

                // ��ʂƉ��ʂ��c�Ɍ���
                Debug.DrawLine(top_outer, bottom_outer, lineColor);
                Debug.DrawLine(top_inner, bottom_inner, lineColor);

                // ���ˏ�̐�
                Debug.DrawLine(top_inner, top_outer, lineColor);       // ��� �����O
                Debug.DrawLine(bottom_inner, bottom_outer, lineColor); // ��� �����O

                prev_top_outer = top_outer;
                prev_top_inner = top_inner;
                prev_bottom_outer = bottom_outer;
                prev_bottom_inner = bottom_inner;
            }
        }
    }
}
