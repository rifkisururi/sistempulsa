﻿using Pulsa.Core.Interface;

namespace Pulsa.DataAccess.Interface
{
    public interface IUserSaldoHistoryRepository : IGenericRepository<Domain.Entities.user_saldo_history_detail>
    {
        // dibuat kosong jika tidak menambahkan spesial query yang digunakan terus menerus
    }
}
