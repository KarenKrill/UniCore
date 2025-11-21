namespace KarenKrill.UniCore.Instantiation.Abstractions
{
    public interface IFactory { }

    public interface IFactory<out ProducedType> : IFactory
    {
        ProducedType Create();
    }

    public interface ICachingFactory<out ProducedType> : IFactory<ProducedType>
    {
        ProducedType GetOrCreate();
    }

    public interface IFactory<out ProducedType, in ConsumedType> : IFactory
    {
        ProducedType Create(ConsumedType value);
    }

    public interface ICachingFactory<out ProducedType, in ConsumedType> : IFactory<ProducedType, ConsumedType>
    {
        ProducedType GetOrCreate(ConsumedType value);
    }

    public interface IBaseFactory<in ProducedTypeBase> : IFactory where ProducedTypeBase : class
    {
        ProducedType Create<ProducedType>() where ProducedType : ProducedTypeBase;
    }

    public interface ICachingBaseFactory<in ProducedTypeBase> : IBaseFactory<ProducedTypeBase> where ProducedTypeBase : class
    {
        ProducedType GetOrCreate<ProducedType>() where ProducedType : ProducedTypeBase;
    }
}
