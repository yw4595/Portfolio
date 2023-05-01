/// <summary>
/// Flight system. This script is Core plane system
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace AirStrikeKit
{
    // included all necessary component
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]



    public class TFighterSystem : MonoBehaviour
    {

        public float AccelerationSpeed = 0.5f;
        // acceleration speed per frame
        public float Speed = 500.0f;
        // Speed
        public float SpeedMax = 60.0f;
        // Max speed
        public float RotationSpeed = 0f;
        // Turn Speed
        public float SpeedTakeoff = 40;
        // Min speed to take off
        public float SpeedPitch = 2;
        // rotation X
        public float SpeedRoll = 3;
        // rotation Z
        public float SpeedYaw = 1;
        // rotation Y
        public float DampingTarget = 10.0f;
        // rotation speed to facing to a target
        public bool AutoPilot = false;
        // if True this plane will follow a target automatically
        private float MoveSpeed = 0;
        // normal move speed
        public float VelocitySpeed = 0;

        [HideInInspector]
        public bool SimpleControl = false;
        // set true is enabled casual controling
        [HideInInspector]
        public bool FollowTarget = false;
        [HideInInspector]
        public Vector3 PositionTarget = Vector3.zero;
        // current target position

        // weapon system
        private Vector3 positionTarget = Vector3.zero;
        private Quaternion mainRot = Quaternion.identity;

        [HideInInspector] public float roll = 0;
        [HideInInspector] public float pitch = 0;
        [HideInInspector] public float yaw = 0;

        public Vector2 LimitAxisControl = new Vector2(2, 1);
        // limited of axis rotation magnitude

        private float gravityVelocity = 0;

        GameObject TextvelocityTarget;

        void Start()
        {
            TextvelocityTarget = GameObject.Find("HUD_Mach Speed Num");
            mainRot = this.transform.rotation;
            MoveSpeed = Speed;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        
        [HideInInspector]
        public float normalFlySpeed;
        private float speedDelta;

        void FixedUpdate()
        {
            Quaternion AddRot = Quaternion.identity;
            Vector3 velocityTarget = Vector3.zero;
            //normalFlySpeed = Mathf.Clamp(VelocitySpeed / (SpeedTakeoff * 2), 0, 1);
            AddRot.eulerAngles = new Vector3(pitch, yaw, roll);
            mainRot *= AddRot;
            velocityTarget = (GetComponent<Rigidbody>().rotation * Vector3.forward) * VelocitySpeed;//有影响
            GetComponent<Rigidbody>().rotation = Quaternion.Lerp(GetComponent<Rigidbody>().rotation, mainRot, Time.fixedDeltaTime * RotationSpeed);
            gravityVelocity += (Physics.gravity.y * ((1 - Mathf.Clamp(VelocitySpeed / (SpeedTakeoff * 2), 0, 1)) + Vector3.Dot(Physics.gravity.normalized, velocityTarget.normalized + (Vector3.up * 0.5f)))) * Time.fixedDeltaTime;
            gravityVelocity = Mathf.Clamp(gravityVelocity, -float.MaxValue, 0);
            velocityTarget.y += gravityVelocity;
            yaw = Mathf.Lerp(yaw, 0, Time.deltaTime);
            Vector3 velocityChange = (velocityTarget - GetComponent<Rigidbody>().velocity);
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);//关键语句,AddForce
            VelocitySpeed = (Speed + MoveSpeed);
            if (speedDelta < 1) MoveSpeed = Mathf.Lerp(MoveSpeed, Speed, Time.fixedDeltaTime * 0.1f);



        }


        // Input function. ( roll and pitch)
        public void AxisControl(Vector2 axis)
        {
            roll = Mathf.Lerp(roll, Mathf.Clamp(axis.x, -LimitAxisControl.x, LimitAxisControl.x) * SpeedRoll, Time.deltaTime);
            float pitchVel = Mathf.Clamp(VelocitySpeed / (SpeedTakeoff * 2), 0, 1);
            pitch = Mathf.Lerp(pitch, Mathf.Clamp(axis.y, -LimitAxisControl.y, LimitAxisControl.y) * SpeedPitch, Time.deltaTime * pitchVel);
        }

        //Input function ( yaw)
        public void TurnControl(float turn)
        {
            yaw += turn * Time.deltaTime * SpeedYaw;
        }


        // Speed up
        public void SpeedUp(float delta)
        {
            if (delta < 0)
                delta = 0;

            if (delta > 0)
                MoveSpeed = Mathf.Lerp(MoveSpeed, SpeedMax, Time.deltaTime * AccelerationSpeed);

            speedDelta = delta;
            TextvelocityTarget.GetComponent<Text>().text = MoveSpeed.ToString();
        }
        
    }
}
