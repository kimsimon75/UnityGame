namespace RaycastPro.Detectors
{
    using System.Collections.Generic;
    using UnityEngine;
    using Sensor;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Detectors/" + nameof(AvatarDetector))]
    public sealed class AvatarDetector : Detector, IPulse
    {
        [Tooltip("Collider your sounds on the specified layer and assign a special Collider Detector to automatically feed the sounds to the filtering source.")]
        public ColliderDetector avatarFinder;

        [Tooltip("based on seconds")]
        public float changingRate = 1f;

        public int iteration = 1;

        public List<AvatarDefinition> avatarSensors = new List<AvatarDefinition>();
        public readonly Dictionary<AvatarDefinition, float> viewPercentage = new Dictionary<AvatarDefinition, float>();
        
        public override bool Performed
        {
            get => avatarFinder.DetectedColliders.Count > 0;
            protected set { }
        }
        
        
        public TimeMode timeMode = TimeMode.DeltaTime;

        #region Public Methods

        public bool IsDetectedPart(AvatarDefinition target, HumanBodyBones bone) => !Physics.Linecast(transform.position, target.animator.GetBoneTransform(bone).position, blockLayer, triggerInteraction);

        #endregion

        #region Temps

        private float dist, loudness;

        #endregion
        private void Start() // Refreshing
        {
            Sync();
        }
        /// <summary>
        /// Sound Finder will be sync on source
        /// </summary>
        public void Sync()
        {
            avatarFinder?.SyncDetection(avatarSensors, AS => viewPercentage.Add(AS, 0f), AS => viewPercentage.Remove(AS));
        }
        public void UnSync()
        {
            avatarFinder?.UnSyncDetection(avatarSensors);
        }

        
        protected override void OnCast()
        {
#if UNITY_EDITOR
            GizmoGate = PanelGate = null;
#endif
            
            foreach (var avatarSensor in avatarSensors)
            {
                if (!viewPercentage.ContainsKey(avatarSensor))
                {
                    viewPercentage.Add(avatarSensor, 0f);
                }

                for (int i = 0; i < iteration; i++)
                {
                    var index = UnityEngine.Random.Range(0, avatarSensor.bones.Count);

                    var blocked = Physics.Linecast(avatarFinder.transform.position,
                        (avatarSensor.bones)[index].position, out var hit, blockLayer.value, triggerInteraction);
                    //
                    viewPercentage[avatarSensor] =
                        Mathf.Lerp(viewPercentage[avatarSensor], blocked ? 0 : 1, GetDelta(timeMode)*changingRate);


#if UNITY_EDITOR
                    GizmoGate += () =>
                    {
                        if (avatarSensor && viewPercentage.ContainsKey(avatarSensor))
                        {
                            DrawBlockLine(transform.position, (avatarSensor.bones)[index].position, hit);
                            Handles.Label(avatarSensor.transform.position, viewPercentage[avatarSensor].ToString("P"));
                        }
                    };

                    PanelGate += () =>
                    {
                        BeginHorizontal();
                        PercentProgressField(viewPercentage[avatarSensor], avatarSensor.name);
                        EndHorizontal();
                    };
#endif
                }
            }
        }
#if UNITY_EDITOR
       internal override string Info => "Detect Avatar Definitions and feedback the view percentage."+ HAccurate + HIPulse+HDependent+HPreview;
        internal override void OnGizmos()
        {
            EditorUpdate();
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(avatarFinder)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(changingRate)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(iteration)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(blockLayer)));
                
                PropertyTimeModeField(_so.FindProperty(nameof(timeMode)));
            }

            if (hasGeneral)
            {
                GeneralField(_so);
                BaseField(_so);
            }

            if (hasEvents)
            {
                // EventField(_so);
                // if (EventFoldout) RCProEditor.EventField(_so, events);
            }
            if (hasInfo) InformationField(PanelGate);
        }
        protected override void DrawDetectorGuide(Vector3 point) { }
#endif
    }
}