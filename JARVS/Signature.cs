using JARVS.Models;
using System.Security.Cryptography;
using System.Text;

namespace JARVS;

public static class Signature
{
    private static string Sign(string secretKey, string queryString)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        byte[] queryStringBytes = Encoding.UTF8.GetBytes(queryString);
        HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
        byte[] bytes = hmacsha256.ComputeHash(queryStringBytes);
        var _signature = BitConverter.ToString(bytes).Replace("-", "").ToLower();
        queryString = queryString + "&signature=" + _signature;
        return queryString;
    }

    public static string GetQueryStringForLimit(Account account, OrderInfo orderInfo)
    {
        Utils.CalculateOrderPrice(orderInfo);
        string queryString = (orderInfo.IsShort) ?
            $"symbol={orderInfo.Symbol.symbol}&side=SELL&positionSide=SHORT&type=LIMIT&quantity={orderInfo.OrderQuantity}&price={orderInfo.OrderMarkPrice}&timeInForce=GTC"
            : $"symbol={orderInfo.Symbol.symbol}&side=BUY&positionSide=LONG&type=LIMIT&quantity={orderInfo.OrderQuantity}&price={orderInfo.OrderMarkPrice}&timeInForce=GTC";

        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        queryString = queryString + "&timestamp=" + timestamp;
        queryString = Sign(account.SecretKey, queryString);
        return queryString;
    }

    public static string GetQueryStringForBalance(Account account)
    {
        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        string queryString = "timestamp=" + timestamp;
        queryString = Sign(account.SecretKey, queryString);
        return queryString;
    }

    public static string GetQueryStringForLeverage(Account account, FuturesExchangeInfoSymbolResponse symbol,int leverage = 10)
    {
        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        string queryString = $"symbol={symbol.symbol}&leverage={leverage}&timestamp=" + timestamp;
        queryString = Sign(account.SecretKey, queryString);
        return queryString;
    }

    public static string GetQueryStringForMarginType(Account account, FuturesExchangeInfoSymbolResponse symbol)
    {
        long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        string queryString = $"symbol={symbol.symbol}&marginType=ISOLATED&timestamp=" + timestamp;
        queryString = Sign(account.SecretKey, queryString);
        return queryString;
    }
}