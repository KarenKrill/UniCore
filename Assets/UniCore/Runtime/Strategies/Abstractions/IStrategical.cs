namespace KarenKrill.UniCore.Strategies.Abstractions
{
    public interface IStrategical<StrategyType> where StrategyType : IStrategy
    {
        void SetStrategy(StrategyType strategy);
    }
}
