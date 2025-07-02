using UnityEngine;

namespace Assets.Scripts.Utils.Algorithm.ConvexHull
{
    public class Test_Slice : MonoBehaviour
    {
        public MeshFilter targetFilter;
        public Vector3 planeNormal = Vector3.one;
        public Vector3 planeOffset = new Vector3(0.1f, 0.2f, -0.1f);

        private void OnGUI()
        {
            if(GUI.Button(new Rect(20, 40, 80, 20), "Slice !"))
            {
                Plane plane = new Plane(planeNormal, planeOffset);
                MeshSlicer.Slice(targetFilter.mesh, plane, out Mesh lower, out Mesh upper);

                GameObject lowerGO = new GameObject("Lower");
                lowerGO.AddComponent<MeshFilter>().mesh = lower;
                lowerGO.AddComponent<MeshRenderer>().material = targetFilter.GetComponent<MeshRenderer>().sharedMaterial;

                GameObject upperGO = new GameObject("Upper");
                upperGO.AddComponent<MeshFilter>().mesh = upper;
                upperGO.AddComponent<MeshRenderer>().material = targetFilter.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
    }
}
