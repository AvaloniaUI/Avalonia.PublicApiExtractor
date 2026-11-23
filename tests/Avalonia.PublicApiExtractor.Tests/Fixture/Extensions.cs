using System.Collections.Generic;

namespace Fixture
{
    public static class WidgetExtensions
    {
        extension(Widget widget)
        {
            public int Doubled => widget.Size * 2;
            public void Grow(int by) { }
            public static Widget Create() => new(0);
        }

        extension<T>(IEnumerable<T> items) where T : class
        {
            public bool IsEmpty => false;
        }

        public static int Classic(this Widget widget) => 0;
    }
}

public class GlobalType
{
    public void Paint(Fixture.Color color = Fixture.Color.Green, Fixture.Color fallback = (Fixture.Color)7) { }
}
