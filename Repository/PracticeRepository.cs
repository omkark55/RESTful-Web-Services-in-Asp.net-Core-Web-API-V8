using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Reflection;
using WebApi_Angular.Common;
using WebApi_Angular.DBContext;
using WebApi_Angular.Models;

namespace WebApi_Angular.Repository
{
    public class PracticeRepository : BaseRepository<Employee>
    {
        private readonly PracticeDbContext _DbContext;
        public DataPager pager { get; private set; }
        private readonly IMemoryCache memoryCache;
        private const string GetAllProductsCacheKey = "GetAllProducts";
       
        public PracticeRepository(PracticeDbContext dbContext, IMemoryCache cache) : base(dbContext, cache)
        {
            _DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            memoryCache = cache ?? throw new ArgumentNullException("not cached");
        }

        public IEnumerable<Employee> GetAll(dynamic parameters)
        {
            try
            {
                Employee oFilter = new Employee();
                if (parameters != null)
                {
                    oFilter = JsonConvert.DeserializeObject<Employee>(Convert.ToString(parameters.Model));
                    pager = JsonConvert.DeserializeObject<DataPager>(Convert.ToString(parameters.DataPager)) ?? new DataPager();
                }
                if (_DbContext == null)
                {
                    return Enumerable.Empty<Employee>();
                }

                if (oFilter == null)
                {
                    return Enumerable.Empty<Employee>();
                    //return new List<Employee>();
                }

                if (pager == null)
                {
                    return new List<Employee>();
                }
                if (memoryCache.TryGetValue(GetAllProductsCacheKey, out IEnumerable<Employee> employees))
                {
                    return employees;
                }
                var query = (from t1 in _DbContext.Set<Employee>()
                             where string.IsNullOrEmpty(oFilter.Name) || t1.Name.ToUpper().Trim() == oFilter.Name.ToUpper().Trim()
                             select new Employee
                             {
                                 Id=t1.Id,
                                 Name = t1.Name,
                                 Department = t1.Department,
                                 Gender = t1.Gender,
                                 Salary=(int)t1.Salary,
                                 RecordUptoDate = t1.RecordUptoDate
                             });
                pager.RecordCount = query.Count();
                

                    var result = query
                    .OrderBy(x => x.Name)
                    .ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes

                memoryCache.Set(GetAllProductsCacheKey, result, cacheEntryOptions);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        public Employee GetById(int id)
        {
            try
            {
                Employee oFilter = new Employee();
                
                if (_DbContext == null)
                {
                    return new Employee();
                }
                
                var query = (from t1 in _DbContext.Set<Employee>()
                             where  t1.Id == id
                             select new Employee
                             {
                                 Id = t1.Id,
                                 Name = t1.Name,
                                 Department = t1.Department,
                                 Gender = t1.Gender,
                                 Salary = (int)t1.Salary,
                                 RecordUptoDate = t1.RecordUptoDate
                             });

                var result = query
                    .OrderBy(x => x.Name)
                    .FirstOrDefault();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
      
        public void Insert(Employee employee)
        {
            try
            {
                _DbContext.Add(employee);
                _DbContext.SaveChanges();
            }
            catch (SqlException ex)
            {

                ex.ToString();
            }
        }

        public Employee GetEntity(Employee employee)
        {
            return Get(employee, employee);
        }

        public ResponseResult UpdateEntity(Employee employee)
        {

            return Update(employee);
        }

        public bool DeleteEntity(int Id)
        {
            return SoftDelete(Id);
        }

    }
}
