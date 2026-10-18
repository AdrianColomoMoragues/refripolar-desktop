using ProyectoRefriPolar.Model;
using RestSharp;
using System.Collections.ObjectModel;

namespace ProyectoRefriPolar.Services
{
    class EmpleadosService
    {
        private const string Recurso = "empleados";

        // GET
        public ObservableCollection<Empleados> GetEmpleados() =>
            ApiClient.Get<ObservableCollection<Empleados>>(Recurso) ?? new ObservableCollection<Empleados>();

        public Empleados GetEmpleado(int id) => ApiClient.Get<Empleados>($"{Recurso}/{id}");

        public int GetMaxId() => ApiClient.GetMaxId(Recurso);

        // POST
        public RestResponse PostEmpleado(Empleados empleadoNuevo) =>
            ApiClient.Send(Recurso, Method.Post, empleadoNuevo);

        // PUT
        public RestResponse PutEmpleado(Empleados empleadoActualizar) =>
            ApiClient.Send(Recurso, Method.Put, empleadoActualizar);

        // DELETE
        public RestResponse DeleteEmpleado(int id) =>
            ApiClient.Send($"{Recurso}/{id}", Method.Delete);
    }
}
