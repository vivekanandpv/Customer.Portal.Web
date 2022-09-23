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
    public class BankCustomerService : IBankCustomerService {
        private readonly CustomerPortalContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BankCustomerService> _logger;

        public BankCustomerService(CustomerPortalContext context, IMapper mapper, ILogger<BankCustomerService> logger) {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BankCustomerViewModel>> GetAllAsync() {
            var customers = await _context
                .Customers
                .Include(c => c.Accounts)
                .ToListAsync();

            return customers
                .Select(c => {
                    var viewModel = _mapper.Map<BankCustomer, BankCustomerViewModel>(c);

                    var accounts = c.Accounts.Select(a => _mapper.Map<Account, AccountViewModel>(a)).ToList();

                    viewModel.Accounts = accounts;

                    return viewModel;
                });
        }

        public async Task<BankCustomerViewModel> GetByIdAsync(int id) {
            var customerDb = await GetCustomer(id);
            var viewModel = _mapper.Map<BankCustomer, BankCustomerViewModel>(customerDb);
            
            var accounts = customerDb
                .Accounts
                .Where(a => a.CustomerId == id)
                .Select(a => _mapper.Map<Account, AccountViewModel>(a))
                .ToList();
            
            viewModel.Accounts = accounts;

            return viewModel;
        }

        public async Task<BankCustomerViewModel> CreateAsync(BankCustomerCreateViewModel customer) {
            var customerEntity = _mapper.Map<BankCustomerCreateViewModel, BankCustomer>(customer);
            await _context.AddAsync(customerEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<BankCustomer, BankCustomerViewModel>(customerEntity);
        }

        public async Task<BankCustomerViewModel> UpdateAsync(int id, BankCustomerUpdateViewModel customer) {
            if (id != customer.Id) {
                throw new DomainInvariantException($"Discrepancy in the customer {id} and {customer.Id}");
            }
            
            var customerDb = await GetCustomer(id);
            _mapper.Map<BankCustomerUpdateViewModel, BankCustomer>(customer, customerDb);

            await _context.SaveChangesAsync();

            return _mapper.Map<BankCustomer, BankCustomerViewModel>(customerDb);
        }

        public async Task DeleteAsync(int id) {
            var customerDb = await GetCustomer(id);

            _context.Customers.Remove(customerDb);

            await _context.SaveChangesAsync();
        }

        private async Task<BankCustomer> GetCustomer(int id) {
            var customerDb = await _context
                .Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customerDb == null) {
                throw new RecordNotFoundException($"Could not find the customer with id: {id}");
            }

            return customerDb;
        }
    }
}