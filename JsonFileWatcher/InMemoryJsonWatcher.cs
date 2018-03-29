using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JsonFileWatcher
{
    public class InMemoryJsonWatcher
    {
        private MainWindow MW = null;
        Application application;
        public InMemoryJsonWatcher()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                application = new Application();
                MW = new MainWindow();
                MW.ShowDialog();
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            Thread.Sleep(1000);
        }

        public void OnDataUpdate(string data)
        {
            application.Dispatcher.BeginInvoke(new Action(() =>
            {
                MW?.OnSourceUpdate(data);
            }));
        }
    }
}
