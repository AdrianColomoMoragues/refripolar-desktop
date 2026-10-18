using ProyectoRefriPolar.Model;
using RestSharp;
using System.Collections.ObjectModel;

namespace ProyectoRefriPolar.Services
{
    class EncargosService
    {
        private const string Recurso = "encargos";

        // GET
        public ObservableCollection<Encargos> GetEncargos() =>
            ApiClient.Get<ObservableCollection<Encargos>>(Recurso) ?? new ObservableCollection<Encargos>();

        public Encargos GetEncargo(int id) => ApiClient.Get<Encargos>($"{Recurso}/{id}");

        public int GetMaxId() => ApiClient.GetMaxId(Recurso);

        // POST
        public RestResponse PostEncargo(Encargos encargoNuevo) =>
            ApiClient.Send(Recurso, Method.Post, encargoNuevo);

        // PUT
        public RestResponse PutEncargo(Encargos encargoActualizar)
        {
            // Si no se indica encargado, se conserva el que ya tenía el encargo
            if (encargoActualizar.idEncargado == null)
            {
                encargoActualizar.idEncargado = GetEncargo(encargoActualizar.id).idEncargado;
            }
            return ApiClient.Send(Recurso, Method.Put, encargoActualizar);
        }

        // DELETE
        public RestResponse DeleteEncargo(int id) =>
            ApiClient.Send($"{Recurso}/{id}", Method.Delete);
    }
}
