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
using System.Globalization;
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
        IPenggunaMutasiRepository _penggunaMutasi;
        IPenggunaRepository _pengguna;
        ISerpulService _serpul;
        IDflashService _dflash;
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
            IPenggunaMutasiRepository penggunaMutasi,
            ISerpulService serpul,
            IDflashService dflash,
            PulsaDataContext context
        ) {
            _unitOfWork= unitOfWork;
            _configuration= configuration;
            _produk = produkRepository;
            _produkDetail = produkDetail;
            _produkSuppliyer = produkSuppliyer;
            _penggunaTransaksi = penggunaTransaksi;
            _pengguna = pengguna;
            _penggunaMutasi = penggunaMutasi;
            _serpul = serpul;
            _mapper = mapper;
            _context = context;
            _dflash = dflash;
        }

        public List<Domain.Entities.Produk> getAllProduk()
        {
            return _produk.GetAll().ToList();
        }
        public Guid transaksi(string product_id, string suppliyer, string dest, Guid pengguna)
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
            pt.pengguna = pengguna;
            pt.ref_id = GenerateUniqueString(); 

            _penggunaTransaksi.Add(pt);
            _penggunaTransaksi.Save();


            return pt.id;
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

        public string fixorder(Guid id)
        {
            var transaksi = _penggunaTransaksi.GetById(id);
            var pengguna = _pengguna.Find(a => a.id == transaksi.pengguna).SingleOrDefault();
            if (pengguna.saldo > transaksi.harga_jual) {

                // update status transaksi
                transaksi.status_transaksi = 1;
                _penggunaTransaksi.Update(transaksi);
                _penggunaTransaksi.Save();

                // log mutasi saldo
                var dataMutasi = new Pengguna_mutasi();
                dataMutasi.saldo_sebelum = pengguna.saldo;
                dataMutasi.saldo_sesudah = pengguna.saldo - transaksi.harga_jual;
                dataMutasi.mutasi = -1* transaksi.harga_jual;
                dataMutasi.type_transaksi = "Pembelian";
                dataMutasi.id_transaksi = transaksi.id;
                dataMutasi.pengguna_id = transaksi.pengguna;
                _penggunaMutasi.Add(dataMutasi);
                _penggunaMutasi.Save();

                // potong saldo
                pengguna.saldo = pengguna.saldo - transaksi.harga_jual;
                _pengguna.Update(pengguna);
                _pengguna.Save();

                // hit ppob server
                if (transaksi.suppliyer.ToLower() == "dflash")
                {
                    _dflash.order(transaksi.product_id, transaksi.tujuan, transaksi.ref_id);
                }

                return "1";
                
            }
            return "0";
        }

        public List<MutasiDTO> listMutasi(Guid idPengguna, int start, int jumlah)
        {
            var transaksi = _penggunaMutasi.Find(a => a.pengguna_id == idPengguna)
                .OrderByDescending(a => Convert.ToInt64(a.createdAt))
                .Take(jumlah+start).Skip(start).ToList();
            var listMutasi = new List<MutasiDTO>();
            
            foreach(var item in transaksi)
            {
                var dtMutasi = new MutasiDTO();
                dtMutasi.id = item.id_transaksi;
                dtMutasi.type = item.type_transaksi;
                dtMutasi.saldo_sebelum = item.saldo_sebelum.ToString("N0");
                dtMutasi.saldo_sesudah= item.saldo_sesudah.ToString("N0");
                dtMutasi.jumlah_mutasi= item.mutasi.ToString("N0");
                dtMutasi.created_at = item.createdAt;
                if (dtMutasi.type.ToLower() == "pembelian") {
                    var detailMutasi = _penggunaTransaksi.GetById(item.id_transaksi);                    
                    var detailProduk = _produkSuppliyer.Find(a => a.supplier == detailMutasi.suppliyer && a.product_id == detailMutasi.product_id).FirstOrDefault();
                    dtMutasi.produk = detailProduk?.product_name;
                    string noteTransaksi = detailMutasi.tujuan + " <br>"; 
                    if (detailMutasi.status_transaksi == 1) {
                        noteTransaksi += " sedang di proses";
                    }
                    else if (detailMutasi.status_transaksi == 2)
                    {
                        noteTransaksi += " sukses SN " + detailMutasi.sn;
                    }
                    else if (detailMutasi.status_transaksi == 3)
                    {
                        noteTransaksi += " gagal " + detailMutasi.sn;
                    }

                    dtMutasi.note = noteTransaksi;
                    listMutasi.Add(dtMutasi);
                }
            }

            return listMutasi;
        }


        public async Task<DetailTransaksiDTO> detailTransaksi(Guid id) {
            var detailTransaksi = _penggunaTransaksi.GetById(id);
            if (detailTransaksi.suppliyer == "dflash" && detailTransaksi.status_transaksi == 1) {
                await _dflash.cekTransaksiPending(detailTransaksi);
            }

            var dtMutasi = new DetailTransaksiDTO();
            var detailMutasi = _penggunaMutasi.Find(a => a.id_transaksi == id).FirstOrDefault();
            var detailProduk = _produkSuppliyer.Find(a => a.supplier == detailTransaksi.suppliyer && a.product_id == detailTransaksi.product_id).FirstOrDefault();

            dtMutasi.id = id;
            dtMutasi.dest = detailTransaksi.tujuan;
            dtMutasi.product_name = detailProduk.product_name;
            dtMutasi.product_detail = detailProduk.product_detail;
            dtMutasi.product_syarat = detailProduk.product_syarat;
            dtMutasi.product_zona = detailProduk.product_zona;
            dtMutasi.product_id = detailProduk.product_id;
            dtMutasi.created_at = detailProduk.updated_at;
            dtMutasi.price_buyer = detailTransaksi.harga_jual_agen.GetValueOrDefault().ToString("C");
            dtMutasi.saldo_sebelum = detailMutasi.saldo_sebelum.ToString("C");
            dtMutasi.saldo_sesudah = detailMutasi.saldo_sesudah.ToString("C");
            dtMutasi.jumlah_mutasi = detailMutasi.mutasi.ToString("C");
            dtMutasi.nama_pembeli = "";
            dtMutasi.sn = detailTransaksi.sn;
            dtMutasi.status = detailTransaksi.status_transaksi;

            return dtMutasi;
        }

        private static string GenerateUniqueString()
        {
            DateTime now = DateTime.Now;
            string month = now.Month.ToString("D2"); // D2 menghasilkan dua digit
            string day = now.Day.ToString("D2"); // D2 menghasilkan dua digit
            string year = now.Year.ToString();
            string sequenceNumber = GenerateRandom(4);

            // Menggabungkan semua komponen menjadi satu string
            string uniqueString = $"{year}{month}{day}{sequenceNumber}";

            return uniqueString;
        }

        private static string GenerateRandom(int length)
        {
            const string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            string randomString = new string(Enumerable.Repeat(alphanumericChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomString;
        }
    }
}
