using System;
using System.Windows;

namespace MindSculptor.App.MtgaOverlay.Views.Converters
{
    internal static class ResourceKeyBindingFactory
    {
        public static DependencyProperty CreateResourceKeyBindingProperty(DependencyProperty boundProperty, Type ownerType)
            => DependencyProperty.RegisterAttached($"{boundProperty.Name}ResourceKeyBinding", typeof(object), ownerType, new PropertyMetadata(null, (dp, e) =>
            {
                var element = dp as FrameworkElement;
                element?.SetResourceReference(boundProperty, e.NewValue);
            }));
    }
}
