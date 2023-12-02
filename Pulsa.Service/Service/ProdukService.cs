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
    public class ProdukService : IProdukService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IProdukRepository _produk;
        IProdukDetailRepository _produkDetail;
        ISupplier_produkRepository _produkSuppliyer;
        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        public ProdukService(
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
        public List<CariProdukDTO> getProdukByType(string type, string brand, string typeProduk)
        {
            if (!string.IsNullOrEmpty(typeProduk))
                brand = "";

            if (brand.ToLower() == "xl" && type.ToLower() == "pulsa")
                brand = "Axis";

            var data = (from p in _context.produks.Where(a =>
                            (a.brand.Contains(brand) || brand == "")
                            && a.category.ToLower() == type.ToLower()
                            && ( a.type_produk.ToLower() == typeProduk.ToLower() || typeProduk == "" )
                        )
                        join pd in _context.produk_details on p.product_id.ToLower() equals pd.product_id.ToLower()
                        join ps in _context.supplier_produks on pd.suppliyer_product_id.ToLower() equals ps.product_id.ToLower()
                        where
                            pd.suppliyer.ToLower() == ps.supplier.ToLower()

                        select new CariProdukDTO
                        {
                            product_id = p.product_id,
                            product_name = p.product_name,
                            product_detail = p.product_detail,
                            product_syarat = p.product_syarat,
                            product_zona = p.product_zona,
                            suppliyer = pd.suppliyer,
                            price = (p.margin ?? 0) + ps.product_price + (p.bagihasil1 ?? 0) + (p.bagihasil2 ?? 0),
                            price_suggest = p.price_suggest ?? 0
                        }).Distinct();
            return data.ToList();
        }

        public CariProdukDTO getProdukSuppliyer(string produkId, string suppliyer)
        {
            var data = (from p in _context.produks
                        join pd in _context.produk_details on p.product_id.ToLower() equals pd.product_id.ToLower()
                        join ps in _context.supplier_produks on pd.suppliyer_product_id.ToLower() equals ps.product_id.ToLower() 
                        where
                            pd.suppliyer.ToLower() == ps.supplier.ToLower()
                            && pd.suppliyer.ToLower() == suppliyer.ToLower()
                            && pd.suppliyer_product_id.ToLower() == produkId.ToLower()
                        select new CariProdukDTO
                        {
                            product_id = p.product_id,
                            product_name = p.product_name,
                            product_detail = p.product_detail,
                            product_syarat = p.product_syarat,
                            product_zona = p.product_zona,
                            suppliyer = pd.suppliyer,
                            price = (p.margin ?? 0) + ps.product_price + (p.bagihasil1 ?? 0) + (p.bagihasil2 ?? 0),
                            price_suggest = p.price_suggest ?? 0
                        }).FirstOrDefault();
            return data;
        }

        public string cekOperator(string dest) {
            string operatorName = string.Empty;
            string[] prefixIndosat = { "0814", "0815", "0816", "0855", "0856", "0857", "0858" };
            string[] prefixTelkomsel = { "0811", "0812", "0813", "0821", "0822", "0823", "0852", "0853", "0851" };
            string[] prefixTri = { "0895", "0896", "0897", "0898", "0899" };
            string[] prefixAxis = { "0838", "0831", "0832", "0833"};
            string[] prefixXl = { "0817", "0818", "0819", "0859", "0877", "0878"};
            string[] prefixSMART = { "0881", "0882", "0883", "0884", "0885", "0886", "0887", "0888", "0889" };
            string[] prefixByu= { "085155", "085156", "085157", "085158" };

            if (prefixIndosat.Any(prefix => dest.StartsWith(prefix)))
            {
                operatorName = "Indosat";
            }
            else if(prefixTelkomsel.Any(prefix => dest.StartsWith(prefix))){
                if (prefixByu.Any(prefix => dest.StartsWith(prefix)))
                {
                    operatorName = "By.U";
                }
                else {
                    operatorName = "Telkomsel";
                }
            }
            else if(prefixTri.Any(prefix => dest.StartsWith(prefix))){
                operatorName = "Tri";
            }
            else if(prefixAxis.Any(prefix => dest.StartsWith(prefix)))
            {
                operatorName = "Axis";
            }
            else if(prefixXl.Any(prefix => dest.StartsWith(prefix)))
            {
                operatorName = "XL";
            }
            else if(prefixSMART.Any(prefix => dest.StartsWith(prefix)))
            {
                operatorName = "Smartfren";
            }

            return operatorName;
        }

        public IQueryable<string> listTypeProduk(string category) {

            var list = _produk.Find(a => a.category.ToLower() == category.ToLower()).Select(a => a.type_produk).Distinct();
            return list;

        }
    }
}
