using System;
using UnityEngine;
#pragma warning disable 649
namespace UnityStandardAssets.Vehicles.Car
{
    [DisallowMultipleComponent]
    public class CarController : MonoBehaviour
    {
        public WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] private WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        public float m_MaximumSteerAngle;

        [SerializeField] private float m_SlipLimit;

        private float m_SteerAngle;
        private Rigidbody m_Rigidbody;

        public bool Skidding;
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude; }}
        public float Revs;
        public float AccelInput { get; private set; }

        // Use this for initialization
        private void Start()
        {
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(float steering, float accel, float footbrake, float handbrake)
        {
            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1 * Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;
            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].GetWorldPose(out Vector3 position, out Quaternion quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;
            }
            ApplyDrive(AccelInput, BrakeInput);
            CheckForWheelSpin();
        }
        /// <summary>
        /// Çý¶¯³µÁ¾
        /// </summary>
        /// <param name="accel"></param>
        /// <param name="footbrake"></param>
        private void ApplyDrive(float accel, float footbrake)
        {
            float thrustTorque = accel * (2000 / 4f);
            float brakeTorque = footbrake * 2000;
            m_WheelColliders[0].motorTorque = m_WheelColliders[1].motorTorque = thrustTorque;
            m_WheelColliders[2].motorTorque = m_WheelColliders[3].motorTorque = 0;

            m_WheelColliders[0].brakeTorque = m_WheelColliders[1].brakeTorque = brakeTorque;
            m_WheelColliders[2].brakeTorque = m_WheelColliders[3].brakeTorque = 0;
        }
        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);
                //Skidding = Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit;
                // is the tire slipping above the given threshhold
                if (Skidding)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }

                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }

        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
