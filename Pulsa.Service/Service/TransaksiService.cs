using AutoMapper;
using Microsoft.Extensions.Configuration;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Service
{
    public class TransaksiService : ITransaksiService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IProdukRepository _produk;
        IProdukDetailRepository _produkDetail;
        ISupplier_produkRepository _produkSuppliyer;
        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        public TransaksiService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IProdukRepository produkRepository,
            IProdukDetailRepository produkDetail,
            ISupplier_produkRepository produkSuppliyer,
            IMapper mapper,
            PulsaDataContext context
        ) {
            _unitOfWork= unitOfWork;
            _configuration= configuration;
            _produk = produkRepository;
            _produkDetail = produkDetail;
            _produkSuppliyer = produkSuppliyer;
            _mapper = mapper;
            _context = context;
        }

        public List<Domain.Entities.Produk> getAllProduk() { 
            return _produk.GetAll().ToList();
        }
        //public List<CariProdukDTO> saveDraft(string dest, string idProduk, string suppliyer)
        //{
            
        //}

    }
}
