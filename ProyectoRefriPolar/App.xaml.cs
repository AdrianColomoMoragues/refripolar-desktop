using ProyectoRefriPolar.Services;
using System;
using System.Windows;
using System.Windows.Threading;

namespace ProyectoRefriPolar
{
    public partial class App : Application
    {
        public App()
        {
            // Si la API no responde (p. ej. el servidor gratuito está arrancando),
            // se avisa al usuario en vez de cerrar la aplicación.
            DispatcherUnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            for (Exception? ex = e.Exception; ex != null; ex = ex.InnerException)
            {
                if (ex is ApiException apiError)
                {
                    MessageBox.Show(apiError.Message, "RefriPolar · Sin conexión con la API",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}
