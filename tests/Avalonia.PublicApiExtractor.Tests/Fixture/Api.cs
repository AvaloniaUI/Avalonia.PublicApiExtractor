using System;

namespace Fixture
{
    public enum Color { Red, Green }

    public interface IShape
    {
        int Area { get; }
    }

    public class Widget : IShape, IDisposable
    {
        public const string Name = "widget";
        public Widget(int size) { }
        public int Size { get; set; }
        int IShape.Area => 0;
        void IDisposable.Dispose() { }
        public static Widget operator +(Widget a, Widget b) => a;
        public virtual void Resize(int width, int height = 10) { }
        internal void InternalMethod() { }
        private void PrivateMethod() { }
    }

    internal class InternalType { }
}
