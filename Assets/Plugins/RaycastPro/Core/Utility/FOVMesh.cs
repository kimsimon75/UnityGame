namespace RaycastPro.Sensor
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [RequireComponent(typeof(MeshCollider))]
    public sealed class FOVMesh : BaseUtility
    {
      [Tooltip("Length of the view cone.")]
        public float Length = 5f;

        [Tooltip("Base size of the cone.")]
        public float BaseSize = 0.5f;

        [Range(1f, 180f)]
        public float HorizontalFOV = 90f;

        [Range(1f, 180f)]
        public float VerticalFOV = 90f;

        [Range(3, 32)]
        public int SegmentCount = 8;
        
        Mesh fovMesh = new Mesh();
        public Mesh GetFOVMesh => fovMesh;


        public MeshCollider meshCollider = new MeshCollider();
        
        private void Reset()
        {
            // تلاش برای گرفتن MeshCollider
            meshCollider = GetComponent<MeshCollider>();

            // اگر وجود نداشت، خودش اضافه کند
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
                Debug.Log("MeshCollider was missing. A new one was added automatically to: " + gameObject.name);
            }
        }
// عرض مربع ابتدایی
public float BaseDepth = 0.001f;   // چقدر عقب‌تر از نقطه شروع قرار بگیرد

void GenerateFOVMeshCollider()
{
    // 4 vertex for base quad + 1 center + grid
    int baseVertexCount = 4;
    int baseTriangleCount = 2 * 3;

    int gridVertexCount = 1 + SegmentCount * SegmentCount;
    int gridTriangleCount = (SegmentCount - 1) * (SegmentCount - 1) * 2 * 3;

    int totalVertices = baseVertexCount + gridVertexCount;
    int totalTriangles = baseTriangleCount + gridTriangleCount;

    Vector3[] vertices = new Vector3[totalVertices];
    int[] triangles = new int[totalTriangles];

    // 🟩 Base quad setup (مربع در پشت دید)
    Vector3 baseDir = Quaternion.Euler(0, 0, 0) * Vector3.forward; // جهت وسط
    Vector3 baseCenter = -baseDir * BaseDepth;

    // بدست آوردن جهت‌های عمودی و افقی نسبت به جهت میانی
    Vector3 right = Quaternion.Euler(0, 90, 0) * baseDir;
    Vector3 up = Quaternion.Euler(-90, 0, 0) * baseDir;

    float half = BaseSize * 0.5f;
    vertices[0] = baseCenter + (-right - up) * half;
    vertices[1] = baseCenter + (right - up) * half;
    vertices[2] = baseCenter + (-right + up) * half;
    vertices[3] = baseCenter + (right + up) * half;

    // 🔺 دو مثلث پایه
    int tri = 0;
    triangles[tri++] = 0;
    triangles[tri++] = 2;
    triangles[tri++] = 1;

    triangles[tri++] = 1;
    triangles[tri++] = 2;
    triangles[tri++] = 3;

    // ⚙️ رأس مرکز مخروط
    int offset = baseVertexCount;
    vertices[offset] = Vector3.zero;

    int index = offset + 1;
    for (int y = 0; y < SegmentCount; y++)
    {
        float pitch = Mathf.Lerp(-VerticalFOV / 2f, VerticalFOV / 2f, y / (float)(SegmentCount - 1));

        for (int x = 0; x < SegmentCount; x++)
        {
            float yaw = Mathf.Lerp(-HorizontalFOV / 2f, HorizontalFOV / 2f, x / (float)(SegmentCount - 1));
            Vector3 dir = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;
            vertices[index++] = dir.normalized * Length;
        }
    }

    // ⛓️ مثلث‌های مش اصلی
    for (int y = 0; y < SegmentCount - 1; y++)
    {
        for (int x = 0; x < SegmentCount - 1; x++)
        {
            int i0 = offset + 1 + y * SegmentCount + x;
            int i1 = i0 + 1;
            int i2 = i0 + SegmentCount;
            int i3 = i2 + 1;

            triangles[tri++] = i0;
            triangles[tri++] = i2;
            triangles[tri++] = i1;

            triangles[tri++] = i1;
            triangles[tri++] = i2;
            triangles[tri++] = i3;
        }
    }

    // 📦 ساخت مش
    if (fovMesh == null)
        fovMesh = new Mesh { name = "FOV_Mesh" };
    else
        fovMesh.Clear();

    // بررسی محدودیت کانکس مش
    int triangleCount = triangles.Length / 3;
    
    if (triangleCount > 255)
    {
        Debug.LogWarning($"FOV Mesh has {triangleCount} triangles, which exceeds Unity's convex mesh limit. Reducing SegmentCount.");
    
        while (((SegmentCount - 1) * (SegmentCount - 1) * 2 + 2) > 255 && SegmentCount > 2)
            SegmentCount--;
    
        GenerateFOVMeshCollider(); // دوباره تولید کن
        return;
    }
    
    fovMesh.vertices = vertices;
    fovMesh.triangles = triangles;
    fovMesh.RecalculateNormals();
    fovMesh.RecalculateBounds();

    // جلوگیری از تجاوز محدودیت کانکس
    bool forceNonConvex = triangles.Length / 3 > 255;
    if (forceNonConvex)
        Debug.LogWarning($"FOV Mesh has too many triangles ({triangles.Length / 3}). Using non-convex collider.");
    
    meshCollider.sharedMesh = fovMesh;
    meshCollider.convex = !forceNonConvex;
    meshCollider.isTrigger = true;
    }




        public override bool Performed { get; protected set; }

        protected override void OnCast()
        {
            GenerateFOVMeshCollider();
        }

#if UNITY_EDITOR
        internal override string Info => "Auxiliary tool for creating dynamic view FOV Mesh. Best suitable for Mesh Detector" + HUtility + HDependent;
        internal override void OnGizmos()
        {
            if (IsSceneView) OnCast();
            
            if (fovMesh == null) return;
            Gizmos.color = Color.green;
            foreach (var v in fovMesh.vertices)
            {
                Gizmos.DrawSphere(transform.TransformPoint(v), 0.05f);
            }
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(meshCollider)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(Length)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(BaseSize)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(HorizontalFOV)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(VerticalFOV)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(SegmentCount)));
            }
            if (hasGeneral)
            {
                BaseField(_so);
            }
        }
#endif
    }
}