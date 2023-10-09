namespace JARVS.Models;

public class FuturesBalanceResponse
{
    public string accountAlias { get; set; }
    public string asset { get; set; }
    public string balance { get; set; }
    public string crossWalletBalance { get; set; }
    public string crossUnPnl { get; set; }
    public string availableBalance { get; set; }
    public string maxWithdrawAmount { get; set; }
    public bool marginAvailable { get; set; }
    public long updateTime { get; set; }
}