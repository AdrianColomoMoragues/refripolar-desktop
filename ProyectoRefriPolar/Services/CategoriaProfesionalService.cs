using ProyectoRefriPolar.Model;
using System.Collections.ObjectModel;

namespace ProyectoRefriPolar.Services
{
    /// <summary>
    /// Las categorías profesionales son datos de referencia: la API solo permite consultarlas.
    /// </summary>
    class CategoriaProfesionalService
    {
        public ObservableCollection<CategoriaProfesional> GetCategoriasProfesionales() =>
            ApiClient.Get<ObservableCollection<CategoriaProfesional>>("categoriasprofesionales")
            ?? new ObservableCollection<CategoriaProfesional>();
    }
}
