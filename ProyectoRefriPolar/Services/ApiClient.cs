using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;

namespace ProyectoRefriPolar.Services
{
    /// <summary>
    /// Error al comunicarse con la API REST. La aplicación lo captura en
    /// <see cref="App"/> y lo muestra al usuario en lugar de cerrarse.
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(string message, Exception? inner = null) : base(message, inner) { }
    }

    /// <summary>
    /// Cliente HTTP compartido por todos los servicios. La URL base se configura
    /// en <c>App.config</c> (ajuste <c>endpoint</c>).
    /// </summary>
    internal static class ApiClient
    {
        // La API de demostración está en un plan gratuito que se "duerme" tras un
        // rato sin uso: la primera petición puede tardar alrededor de un minuto.
        private const int TimeoutMs = 120_000;

        private static readonly RestClient client = new RestClient(
            new RestClientOptions(Properties.Settings.Default.endpoint) { MaxTimeout = TimeoutMs });

        /// <summary>GET que devuelve el JSON deserializado, o <c>default</c> si no hay datos (204/404).</summary>
        public static T Get<T>(string resource)
        {
            RestResponse response = client.Execute(new RestRequest(resource, Method.Get));
            EnsureReachable(response);

            if (response.StatusCode == HttpStatusCode.NoContent
                || response.StatusCode == HttpStatusCode.NotFound
                || string.IsNullOrWhiteSpace(response.Content))
            {
                return default!;
            }
            EnsureNoServerError(response);
            return JsonConvert.DeserializeObject<T>(response.Content)!;
        }

        /// <summary>Lee el último id registrado de un recurso (<c>/{recurso}/maxid</c>).</summary>
        public static int GetMaxId(string resource)
        {
            RestResponse response = client.Execute(new RestRequest($"{resource}/maxid", Method.Get));
            EnsureReachable(response);
            EnsureNoServerError(response);
            JArray jsonArray = JArray.Parse(response.Content!);
            return (int)jsonArray[0]["maxid"]!;
        }

        /// <summary>POST / PUT / DELETE. Si se indica <paramref name="body"/>, se envía como JSON.</summary>
        public static RestResponse Send(string resource, Method method, object? body = null)
        {
            var request = new RestRequest(resource, method);
            if (body != null)
            {
                string data = JsonConvert.SerializeObject(body);
                request.AddParameter("application/json", data, ParameterType.RequestBody);
            }
            RestResponse response = client.Execute(request);
            EnsureReachable(response);
            EnsureNoServerError(response);
            return response;
        }

        private static void EnsureReachable(RestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new ApiException(
                    "No se ha podido conectar con la API de RefriPolar.\n\n"
                    + "Si es la primera petición en un rato, el servidor puede estar arrancando "
                    + "(tarda alrededor de un minuto). Inténtalo de nuevo en unos segundos.\n\n"
                    + $"Servidor: {Properties.Settings.Default.endpoint}",
                    response.ErrorException);
            }
        }

        private static void EnsureNoServerError(RestResponse response)
        {
            if ((int)response.StatusCode >= 500)
            {
                throw new ApiException(
                    $"La API ha devuelto un error ({(int)response.StatusCode}). "
                    + "Puede que el servidor esté arrancando; inténtalo de nuevo en un minuto.");
            }
        }
    }
}
