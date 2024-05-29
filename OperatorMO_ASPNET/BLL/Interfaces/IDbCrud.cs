using OperatorMO_ASPNET.DAL.Models;
using OperatorMO_ASPNET.DAL.Repository;
using OperatorMO_ASPNET.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    // Определяем интерфейс для работы с базой данных
    public interface IDbCrud
    {

        IEnumerable<Depositing> GetDepositingsForContract(int contractId);
        IEnumerable<WriteOffs> GetWriteOffsForContract(int contractId);

        IEnumerable<WriteOffs> GetAllWriteOffs();

        WriteOffs GetWriteOffs(int Id);

        void CreateWriteOffs(WriteOffs writeOffs);

        IEnumerable<CategoryTransaction> GetAllCategory();


        // Получение всех Clientов из базы данных
        IEnumerable<Client> GetAllClient();
        // Получение Clientа по его идентификатору
        Client GetClient(int clvId);
        // Создание нового Clientа в базе данных
        void CreateClient(Client cl);
        // Обновление информации о Clientе в базе данных
        void UpdateClient(Client cl);
        // Удаление Clientа из базы данных по его идентификатору
        void DeleteClient(int id);

        // Получение всех Tariffов из базы данных
        IEnumerable<Tariff> GetAllTariff();
        // Получение Tariffа по его идентификатору
        Tariff GetTariff(int trfId);
        // Создание нового Tariffа в базе данных
        void CreateTariff(Tariff trf);
        // Обновление информации о Tariffе в базе данных
        void UpdateTariff(Tariff trf);
        // Удаление Tariffа из базы данных по его идентификатору
        void DeleteTariff(int id);

        // Получение всех Contractов из базы данных
        IEnumerable<Contract> GetAllContract();
        // Получение Contractа по его идентификатору
        Contract GetContract(int dgvId);
        // Создание нового Contractа в базе данных
        void CreateContract(Contract dgv);
        // Обновление информации о Contractе в базе данных
        void UpdateContract(Contract dgv);
        // Удаление Contractа из базы данных по его идентификатору
        void DeleteContract(int id);



     



        void CreateServiceConnected(ServicesConnected ServiceConnected);
        IEnumerable<ServicesConnected> GetAllServicesConnected();

        ServicesConnected GetServicesConnected(int Id);
    
        void UpdateServicesConnected(ServicesConnected service);

        void DeleteServicesConnected(int contractId, int serviceId);










        IEnumerable<CostDetails> GetAllCostDetails();

        CostDetails GetCostDetailsByContractIdFK(int contractIdFK);

        //void UpdateServicesConnected(ServicesConnected service);

        //void DeleteServicesConnected(int id);

        IEnumerable<EventsContract> GetAllEvents();

        IEnumerable<CategoryEvents> GetAllCategoryEvents();


        void CreateTransactions(Transactions Transactions);
  
        void UpdateTransactions(Transactions Transactions);
        IEnumerable<Transactions> GetAllTransactions();
        Transactions GetTransaction(int Id);



        IEnumerable<CategoryTransaction> GetAllCategoryTransaction();
        CategoryTransaction GetCategoryTransaction(int Id);

        void CreateEvents(EventsContract events);




        IEnumerable<Chat> GetAllMessages();

        Chat GetMessage(int Id);

        void UpdateMessage(Chat message);

        void DeleteMessage(int id);




        IEnumerable<Services> GetAllServices();

        Services GetServices(int Id);

        void UpdateServices(Services service);

        void DeleteServices(int messageId);
        void CreateMessage(Chat message);



        void CreateUser(User user);
        IEnumerable<User> GetAllUser();
        User GetUser(int Id);
        void UpdateUser(User user);
        void DeleteUser(int id);




        // Сохранение изменений в базе данных
        int Save();
    }
}