using UnityEngine;

namespace Assets.Scripts.Utils.Alrolithm.ConvexHull
{
    /// <summary>
    /// ���� a, b �� ���� ��
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
