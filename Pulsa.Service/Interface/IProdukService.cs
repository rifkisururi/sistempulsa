﻿using Pulsa.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsa.Service.Interface
{
    public interface IProdukService
    {
        public List<Domain.Entities.Produk> getAllProduk();
        public List<CariProdukDTO> getProdukByType(string type, string brand);
        public string cekOperator(string dest);
        public CariProdukDTO getProdukSuppliyer(string produkId, string suppliyer);
    }
}
