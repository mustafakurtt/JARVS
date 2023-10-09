using System.Net;
using System.Security.Cryptography;
using System.Text;
using JARVS.Models;
using Newtonsoft.Json;
namespace JARVS;

public static class Utils
{
    public static async Task<Response<T>> CheckData<T>(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            ErrorResponse errorResponse = await Deserialize<ErrorResponse>(response);
            return new Response<T>(default, false, errorResponse.msg);
        }

        T? data = await Deserialize<T>(response);
        return new Response<T>
        {
            Data = data,
            IsSuccess = true
        };
    }

    public static async Task<Response> CheckData(HttpResponseMessage response)
    {
        if (response.StatusCode != HttpStatusCode.OK)
        {
            ErrorResponse errorResponse = await Deserialize<ErrorResponse>(response);
            return new Response(false, errorResponse.msg);
        }

        return new Response
        {
            IsSuccess = true,
            Message = "Success"
        };
    }

    private static async Task<T> Deserialize<T>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        T result = JsonConvert.DeserializeObject<T>(responseContent);
        return result;
    }

    public static string ReplacePrice(string price)
    {
        return price.Replace(".", ",");
    }

    public static void CalculateOrderPrice(OrderInfo orderInfo)
    {

        var tickSizeString = orderInfo.Symbol.filters.Find(p => p.filterType == "PRICE_FILTER").tickSize;
        var tickSize = Convert.ToDouble(tickSizeString.Replace(".", ","));

        double orderMarkPrice = (((orderInfo.MarkPrice * orderInfo.Precision) / 100) + orderInfo.MarkPrice);
        double rest = (orderMarkPrice % tickSize);
        orderMarkPrice -= rest;
        orderMarkPrice = Math.Round(orderMarkPrice, orderInfo.Symbol.pricePrecision);
        //price
        var orderMarkPriceString = orderMarkPrice.ToString().Replace(",", ".");
        orderInfo.OrderMarkPrice = orderMarkPriceString;

        //quantityCalculate
        double orderQuantity = ((orderInfo.Account.FuturesBalance * orderInfo.Leverage) * orderInfo.Percentange / 100) / orderMarkPrice;
        orderQuantity = Math.Round(orderQuantity, orderInfo.Symbol.quantityPrecision);
        var orderQuantityString = orderQuantity.ToString().Replace(",", ".");
        orderInfo.OrderQuantity = orderQuantityString;
    }

    public static int LeverageChecker(FuturesExchangeInfoSymbolResponse symbol)
    {
        if (symbol.symbol == "BNBUSDT") return 20;
        if (symbol.symbol == "BTCUSDT") return 20;
        if (symbol.symbol == "DOGEUSDT") return 20;
        return 10;
    }
}


public class Response<T> : Response
{
    public T? Data { get; set; }

    public Response()
    {
        
    }

    public Response(T? data, bool isSuccess, string message) : base(isSuccess,message)
    {
        Data = data;
    }
}

public class Response
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public Response()
    {
        
    }

    public Response(bool isSuccess, string message): this()
    {
        IsSuccess = isSuccess;
        Message = message;
    }
}

public class ErrorResponse
{
    public int code { get; set; }
    public string msg { get; set; }
}