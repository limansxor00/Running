using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Gamekit2D
{
    public class CharacterAddSensing : MonoBehaviour
    {

        GameObject obj = null;
        public int Sensing = 0;
        public Vector2 preObjPos;
        public bool IsGrounded =false ;

        public bool isEndPoint = false; 

        public AudioClip clip;

        public float gravity = 50f;
        protected const float k_GroundedStickingVelocityMultiplier = 5f;

        Vector2 _MoveVector;
        Animator _Animator;

        CharacterController2D _CharacterController2D;

       
        private void Awake()
        {
            _CharacterController2D = GetComponent<CharacterController2D>();
            _Animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("collision.gameObject.layer" + collision.gameObject.layer);
            if (collision.gameObject.layer == 31)
            {
                IsGrounded = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == 31)
            {
                IsGrounded = false;
            }

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other != null && other.GetComponent<ScoreCoin>() != null)
            {
                obj = other.gameObject;
                preObjPos = obj.transform.position;
                Sensing = 1;
            }

            if(other.name =="endPoint")
            {
                isEndPoint = true;
            }
            

        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (obj != null)
            {
                Sensing = 0;
                GameObject.Destroy(obj);
               
                //    Debug.Log("삭제 완료");

                obj = null;
            }
            if (other.name == "endPoint")
            {
                isEndPoint = false;
            }
        }

        public void SetHorizontalMovement(float newHorizontalMovement)
        {
            _MoveVector.x = newHorizontalMovement;
        }

        public void SetVerticalMovement(float newVerticalMovement)
        {
            _MoveVector.y = newVerticalMovement;
        }

        private void Update()
        {
           
        }

        void FixedUpdate()
        {
             _CharacterController2D.Move(_MoveVector * Time.deltaTime);

            // GetComponent<Rigidbody2D>().MovePosition(_MoveVector * Time.deltaTime);
           // GetComponent<Rigidbody2D>().AddForce(_MoveVector * Time.deltaTime);

            if (_MoveVector.y > 0)
            {
                _Animator.SetBool(Animator.StringToHash("isJump"), true);
            }
            else
            {
                _Animator.SetBool(Animator.StringToHash("isJump"), false);
            }

            GroundedVerticalMovement();
        }

        public void GroundedVerticalMovement()
        {
            _MoveVector.y -= gravity * Time.deltaTime;

            if (_MoveVector.y < -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier)
            {
                _MoveVector.y = -gravity * Time.deltaTime * k_GroundedStickingVelocityMultiplier;
            }
        }

    }
}