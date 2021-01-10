using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Gamekit2D
{
    public class RunningGameManager : MonoBehaviour 
    {
        // 싱글톤 패턴일 경우 
        //public static RunningGameManager Instance
        //{
        //    get { return _Instance; }
        //}
        //protected static RunningGameManager _Instance;


        // 뷰잉 목적인지 인풋 목적인지 잘 구분 해야 할듯 

        // == 설정값 ==
        [Header("=== 특별한 옵션 ===")]
        [Header("움직임 추적")]
        public bool isMotionTracking = false; // 특별 옵션
        [Header("비트코인 시세 알아보기")]
        public bool isBitCoinEnable = false; // 특별 옵션
        [Header("오브젝트 등록")]
        public Text UIScore;
        public Text UIBest_Score;
        public GameObject CreateCoinPos;
        public Transform CoinParent;
        public GameObject effect1;
        // == 감시하는 값 == 
        [Header("감시하는 값")]
        [SerializeField]
        private bool isRunningStart = false;
        // == 내부 변수 ==
        
       
      

        //상태 관련 
        private bool isScoreUpEnable = false;

        private bool isJump = false;
        private bool isDoubleJumpEnable = false;
        private bool isDoubleJump = false;
        private bool isUpTouch = true;

        private bool isTouch = true;
        // 비트 코인 시세 
        private float tradePrice;
        //점수
        private int Score;
        CoinAPILoading coinAPILoading;

        //치환 목적 변수
        // PlayerCharacter playerCharacter;
        private GameObject Player;
        CharacterAddSensing characterAddSensing;
       

        private void Awake()
        {
            // 싱글톤 용
            //if (_Instance == null)
            //    _Instance = this;
            
            Player = GameObject.Find("cat"); // 여기서 고양이 오브젝트를 플레이어 
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

            GetComponent<GoogleAdMobController>().RequestAndLoadRewardedInterstitialAd(); // 광고 넣기 준비

            yield return new WaitForSeconds(0.05f); 
            UIBest_Score.text = GetComponent<SQLCon>().configs[1].record; // 기록 불러 오기 디비 관련 
            yield return new WaitForSeconds(0.95f);

            isRunningStart = true;

            if (characterAddSensing.Sensing == 0)
                isScoreUpEnable = true;
            if(isMotionTracking )
            StartCoroutine(CoinPosAutoCreate());

            yield break;
        }

        IEnumerator CoinPosAutoCreate() // 궤적 에 아이템 넣기 
        {
            while(true)
            {
                Instantiate( CreateCoinPos, Player.transform.position+new Vector3(0,0.3f,0), Quaternion.identity, CoinParent);
                yield return new WaitForSeconds(0.3f);
            }
            yield break;
        }

        // Update is called once per frame
        void Update()
        {
            switch (characterAddSensing.Sensing)
            {
                case 0:
                    isScoreUpEnable = true;
                    break;
                case 1:
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
                    UIScore.text = (Score * tradePrice).ToString("###,###");
                    GetComponent<AudioSource>().Play();
                    Instantiate(effect1, characterAddSensing.preObjPos, Quaternion.identity);
                    isScoreUpEnable = false;
                    break;
                case 2:
                    if (Score > int.Parse(UIBest_Score.text))
                    {
                        GetComponent<SQLCon>().DataBaseUpdate(Score);
                    }
                    characterAddSensing.Sensing = 0;
                    GetComponent<GoogleAdMobController>().ShowRewardedInterstitialAd(); // 광고 넣기
                    break;
                default:
                    break;
            }
  
        

            if (isRunningStart)
            {

                if (characterAddSensing.isFailure) // 게임 실패 조건 
                {
                    if (Score > int.Parse(UIBest_Score.text))
                    {
                        GetComponent<SQLCon>().DataBaseUpdate(Score);
                    }
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
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
