using JARVS.Models;

namespace JARVS;

public class OrderInfo
{
    public Account Account { get; set; }
    public FuturesExchangeInfoSymbolResponse Symbol { get; set; }
    public double MarkPrice { get; set; }
    public int Leverage { get; set; }
    public bool IsShort { get; set; }
    public string OrderType { get; set; } = "LONG";
    public bool StopWatch { get; set; }
    public double Precision { get; set; }
    public double Percentange { get; set; }
    public string OrderMarkPrice { get; set; }
    public string OrderQuantity { get; set; }

    public OrderInfo(Account account, FuturesExchangeInfoSymbolResponse symbol, int leverage, bool isShort, bool StopWatch, double percentange = 80)
    {
        this.Account = account;
        this.Symbol = symbol;
        this.Leverage = leverage;
        this.IsShort = isShort;
        this.StopWatch = StopWatch;
        this.Percentange = percentange;
        this.Precision = 1;

        if (IsShort) this.Precision *= -1;
        if (IsShort) this.OrderType = "SHORT";

        SetMarkPrice(symbol.markPrice);
    }

    void SetMarkPrice(string markPrice)
    {
        markPrice = Utils.ReplacePrice(markPrice);
        this.MarkPrice = Convert.ToDouble(markPrice);
    }

}