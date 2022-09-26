using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Customer.Portal.Web.Context;
using Customer.Portal.Web.Exceptions;
using Customer.Portal.Web.Models;
using Customer.Portal.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Customer.Portal.Web.Services { 
    public class AccountService : IAccountService {
        private readonly CustomerPortalContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(CustomerPortalContext context, IMapper mapper, ILoggerFactory loggerFactory) {
            _context = context;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<AccountService>();
        }

        public async Task<IEnumerable<AccountViewModel>> GetAllAsync() {
            return await _context.Accounts.Select(a => _mapper.Map<Account, AccountViewModel>(a)).ToListAsync();
        }

        public async Task<AccountViewModel> GetByIdAsync(int id) {
            var accountDb = await GetAccount(id);

            var viewModel = _mapper.Map<AccountViewModel>(accountDb);
            var customerViewModel = _mapper.Map<BankCustomer, BankCustomerViewModel>(accountDb.Customer);
            viewModel.Customer = customerViewModel;

            return viewModel;
        }

        public async Task<IEnumerable<AccountViewModel>> GetByCustomerAsync(int id) {
            return await _context.Accounts.Where(a => a.CustomerId == id).Select(a => _mapper.Map<AccountViewModel>(a)).ToListAsync();
        }

        public async Task<AccountViewModel> CreateAsync(AccountCreateViewModel account) {
            var accountEntity = _mapper.Map<AccountCreateViewModel, Account>(account);
            await _context.Accounts.AddAsync(accountEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Account, AccountViewModel>(accountEntity);
        }

        public async Task<AccountViewModel> UpdateAsync(int id, AccountUpdateViewModel account) {
            if (id != account.Id) {
                throw new DomainInvariantException($"Discrepancy in the account {id} and {account.Id}");
            }

            var accountDb = await GetAccount(id, false);
            _mapper.Map<AccountUpdateViewModel, Account>(account, accountDb);

            await _context.SaveChangesAsync();
            
            return _mapper.Map<Account, AccountViewModel>(accountDb);
        }

        public async Task DeleteAsync(int id) {
            var accountDb = await GetAccount(id, false);

            _context.Accounts.Remove(accountDb);
            await _context.SaveChangesAsync();
        }
        
        private async Task<Account> GetAccount(int id, bool withCustomer = true) {
            IQueryable<Account> query = _context.Accounts;

            if (withCustomer) {
                query = query.Include(a => a.Customer);
            }
            
            var accountDb = await query.FirstOrDefaultAsync(c => c.Id == id);

            if (accountDb == null) {
                throw new RecordNotFoundException($"Could not find the account with id: {id}");
            }

            return accountDb;
        }
    }
}