namespace JARVS.Models;

public class SymbolPriceTickerResponse
{
    public string symbol { get; set; }
    public string price { get; set; }
    public long time { get; set; }
}