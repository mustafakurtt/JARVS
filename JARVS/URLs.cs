using JARVS.Models;

namespace JARVS;

public static class URLs
{
    public static string TestnetRest = "https://testnet.binancefuture.com/";
    public static string TestnetWS = "wss://stream.binancefuture.com";
    public static string BaseEndpoint = "https://fapi.binance.com";
    public static string TestConnectivity = "/fapi/v1/ping";
    public static string CheckServerTime = "/fapi/v1/time";
    public static string ExchangeInformation = "/fapi/v1/exchangeInfo";
}
