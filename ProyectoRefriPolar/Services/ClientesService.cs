using ProyectoRefriPolar.Model;
using RestSharp;
using System.Collections.ObjectModel;

namespace ProyectoRefriPolar.Services
{
    class ClientesService
    {
        private const string Recurso = "clientes";

        // GET
        public ObservableCollection<Clientes> GetClientes() =>
            ApiClient.Get<ObservableCollection<Clientes>>(Recurso) ?? new ObservableCollection<Clientes>();

        public Clientes GetCliente(int id) => ApiClient.Get<Clientes>($"{Recurso}/{id}");

        public int GetMaxId() => ApiClient.GetMaxId(Recurso);

        // POST
        public RestResponse PostCliente(Clientes clienteNuevo) =>
            ApiClient.Send(Recurso, Method.Post, clienteNuevo);

        // PUT
        public RestResponse PutCliente(Clientes clienteActualizar) =>
            ApiClient.Send(Recurso, Method.Put, clienteActualizar);

        // DELETE
        public RestResponse DeleteCliente(int id) =>
            ApiClient.Send($"{Recurso}/{id}", Method.Delete);
    }
}
