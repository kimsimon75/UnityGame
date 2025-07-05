namespace RaycastPro.Detectors
{
    using UnityEngine;
    using System;
    using System.Linq;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    [RequireComponent(typeof(MeshCollider))]
    [AddComponentMenu("RaycastPro/Detectors/" + nameof(MeshDetector))]
    public sealed class MeshDetector: ColliderDetector, IPulse
    {
        [SerializeField] private MeshCollider meshCollider;

        [SerializeField] private bool limited;
        [SerializeField] private int limitCount = 3;

        public Vector3 weightPoint;

        [Tooltip("Forces to check nearest and furthest point accurately. [It will get more performance cost]")]
        public bool accurate = false;

        public LayerMask raycastMask;
        public Vector3 WeightPoint => transform.position + weightPoint;

        public bool Limited
        {
            get => limited;
            set
            {
                limited = value;
                if (value)
                {
                    colliders = new Collider[limitCount];
                }
            }
        }

        public int LimitCount
        {
            get => limitCount;
            set
            {
                limitCount = Mathf.Max(0, value);
                colliders = new Collider[limitCount];
            }
        }

        #region Temps
        private Vector3 _h;
        private float m;
        private float cylinderH;
        private Vector3 h;
        private Vector3 _dir;
        #endregion

        private SphereCollider miniSphere;
        private void Reset()
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        private float _vDis;
        private Vector3 _vDir;
        
        // Example method to draw the convex mesh wire gizmo

        private RaycastHit _hit, _hitA, _hitR;
        private int RaycastCounter(Vector3 point, Vector3 to, int hitCount = 0)
        {
            void Pass()
            {
                //Debug.DrawRay(_hitA.point, _hitA.normal, Color.gray, 1f);
                if (_hitA.transform == transform) hitCount += 1;
            }
            //Debug.DrawLine(point, to, Color.cyan, 1f);
            
            if (Physics.Linecast(point, to, out _hitA, raycastMask, QueryTriggerInteraction.Collide))
            {
                Pass();
                return RaycastCounter(_hitA.point+(to-point).normalized*.01f, to, hitCount);
            }
            return hitCount;
        }

        private bool CheckMeshPass(Vector3 point) => RaycastCounter(point, WeightPoint) % 2 == 0;
        protected override void OnCast()
        {
            CachePrevious();
#if UNITY_EDITOR
            CleanGate();
#endif
            if (limited)
            {
                Array.Clear(colliders, 0, colliders.Length);
                Physics.OverlapBoxNonAlloc(meshCollider.bounds.center, meshCollider.bounds.extents, colliders, transform.rotation, detectLayer.value, triggerInteraction);
            }
            else
            {
                colliders = Physics.OverlapBox(meshCollider.bounds.center, meshCollider.bounds.extents, transform.rotation, detectLayer.value, triggerInteraction);
            }
            
            Clear();
            
            var _t = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            if (IsIgnoreSolver)
            {
                foreach (var c in colliders)
                {
                    
                    if (c.transform != transform && TagPass(c))
                    {
                        if (accurate)
                        {
                            var _ps = GetNearAndFurthestPoint(c, WeightPoint);
                            if (CheckMeshPass(_ps.near) || CheckMeshPass(_ps.far))
                            {
#if UNITY_EDITOR
                                PassColliderGate(c);
#endif
                                DetectedColliders.Add(c);
                            }
                        }
                        else
                        {
                            if (CheckMeshPass(c.bounds.center))
                            {
#if UNITY_EDITOR
                                PassColliderGate(c);
#endif
                                DetectedColliders.Add(c);
                            }
                        }

                    }
                }
            }
            else
            {

                foreach (var c in colliders)
                {
                    if (c.transform == transform || !TagPass(c)) continue;
                    TDP = DetectFunction(c); // 1: Get Detect Point
                    if (CheckMeshPass(TDP) && LOSPass(TDP, c))
                    {
                        DetectedColliders.Add(c);
                    }
                    
                }
            }
            Physics.queriesHitBackfaces = _t;
            
            EventPass();
        }
#if UNITY_EDITOR
        internal override string Info => "The ability to detect points in the dominant convex mesh." + HDependent + HCDetector + HLOS_Solver + HRotatable + HScalable + HINonAllocator;
        internal override void OnGizmos()
        {
            EditorUpdate();
            GizmoColor = Performed ? DetectColor : DefaultColor;
            
            if (meshCollider)
            {
                Gizmos.DrawWireMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
                Handles.color = HelperColor;
                DrawLine(transform.position, WeightPoint, true);
                Gizmos.DrawSphere(WeightPoint, DiscSize);
                Handles.Label(WeightPoint, "<color=F2CD60>Weight Point</color>", RCProEditor.LabelStyle);
            }
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true, bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(meshCollider)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(raycastMask)));
                BeginHorizontal();
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(weightPoint)));
                GUI.enabled = solverType == SolverType.Ignore;
                MiniField(_so.FindProperty(nameof(accurate)), "A".ToContent("Accurate: Forces to check nearest and furthest side of collider for more performance cost."));

                GUI.enabled = true;
                EndHorizontal();                


            }
            if (hasGeneral) ColliderDetectorGeneralField(_so);

            if (hasEvents)
            {
                EventField(_so); if (EventFoldout) RCProEditor.EventField(_so, CEventNames);
            }

            if (hasInfo) InformationField(PanelGate);
        }

        private Vector3 direct, cross;
        private float distance;
        private ISceneGUI _sceneGUIImplementation;
        protected override void DrawDetectorGuide(Vector3 point)
        {
        }
#endif

    }
}