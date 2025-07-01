using UnityEngine;

namespace Assets.Scripts.Utils.Alrolithm.ConvexHull
{
    /// <summary>
    /// 두점 a, b 에 대한 선
    /// </summary>
    public struct Line
    {
        public Line(Vector3 a, Vector3 b)
        {
            this.A = a;
            this.B = b;
        }

        public readonly Vector3 A;
        public readonly Vector3 B;

        public float Length()
        {
            return Vector3.Distance(A, B);
        }
    }
}
