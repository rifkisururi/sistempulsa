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
    public class PenggunaService : IPenggunaService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _configuration;
        IPenggunaRepository _pengguna;
        private IMapper _mapper;
        private readonly PulsaDataContext _context;
        public PenggunaService(
            IPenggunaRepository pengguna,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            PulsaDataContext context
        ) {
            _pengguna = pengguna;
            _unitOfWork= unitOfWork;
            _configuration= configuration;
            _mapper = mapper;
            _context = context;
        }

        public Domain.Entities.Pengguna cekPengguna(Guid id, string fullname, string email) {
            var data = _pengguna.Find(a => a.id == id).SingleOrDefault();
            if (data == null)
            {
                var pengguna = new Pengguna();
                pengguna.id = id;
                pengguna.nama = fullname;
                pengguna.email = email;
                _pengguna.Add(pengguna);
                _pengguna.Save();
            }

            return data;
        }
    }
}
