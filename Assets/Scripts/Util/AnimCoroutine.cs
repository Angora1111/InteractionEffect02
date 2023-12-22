using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngoraUtility
{
    public class AnimCoroutine
    {
        /// <summary>
        /// t �����߂�֐��̃^�C�v
        /// </summary>
        public enum TFunctionType
        {
            Liner,
            FastToSlow,
            SlowToFast,
            Parabola,
            NeedleCurve
        }

        /// <summary>
        /// �A�j���[�V�����̓��e����������f���Q�[�g
        /// </summary>
        /// <param name="t"></param>
        public delegate void AnimFunc(float t);
        /// <summary>
        /// t �̒l�����߂�֐�����������f���Q�[�g
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public delegate float TFunc(float rate);

        // �o�ߎ���
        private float timer;

        // ���s����
        private float waitTime;

        // �A�j���[�V�������e�̃f���Q�[�g
        private AnimFunc animFunc;

        // �J�X�^���p�� t �Z�o�p�̃f���Q�[�g
        private TFunc tfunc;

        // �O���ǂݎ��p�̃p�����[�^
        private List<(int, float)> log;
        public string GetLogAsText
        {
            get
            {
                if (log.Count == 0) return "empty";
                string retVal = string.Empty;
                log.ForEach((l) => retVal += $"{l.Item1}\t{l.Item2}\r\n");
                return retVal;
            }
        }

        // �R���X�g���N�^
        public AnimCoroutine(float waitTime, TFunctionType type, AnimFunc animFunc, TFunc tfunc = null)
        {
            // �����ɏ]���Đݒ肷��
            this.timer = 0;
            this.waitTime = waitTime;
            var newTFunc = tfunc;
            if (newTFunc == null) newTFunc = GetTFuncFromType(type);
            this.tfunc = newTFunc;
            this.animFunc = animFunc;
        }
        public AnimCoroutine(float waitTime, AnimationCurve tCurve, AnimFunc animFunc)
        {
            // �����ɏ]���Đݒ肷��
            this.timer = 0;
            this.waitTime = waitTime;
            this.tfunc = (rate) =>
            {
                return tCurve.Evaluate(rate);
            };
            this.animFunc = animFunc;
        }

        private TFunc GetTFuncFromType(TFunctionType type)
        {
            TFunc newTfunc = null;
            switch (type)
            {
                case TFunctionType.Liner:
                    newTfunc = (rate) =>
                    {
                        rate = Mathf.Clamp01(rate);
                        return rate;
                    };
                    break;
                case TFunctionType.FastToSlow:
                    newTfunc = (rate) =>
                    {
                        rate = Mathf.Clamp01(rate);
                        return Mathf.Sin(rate * Mathf.PI * 0.5f);
                    };
                    break;
                case TFunctionType.SlowToFast:
                    newTfunc = (rate) =>
                    {
                        rate = Mathf.Clamp01(rate);
                        return 1f - Mathf.Cos(rate * Mathf.PI * 0.5f);
                    };
                    break;
                case TFunctionType.Parabola:
                    newTfunc = (rate) =>
                    {
                        rate = Mathf.Clamp01(rate);
                        return Mathf.Sin(rate * Mathf.PI);
                    };
                    break;
                case TFunctionType.NeedleCurve:
                    newTfunc = (rate) =>
                    {
                        rate = Mathf.Clamp01(rate);
                        return 1f - Mathf.Abs(Mathf.Cos(rate * Mathf.PI));
                    };
                    break;
            }
            return newTfunc;
        }

        /// <summary>
        /// �R���[�`��
        /// </summary>
        /// <returns></returns>
        public IEnumerator Anim()
        {
            while (timer <= waitTime)
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                timer += Time.deltaTime;
                float t = tfunc(timer / waitTime);
                if (t <= 1f) animFunc(t);

                yield return new WaitForEndOfFrame();

                sw.Stop();
                if (log == null) log = new List<(int, float)>();
                log.Add((log.Count + 1, sw.ElapsedMilliseconds));
            }
        }
    }
}
