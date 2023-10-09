using JARVS.Models;

namespace JARVS;

public class FuturesManager
{
    private readonly Account _account;
    public List<FuturesExchangeInfoSymbolResponse>? futuresSymbolList;

    public FuturesManager(Account account)
    {
        _account = account;
        this.futuresSymbolList = null;
    }

    public async Task<List<FuturesExchangeInfoSymbolResponse>> GetExchangeInfoAsync()
    {
        List<FuturesExchangeInfoSymbolResponse> symbols = new List<FuturesExchangeInfoSymbolResponse>();
        var response = await _account.UserHttpClient.GetAsync("fapi/v1/exchangeInfo");

        var result = await Utils.CheckData<FuturesExchangeInfoResponse>(response);
        if (!result.IsSuccess) Console.WriteLine($"Futures Exchange Info Don't Load Correctly. msg:{result.Message}");

        foreach (var symbol in result.Data.symbols)
        {
            if (symbol.pair.EndsWith("USDT") && symbol.contractType == "PERPETUAL" && symbol.status == "TRADING")
            {
                symbols.Add(symbol);
            }
        }
        this.futuresSymbolList = symbols;
        return symbols;
    }

    public async Task PostLimitOrderAsync(OrderInfo orderInfo)
    {
        string queryString = Signature.GetQueryStringForLimit(_account, orderInfo);
        var response = await _account.UserHttpClient.PostAsync($"/fapi/v1/order?{queryString}", null);
        Console.WriteLine(orderInfo.OrderMarkPrice);
        var result = await Utils.CheckData<FuturesOrderResponse>(response);
        if (!result.IsSuccess || result.Data == null) throw new Exception($"Futures Limit Order Error. msg:{result.Message}");
        Console.WriteLine($"ORDER CREATED. {orderInfo.OrderType}");

    }

    public async Task<double> GetFuturesAvailableBalanceAsync()
    {
        string queryString = Signature.GetQueryStringForBalance(_account);
        var response = await _account.UserHttpClient.GetAsync($"/fapi/v2/balance?{queryString}");
        var result = await Utils.CheckData<List<FuturesBalanceResponse>>(response);
        if (!result.IsSuccess || result.Data == null) throw new Exception($"Futures Balance Error. msg:{result.Message}");
        string balanceString = result.Data.Find(p => p.asset == "USDT").availableBalance.Replace(".", ",");
        double futuresBalance = Math.Round(Convert.ToDouble(balanceString), 2);
        _account.FuturesBalance = futuresBalance;
        return futuresBalance;
    }

    public async Task GetSingleSymbolPriceAsync(string symbol)
    {
        var response = await _account.UserHttpClient.GetAsync($"/fapi/v1/ticker/price?symbol={symbol}");
        var result = await Utils.CheckData<SymbolPriceTickerResponse>(response);
        if (!result.IsSuccess || result.Data == null) throw new Exception($"Futures Balance Error. msg:{result.Message}");
        Console.WriteLine($"{result.Data.symbol}:{result.Data.price}");
    }

    public async Task GetAllSymbolPriceAsync()
    {
        var response = await _account.UserHttpClient.GetAsync($"/fapi/v1/ticker/price");
        var result = await Utils.CheckData<List<SymbolPriceTickerResponse>>(response);
        if (!result.IsSuccess || result.Data == null) throw new Exception($"Futures Balance Error. msg:{result.Message}");
        foreach (SymbolPriceTickerResponse symbolPriceTicker in result.Data)
        {
            FuturesExchangeInfoSymbolResponse futuresExchangeInfoSymbol =
                futuresSymbolList.Find(p => p.symbol == symbolPriceTicker.symbol);
            if (futuresExchangeInfoSymbol != null)
            {
                futuresExchangeInfoSymbol.markPrice = symbolPriceTicker.price;
            }
        }
    }

    public async Task ChangeAllLeverage()
    {
        foreach (FuturesExchangeInfoSymbolResponse symbol in this.futuresSymbolList)
        {
            var queryString = Signature.GetQueryStringForLeverage(_account, symbol,10);
            var response = await _account.UserHttpClient.PostAsync($"/fapi/v1/leverage?{queryString}", null);
            var result = await Utils.CheckData<FuturesLeverageResponse>(response);
            Console.WriteLine(result.IsSuccess + " " + symbol.symbol);
            if (!result.IsSuccess || result.Data == null)
            {
                var queryString2 = Signature.GetQueryStringForLeverage(_account, symbol,8);
                var response2 = await _account.UserHttpClient.PostAsync($"/fapi/v1/leverage?{queryString2}", null);
                var result2 = await Utils.CheckData<FuturesLeverageResponse>(response2);
                Console.WriteLine(result2.IsSuccess + " " + symbol.symbol);
            }
        }
    }

    public async Task ChangeAllMarginType()
    {
        foreach (FuturesExchangeInfoSymbolResponse symbol in this.futuresSymbolList)
        {
            var queryString = Signature.GetQueryStringForMarginType(_account, symbol);
            var response = await _account.UserHttpClient.PostAsync($"/fapi/v1/marginType?{queryString}", null);
            var result = await Utils.CheckData<FuturesLeverageResponse>(response);
            Console.WriteLine(result.IsSuccess + " " + symbol.symbol);
        }
    }

    public async Task ManagerIsReady()
    {
        try
        {
            await GetFuturesAvailableBalanceAsync();
            await GetExchangeInfoAsync();
            await GetAllSymbolPriceAsync();
            foreach (var infoSymbolResponse in futuresSymbolList)
            {
                Console.WriteLine($"{infoSymbolResponse.symbol}:{infoSymbolResponse.markPrice}");
            }
            Console.WriteLine($"Symbols Ready! {futuresSymbolList.Count}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}