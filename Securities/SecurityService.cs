using FluentResults;

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

public record ExecuteSecurityRequest
{
    public List<string> Isins { get; init; } = [];
}

public class SecurityService(
    IIsinsPricesService _isinsPricesService, 
    IPricesRepository _pricesRepository)
{
    public async Task<Result> ExecuteAsync(ExecuteSecurityRequest request)
    {
        var pricesResponse = await _isinsPricesService.GetPrices(request.Isins);

        var prices = pricesResponse.Select(price => new PriceEntity(price.Isin, price.Price));

        await _pricesRepository.SaveBatch(prices);

        return Result.Ok();
    }
}
