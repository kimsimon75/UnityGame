using UnityEngine.Serialization;

namespace RaycastPro.RaySensors
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(NoiseRay))]
    public sealed class NoiseRay : RaySensor, IPulse
    {
        private Vector3 currentDirection;
        private Vector2 random;
        
        [Tooltip("It will pulse (change ray direction) before casting the ray, suggest to off pulse time for using it.")]
        public bool pulseWhenCast;
        public bool onUnitCircle;
        
        public Vector2 noiseRange = new Vector2(.2f, .2f);

        [Tooltip("Automatic refresh ray changing direction Time.")]
        public float pulse = .4f;
        private float currentTime;
        
        public VectorEvent onPulse;

        private void Reset()
        {
            OnPulse();
        }

        private void OnEnable()
        {
            OnPulse();
        }

        public void OnPulse()
        {
            random = onUnitCircle ? Random.insideUnitCircle.normalized : Random.insideUnitCircle;
            
            random.x *= noiseRange.x;
            random.y *= noiseRange.y;
            
            if (local)
            {
                currentDirection = LocalDirection + transform.TransformDirection(random);
            }
            else
            {
                currentDirection = new Vector3(direction.x + random.x, direction.y + random.y, direction.z);
            }
            onPulse?.Invoke(currentDirection);
        }
        protected override void OnCast()
        {
            
            if (pulse > 0)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= pulse)
                {
                    currentTime = 0;
                    OnPulse();
                }
            }

            if (pulseWhenCast)
            {
                OnPulse();
            }
            
            // Cone Shape
            Physics.Linecast(transform.position, transform.position + currentDirection, out hit, detectLayer.value, triggerInteraction);
        }

#if UNITY_EDITOR
        internal override string Info =>   "Periodically Emit rays randomly aside the certain oval area." + HIPulse + HAccurate + HDirectional;

        private Vector3 _p;
        internal override void OnGizmos()
        {
            EditorUpdate();
            _p = transform.position;
            GizmoColor = Performed ? DetectColor : DefaultColor;
            
            DrawLineZTest(_p, _p + Direction, true, HelperColor);
            
            Gizmos.color = HelperColor;
            DrawEllipse(transform.position + Direction, Direction, transform.up, noiseRange.x, noiseRange.y);

            
            if (IsManuelMode)
            {
                Gizmos.DrawRay(transform.position, currentDirection);
            }
            else
            {
                DrawBlockLine(_p, _p + currentDirection, hit);
            }

            DrawNormal(hit);
        }

        private static readonly string[] eventsName = new string[]
            {"onDetect", "onPulse", "onBeginDetect", "onEndDetect", "onChange", "onCast"};
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(onUnitCircle)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(pulseWhenCast)));
                BeginHorizontal();
                
                PropertyMaxField(_so.FindProperty(nameof(pulse)), -1);
                if (GUILayout.Button("Pulse", GUILayout.Width(60))) OnPulse();
                EndHorizontal();
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(noiseRange)));
                

            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo) InformationField();

        }
#endif
        


        public override Vector3 Tip => transform.position + currentDirection;
        public override Vector3 RawTip => transform.position + currentDirection;
        public override float RayLength => direction.magnitude;
        public override Vector3 Base => transform.position;
    }
}