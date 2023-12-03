using UnityEngine;

namespace AngoraUtility
{
    /// <summary>
    /// Transform�̂����\�b�h�̊g��
    /// </summary>
    public static class ExTransform
    {
        // ***** EulerAngles �֌W ****************************************************************************************************************

        /// <summary>
        /// ���W��
        /// </summary>
        public enum AXIS
        {
            X, Y, Z
        }

        /// <summary>
        /// ���`�I�ɂȂ�悤�ɁA�␳���� EulerAngles ���擾����i�Ώۂ�Transform, ��Ƃ��鎲, ��l�j
        /// </summary>
        /// <param name="argTransform"></param>
        /// <param name="argAxis"></param>
        /// <param name="startAngle"></param>
        /// <returns></returns>
        public static Vector3 GetFixEulerAngles(Transform argTransform, ExTransform.AXIS argAxis = AXIS.Z, float startAngle = 0f)
        {
            // ���� rotation ��\�� EulerAngle �� 2���߂�
            var eulerAngle_1 = argTransform.eulerAngles;
            var eulerAngle_2 = NormalizeEulerAngles(new Vector3(180 - eulerAngle_1.x, 180 + eulerAngle_1.y, 180 + eulerAngle_1.z));

            // �w�肵��������ɁA���`�I�ɕω�����悤�� EulerAngle ��I��
            switch (argAxis)
            {
                case AXIS.X:
                    if (DegDist(eulerAngle_1.x, startAngle) <= DegDist(eulerAngle_2.x, startAngle))
                    {
                        return eulerAngle_1;
                    }
                    else return eulerAngle_2;
                case AXIS.Y:
                    if (DegDist(eulerAngle_1.y, startAngle) <= DegDist(eulerAngle_2.y, startAngle))
                    {
                        return eulerAngle_1;
                    }
                    else return eulerAngle_2;
                case AXIS.Z:
                    if (DegDist(eulerAngle_1.z, startAngle) <= DegDist(eulerAngle_2.z, startAngle))
                    {
                        return eulerAngle_1;
                    }
                    else return eulerAngle_2;
            }

            // �����K�؂ɐݒ肳��Ă��Ȃ���΁A�G���[���b�Z�[�W��\������
            Debug.LogError("�yExMathf�z�����K�؂ɐݒ肳��Ă��܂���");
            return argTransform.eulerAngles;
        }

        /// <summary>
        /// 0 �` 180, -180 �` 0 �ɕ␳����
        /// </summary>
        /// <param name="argAngle"></param>
        /// <returns></returns>
        public static Vector3 FixAnglesRange180(Vector3 argAngle)
        {
            Vector3 retAngle;
            retAngle.x = argAngle.x > 180 ? argAngle.x - 360 : argAngle.x;
            retAngle.y = argAngle.y > 180 ? argAngle.y - 360 : argAngle.y;
            retAngle.z = argAngle.z > 180 ? argAngle.z - 360 : argAngle.z;
            return retAngle;
        }

        // �p�x�� 0�`360 �Ɋۂߍ���
        private static Vector3 NormalizeEulerAngles(Vector3 argAngle)
        {
            Vector3 retAngle;
            retAngle.x = (argAngle.x + 360f) % 360;
            retAngle.y = (argAngle.y + 360f) % 360;
            retAngle.z = (argAngle.z + 360f) % 360;
            return retAngle;
        }

        // �p�x�̍��̑傫�������߂�
        private static float DegDist(float argA, float argB)
        {
            // 0 �` 360 �ɕ␳����
            float a = (argA + 360f) % 360;
            float b = (argB + 360f) % 360;

            // �������Ƃ߂�
            var dist = Mathf.Abs(a - b);

            // �ŒZ�ɕ␳����
            return dist > 180 ? 360 - dist : dist;
        }


        // ***** localScale �֌W ****************************************************************************************************************

        public static class Anchor
        {
            /// <summary>
            /// �O�ʂ́A���́A���i0, 0, 0�j
            /// </summary>
            public static Vector3 ForwardLowerLeft { get { return new Vector3(0f, 0f, 0f); } }
            /// <summary>
            /// �O�ʂ́A���́A�^�񒆁i0.5, 0, 0�j
            /// </summary>
            public static Vector3 ForwardLowerCenter { get { return new Vector3(0.5f, 0f, 0f); } }
            /// <summary>
            /// �O�ʂ́A���́A�E�i1, 0, 0�j
            /// </summary>
            public static Vector3 ForwardLowerRight { get { return new Vector3(1f, 0f, 0f); } }
            /// <summary>
            /// �O�ʂ́A�^�񒆂́A���i0, 0.5, 0�j
            /// </summary>
            public static Vector3 ForwardMiddleLeft { get { return new Vector3(0f, 0.5f, 0f); } }
            /// <summary>
            /// �O�ʂ́A�^�񒆂́A�^�񒆁i0.5, 0.5, 0�j
            /// </summary>
            public static Vector3 ForwardMiddleCenter { get { return new Vector3(0.5f, 0.5f, 0f); } }
            /// <summary>
            /// �O�ʂ́A�^�񒆂́A�E�i1, 0.5, 0�j
            /// </summary>
            public static Vector3 ForwardMiddleRight { get { return new Vector3(1f, 0.5f, 0f); } }
            /// <summary>
            /// �O�ʂ́A��́A���i0, 1, 0�j
            /// </summary>
            public static Vector3 ForwardUpperLeft { get { return new Vector3(0f, 1f, 0f); } }
            /// <summary>
            /// �O�ʂ́A��́A�^�񒆁i0.5, 1, 0�j
            /// </summary>
            public static Vector3 ForwardUpperCenter { get { return new Vector3(0.5f, 1f, 0f); } }
            /// <summary>
            /// �O�ʂ́A��́A�E�i1, 1, 0�j
            /// </summary>
            public static Vector3 ForwardUpperRight { get { return new Vector3(1f, 1f, 0f); } }

            /// <summary>
            /// ���ʂ́A���́A���i0, 0, 0.5�j
            /// </summary>
            public static Vector3 DefaultLowerLeft { get { return new Vector3(0f, 0f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A���́A�^�񒆁i0.5, 0, 0.5�j
            /// </summary>
            public static Vector3 DefaultLowerCenter { get { return new Vector3(0.5f, 0f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A���́A�E�i1, 0, 0.5�j
            /// </summary>
            public static Vector3 DefaultLowerRight { get { return new Vector3(1f, 0f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A�^�񒆂́A���i0, 0.5, 0.5�j
            /// </summary>
            public static Vector3 DefaultMiddleLeft { get { return new Vector3(0f, 0.5f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A�^�񒆂́A�^�񒆁i0.5, 0.5, 0.5�j
            /// </summary>
            public static Vector3 DefaultMiddleCenter { get { return new Vector3(0.5f, 0.5f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A�^�񒆂́A�E�i1, 0.5, 0.5�j
            /// </summary>
            public static Vector3 DefaultMiddleRight { get { return new Vector3(1f, 0.5f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A��́A���i0, 1, 0.5�j
            /// </summary>
            public static Vector3 DefaultUpperLeft { get { return new Vector3(0f, 1f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A��́A�^�񒆁i0.5, 1, 0.5�j
            /// </summary>
            public static Vector3 DefaultUpperCenter { get { return new Vector3(0.5f, 1f, 0.5f); } }
            /// <summary>
            /// ���ʂ́A��́A�E�i1, 1, 0.5�j
            /// </summary>
            public static Vector3 DefaultUpperRight { get { return new Vector3(1f, 1f, 0.5f); } }

            /// <summary>
            /// ��ʂ́A���́A���i0, 0, 1�j
            /// </summary>
            public static Vector3 BackLowerLeft { get { return new Vector3(0f, 0f, 1f); } }
            /// <summary>
            /// ��ʂ́A���́A�^�񒆁i0.5, 0, 1�j
            /// </summary>
            public static Vector3 BackLowerCenter { get { return new Vector3(0.5f, 0f, 1f); } }
            /// <summary>
            /// ��ʂ́A���́A�E�i1, 0, 1�j
            /// </summary>
            public static Vector3 BackLowerRight { get { return new Vector3(1f, 0f, 1f); } }
            /// <summary>
            /// ��ʂ́A�^�񒆂́A���i0, 0.5, 1�j
            /// </summary>
            public static Vector3 BackMiddleLeft { get { return new Vector3(0f, 0.5f, 1f); } }
            /// <summary>
            /// ��ʂ́A�^�񒆂́A�^�񒆁i0.5, 0.5, 1�j
            /// </summary>
            public static Vector3 BackMiddleCenter { get { return new Vector3(0.5f, 0.5f, 1f); } }
            /// <summary>
            /// ��ʂ́A�^�񒆂́A�E�i1, 0.5, 1�j
            /// </summary>
            public static Vector3 BackMiddleRight { get { return new Vector3(1f, 0.5f, 1f); } }
            /// <summary>
            /// ��ʂ́A��́A���i0, 1, 1�j
            /// </summary>
            public static Vector3 BackUpperLeft { get { return new Vector3(0f, 1f, 1f); } }
            /// <summary>
            /// ��ʂ́A��́A�^�񒆁i0.5, 1, 1�j
            /// </summary>
            public static Vector3 BackUpperCenter { get { return new Vector3(0.5f, 1f, 1f); } }
            /// <summary>
            /// ��ʂ́A��́A�E�i1, 1, 1�j
            /// </summary>
            public static Vector3 BackUpperRight { get { return new Vector3(1f, 1f, 1f); } }
        }

        /// <summary>
        /// �w�肵���A���J�[���Œ肵�āAlocalScale��ύX����
        /// </summary>
        /// <param name="argT"></param>
        /// <param name="argAnchor"></param>
        public static void ChengeLocalScaleFromAnchor(Transform argT, Vector3 anchor, Vector3 argScale)
        {
            // �ύX�O�̒l��ۑ�
            var _pScale = argT.localScale;

            // localScale�̕ύX
            argT.localScale = argScale;

            // ��_�����炳�Ȃ��悤�Ɉړ�
            var xGap = argScale.x - _pScale.x;
            var yGap = argScale.y - _pScale.y;
            var zGap = argScale.z - _pScale.z;
            var fixXRate = 0.5f - anchor.x;
            var fixYRate = 0.5f - anchor.y;
            var fixZRate = 0.5f - anchor.z;
            argT.localPosition += new Vector3(xGap * fixXRate, yGap * fixYRate, zGap * fixZRate);
        }
    }
}
