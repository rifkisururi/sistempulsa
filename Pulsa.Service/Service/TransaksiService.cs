using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Pulsa.Data;
using Pulsa.DataAccess.Interface;
using Pulsa.DataAccess.Repository;
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
        IPenggunaTransaksiRepository _penggunaTransaksi;
        IPenggunaRepository _pengguna;
        ISerpulService _serpul;
        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        public TransaksiService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IProdukRepository produkRepository,
            IProdukDetailRepository produkDetail,
            ISupplier_produkRepository produkSuppliyer,
            IMapper mapper,
            IPenggunaTransaksiRepository penggunaTransaksi,
            IPenggunaRepository pengguna,
            ISerpulService serpul,
            PulsaDataContext context
        ) {
            _unitOfWork= unitOfWork;
            _configuration= configuration;
            _produk = produkRepository;
            _produkDetail = produkDetail;
            _produkSuppliyer = produkSuppliyer;
            _penggunaTransaksi = penggunaTransaksi;
            _pengguna = pengguna;
            _serpul = serpul;
            _mapper = mapper;
            _context = context;
        }

        public List<Domain.Entities.Produk> getAllProduk()
        {
            return _produk.GetAll().ToList();
        }
        public Guid transaksi(string product_id, string suppliyer, string dest, Guid penggunan)
        {

            var produkDetailSuppiyer = _produkSuppliyer.Find(a => a.product_id == product_id && a.supplier == suppliyer).FirstOrDefault();
            var produk = _produk.Find(a => a.product_id == product_id).FirstOrDefault();

            Pengguna_Traksaksi pt = new Pengguna_Traksaksi();
            pt.product_id = produk.product_id;
            pt.suppliyer = suppliyer;
            pt.tujuan = dest;
            pt.harga = produkDetailSuppiyer.product_price;
            pt.harga_jual = (produk.margin ?? 0) + produkDetailSuppiyer.product_price + (produk.bagihasil1 ?? 0) + (produk.bagihasil2 ?? 0);
            pt.status_transaksi = 0;
            pt.pengguna = penggunan;

            _penggunaTransaksi.Add(pt);
            _penggunaTransaksi.Save();


            return pt.id;
            //return _produk.GetAll().ToList();
        }

        public Pengguna_Traksaksi getDetailTransaksi(Guid id)
        {
            return _penggunaTransaksi.GetById(id);
        }

        public bool verifikasiPin(Guid id, string pin)
        {
            var pengguna = _pengguna.Find(a => a.id == id ).FirstOrDefault();
            if (pengguna.pin != pin) {
                pengguna.gagal = pengguna.gagal + 1;
                if (pengguna.gagal == 3) {
                    pengguna.isBlokir = true;
                }
                _pengguna.Update(pengguna);
                _pengguna.Save();
                return false;
            }
            return true;
        }

        public void fixorder(Guid id)
        {
            var transaksi = _penggunaTransaksi.GetById(id);
            var pengguna = _pengguna.Find(a => a.id == transaksi.pengguna).SingleOrDefault();
            if (pengguna.saldo > transaksi.harga_jual) {
                transaksi.status_transaksi = 1;
                _penggunaTransaksi.Update(transaksi);
                _penggunaTransaksi.Save();

                // potong saldo

                // hit ppob server
                if (transaksi.suppliyer.ToLower() == "serpul")
                {
                    // hit serpul
                    String refId = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                    _serpul.orderPrabayar(transaksi.product_id, transaksi.tujuan, Convert.ToString(transaksi.id));
                }

                // log mutasi saldo
                


            }


        }

    }
}
