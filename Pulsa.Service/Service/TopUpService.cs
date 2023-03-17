using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using pulsa.ViewModel;
using Pulsa.DataAccess.Interface;
using Pulsa.Domain.Entities;
using Pulsa.Service.Interface;
using Pulsa.ViewModel;

namespace Pulsa.Service.Service
{
    public class TopUpService : ITopUpService
    {

        Pulsa.DataAccess.Interface.IUnitOfWork _unitOfWork;
        Pulsa.DataAccess.Interface.ITagihanMasterRepository _tagihanMaster;
        Pulsa.DataAccess.Interface.ITopupRepository _topupRepository;
        Pulsa.DataAccess.Interface.IPenggunaRepository _penggunaRepository;
        IUserSaldoHistoryRepository _userSaldoRepository;
        //Pulsa.DataAccess.Interface.ITagihanDetailRepository _tagihanDetail;
        private IMapper _mapper;

        public TopUpService(
            IUnitOfWork unitOfWork,
            ITagihanMasterRepository tagihanMasterRepository,
            ITopupRepository topupRepository,
            IMapper mapper,
            IPenggunaRepository penggunaRepository,
            IUserSaldoHistoryRepository userSaldoHistoryRepository
            //ITagihanDetailRepository tagihanDetailRepository
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tagihanMaster = tagihanMasterRepository;
            _topupRepository = topupRepository;
            _penggunaRepository= penggunaRepository;
            _userSaldoRepository = userSaldoHistoryRepository;
            //_tagihanDetail= tagihanDetailRepository;
        }
        public bool add(InputTopUpDTO dt, Guid idPengguna)
        {
            TopUp tm = _mapper.Map<TopUp>(dt);
            tm.idpengguna = idPengguna; 
            tm.id = Guid.NewGuid();
            // todo:rubah ke _unitOfWork
            _topupRepository.Add(tm);
            _topupRepository.Save();
            var save = _unitOfWork.Complete();
            return save;


        }

        public List<VmRequestTopup> listRequestTopup() { 
            var request = _topupRepository.Find( a => a.status == 1 ).Include( a => a.pengguna).ToList();
            var dataRequest = _mapper.Map<List<VmRequestTopup>>(request);
            return dataRequest;
        }

        public List<VmRequestTopup> listRequestTopupHistory()
        {
            var request = _topupRepository.Find(a => a.status != 1).Include(a => a.pengguna).ToList();
            var dataRequest = _mapper.Map<List<VmRequestTopup>>(request);
            return dataRequest;
        }

        public bool action(string action, Guid idPengguna, Guid idRequest)
        {
            var dataTopUp = _topupRepository.GetById(idRequest);
            // referss saldo dari table history
            
            // get saldo awal sebelum request
            var pengguna = _penggunaRepository.Find(a => a.id == dataTopUp.idpengguna).SingleOrDefault();
            int saldoAwal = pengguna.saldo;
            int saldoAhir = saldoAwal + dataTopUp.jumlah;

            // update data tbl topup
            // set saldo sebelum dan sesudah
            dataTopUp.action_by = idPengguna;
            dataTopUp.saldo_awal = saldoAwal;
            dataTopUp.saldo_akhir = saldoAhir;
            dataTopUp.waktu_action = Convert.ToString(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            if (action.ToLower() == "approve") {
                dataTopUp.status = 2;
                _topupRepository.Update(dataTopUp);

                // update saldo di tbl pengguna
                pengguna.saldo = saldoAhir;
                _penggunaRepository.Update(pengguna);

                // insert ke table history saldo
                user_saldo_history_detail his = new user_saldo_history_detail();
                his.id_transaksi = idRequest;
                his.idpengguna = dataTopUp.idpengguna;
                _userSaldoRepository.Add(his);

                _topupRepository.Save();
                _penggunaRepository.Save();
                _userSaldoRepository.Save();
            }
            else if (action.ToLower() == "reject")
            {
                dataTopUp.status = 3;
                _topupRepository.Save();
            }
            // todo:rubah ke _unitOfWork

            var resutl = _unitOfWork.Complete();
            return resutl;
        }
        public int saldo(Guid idPengguna) {
            return _penggunaRepository.GetById(idPengguna).saldo;
        }

    }
}
