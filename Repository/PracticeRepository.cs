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
        //public PracticeRepository() : base(null)
        //{
        //    //pager = new DataPager();
        //}
        //public PracticeRepository() //: base(null)
        //{

        //}
        public PracticeRepository(PracticeDbContext dbContext, IMemoryCache cache) : base(dbContext, cache)
        {
           // this.pager = new DataPager();
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
                    //.Skip((int)pager.Skip)
                    //.Take(pager.PageSize)
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
                    //.Skip((int)pager.Skip)
                    //.Take(pager.PageSize)
                    .FirstOrDefault();

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
        //public IEnumerable<Presentation.GlobalDBMaster.IndustryMaster> GetAll(dynamic parameters, IUserInfo userInfo)
        //{

        //    Presentation.GlobalDBMaster.IndustryMaster oFilter = new Presentation.GlobalDBMaster.IndustryMaster();
        //    if (parameters != null)
        //    {
        //        oFilter = JsonConvert.DeserializeObject<Presentation.GlobalDBMaster.IndustryMaster>(Convert.ToString(parameters.Model));
        //        pager = JsonConvert.DeserializeObject<DataPager>(Convert.ToString(parameters.DataPager)) ?? new DataPager();
        //    }
        //    pager.Normalize(defaultSortBy: "IndustryDescription", defaultPageSize: Global.PageSize);

        //    #region Old code before MVC Client
        //    //// Start with base query
        //    //IQueryable<IndustryMaster> query = _globalDBContext.IndustryMaster;
        //    //// Apply filter only if PlotNumber is provided
        //    //if (oFilter != null && !string.IsNullOrEmpty(oFilter.IndustryID.ToString()))
        //    //{
        //    //    var filterPlot = oFilter.IndustryID.ToString().Trim().ToUpper();
        //    //    query = query.Where(t => t.IndustryID.ToString().Trim().ToUpper() == filterPlot);
        //    //}
        //    #endregion

        //    var query = (from t1 in _globalDBContext.Set<Domain.GlobalDBMaster.IndustryMaster>()
        //                 where
        //                 (string.IsNullOrEmpty(oFilter.IndustryDescription) || t1.IndustryDescription.ToUpper().Contains(oFilter.IndustryDescription.ToUpper())) &&
        //                 (oFilter.IndustryID == 0 || t1.IndustryID == oFilter.IndustryID)

        //                 select new Presentation.GlobalDBMaster.IndustryMaster
        //                 {
        //                     IndustryDescription = t1.IndustryDescription,
        //                     Active = t1.Active,
        //                     IndustryID = t1.IndustryID,
        //                     RecordUptoDate = t1.RecordUptoDate
        //                     //IndustryGuid = t1.EntityId

        //                 });

        //    pager.RecordCount = query.Count();

        //    var result = query
        //        .OrderBy(x => x.IndustryID)
        //        .Skip(pager.Skip)
        //        .Take(pager.PageSize)
        //        .ToList();

        //    return result;

        //}

        //protected override void Dispose(bool disposing)
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}

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
           //dynamic parameters = JsonConvert.SerializeObject(employee);
            return Get(employee, employee);
           
            //if (parameters == null)
            //    throw new ArgumentNullException(nameof(parameters));
            //int id = Convert.ToInt32(parameters["Model"]?["Id"]);
            //return _DbContext.Set<Employee>().Find(id);
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
