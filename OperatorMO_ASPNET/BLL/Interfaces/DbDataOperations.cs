using BLL.Interfaces;

using System.Collections.Generic;
using System.Linq;
using OperatorMO_ASPNET.DAL.Models;
using System.Collections.ObjectModel;
using OperatorMO_ASPNET.DAL.Models;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OperatorMO_ASPNET.DAL.Repository;
using Microsoft.EntityFrameworkCore;


namespace BLL.Interfaces
{
    // Класс для работы с данными из базы данных
    public class DbDataOperations : IDbCrud
    {
        IDbRepos db;

        // Конструктор класса, принимающий интерфейс IDbRepos
        public DbDataOperations(IDbRepos repos)
        {
            this.db = repos;
        }
        public void CreateServiceConnected(ServicesConnected ServiceConnected)
        {
            db.ServicesConnectedRepository.Insert(ServiceConnected);
            Save();
        }
        // Получение всех 
        public IEnumerable<WriteOffs> GetAllWriteOffs()
        {
            return db.WriteOffsRepository.Get();
        }
        public void CreateWriteOffs(WriteOffs WriteOffs)
        {
            db.WriteOffsRepository.Insert(WriteOffs);
            Save();
        }
        // Получение по ID
        public WriteOffs GetWriteOffs(int Id)

        {
            return db.WriteOffsRepository.GetByID(Id);
        }
        public IEnumerable<WriteOffs> GetWriteOffsForContract(int contractId)
        {
            return db.WriteOffsRepository.Get().Where(w => w.ContractId_FK == contractId).ToList();
        }
        public IEnumerable<Depositing> GetDepositingsForContract(int contractId)
        {
            return db.DepositingsRepository.Get().Where(w => w.ContractId_FK == contractId).ToList();
        }

        //// Получение всех 
        //public IEnumerable<Services> GetAllServices()
        //{
        //    return db.ServicesRepository.Get();
        //}
        //// Получение по ID
        //public Services GetServices(int Id)

        //{
        //    return db.ServicesRepository.GetByID(Id);
        //}



        // Получение всех 
        public IEnumerable<CategoryEvents> GetAllCategoryEvents()
        {
            return db.CategoryEventsRepository.Get();
        }


        // Получение всех 
        public IEnumerable<EventsContract> GetAllEvents()
        {
            return db.EventsRepository.Get();
        }


        // Создание нового 
        public void CreateEvents(EventsContract events)
        {
            db.EventsRepository.Insert(events);
            Save();
        }


        // Получение всех 
        public IEnumerable<CategoryTransaction> GetAllCategory()
        {
            return db.CategoryTransactionRepository.Get();
        }



        // Получение всех 
        public IEnumerable<Services> GetAllServices()
        {
            return db.ServicesRepository.Get();
        }
        // Получение по ID
        public Services GetServices(int Id)

        {
            return db.ServicesRepository.GetByID(Id);
        }


        // Создание нового 
        public void CreateServices(Services Services)
        {
            db.ServicesRepository.Insert(Services);
            Save();
        }

        // Обновление информации 
        public void UpdateServices(Services Services)
        {
            db.ServicesRepository.Update(Services);
            Save();
        }

        // Удаление  по ID
        public void DeleteServices(int id)
        {
            db.ServicesRepository.Delete(id);
            Save();
        }















        // Получение всех 
        public IEnumerable<CostDetails> GetAllCostDetails()
        {
            return db.CostDetailsRepository.Get();
        }
        // Получение по ID
        public CostDetails GetCostDetailsByContractIdFK(int contractIdFK)

        {
            return db.CostDetailsRepository.GetByID(contractIdFK);
        }


        // Получение всех 
        public IEnumerable<CategoryTransaction> GetAllCategoryTransaction()
        {
            return db.CategoryTransactionRepository.Get();
        }
        // Получение по ID
        public CategoryTransaction GetCategoryTransaction(int Id)

        {
            return db.CategoryTransactionRepository.GetByID(Id);
        }




        //// Создание нового 
        //public void CreateServices(Services Services)
        //{
        //    db.ServicesRepository.Insert(Services);
        //    Save();
        //}

        //// Обновление информации 
        //public void UpdateServices(Services Services)
        //{
        //    db.ServicesRepository.Update(Services);
        //    Save();
        //}

        //// Удаление  по ID
        //public void DeleteServices(int id)
        //{
        //    db.ServicesRepository.Delete(id);
        //    Save();
        //}






        // Получение всех 
        public IEnumerable<Transactions> GetAllTransactions()
        {
            return db.TransactionsRepository.Get();
        }

        // Получение по ID
        public Transactions GetTransaction(int Id)

        {
            return db.TransactionsRepository.GetByID(Id);
        }



        // Создание нового 
        public void CreateTransactions(Transactions Transactions)
        {
            db.TransactionsRepository.Insert(Transactions);
            Save();
        }

        // Обновление информации 
        public void UpdateTransactions(Transactions Transactions)
        {
            db.TransactionsRepository.Update(Transactions);
            Save();
        }






        //// Получение всех 
        //public IEnumerable<Chat> GetAllMessages()
        //{
        //    return db.ChatRepository.Get();
        //}
        //// Получение по ID
        //public Chat GetMessage(int Id)

        //{
        //    return db.ChatRepository.GetByID(Id);
        //}


        //// Создание нового 
        //public void CreateMessage(Chat message)
        //{
        //    db.ChatRepository.Insert(message);
        //    Save();
        //}

        //// Обновление информации 
        //public void UpdateMessages(Chat message)
        //{
        //    db.ChatRepository.Update(message);
        //    Save();
        //}

        //// Удаление  по ID
        //public void DeleteMessage(int id)
        //{
        //    db.ChatRepository.Delete(id);
        //    Save();
        //}






        // Получение всех 
        public IEnumerable<User> GetAllUser()
        {
            return db.UserRepository.Get();
        }
        // Получение по ID
        public User GetUser(int Id)

        {
            return db.UserRepository.GetByID(Id);
        }


        // Создание нового 
        public void CreateUser(User user)
        {
            db.UserRepository.Insert(user);
            Save();
        }

        // Обновление информации 
        public void UpdateUser(User user)
        {
            db.UserRepository.Update(user);
            Save();
        }

        // Удаление  по ID
        public void DeleteUser(int id)
        {
            db.UserRepository.Delete(id);
            Save();
        }








        // Создание нового 
        public void CreateMessage(Chat message)
        {
            db.ChatRepository.Insert(message);
            Save();
        }

        // Получение всех сообщений
        public IEnumerable<Chat> GetAllMessages()
        {
            return db.ChatRepository.Get();
        }
        // Получение по ID
        public Chat GetMessage(int Id)

        {
            return db.ChatRepository.GetByID(Id);
        }


        // Создание нового 
        public void CreateMessage(User user)
        {
            db.UserRepository.Insert(user);
            Save();
        }

        // Обновление информации - редактирование информации
        public void UpdateMessage(Chat message)
        {
            db.ChatRepository.Update(message);
            Save();
        }

        // Удаление  по ID
        public void DeleteMessage(int messageId)
        {
            db.ChatRepository.Delete(messageId);
            Save();
        }




        // Получение договора по ID
        public Contract GetContract(int id)
        {
            return db.ContractRepository.GetByID(id);
        }

        // Получение всех договоров
        public IEnumerable<Contract> GetAllContract()
        {
            return db.ContractRepository.Get();
        }

        // Создание нового договора
        public void CreateContract(Contract Contract)
        {
            db.ContractRepository.Insert(Contract);
            Save();
        }

        // Обновление информации о договоре
        public void UpdateContract(Contract Contract)
        {
            db.ContractRepository.Update(Contract);
            Save();
        }

        // Удаление договора по ID
        public void DeleteContract(int id)
        {
            db.ContractRepository.Delete(id);
            Save();
        }

        // Получение Clientа по ID
        public Client GetClient(int id)
        {
            return db.ClientRepository.GetByID(id);
        }

        // Получение всех Clientов
        public IEnumerable<Client> GetAllClient()
        {
            return db.ClientRepository.Get();
        }

        // Создание нового Clientа
        public void CreateClient(Client Client)
        {
            db.ClientRepository.Insert(Client);
            Save();
        }

        // Обновление информации о Clientе
        public void UpdateClient(Client Client)
        {
            db.ClientRepository.Update(Client);
            Save();
        }

        // Удаление Clientа по ID
        public void DeleteClient(int id)
        {
            db.ClientRepository.Delete(id);
            Save();
        }

        // Получение Tariffа по ID
        public Tariff GetTariff(int id)
        {
            return db.TariffRepository.GetByID(id);
        }

        // Получение всех Tariffов
        public IEnumerable<Tariff> GetAllTariff()
        {
            return db.TariffRepository.Get();
        }

        // Создание нового Tariffа
        public void CreateTariff(Tariff Tariff)
        {
            db.TariffRepository.Insert(Tariff);
            Save();
        }

        // Обновление информации о Tariffе
        public void UpdateTariff(Tariff Tariff)
        {
            db.TariffRepository.Update(Tariff);
            Save();
        }

        // Удаление Tariffа по ID
        public void DeleteTariff(int id)
        {
            db.TariffRepository.Delete(id);
            Save();
        }



        
        // Получение всех 
        public IEnumerable<ServicesConnected> GetAllServicesConnected()
        {
            return db.ServicesConnectedRepository.Get();
        }
        // Получение по ID
        public ServicesConnected GetServicesConnected(int Id)

        {
            return db.ServicesConnectedRepository.GetByID(Id);
        }


        // Создание нового 
        public void CreateServicesConnected(ServicesConnected ServicesConnected)
        {
            db.ServicesConnectedRepository.Insert(ServicesConnected);
            Save();
        }

        // Обновление информации 
        public void UpdateServicesConnected(ServicesConnected ServicesConnected)
        {
            db.ServicesConnectedRepository.Update(ServicesConnected);
            Save();
        }

        // Удаление  по ID
        public void DeleteServicesConnected(int contractId, int serviceId)
        {
            var connectedService = db.ServicesConnectedRepository.Get()
                .FirstOrDefault(sc => sc.ContractId_FK == contractId && sc.ServiceId_FK == serviceId);
            if (connectedService != null)
            {
                db.ServicesConnectedRepository.Delete(connectedService.CSId);
                Save();
            }
            else
            {
                throw new Exception($"Connected service with serviceId {serviceId} and contractId {contractId} not found.");
            }
        }





        // Сохранение изменений в базе данных
        public int Save()
        {
            if (db.Save() > 0) return 0;
            return 1;
        }

    
    }
}