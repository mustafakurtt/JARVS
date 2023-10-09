// See https://aka.ms/new-console-template for more information

using JARVS;
using JARVS.Models;

Console.WriteLine("Hello, World!");

Account account = new Account(
    "JARVS",
    "14200d1b049dee11c81428f5af73cc8fb20645179741607dff4df9f262f3a335",
    "f464c4152cdd2d76d88595c5402ca7a01c27a2eaa0eca3f43312c2bc0b6f7b36",
    20);

#region Account
Console.Title = account.UserName;
Console.ForegroundColor = ConsoleColor.Magenta;
#endregion


#region Main
FuturesManager futuresManager = new FuturesManager(account);
await futuresManager.ManagerIsReady();
//#endregion
//AllMarketMarkPriceStream allMarketMarkPriceStream = new AllMarketMarkPriceStream(futuresManager.futuresSymbolList);
//allMarketMarkPriceStream.ConnectToMarkPriceStream();
//#region BalanceThread
Thread thread = new Thread(async () => await GetBalance());
thread.Start();
async Task GetBalance()
{
    while (true)
    {
        await futuresManager.GetFuturesAvailableBalanceAsync();
        await Task.Delay(10000);
    }
}
#endregion

while (true)
{
    string? input = "";
    input = Console.ReadLine()?.ToUpperInvariant().Replace("İ", "I").Replace("ı", "I");
    int percentange = 80;
    string[] inputArray = input.Split(" ").Where((i) => i != "").ToArray();
    string coinName = "";

    if (inputArray.Length > 0)
    {
        coinName = inputArray[0];
    }
    if (inputArray.Length > 1)
    {
        //check percent
        try
        {
            percentange = int.Parse(inputArray[inputArray.Length - 1]);
            if (percentange > 80) percentange = 80;
            if (percentange < 10) percentange = 10;

        }
        catch (Exception e)
        {
            if (e.Message != "Input string was not in a correct format.")
            {
                Console.WriteLine(e.Message);
                continue;
            }
        }
    }

    bool isShort = inputArray.FirstOrDefault(p => p == "S") != null;
    if (input == "")
    {
        await futuresManager.GetFuturesAvailableBalanceAsync();
        Console.WriteLine($"Balance :{account.FuturesBalance}");
    }
    else if (input == "CLEAR") Console.Clear();
    else if (input == "LEVERAGE") await futuresManager.ChangeAllLeverage();
    else if (input == "MARGIN") await futuresManager.ChangeAllMarginType();
    else if (input.Length > 0)
    {
        FuturesExchangeInfoSymbolResponse? futuresExchangeInfoSymbol = null;
        futuresExchangeInfoSymbol = futuresManager.futuresSymbolList?.Find(s => s.symbol == coinName + "USDT");
        if (futuresExchangeInfoSymbol == null) futuresExchangeInfoSymbol = futuresManager.futuresSymbolList?.Find(s => s.symbol == "1000" + coinName + "USDT");

        if (futuresExchangeInfoSymbol != null)
        {
            var leverage = Utils.LeverageChecker(futuresExchangeInfoSymbol);

            OrderInfo orderInfo = new OrderInfo(account, futuresExchangeInfoSymbol, leverage, isShort, false, percentange);
            Console.WriteLine(orderInfo.MarkPrice);
            try
            {
                futuresManager.PostLimitOrderAsync(orderInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            continue;
        }
        Console.WriteLine("Coin Not Found");
    }

}


