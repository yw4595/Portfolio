/// <summary>
/// Player controller.
/// </summary>
using UnityEngine;
using System.Collections;

namespace AirStrikeKit
{
    [RequireComponent(typeof(TFighterSystem))]

    public class PlayerController : MonoBehaviour
    {

        TFighterSystem flight;
        // Core plane system

        public bool Active = true;
        public bool SimpleControl;
        // make it easy to control Plane will turning easier.
        public bool Acceleration;
        // Mobile*** enabled gyroscope controller
        public float AccelerationSensitivity = 5;
        // Mobile*** gyroscope sensitivity

        // Mobile*** slice
        public GUISkin skin;
        public bool ShowHowto;


        void Awake()
        {

        }

        void Start()
        {
            flight = this.GetComponent<TFighterSystem>();
         

        }

        void Update()
        {
            if (!flight || !Active)
                return;
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            // On Desktop
            DesktopController();
#else
			// On Mobile device
			MobileController ();
#endif

        }

        void DesktopController()
        {
            // Desktop controller
            flight.SimpleControl = SimpleControl;

            // lock mouse position to the center.


            flight.AxisControl(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            if (SimpleControl)
            {
                flight.TurnControl(Input.GetAxis("Mouse X"));
            }

            flight.TurnControl(Input.GetAxis("Horizontal"));
            flight.SpeedUp(Input.GetAxis("Vertical"));



        }

        void MobileController()
        {
            // Mobile controller

            flight.SimpleControl = SimpleControl;

            if (Acceleration)
            {
                // get axis control from device acceleration
                Vector3 acceleration = Input.acceleration;
                Vector2 accValActive = new Vector2(acceleration.x, (acceleration.y + 0.3f) * 0.5f) * AccelerationSensitivity;


                flight.AxisControl(accValActive);
                flight.TurnControl(accValActive.x);
            }
            else
            {

                // get axis control from touch screen


            }

            // slice speed

        }


        // you can remove this part..
        void OnGUI()
        {
            if (!ShowHowto)
                return;

            if (skin)
                GUI.skin = skin;

            if (GUI.Button(new Rect(20, 150, 200, 40), "Gyroscope " + Acceleration))
            {
                Acceleration = !Acceleration;
            }

            if (GUI.Button(new Rect(20, 200, 200, 40), "Change View"))
            {

            }

            if (GUI.Button(new Rect(20, 250, 200, 40), "Change Weapons"))
            {

            }

            if (GUI.Button(new Rect(20, 300, 200, 40), "Simple Control " + SimpleControl))
            {
                if (flight)
                    SimpleControl = !SimpleControl;
            }

            GUI.Label(new Rect(20, 350, 500, 40), "you can remove this in OnGUI in PlayerController.cs");
        }
    }
}
