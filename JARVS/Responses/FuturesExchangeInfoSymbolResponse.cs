namespace JARVS.Models;

public class FuturesExchangeInfoSymbolResponse
{
    public string symbol { get; set; }
    public string pair { get; set; }
    public string contractType { get; set; }
    public long deliveryDate { get; set; }
    public long onboardDate { get; set; }
    public string status { get; set; }
    public string maintMarginPercent { get; set; }
    public string requiredMarginPercent { get; set; }
    public string baseAsset { get; set; }
    public string quoteAsset { get; set; }
    public string marginAsset { get; set; }
    public int pricePrecision { get; set; }
    public int quantityPrecision { get; set; }
    public string baseAssetPrecision { get; set; }
    public string quotePrecision { get; set; }
    public string underlyingType { get; set; }
    public List<string> underlyingSubType { get; set; }
    public long settlePlan { get; set; }
    public string triggerProtect { get; set; }
    public string liquidationFee { get; set; }
    public string marketTakeBound { get; set; }
    public List<FuturesExchangeInfoFiltersResponse> filters { get; set; }
    public List<string> orderTypes { get; set; }
    public List<string> timeInForce { get; set; }
    public string markPrice { get; set; }

}