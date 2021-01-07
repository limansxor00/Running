using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class CoinAPILoading 
{
    // Start is called before the first frame update

   
    public Upbit_BitCoin upbit_BitCoin;
    public IEnumerator HttpHeaderGet()
    {
        string url = "https://crix-api-endpoint.upbit.com/v1/crix/candles/days/?code=CRIX.UPBIT.KRW-BTC";
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();


        Debug.Log(request.downloadHandler.text);

      
       List<Upbit_BitCoin> stuff = JsonConvert.DeserializeObject<List<Upbit_BitCoin>>(request.downloadHandler.text);
        upbit_BitCoin = stuff[0];
        //  Debug.Log("비트코인 가격" + stuff[0].tradePrice);

        yield break;

    }
    // Update is called once per frame


   public class Upbit_BitCoin
    {
        public string code;
        public string candleDateTime;
        public string candleDateTimeKst;
        public string openingPrice;
        public string highPrice;
        public string lowPrice;
        public string tradePrice; // <====
        public string candleAccTradeVolume;
        public string candleAccTradePrice;
        public string timestamp;
        public string prevClosingPrice;
        public string change;
        public string changePrice;
        public string signedChangePrice;
        public string changeRate;
        public string signedChangeRate;
    }
}
