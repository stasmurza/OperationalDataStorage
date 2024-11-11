namespace OperationalDataStorage.Application.Models.Ohlcs;

public struct GetOhlcsOutput
{
    public IEnumerable<OhlcDto> Ohlcs { get; set; }
}
