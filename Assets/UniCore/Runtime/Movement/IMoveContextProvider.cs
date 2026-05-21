using UnityEngine;

namespace KarenKrill.UniCore.Movement
{
    public interface IMoveContextProvider
    {
        public object Context { get; }
    }

    public interface IMoveContextProvider<T> : IMoveContextProvider
    {
        public T TypedContext { get; }
    }

    public abstract class MoveContextProviderBase : IMoveContextProvider
    {
        public abstract object Context { get; }
    }

    public abstract class MoveContextProviderBase<T> : MoveContextProviderBase, IMoveContextProvider<T>, IMoveContextProvider
    {
        public override object Context => TypedContext;

        public abstract T TypedContext { get; }
    }

    public abstract class MoveContextProviderBehaviour : MonoBehaviour, IMoveContextProvider
    {
        public abstract object Context { get; }
    }

    public abstract class MoveContextProviderBehaviour<T> : MoveContextProviderBehaviour, IMoveContextProvider<T>, IMoveContextProvider
    {
        public override object Context => TypedContext;

        public abstract T TypedContext { get; }
    }
}
