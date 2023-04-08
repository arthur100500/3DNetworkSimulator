using System;
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
        [SerializeField] public float sagging = 2;
        [SerializeField] private int pCnt = 50;

        private readonly List<Vector3> _points = new();
        private Mesh _mesh;
        private Vector3 _p1Prev = Vector3.negativeInfinity;
        private Vector3 _p2Prev = Vector3.negativeInfinity;
        private float _pSagging;

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
            Generate(_points);
        }

        private void GenPoints()
        {
            if (Vector3.Distance(AbsolutePosition(p1), _p1Prev) + Vector3.Distance(AbsolutePosition(p2), _p2Prev) <
                0.01f && Math.Abs(_pSagging - sagging) < 0.001f) return;

            _p1Prev = AbsolutePosition(p1);
            _p2Prev = AbsolutePosition(p2);
            _pSagging = sagging;

            _points.Clear();

            for (var i = 0; i <= pCnt; i++)
            {
                var l = i / (float)pCnt;
                _points.Add(CheckFloor(Vector3.Lerp(_p1Prev, _p2Prev, l) + Vector3.down * CalcSagging(l)));
            }
        }

        private static Vector3 AbsolutePosition(Transform p)
        {
            return p.position;
        }

        private Vector3 CheckFloor(Vector3 arg)
        {
            arg.y = Mathf.Max(arg.y, floor.position.y + width);

            return arg;
        }

        private float CalcSagging(float i)
        {
            return 0.25f * sagging - (i - 0.5f) * (i - 0.5f) * sagging;
        }

        private void Generate(List<Vector3> points)
        {
            var vertices = new Vector3[points.Count * 4];
            var indices = new int[points.Count * 24];

            var v1 = new Vector3(0, 0, width);
            var v2 = new Vector3(0, width, 0);
            var v3 = new Vector3(0, 0, -width);
            var v4 = new Vector3(0, -width, 0);

            // Fill Verts
            var i = 0;
            var prev = points[0];
            foreach (var go in points)
            {
                var essenceRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), go - prev);

                vertices[i++] = go + essenceRotation * v1;
                vertices[i++] = go + essenceRotation * v2;
                vertices[i++] = go + essenceRotation * v3;
                vertices[i++] = go + essenceRotation * v4;

                prev = go;
            }

            // Fill indecies
            for (i = 0; i < points.Count * 4 - 5; i++)
            {
                indices[i * 6 + 0] = i + 0;
                indices[i * 6 + 1] = i + 4;
                indices[i * 6 + 2] = i + 1;
                indices[i * 6 + 3] = i + 5;
                indices[i * 6 + 4] = i + 1;
                indices[i * 6 + 5] = i + 4;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = indices;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}