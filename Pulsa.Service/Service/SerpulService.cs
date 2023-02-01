using Microsoft.Extensions.Configuration;
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
        IConfiguration _configuration;
        public string apiKey;
        public SerpulService(IUnitOfWork unitOfWork, IConfiguration configuration, SerpulHelper serpulHelper) {
            _unitOfWork = unitOfWork;
            _serpulHelper = serpulHelper;
            _configuration = configuration;
            apiKey = _configuration["email_template_path"];
        }

        public void getSaldo()
        {
            var saldo = _serpulHelper.getSaldo();

            var a = 0;

        }
    }
}
