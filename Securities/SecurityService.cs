using System;
using System.Collections.Generic;
using System.Text;

namespace Securities;

public record SecurityPrice(string Isin, decimal Price);

public interface IIsinsPricesService
{
    Task<IEnumerable<SecurityPrice>> GetPrices(List<string> isins);
}

public class PriceEntity
{
    public PriceEntity(string isin, decimal price)
    {
        Isin = isin;
        Price = price;
    }

    public string Isin { get; set; }

    public decimal Price { get; set; }

}

public interface IPricesRepository
{
    Task SaveBatch(IEnumerable<PriceEntity> priceEntity);
}

public class SecurityService(IIsinsPricesService _isinsPricesService, IPricesRepository _pricesRepository)
{
    public async Task ExecuteAsync(List<string> isins)
    {
        var pricesResponse = await _isinsPricesService.GetPrices(isins);

        var prices = pricesResponse.Select(price => new PriceEntity(price.Isin, price.Price));

        await _pricesRepository.SaveBatch(prices);
    }
}
