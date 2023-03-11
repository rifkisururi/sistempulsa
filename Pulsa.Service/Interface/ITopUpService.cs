using pulsa.ViewModel;
using Pulsa.Domain.Entities;
using Pulsa.ViewModel;

namespace Pulsa.Service.Interface
{
    public interface ITopUpService
    {
        public bool add(InputTopUpDTO dt, Guid idPengguna);
        public int saldo(Guid idPengguna);
        public bool action(string action, Guid idPengguna, Guid idRequest);
        public List<VmRequestTopup> listRequestTopup();
        public List<VmRequestTopup> listRequestTopupHistory();
        
    }
}
