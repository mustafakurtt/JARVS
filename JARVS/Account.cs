namespace JARVS;

public class Account
{
    public string UserName { get; set; }
    public string PublicKey { get; set; }
    public string SecretKey { get; set; }
    public string? ListenKey { get; set; }
    public double FuturesBalance { get; set; }
    public int DefaultLeverage { get; set; }
    public HttpClient UserHttpClient { get; set; }

    public Account()
    {
        
    }
    public Account(string userName, string publicKey, string secretKey, int defaultLeverage)
    {
        UserName = userName;
        PublicKey = publicKey;
        SecretKey = secretKey;
        DefaultLeverage = defaultLeverage;
        UserHttpClient = new HttpClient();
        UserHttpClient.BaseAddress = new Uri(URLs.TestnetRest);
        UserHttpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", publicKey);
    }
}