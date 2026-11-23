using System;

namespace Fixture
{
    public enum Color
    {
        /// <summary>The red one.</summary>
        Red,
        Green,
    }

    public delegate T Factory<T>(int x) where T : class;

    public interface IShape
    {
        int Area { get; }
    }

    /// <summary>
    /// A widget.
    /// </summary>
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
        /// <summary>Resizes the widget.</summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        public virtual void Resize(int width, int height = 10) { }
        public void Resolve(string @ref) { }
        internal void InternalMethod() { }
        private void PrivateMethod() { }
    }

    internal class InternalType { }
}
