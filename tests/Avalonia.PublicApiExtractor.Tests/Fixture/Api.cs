using System;

namespace Fixture
{
    public enum Color { Red, Green }

    public delegate T Factory<T>(int x) where T : class;

    public interface IShape
    {
        int Area { get; }
    }

    public class Widget : IShape, IDisposable
    {
        public const string Name = "widget";
        public Widget(int size) { }
        public int Size { get; set; }
        public int PrivateSetter { get; private set; }
        public int ProtectedSetter { get; protected set; }
        public int InitOnly { get; init; }
        public delegate void Callback();
        int IShape.Area => 0;
        void IDisposable.Dispose() { }
        public static Widget operator +(Widget a, Widget b) => a;
        public virtual void Resize(int width, int height = 10) { }
        public void Resolve(string @ref) { }
        internal void InternalMethod() { }
        private void PrivateMethod() { }
    }

    internal class InternalType { }
}
