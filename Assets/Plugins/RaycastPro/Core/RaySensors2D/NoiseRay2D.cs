using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace RaycastPro.RaySensors2D
{
    [AddComponentMenu("RaycastPro/Rey Sensors/"+nameof(NoiseRay2D))]
    public sealed class NoiseRay2D : RaySensor2D
    {
        private Vector3 currentDirection;
        
        private float random;
        
        [Tooltip("radius of Noise")]
        public float noiseRange;

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

        private void OnPulse()
        {
            random = Random.Range(-noiseRange/2, noiseRange/2);
            
            currentDirection = direction;
            currentDirection.y += random;
            if (local)
            {
                currentDirection = transform.TransformDirection(currentDirection);
            }
            onPulse?.Invoke(currentDirection);
        }
        protected override void OnCast()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= pulse)
            {
                currentTime = 0;
                OnPulse();
            }
            hit = Physics2D.Linecast(transform.position, transform.position + currentDirection, detectLayer.value, MinDepth, MaxDepth);
            isDetect = FilterCheck(hit);
        }

#if UNITY_EDITOR
        internal override string Info => "Periodically Emit rays randomly aside the certain oval area."+ HIPulse + HAccurate + HDirectional;

        private Vector3 p1, p2;
        
        private static readonly string[] eventsName = new string[]
            {"onDetect", "onPulse", "onBeginDetect", "onEndDetect", "onChange", "onCast"};
        
        internal override void OnGizmos()
        {
            EditorUpdate();

            p1 = transform.position;
            p2 = transform.position + currentDirection.ToDepth();
            if (IsManuelMode)
            {
                DrawLineZTest(p1, p2, false, DefaultColor.Alpha(ClampedAlphaCharge));
            }
            else
            {
                DrawBlockLine(p1, p2, hit, z, 1);
            }
            DrawNormal2D(hit, z);
            DrawDepthLine(p1, p2);
            
            //DrawDepthLine(, HelperColor);
            DrawNormalFilter();
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true, bool hasEvents = true, bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                BeginHorizontal();
                PropertyMaxField(_so.FindProperty(nameof(pulse)));
                if (GUILayout.Button("Pulse", GUILayout.Width(60))) OnPulse();
                EndHorizontal();
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(noiseRange)));
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so, eventsName);
            if (hasInfo) HitInformationField();
        }
#endif
        public override Vector3 Tip => transform.position + currentDirection.ToDepth();
        
        public override Vector3 RawTip => transform.position + currentDirection.ToDepth();
        public override float RayLength => direction.magnitude;
        public override Vector3 Base => transform.position;
    }
}