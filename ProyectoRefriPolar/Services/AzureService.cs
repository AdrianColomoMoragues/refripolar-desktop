using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Windows;

namespace ProyectoRefriPolar.Services
{
    /// <summary>
    /// Fotos de los empleados en Azure Blob Storage.
    ///
    /// La cadena de conexión NO está en el código: se lee de la variable de entorno
    /// <c>REFRIPOLAR_AZURE_STORAGE</c>. Sin ella, la aplicación funciona igual y las
    /// fotos existentes se siguen viendo (son URLs públicas), pero no se pueden subir fotos nuevas.
    /// </summary>
    class AzureService
    {
        public const string VariableEntorno = "REFRIPOLAR_AZURE_STORAGE";
        private const string Contenedor = "2dam";
        private const string UrlBase = "https://colmor.blob.core.windows.net/" + Contenedor + "/";

        private static string? CadenaConexion =>
            Environment.GetEnvironmentVariable(VariableEntorno);

        public static bool SubidaConfigurada => !string.IsNullOrWhiteSpace(CadenaConexion);

        /// <summary>Devuelve la URL pública de una imagen a partir de su nombre o URL.</summary>
        public string url(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta) || ruta.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return ruta;
            }
            return UrlBase + Uri.EscapeDataString(ruta);
        }

        /// <summary>
        /// Si <paramref name="rutaImagen"/> es un archivo local, lo sube al contenedor y devuelve su URL.
        /// Si ya es una URL, la devuelve tal cual.
        /// </summary>
        public string? guarda(string rutaImagen)
        {
            if (rutaImagen.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return rutaImagen;
            }

            if (!SubidaConfigurada)
            {
                MessageBox.Show(
                    "La subida de fotos no está configurada en este equipo, así que el empleado "
                    + "se guardará sin foto.\n\n"
                    + $"Para activarla, define la variable de entorno {VariableEntorno} "
                    + "con la cadena de conexión de Azure Storage (ver README).",
                    "RefriPolar", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            var contenedor = new BlobServiceClient(CadenaConexion).GetBlobContainerClient(Contenedor);
            string nombreImagen = Path.GetFileName(rutaImagen);
            var blob = contenedor.GetBlobClient(nombreImagen);

            if (!blob.Exists())
            {
                using Stream stream = File.OpenRead(rutaImagen);
                contenedor.UploadBlob(nombreImagen, stream);
            }
            return blob.Uri.AbsoluteUri;
        }
    }
}
