using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{
    public class RunningGameManager : MonoBehaviour 
    {
        //public static RunningGameManager Instance
        //{
        //    get { return _Instance; }
        //}
        //protected static RunningGameManager _Instance;

        GameObject Player;

        public InputComponent.InputButton Jump = new InputComponent.InputButton(KeyCode.Space, InputComponent.XboxControllerButtons.A);
        public InputComponent.InputAxis Horizontal = new InputComponent.InputAxis(KeyCode.D, KeyCode.A, InputComponent.XboxControllerAxes.LeftstickHorizontal);

        public bool isRunningStart = false;

        public Text UIScore;
        public Text UIBest_Score;

        public GameObject effect1;

        public GameObject CreateCoinPos;
        public Transform CoinParent;

        public bool isMotionTracking = false; // 특별 옵션
        public bool isBitCoinEnable = false; // 특별 옵ㅅ

        private int Score;

        private bool isScoreUpEnable = false;

        float tradePrice;

        private bool isJump = false;
        private bool isDoubleJumpEnable = false;
        private bool isDoubleJump = false;
        private bool isUpTouch = true;

   

       // PlayerCharacter playerCharacter;

        CharacterAddSensing characterAddSensing;
        CoinAPILoading coinAPILoading;
        private void Awake()
        {
            //if (_Instance == null)
            //    _Instance = this;
            
            Player = GameObject.Find("cat");
            characterAddSensing = Player.GetComponent<CharacterAddSensing>();

         //   playerCharacter = Player.GetComponent<PlayerCharacter>();
        }
        // Start is called before the first frame update
        IEnumerator Start()
        {

            UIScore.text = "0";
            isRunningStart = false;
            // 여기서 API 불러 와보자 
             coinAPILoading = new CoinAPILoading();
            if( isBitCoinEnable)
            yield return StartCoroutine(coinAPILoading.HttpHeaderGet());
            yield return new WaitForSeconds(0.1f);

            GetComponent<GoogleAdMobController>().RequestAndLoadRewardedInterstitialAd(); // 광고 넣기 

            UIBest_Score.text = GetComponent<SQLCon>().configs[1].record; // 기록 불러 오기 
            yield return new WaitForSeconds(0.9f);
            isRunningStart = true;
            if (characterAddSensing.Sensing == 0)
                isScoreUpEnable = true;
            if(isMotionTracking )
            StartCoroutine(CoinPosAutoCreate());

           
            yield break;
        }

        IEnumerator CoinPosAutoCreate()
        {
            while(true)
            {
                Instantiate( CreateCoinPos, Player.transform.position+new Vector3(0,0.3f,0), Quaternion.identity, CoinParent);
                yield return new WaitForSeconds(0.3f);
            }
            yield break;
        }

        bool isTouch = true;

      
        // Update is called once per frame
        void Update()
        {
            
            if (characterAddSensing.Sensing == 0)
                isScoreUpEnable = true;
            else if (isScoreUpEnable && characterAddSensing.Sensing ==1)
            {
                Score++;
                if (isBitCoinEnable)
                {
                    tradePrice = float.Parse(coinAPILoading.upbit_BitCoin.tradePrice);
                }
                else
                {
                    tradePrice = 1;
                }
               // Debug.Log("비트코인 가격" + coinAPILoading.upbit_BitCoin.tradePrice);
                UIScore.text = (Score* tradePrice).ToString("###,###");
                GetComponent<AudioSource>().Play();
                Instantiate(effect1, characterAddSensing.preObjPos, Quaternion.identity);
                isScoreUpEnable = false;
            }

            if(characterAddSensing.isEndPoint)
            {
                if (Score > int.Parse(UIBest_Score.text))
                {
                    GetComponent<SQLCon>().DataBaseUpdate(Score);
                }

                GetComponent<GoogleAdMobController>().ShowRewardedInterstitialAd(); // 광고 넣기
                characterAddSensing.isEndPoint = false;
            }


            if (Player.transform.position.y < -3f)
            {
                if (Score > int.Parse(UIBest_Score.text))
                {
                    GetComponent<SQLCon>().DataBaseUpdate(Score);
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (isRunningStart)
            {


                if (characterAddSensing.IsGrounded) // 땅에 닿은거 확ㅇ
                {
                    isJump = false;
                    isDoubleJump = false;
                    characterAddSensing.IsGrounded = false;
                }

                // == 터치 이벤트 
                if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && isUpTouch))
                {

                    if (!isJump && !isDoubleJump)
                    {
                        characterAddSensing.SetVerticalMovement(15);
                        isJump = true; // 점프 중 
                        isDoubleJumpEnable = true; // 더블 점프 가능
                        Debug.Log("점프중");
                       
                    }
                    else if (isJump && isDoubleJumpEnable)
                    {
                        characterAddSensing.SetVerticalMovement(15);
                        isJump = false;
                        isDoubleJump = true; // 더블 점프 중
                        isDoubleJumpEnable = false;
                        Debug.Log("더블 점프중 ");
                       
                    }
                    else
                    {
                        // playerCharacter.SetVerticalMovement(0);
                       // Debug.Log("점프 커멘드 무시 ");
                    }
                    isUpTouch = false;
                }


                if (Input.GetMouseButtonUp(0)) //
                {
                   // playerCharacter.SetVerticalMovement(0);
                }

                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        //  playerCharacter.SetVerticalMovement(0);
                        isUpTouch = true;
                    }
                }

               
            }

            


        
        }



        void FixedUpdate()
        {
            if (isRunningStart)
            {
                characterAddSensing.SetHorizontalMovement(5);
            }

            // Player.GetComponent<CharacterController2D>().Move(new Vector2(5*Time.deltaTime, 0));
        }
    }
}
