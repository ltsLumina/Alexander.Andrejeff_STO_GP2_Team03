using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPolygonFill : Graphic
{
    
    public Vector2[] points;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Length == 3) return;

        for (int i = 0; i < points.Length; i++)
        {
            vh.AddVert(points[i], color, Vector2.zero);
        }

        for (int i = 1; i < points.Length; i++)
        {
            vh.AddTriangle(0,i,i+1);
        }

    }
    
    
}


public static class Triangulator
{
    public static int[] Triangulate(Vector2[] points)
    {
        List<int> indices = new List<int>();

        int n = points.Length;
        if (n < 3) return indices.ToArray();

        int[] V = new int[n];
        if (Area(points) > 0)
            for (int i = 0; i < n; i++) V[i] = i;
        else
            for (int i = 0; i < n; i++) V[i] = (n - 1) - i;

        int nv = n;
        int count = 2 * nv;

        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0) break;

            int u = v;
            if (nv <= u) u = 0;
            v = u + 1;
            if (nv <= v) v = 0;
            int w = v + 1;
            if (nv <= w) w = 0;

            if (Snip(points, u, v, w, nv, V))
            {
                indices.Add(V[u]);
                indices.Add(V[v]);
                indices.Add(V[w]);

                for (int s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        return indices.ToArray();
    }

    static float Area(Vector2[] pts)
    {
        float A = 0;
        for (int p = pts.Length - 1, q = 0; q < pts.Length; p = q++)
            A += pts[p].x * pts[q].y - pts[q].x * pts[p].y;
        return A * 0.5f;
    }

    static bool Snip(Vector2[] pts, int u, int v, int w, int n, int[] V)
    {
        Vector2 A = pts[V[u]];
        Vector2 B = pts[V[v]];
        Vector2 C = pts[V[w]];

        if (Mathf.Epsilon > (((B.x - A.x)*(C.y - A.y)) - ((B.y - A.y)*(C.x - A.x))))
            return false;

        for (int p = 0; p < n; p++)
        {
            if (p == u || p == v || p == w) continue;
            Vector2 P = pts[V[p]];
            if (PointInTriangle(A, B, C, P)) return false;
        }
        return true;
    }

    static bool PointInTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax = C.x - B.x, ay = C.y - B.y;
        float bx = A.x - C.x, by = A.y - C.y;
        float cx = B.x - A.x, cy = B.y - A.y;
        float apx = P.x - A.x, apy = P.y - A.y;
        float bpx = P.x - B.x, bpy = P.y - B.y;
        float cpx = P.x - C.x, cpy = P.y - C.y;

        float a = ax * apy - ay * apx;
        float b = bx * bpy - by * bpx;
        float c = cx * cpy - cy * cpx;
        return (a >= 0f) && (b >= 0f) && (c >= 0f);
    }
}


