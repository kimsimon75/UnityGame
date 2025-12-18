using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OctagonPrismMesh : MonoBehaviour
{
    [Min(0.001f)] public float radius = 0.5f;   // 중심~꼭짓점 거리
    [Min(0.001f)] public float height = 1.0f;   // 기둥 높이
    [Range(3, 64)] public int sides = 8;        // 8이면 팔각형

    public bool generateOnAwake = true;

    void Awake()
    {
        if (generateOnAwake) Generate();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) Generate();
    }
#endif

    public void Generate()
    {
        var mf = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        mesh.name = $"Prism_{sides}";

        int n = Mathf.Max(3, sides);
        float halfH = height * 0.5f;

        // ===== Vertices (top n + bottom n) =====
        Vector3[] v = new Vector3[n * 2];
        Vector2[] uv = new Vector2[n * 2];

        for (int i = 0; i < n; i++)
        {
            float a = (Mathf.PI * 2f) * i / n;
            float x = Mathf.Cos(a) * radius;
            float z = Mathf.Sin(a) * radius;

            v[i]     = new Vector3(x, +halfH, z); // top
            v[i + n] = new Vector3(x, -halfH, z); // bottom

            // 대충 쓰는 UV(원형 투영 느낌)
            uv[i]     = new Vector2((x / radius + 1f) * 0.5f, (z / radius + 1f) * 0.5f);
            uv[i + n] = uv[i];
        }

        // ===== Triangles =====
        // top: fan around vertex 0
        // bottom: fan around vertex n (reverse winding)
        // sides: quad per edge => 2 triangles
        int topTriCount = (n - 2);
        int bottomTriCount = (n - 2);
        int sideTriCount = n * 2;

        int[] tris = new int[(topTriCount + bottomTriCount + sideTriCount) * 3];
        int t = 0;

        // Top cap (clockwise when looking from above? Unity uses clockwise as front by default depending; we'll rely on normals recalc)
        for (int i = 1; i < n - 1; i++)
        {
            tris[t++] = 0;
            tris[t++] = i;
            tris[t++] = i + 1;
        }

        // Bottom cap (reverse)
        for (int i = 1; i < n - 1; i++)
        {
            tris[t++] = n;           // bottom 0
            tris[t++] = n + i + 1;
            tris[t++] = n + i;
        }

        // Sides
        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            int topA = i;
            int topB = next;
            int botA = i + n;
            int botB = next + n;

            // tri 1
            tris[t++] = topA;
            tris[t++] = topB;
            tris[t++] = botB;

            // tri 2
            tris[t++] = topA;
            tris[t++] = botB;
            tris[t++] = botA;
        }
        
        for (int i = 0; i < tris.Length; i += 3)
        {
            (tris[i + 1], tris[i + 2]) = (tris[i + 2], tris[i + 1]); // winding 뒤집기
        }


        mesh.vertices = v;
        mesh.uv = uv;
        mesh.triangles = tris;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        mf.sharedMesh = mesh;
    }
}
