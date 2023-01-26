using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Helper;
using Pulsa.Service.Interface;

namespace Pulsa.Service.Service
{
    public class SerpulService : ISerpulService
    {
        IUnitOfWork _unitOfWork;
        SerpulHelper _serpulHelper;
        public SerpulService(IUnitOfWork unitOfWork, SerpulHelper serpulHelper) {
            _unitOfWork = unitOfWork;
            _serpulHelper = serpulHelper;
        }
        public void getSaldo()
        {
            var saldo = _serpulHelper.getSaldo();

            var a = 0;

        }
    }
}
