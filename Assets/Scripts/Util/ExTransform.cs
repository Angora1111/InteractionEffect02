using UnityEngine;

namespace AngoraUtility
{
    /// <summary>
    /// Transformのもつメソッドの拡張
    /// </summary>
    public static class ExTransform
    {
        // ***** EulerAngles 関係 ****************************************************************************************************************

        /// <summary>
        /// 座標軸
        /// </summary>
        public enum AXIS
        {
            X, Y, Z
        }

        /// <summary>
        /// 線形的になるように、補正した EulerAngles を取得する（対象のTransform, 基準とする軸, 基準値）
        /// </summary>
        /// <param name="argTransform"></param>
        /// <param name="argAxis"></param>
        /// <param name="startAngle"></param>
        /// <returns></returns>
        public static Vector3 GetFixEulerAngles(Transform argTransform, ExTransform.AXIS argAxis = AXIS.Z, float startAngle = 0f)
        {
            // 同じ rotation を表す EulerAngle を 2つ求める
            var eulerAngle_1 = argTransform.eulerAngles;
            var eulerAngle_2 = NormalizeEulerAngles(new Vector3(180 - eulerAngle_1.x, 180 + eulerAngle_1.y, 180 + eulerAngle_1.z));

            // 指定した基準を元に、線形的に変化するように EulerAngle を選ぶ
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

            // 軸が適切に設定されていなければ、エラーメッセージを表示する
            Debug.LogError("【ExMathf】軸が適切に設定されていません");
            return argTransform.eulerAngles;
        }

        /// <summary>
        /// 0 ～ 180, -180 ～ 0 に補正する
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

        // 角度を 0～360 に丸め込む
        private static Vector3 NormalizeEulerAngles(Vector3 argAngle)
        {
            Vector3 retAngle;
            retAngle.x = (argAngle.x + 360f) % 360;
            retAngle.y = (argAngle.y + 360f) % 360;
            retAngle.z = (argAngle.z + 360f) % 360;
            return retAngle;
        }

        // 角度の差の大きさを求める
        private static float DegDist(float argA, float argB)
        {
            // 0 ～ 360 に補正する
            float a = (argA + 360f) % 360;
            float b = (argB + 360f) % 360;

            // 差をもとめる
            var dist = Mathf.Abs(a - b);

            // 最短に補正する
            return dist > 180 ? 360 - dist : dist;
        }


        // ***** localScale 関係 ****************************************************************************************************************

        public static class Anchor
        {
            /// <summary>
            /// 前面の、下の、左（0, 0, 0）
            /// </summary>
            public static Vector3 ForwardLowerLeft { get { return new Vector3(0f, 0f, 0f); } }
            /// <summary>
            /// 前面の、下の、真ん中（0.5, 0, 0）
            /// </summary>
            public static Vector3 ForwardLowerCenter { get { return new Vector3(0.5f, 0f, 0f); } }
            /// <summary>
            /// 前面の、下の、右（1, 0, 0）
            /// </summary>
            public static Vector3 ForwardLowerRight { get { return new Vector3(1f, 0f, 0f); } }
            /// <summary>
            /// 前面の、真ん中の、左（0, 0.5, 0）
            /// </summary>
            public static Vector3 ForwardMiddleLeft { get { return new Vector3(0f, 0.5f, 0f); } }
            /// <summary>
            /// 前面の、真ん中の、真ん中（0.5, 0.5, 0）
            /// </summary>
            public static Vector3 ForwardMiddleCenter { get { return new Vector3(0.5f, 0.5f, 0f); } }
            /// <summary>
            /// 前面の、真ん中の、右（1, 0.5, 0）
            /// </summary>
            public static Vector3 ForwardMiddleRight { get { return new Vector3(1f, 0.5f, 0f); } }
            /// <summary>
            /// 前面の、上の、左（0, 1, 0）
            /// </summary>
            public static Vector3 ForwardUpperLeft { get { return new Vector3(0f, 1f, 0f); } }
            /// <summary>
            /// 前面の、上の、真ん中（0.5, 1, 0）
            /// </summary>
            public static Vector3 ForwardUpperCenter { get { return new Vector3(0.5f, 1f, 0f); } }
            /// <summary>
            /// 前面の、上の、右（1, 1, 0）
            /// </summary>
            public static Vector3 ForwardUpperRight { get { return new Vector3(1f, 1f, 0f); } }

            /// <summary>
            /// 中面の、下の、左（0, 0, 0.5）
            /// </summary>
            public static Vector3 DefaultLowerLeft { get { return new Vector3(0f, 0f, 0.5f); } }
            /// <summary>
            /// 中面の、下の、真ん中（0.5, 0, 0.5）
            /// </summary>
            public static Vector3 DefaultLowerCenter { get { return new Vector3(0.5f, 0f, 0.5f); } }
            /// <summary>
            /// 中面の、下の、右（1, 0, 0.5）
            /// </summary>
            public static Vector3 DefaultLowerRight { get { return new Vector3(1f, 0f, 0.5f); } }
            /// <summary>
            /// 中面の、真ん中の、左（0, 0.5, 0.5）
            /// </summary>
            public static Vector3 DefaultMiddleLeft { get { return new Vector3(0f, 0.5f, 0.5f); } }
            /// <summary>
            /// 中面の、真ん中の、真ん中（0.5, 0.5, 0.5）
            /// </summary>
            public static Vector3 DefaultMiddleCenter { get { return new Vector3(0.5f, 0.5f, 0.5f); } }
            /// <summary>
            /// 中面の、真ん中の、右（1, 0.5, 0.5）
            /// </summary>
            public static Vector3 DefaultMiddleRight { get { return new Vector3(1f, 0.5f, 0.5f); } }
            /// <summary>
            /// 中面の、上の、左（0, 1, 0.5）
            /// </summary>
            public static Vector3 DefaultUpperLeft { get { return new Vector3(0f, 1f, 0.5f); } }
            /// <summary>
            /// 中面の、上の、真ん中（0.5, 1, 0.5）
            /// </summary>
            public static Vector3 DefaultUpperCenter { get { return new Vector3(0.5f, 1f, 0.5f); } }
            /// <summary>
            /// 中面の、上の、右（1, 1, 0.5）
            /// </summary>
            public static Vector3 DefaultUpperRight { get { return new Vector3(1f, 1f, 0.5f); } }

            /// <summary>
            /// 後面の、下の、左（0, 0, 1）
            /// </summary>
            public static Vector3 BackLowerLeft { get { return new Vector3(0f, 0f, 1f); } }
            /// <summary>
            /// 後面の、下の、真ん中（0.5, 0, 1）
            /// </summary>
            public static Vector3 BackLowerCenter { get { return new Vector3(0.5f, 0f, 1f); } }
            /// <summary>
            /// 後面の、下の、右（1, 0, 1）
            /// </summary>
            public static Vector3 BackLowerRight { get { return new Vector3(1f, 0f, 1f); } }
            /// <summary>
            /// 後面の、真ん中の、左（0, 0.5, 1）
            /// </summary>
            public static Vector3 BackMiddleLeft { get { return new Vector3(0f, 0.5f, 1f); } }
            /// <summary>
            /// 後面の、真ん中の、真ん中（0.5, 0.5, 1）
            /// </summary>
            public static Vector3 BackMiddleCenter { get { return new Vector3(0.5f, 0.5f, 1f); } }
            /// <summary>
            /// 後面の、真ん中の、右（1, 0.5, 1）
            /// </summary>
            public static Vector3 BackMiddleRight { get { return new Vector3(1f, 0.5f, 1f); } }
            /// <summary>
            /// 後面の、上の、左（0, 1, 1）
            /// </summary>
            public static Vector3 BackUpperLeft { get { return new Vector3(0f, 1f, 1f); } }
            /// <summary>
            /// 後面の、上の、真ん中（0.5, 1, 1）
            /// </summary>
            public static Vector3 BackUpperCenter { get { return new Vector3(0.5f, 1f, 1f); } }
            /// <summary>
            /// 後面の、上の、右（1, 1, 1）
            /// </summary>
            public static Vector3 BackUpperRight { get { return new Vector3(1f, 1f, 1f); } }
        }

        /// <summary>
        /// 指定したアンカーを固定して、localScaleを変更する
        /// </summary>
        /// <param name="argT"></param>
        /// <param name="argAnchor"></param>
        public static void ChengeLocalScaleFromAnchor(Transform argT, Vector3 anchor, Vector3 argScale)
        {
            // 変更前の値を保存
            var _pScale = argT.localScale;

            // localScaleの変更
            argT.localScale = argScale;

            // 基準点をずらさないように移動
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
