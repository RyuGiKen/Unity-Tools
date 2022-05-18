using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RyuGiKen;
namespace RyuGiKen.Tools
{
    /// <summary>
    /// 简易IK
    /// </summary>
    public class SimpleIK : MonoBehaviour
    {
        public bool LookAt;
        [SerializeField] Bone[] bones;
        [SerializeField] Transform target;

        Vector3 IKPosition;
        //public int maxIterations = 1;

        protected virtual Vector3 localDirection
        {
            get
            {
                return bones[0].transform.InverseTransformDirection(bones[bones.Length - 1].transform.position - bones[0].transform.position);
            }
        }
        void Start()
        {
            if (bones == null || (bones != null && bones.Length < 1))
            {
                Transform temp = this.transform;
                List<Bone> items = new List<Bone>();
                while (true)
                {
                    items.Add(new Bone(temp));
                    if (temp.childCount > 0)
                        temp = temp.GetChild(0);
                    else
                        break;
                }
                bones = items.ToArray();
            }
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].length = bones.Length - 1 - i;
            }
        }
        void Update()
        {
            if (target != null)
                IKPosition = target.position;

            /*Vector3 singularityOffset = maxIterations > 1 ? GetSingularityOffset() : Vector3.zero;
            for (int i = 0; i < maxIterations; i++)
            {
                if (singularityOffset == Vector3.zero && i >= 1 && tolerance > 0 && positionOffset < tolerance * tolerance)
                    break;
                Solve(IKPosition + (i == 0 ? singularityOffset : Vector3.zero));
            }*/
            if (bones.Length >= 2)
            {
                Solve(IKPosition);
                if (LookAt)
                {
                    bones[bones.Length - 1].transform.localPosition = Vector3.forward * (IKPosition - bones[bones.Length - 2].transform.position).magnitude;
                }
            }
        }
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Gizmos.color = Color.cyan;
            for (int i = 1; i < bones.Length; i++)
            {
                Gizmos.DrawLine(bones[i].transform.position, bones[i - 1].transform.position);
            }
            Gizmos.color = Color.green;
            for (int i = 0; i < bones.Length; i++)
            {
                Gizmos.DrawWireSphere(bones[i].transform.position, 0.02f);
            }
#endif
        }
        [System.Serializable]
        public struct Bone
        {
            public Transform transform;
            public float _weight;
            public float weight
            {
                get
                {
                    return _weight.Clamp(0, 1);
                }
                set
                {
                    _weight = value.Clamp(0, 1);
                }
            }
            public int length;
            public Bone(Transform transform, float weight = 1, int length = 0)
            {
                this.transform = transform;
                this._weight = weight.Clamp(0, 1);
                this.length = length;
            }
        }
        void Solve(Vector3 targetPosition)
        {
            for (int i = bones.Length - 2; i > -1; i--)
            {
                float w = bones[i].weight;

                if (w > 0f)
                {
                    Vector3 toLastBone = bones[bones.Length - 1].transform.position - bones[i].transform.position;
                    Vector3 toTarget = targetPosition - bones[i].transform.position;

                    Quaternion targetRotation = Quaternion.FromToRotation(toLastBone, toTarget) * bones[i].transform.rotation;

                    if (w >= 1)
                        bones[i].transform.rotation = targetRotation;
                    else
                        bones[i].transform.rotation = Quaternion.Lerp(bones[i].transform.rotation, targetRotation, w);
                }
            }
        }
    }
}
