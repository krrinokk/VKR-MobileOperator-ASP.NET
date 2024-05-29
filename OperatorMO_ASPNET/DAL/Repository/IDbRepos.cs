using OperatorMO_ASPNET.DAL.Models;
namespace OperatorMO_ASPNET.DAL.Repository
{
    public interface IDbRepos
    {

        GenericRepository<CategoryTransaction> CategoryTransactionRepository { get; }
        // Задаем свойство КлиентRepository типа GenericRepository с параметром Клиент
        GenericRepository<Client> ClientRepository { get; }
        // Задаем свойство ТарифRepository типа GenericRepository с параметром Тариф
        GenericRepository<Tariff> TariffRepository { get; }
        // Задаем свойство DogovorRepository типа GenericRepository с параметром Dogovor
        GenericRepository<Contract> ContractRepository { get; }
        GenericRepository<User> UserRepository { get; }
        GenericRepository<Chat> ChatRepository { get; }
        GenericRepository<ServicesConnected> ServicesConnectedRepository { get; }
        //GenericRepository<Chat> ChatRepository { get; }
        // Задаем метод Save для сохранения изменений в базе данных
        GenericRepository<Services> ServicesRepository { get; }
        GenericRepository<CostDetails> CostDetailsRepository { get; }
        GenericRepository<Transactions> TransactionsRepository { get; }
        GenericRepository<WriteOffs> WriteOffsRepository { get; }
        GenericRepository<Depositing> DepositingsRepository { get; }

        GenericRepository<EventsContract> EventsRepository { get; }


        GenericRepository<CategoryEvents> CategoryEventsRepository { get; }
        int Save();
    }
}
