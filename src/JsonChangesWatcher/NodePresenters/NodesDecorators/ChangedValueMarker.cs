using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace JsonFileWatcher.NodePresenters.NodesDecorators
{
    public class ChangedValueMarker
    {
        private UIElement element;
        private ColorAnimation colorAnimation;
        private Storyboard storyboard;
        public ChangedValueMarker(UIElement uIElement, string propPath)
        {
            element = uIElement;

            colorAnimation = new ColorAnimation
            {
                From = Colors.Red,
                To = Colors.White,
                Duration = TimeSpan.FromSeconds(2),
            };
            storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(propPath));
            Storyboard.SetTarget(colorAnimation, element);
        }

        public void Animate(object sender,PropertyChangedEventArgs a)
        {
            if (a.PropertyName == "Value")
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    storyboard.Begin();
                }));
            }
        }
    }
}
