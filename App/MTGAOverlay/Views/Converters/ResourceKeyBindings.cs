using System.Windows;
using System.Windows.Controls;

namespace MindSculptor.App.MtgaOverlay.Views.Converters
{
    internal static class ResourceKeyBindings
    {
        public static DependencyProperty ContentTemplateResourceKeyBindingProperty = ResourceKeyBindingFactory.CreateResourceKeyBindingProperty(ContentPresenter.ContentTemplateProperty, typeof(ResourceKeyBindings));

        public static void SetContentTemplateResourceKeyBinding(DependencyObject dependencyObject, object resourceKey)
            => dependencyObject.SetValue(ContentTemplateResourceKeyBindingProperty, resourceKey);

        public static object GetContentTemplateResourceKeyBinding(DependencyObject dependencyObject)
            => dependencyObject.GetValue(ContentTemplateResourceKeyBindingProperty);
    }
}
