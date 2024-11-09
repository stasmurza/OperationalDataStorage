using MarketDataAggregator.Application.Models.Ohlcs;
using MarketDataAggregator.Domain.Entities.Ohlcs;

namespace MarketDataAggregator.Application.Ohlcs.Extensions;

public static class OhlcExtensions
{
    public static void Apply(this Ohlc ohlc, OhlcDto dto)
    {
        if (ohlc.Symbol != dto.Symbol) throw new ArgumentOutOfRangeException(nameof(dto));
        if (dto.EndTime <= ohlc.EndTime) return;
        
        ohlc.Open = dto.Open;
        ohlc.Close = dto.Close;
        ohlc.High = dto.High;
        ohlc.Low = dto.Low;
        ohlc.Volume = dto.Volume;
    }

    public static void Apply(this Ohlc result, Ohlc ohlc)
    {
        if (result.Symbol != ohlc.Symbol) throw new ArgumentOutOfRangeException(nameof(ohlc));
        if (ohlc.EndTime <= result.EndTime) return;

        result.Open = ohlc.Open;
        result.Close = ohlc.Close;
        result.High = ohlc.High;
        result.Low = ohlc.Low;
        result.Volume = ohlc.Volume;
    }

    public static void Apply(this Ohlc result, AddOhlcInput input)
    {
        if (result.Symbol != input.Symbol) throw new ArgumentOutOfRangeException(nameof(input));
        if (input.EndTime <= result.EndTime) return;

        result.Open = input.Open;
        result.Close = input.Close;
        result.High = input.High;
        result.Low = input.Low;
        result.Volume = input.Volume;
    }

    public static OhlcDto ToDto(this Ohlc ohlc) => new()
    {
        Id = ohlc.Id,
        StartTime = ohlc.StartTime,
        EndTime = ohlc.EndTime,
        Symbol = ohlc.Symbol,
        Interval = Enum.Parse<Models.Ohlcs.TimeInterval>(ohlc.Interval.ToString()),
        Low = ohlc.Low,
        High = ohlc.High,
        Open = ohlc.Open,
        Close = ohlc.Close,
        Volume = ohlc.Volume,
    };
}
