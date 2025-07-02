using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utils.Algorithm.ConvexHull
{
    public static class MeshSlicer
    {
        const float EPS = 0.00001f; // Plane 에서 위/아래로 떨어져있음을 판단하기위한 최소 한계

        public static void Slice(Mesh mesh, Plane plane, out Mesh lower, out Mesh upper)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<Vector3> lowerVertices = new List<Vector3>();
            List<Vector3> upperVertices = new List<Vector3>();
            List<int> lowerTriangles = new List<int>();
            List<int> upperTriangles = new List<int>();

            List<Vector3> sectionPoints = new List<Vector3>(); // 절단면 교차점

            // 트라이앵글 단위순회
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                Vector3 p1 = vertices[i1];
                Vector3 p2 = vertices[i2];
                Vector3 p3 = vertices[i3];

                float d1 = SignedDistance(p1, plane);
                float d2 = SignedDistance(p2, plane);
                float d3 = SignedDistance(p3, plane);

                bool up1 = d1 >= EPS;
                bool up2 = d2 >= EPS;
                bool up3 = d3 >= EPS;
                bool down1 = d1 <= -EPS;
                bool down2 = d2 <= -EPS;
                bool down3 = d3 <= -EPS;

                if (up1 && up2 & up3)
                {
                    upperTriangles.Add(i1);
                    upperTriangles.Add(i2);
                    upperTriangles.Add(i3);
                    continue;
                }

                if (down1 && down2 & down3)
                {
                    lowerTriangles.Add(i1);
                    lowerTriangles.Add(i2);
                    lowerTriangles.Add(i3);
                    continue;
                }

                Clip(p1, p2, p3, true, d1, d2, d3, plane, upperVertices, upperTriangles, sectionPoints); // 위에꺼
                Clip(p1, p2, p3, false, d1, d2, d3, plane, lowerVertices, lowerTriangles, sectionPoints); // 아래꺼
            }

            // 단면이 존재할 경우
            if (sectionPoints.Count >= 3)
            {
                List<Point2DMapped> point2dMapped = sectionPoints.Select(p => new Point2DMapped(p, plane)).ToList();
                List<Point2DMapped> hull = ConvexHullCalculator.MonotoneChain(point2dMapped);
                TriangulateCap(hull, true, upperVertices, upperTriangles);
                TriangulateCap(hull, false, lowerVertices, lowerTriangles);
            }

            upper = new Mesh();
            upper.SetVertices(upperVertices);
            upper.SetTriangles(upperTriangles, 0);
            upper.RecalculateNormals();

            lower = new Mesh();
            lower.SetVertices(lowerVertices);
            lower.SetTriangles(lowerTriangles, 0);
            lower.RecalculateNormals();
        }

        static void TriangulateCap(List<Point2DMapped> hull, bool isUpper, List<Vector3> targetVertices, List<int> targetTriangles)
        {
            if(hull.Count < 3)
            {
                return;
            }

            int i0 = 0;

            for(int i = 1; i < hull.Count - 1; i++)
            {
                if (isUpper)
                {
                    targetVertices.Add(hull[i0].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                    targetVertices.Add(hull[i].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                    targetVertices.Add(hull[i + 1].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                }
                else
                {
                    targetVertices.Add(hull[i0].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                    targetVertices.Add(hull[i + 1].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                    targetVertices.Add(hull[i].point);
                    targetTriangles.Add(targetVertices.Count - 1);
                }
            }
        }

        static float SignedDistance(Vector3 point, Plane plane)
        {
            return Vector3.Dot(plane.normal, point) + plane.distance;
        }

        /// <summary>
        /// 평면의 교차하는 삼각형을 자르는 함수
        /// TODO : Vertices 메모리 최적화 (이미 사용하던 vertex 를 다른 삼가형에서 쓸때 
        ///                               Vertices 배열 늘리지 말고 원래쓰던 위치를 가리키게 해야함)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="isUpper"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="plane"></param>
        /// <param name="outTrianglesVertices"></param>
        /// <param name="outTriangles"></param>
        /// <exception cref="Exception"></exception>
        static void Clip(Vector3 p1, Vector3 p2, Vector3 p3,
                         bool isUpper,
                         float d1, float d2, float d3,
                         Plane plane,
                         List<Vector3> outTrianglesVertices,
                         List<int> outTriangles,
                         List<Vector3> outSectionVertices)
        {
            // 잘리고 남은 다각형
            // 점이 3개 혹은 4개가 될수 있음
            List<Vector3> polygon = new List<Vector3>();
            List<Vector3> sectionVertices = new List<Vector3>();

            bool a = isUpper ? d1 >= EPS : d1 <= -EPS;
            bool b = isUpper ? d2 >= EPS : d2 <= -EPS;
            bool c = isUpper ? d3 >= EPS : d3 <= -EPS;

            // edge p1p2
            if (a)
            {
                polygon.Add(p1); // p1 이 평면 위에 있으면 추가
            }
            if (a ^ b)
            {
                polygon.Add(Intersect(p1, p2, d1, d2, plane, sectionVertices)); // p1과 p2 가 평면 기준 반대방향일때 교점 추가
            }

            // edge p2p3
            if (b)
            {
                polygon.Add(p2);
            }
            if (b ^ c)
            {
                polygon.Add(Intersect(p2, p3, d2, d3, plane, sectionVertices));
            }

            // edge p1p3
            if (c)
            {
                polygon.Add(p3);
            }
            if (c ^ a)
            {
                polygon.Add(Intersect(p3, p1, d3, d1, plane, sectionVertices));
            }

            // 잘린부분이 면을 형성하지 못했을 경우
            if (polygon.Count < 3)
            {
                return;
            }

            // 1. 현재 버텍스 시작 인덱스 저장
            int baseIndex = outTrianglesVertices.Count;

            // 2. 버텍스 추가
            for (int i = 0; i < polygon.Count; i++)
                outTrianglesVertices.Add(polygon[i]);

            // 3. 삼각형 인덱스 추가
            if (polygon.Count == 3)
            {
                // 노멀 방향 유지 여부에 따라 순서 결정
                if (isUpper)
                {
                    outTriangles.Add(baseIndex);
                    outTriangles.Add(baseIndex + 1);
                    outTriangles.Add(baseIndex + 2);
                }
                else
                {
                    outTriangles.Add(baseIndex + 2);
                    outTriangles.Add(baseIndex + 1);
                    outTriangles.Add(baseIndex);
                }
            }
            else if (polygon.Count == 4)
            {
                if (isUpper)
                {
                    // 삼각형 1
                    outTriangles.Add(baseIndex);
                    outTriangles.Add(baseIndex + 1);
                    outTriangles.Add(baseIndex + 2);

                    // 삼각형 2
                    outTriangles.Add(baseIndex);
                    outTriangles.Add(baseIndex + 2);
                    outTriangles.Add(baseIndex + 3);
                }
                else
                {
                    // 아래쪽 삼각형은 뒤집어서 추가
                    outTriangles.Add(baseIndex + 2);
                    outTriangles.Add(baseIndex + 1);
                    outTriangles.Add(baseIndex);

                    outTriangles.Add(baseIndex + 3);
                    outTriangles.Add(baseIndex + 2);
                    outTriangles.Add(baseIndex);
                }
            }

            outSectionVertices.AddRange(sectionVertices);
        }

        static Vector3 Intersect(Vector3 p1, Vector3 p2, float d1, float d2, Plane plane, List<Vector3> sectionVertices)
        {
            float t = d1 / (d1 - d2);
            Vector3 result = p1 + t * (p2 - p1);
            sectionVertices.Add(result);
            return result;
        }
    }
}