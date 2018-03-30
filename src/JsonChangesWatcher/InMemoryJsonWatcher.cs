using System;
using System.Threading;
using System.Windows;

namespace JsonFileWatcher
{
    public class InMemoryJsonWatcher
    {
        private SimpleWindow MW = null;
        Application application;
        public InMemoryJsonWatcher()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                application = new Application();
                MW = new SimpleWindow();
                MW.ShowDialog();
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void OnDataUpdate(string data)
        {
            application?.Dispatcher.BeginInvoke(new Action(() =>
            {
                MW?.OnSourceUpdate(data);
            }));
        }
    }
}
