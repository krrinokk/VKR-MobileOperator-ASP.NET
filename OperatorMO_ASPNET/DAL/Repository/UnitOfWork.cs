using OperatorMO_ASPNET.DAL.Models;
using OperatorMO_ASPNET.DAL.Repository;
namespace OperatorMO_ASPNET.DAL.Repository
{
    public class UnitOfWork : IDbRepos
    {
        private OperatorContext context = new OperatorContext();
        private GenericRepository<Contract> _dogovorRepository;
        private GenericRepository<Client> _клиентRepository;
        private GenericRepository<Tariff> _тарифRepository;
        private GenericRepository<ServicesConnected> _servicesconnectedRepository;
        private GenericRepository<Services> _servicesRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Chat> _chatRepository;
        private GenericRepository<CostDetails> _costdetailsRepository;
        private GenericRepository<Transactions> _transactionsRepository;
        private GenericRepository<CategoryTransaction> _categoryTransactionRepository;
        private GenericRepository<WriteOffs> _writeOffsRepository;
        private GenericRepository<Depositing> _depositingsRepository;

        private GenericRepository<EventsContract> _eventsRepository;

        private GenericRepository<CategoryEvents> _categoryEventsRepository;

        private readonly ILogger<UnitOfWork> _logger;

        // При создании экземпляра UnitOfWork передается ILogger, который используется для логирования информации о создании экземпляра и создании репозиториев.
        public UnitOfWork(ILogger<UnitOfWork> logger)
        {
            _logger = logger;
            _logger.LogInformation("Creating UnitOfWork instance");
        }

        // Каждый репозиторий представлен свойством с методом get, который при первом обращении создает экземпляр GenericRepository с соответствующим типом и контекстом.





        public GenericRepository<EventsContract> EventsRepository
        {
            get
            {
                try
                {
                    if (this._eventsRepository == null)
                    {
                        _logger.LogInformation("Creating EventsRepository instance");
                        this._eventsRepository = new GenericRepository<EventsContract>(context);
                    }
                    return _eventsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating EventsRepository");
                    throw new Exception("Error creating EventsRepository", ex);
                }
            }
        }




        public GenericRepository<CategoryEvents> CategoryEventsRepository
        {
            get
            {
                try
                {
                    if (this._categoryEventsRepository == null)
                    {
                        _logger.LogInformation("Creating CategoryEventsRepository instance");
                        this._categoryEventsRepository = new GenericRepository<CategoryEvents>(context);
                    }
                    return _categoryEventsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating CategoryEventsRepository");
                    throw new Exception("Error creating CategoryEventsRepository", ex);
                }
            }
        }






        public GenericRepository<WriteOffs> WriteOffsRepository
        {
            get
            {
                try
                {
                    if (this._writeOffsRepository == null)
                    {
                        _logger.LogInformation("Creating WriteOffsRepository instance");
                        this._writeOffsRepository = new GenericRepository<WriteOffs>(context);
                    }
                    return _writeOffsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating WriteOffsRepository");
                    throw new Exception("Error creating WriteOffsRepository", ex);
                }
            }
        }
        public GenericRepository<Depositing> DepositingsRepository
        {
            get
            {
                try
                {
                    if (this._depositingsRepository == null)
                    {
                        _logger.LogInformation("Creating DepositingsRepository instance");
                        this._depositingsRepository = new GenericRepository<Depositing>(context);
                    }
                    return _depositingsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating DepositingsRepository");
                    throw new Exception("Error creating DepositingsRepository", ex);
                }
            }
        }

        public GenericRepository<Contract> ContractRepository
        {
            get
            {
                try
                {
                    if (this._dogovorRepository == null)
                    {
                        _logger.LogInformation("Creating DogovorRepository instance");
                        this._dogovorRepository = new GenericRepository<Contract>(context);
                    }
                    return _dogovorRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating DogovorRepository");
                    throw new Exception("Error creating DogovorRepository", ex);
                }
            }
        }
        public GenericRepository<Transactions> TransactionsRepository
        {
            get
            {
                try
                {
                    if (this._transactionsRepository == null)
                    {
                        _logger.LogInformation("Creating TransactionsRepository instance");
                        this._transactionsRepository = new GenericRepository<Transactions>(context);
                    }
                    return _transactionsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating TransactionsRepository");
                    throw new Exception("Error creating TransactionsRepository", ex);
                }
            }
        }
        public GenericRepository<CategoryTransaction> CategoryTransactionRepository
        {
            get
            {
                try
                {
                    if (this._categoryTransactionRepository == null)
                    {
                        _logger.LogInformation("Creating CategoryTransactionRepository instance");
                        this._categoryTransactionRepository = new GenericRepository<CategoryTransaction>(context);
                    }
                    return _categoryTransactionRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating CategoryTransactionRepository");
                    throw new Exception("Error creating CategoryTransactionRepository", ex);
                }
            }
        }
        public GenericRepository<CostDetails> CostDetailsRepository
        {
            get
            {
                try
                {
                    if (this._costdetailsRepository == null)
                    {
                        _logger.LogInformation("Creating CostDetailsRepository instance");
                        this._costdetailsRepository = new GenericRepository<CostDetails>(context);
                    }
                    return _costdetailsRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating CostDetailsRepository");
                    throw new Exception("Error creating CostDetailsRepository", ex);
                }
            }
        }
        public GenericRepository<User> UserRepository
        {
            get
            {
                try
                {
                    if (this._userRepository == null)
                    {
                        _logger.LogInformation("Creating UserRepository instance");
                        this._userRepository = new GenericRepository<User>(context);
                    }
                    return _userRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating UserRepository");
                    throw new Exception("Error creating UserRepository", ex);
                }
            }
        }






        public GenericRepository<Chat> ChatRepository
        {
            get
            {
                try
                {
                    if (this._chatRepository == null)
                    {
                        _logger.LogInformation("Creating ChatRepository instance");
                        this._chatRepository = new GenericRepository<Chat>(context);
                    }
                    return _chatRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating ChatRepository");
                    throw new Exception("Error creating ChatRepository", ex);
                }
            }
        }




        public GenericRepository<ServicesConnected> ServicesConnectedRepository
        {
            get
            {
                try
                {
                    if (this._servicesconnectedRepository == null)
                    {
                        _logger.LogInformation("Creating ServicesConnectedRepository instance");
                        this._servicesconnectedRepository = new GenericRepository<ServicesConnected>(context);
                    }
                    return _servicesconnectedRepository;
                }
                catch (Exception ex)
                {
                    // Если при создании репозитория возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                    _logger.LogError(ex, "Error creating ServicesConnectedRepository");
                    throw new Exception("Error creating ServicesConnectedRepository", ex);
                }
            }
        }

        public GenericRepository<Client> ClientRepository
        {
            get
            {
                try
                {
                    if (this._клиентRepository == null)
                    {
                        _logger.LogInformation("Creating КлиентRepository instance");
                        this._клиентRepository = new GenericRepository<Client>(context);
                    }
                    return _клиентRepository;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating КлиентRepository");
                    throw new Exception("Error creating КлиентRepository", ex);
                }
            }
        }
        public GenericRepository<Services> ServicesRepository
        {
            get
            {
                try
                {
                    if (this._servicesRepository == null)
                    {
                        _logger.LogInformation("Creating ServicesRepository instance");
                        this._servicesRepository = new GenericRepository<Services>(context);
                    }
                    return _servicesRepository;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating ServicesRepository");
                    throw new Exception("Error creating ServicesRepository", ex);
                }
            }
        }

        public GenericRepository<Tariff> TariffRepository
        {
            get
            {
                try
                {
                    if (this._тарифRepository == null)
                    {
                        _logger.LogInformation("Creating ТарифRepository instance");
                        this._тарифRepository = new GenericRepository<Tariff>(context);
                    }
                    return _тарифRepository;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating ТарифRepository");
                    throw new Exception("Error creating ТарифRepository", ex);
                }
            }
        }



        // Метод Save() сохраняет изменения в контексте данных в базу данных.
        public int Save()
        {
            try
            {
                _logger.LogInformation("Saving changes to the database");
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                // Если при сохранении изменений возникает исключение, оно логируется и выбрасывается новое исключение с сообщением об ошибке.
                _logger.LogError(ex, "Error saving changes to the database");
                throw new Exception("Error saving changes to the database", ex);
            }
        }

        // Метод Dispose освобождает ресурсы, используемые контекстом данных, и вызывается при уничтожении объекта UnitOfWork.
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Освобождение ресурсов контекста данных.
                    //context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            // Сигнализация сборщику мусора о том, что объект уже освобожден.
            GC.SuppressFinalize(this);
        }
    }
}
