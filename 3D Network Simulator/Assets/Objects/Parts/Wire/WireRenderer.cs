using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wire
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WireRenderer : MonoBehaviour
    {
        [SerializeField] public Transform floor;
        [SerializeField] public float width = 0.03f;
        [SerializeField] public Transform p1;
        [SerializeField] public Transform p2;
        [SerializeField] private readonly int pCnt = 50;
        [SerializeField] public float sagging = 2;
        private Mesh _mesh;
        private Vector3 p1_prev = Vector3.negativeInfinity;
        private Vector3 p2_prev = Vector3.negativeInfinity;
        private readonly List<Vector3> points = new();
        private float p_sagging;

        // Start is called before the first frame update
        public void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        // Update is called once per frame
        public void Update()
        {
            GenPoints();
            Generate(points);
        }

        private void GenPoints()
        {
            if (Vector3.Distance(AbsolutePosition(p1), p1_prev) + Vector3.Distance(AbsolutePosition(p2), p2_prev) < 0.01f && p_sagging == sagging)
            {
                return;
            }

            p1_prev = AbsolutePosition(p1);
            p2_prev = AbsolutePosition(p2);
            p_sagging = sagging;

            points.Clear();

            for (int i = 0; i <= pCnt; i++)
            {
                var l = i / (float)pCnt;
                points.Add(CheckFloor(Vector3.Lerp(p1_prev, p2_prev, l) + (Vector3.down * CalcSogg(l))));
            }
        }

        private Vector3 AbsolutePosition(Transform p)
        {
            return p.position;
        }

        private Vector3 CheckFloor(Vector3 arg)
        {
            arg.y = Mathf.Max(arg.y, floor.position.y + width);

            return arg;
        }

        private float CalcSogg(float i)
        {
            return (0.25f * sagging) - ((i - 0.5f) * (i - 0.5f) * sagging);
        }

        private void Generate(List<Vector3> points)
        {
            var verticies = new Vector3[points.Count * 4];
            var indecies = new int[points.Count * 24];

            var v1 = new Vector3(0, 0, width);
            var v2 = new Vector3(0, width, 0);
            var v3 = new Vector3(0, 0, -width);
            var v4 = new Vector3(0, -width, 0);

            // Fill Verts
            int i = 0;
            Vector3 prev = points[0];
            foreach (var go in points)
            {
                var essenceRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), go - prev);

                verticies[i++] = go + (essenceRotation * v1);
                verticies[i++] = go + (essenceRotation * v2);
                verticies[i++] = go + (essenceRotation * v3);
                verticies[i++] = go + (essenceRotation * v4);

                prev = go;
            }

            // Fill indecies
            for (i = 0; i < (points.Count * 4) - 5; i++)
            {
                indecies[(i * 6) + 0] = i + 0;
                indecies[(i * 6) + 1] = i + 4;
                indecies[(i * 6) + 2] = i + 1;
                indecies[(i * 6) + 3] = i + 5;
                indecies[(i * 6) + 4] = i + 1;
                indecies[(i * 6) + 5] = i + 4;
            }

            _mesh.vertices = verticies;
            _mesh.triangles = indecies;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}