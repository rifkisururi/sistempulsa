using AutoMapper;
using Microsoft.Extensions.Configuration;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Service
{
    public class ProdukService : IProdukService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IProdukRepository _produk;
        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        public ProdukService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IProdukRepository produkRepository,
            IMapper mapper,
            PulsaDataContext context
        ) {
            _unitOfWork= unitOfWork;
            _configuration= configuration;
            _produk = produkRepository;
            _mapper = mapper;
            _context = context;
        }

        public List<Domain.Entities.Produk> getAllProduk() { 
            return _produk.GetAll().ToList();
        }
    }
}
