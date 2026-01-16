using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using WebApi_Angular.DBContext;
using WebApi_Angular.Repository;

namespace WebApi_Angular.Controllers
{
    
    [ApiController]
    [ApiExplorerSettings(GroupName = "Practice API")]
    public class PracticeController : ControllerBase
    {
        private string TreeId;
        private string BranchId;
        PracticeRepository _globalDBContext;
        PracticeRepository practiceReposiory;
        WebApi_Angular.DBContext.PracticeDbContext practiceDbContext;
        //public PracticeController(PracticeDbContext dbContext, PracticeRepository practiceRepository)
        //{
        //    practiceDbContext = dbContext;
        //    practiceReposiory = practiceRepository;
        //}   
        public PracticeController( PracticeRepository practiceRepository)
        {
            practiceReposiory = practiceRepository;
        }
        private void SetTenantDetails()
        {
            string BranchCharId = string.Empty;
            if (Request.Headers.TryGetValue("TreeId", out var headerBranchCharId))
            {
                TreeId = headerBranchCharId.FirstOrDefault();
            }

            string SubBranchId = string.Empty;
            if (Request.Headers.TryGetValue("BranchId", out var headerSubBranchId))
            {
                BranchId = headerSubBranchId.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(TreeId) || string.IsNullOrEmpty(BranchId))
            {
                throw new Exception("BranchId or TennantId not found");
            }
        }
        [HttpGet("Get")]
        [Route("api/web/client/{domainModelName}/{parameter?}/get")]
        public IActionResult Get(string domainModelName, string parameter = "")
        {
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                SetTenantDetails();

                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                // string repositoryNamespace = "WebApi_Angular.Repository.PracticeRepository";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker"; //WebApi_Angular.Service.WebApi_Angular.Models.EmployeeBroker


                Type brokerType = Type.GetType($"{brokerNamespace}");
                if (brokerType == null || brokerType == null)
                    return BadRequest("Broker not found.");

                // Deserialize parameter into object matching the model
                dynamic paramObject = JsonConvert.DeserializeObject(parameter);

                //object repositoryInstance = Activator.CreateInstance(brokerType, true);
                object repositoryInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);

                MethodInfo getMethod = brokerType.GetMethod("GetAll");
                if (getMethod == null)
                    return BadRequest("Get method not found in broker");

                var result = getMethod.Invoke(repositoryInstance, new object[] { paramObject });

                //PracticeRepository practiceRepository = new PracticeRepository(practiceDbContext);
                // var result = practiceReposiory.GetAll(paramObject);

                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
            finally
            {
                //if (DbContext?.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                //    DbContext.Database.GetDbConnection().Close();
                //DbContext?.Dispose();
                if (practiceReposiory is IDisposable disposable)
                {
                    disposable.Dispose();
                }
         
                
            }
        }
        [ResponseCache(NoStore = true, Duration = 0, VaryByHeader = "None")]
        [HttpPost("post")]
        [Route("api/web/client/{domainModelName}/{parameter?}/post")]
        public IActionResult Post(string domainModelName, [FromBody] JsonElement domainModelData)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool bBulkSave = false;

            if (domainModelData.TryGetProperty("postType", out JsonElement postTypeElement) &&
                postTypeElement.GetString()?.Equals("Get", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Get(domainModelName, domainModelData.ToString());
            }
            if (domainModelData.TryGetProperty("postType", out JsonElement postTypeSaveElement) &&
              postTypeElement.GetString()?.Equals("BulkSave", StringComparison.OrdinalIgnoreCase) == true)
            {
                bBulkSave = true;
            }
            try
            {
                SetTenantDetails();

                //string modelNamespace = $"RichtechIT.CRM.Domain.MultiTenant.{domainModelName}";
                //string brokerNamespace = $"RichtechIT.CRM.Business.MultiTenant.{domainModelName}Broker";
                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker";


                //Type modelType = Type.GetType($"{modelNamespace}, RichtechIT.CRM.Domain");
                //Type brokerType = Type.GetType($"{brokerNamespace}, RichtechIT.CRM.Business");

                Type modelType = Type.GetType($"{modelNamespace}");
                Type brokerType = Type.GetType($"{brokerNamespace}");

                if (brokerType == null || brokerType == null)
                    return BadRequest("Type resolution failed.");

                if (!domainModelData.TryGetProperty("Model", out JsonElement modelElement))
                    return BadRequest("Missing 'Model'");

                //--------------Multitenant-------------
                //_MasterDbContext ??= Utilities.GetDataContext(Utilities.MasterConfigDatabaseName, domainModelName);
                //ClientDbContextFactory _factory = new ClientDbContextFactory(_MasterDbContext as RTMasterConfigDataContext);
                //ClientDBContext _clientContext = _factory.CreateClientContext(BranchId, TreeId);
                //-------------------------------------------

                //Step 4: Create broker instance and invoke Add method
                //object brokerInstance = _clientContext == null
                //    ? Activator.CreateInstance(brokerType)
                //    : Activator.CreateInstance(brokerType, _clientContext);

                object brokerInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);
                //For regular single object save 
                object modelInstance = null;

                if (!bBulkSave)
                {
                    modelInstance = JsonConvert.DeserializeObject(modelElement.GetRawText(), modelType);
                }
                //For bulk insert save
                else
                {
                    modelInstance = JsonConvert.SerializeObject(modelElement.GetRawText());
                }
                object result = null;

                if (!bBulkSave)
                {
                    MethodInfo addMethod = brokerType.GetMethod("Add");
                    result = addMethod?.Invoke(brokerInstance, new object[] { modelInstance});
                }
                else
                {
                    // 2) Extract the "Model" property (which is the array of records)
                    MethodInfo addAllMethod = brokerType.GetMethod("AddAll");
                    result = addAllMethod?.Invoke(brokerInstance, new object[] { domainModelData });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Exception: {ex.Message}");
            }
            finally
            {
                if (practiceReposiory is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                watch.Stop();
            }
        }

        [ResponseCache(NoStore = true, Duration = 0, VaryByHeader = "None")]
        [Route("api/web/client/{domainModelName}/{parameter?}/post")]
        [HttpPut]
        public IActionResult Put(string domainModelName, [FromBody] JsonElement domainModelData)
        {

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                SetTenantDetails();

                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker"; 


                Type modelType = Type.GetType($"{modelNamespace}");
                Type brokerType = Type.GetType($"{brokerNamespace}");

                if (modelType == null || brokerType == null)
                    return BadRequest("Type resolution failed.");

                if (!domainModelData.TryGetProperty("Model", out JsonElement modelElement))
                    return BadRequest("Missing 'Model'");

                object domainModelInstance = JsonConvert.DeserializeObject(modelElement.GetRawText(), modelType);


                //--------------Multitenant-------------
                //_MasterDbContext ??= Utilities.GetDataContext(Utilities.MasterConfigDatabaseName, domainModelName);
                //ClientDbContextFactory _factory = new ClientDbContextFactory(_MasterDbContext as RTMasterConfigDataContext);
                //ClientDBContext _clientContext = _factory.CreateClientContext(BranchId, TreeId);
                //-------------------------------------------

                //Step 4: Create broker instance and invoke Add method
                object brokerInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);

                MethodInfo editMethod = brokerType.GetMethod("Edit");
                if (editMethod == null)
                    return BadRequest("Edit method not found in broker");

                var result = editMethod.Invoke(brokerInstance, new object[] { domainModelInstance });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Exception occurred: {e.Message}");
            }
            finally
            {
                watch.Stop();
                if (practiceDbContext != null)
                {
                    if (practiceDbContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        practiceDbContext.Database.GetDbConnection().Close();

                    practiceDbContext.Dispose();
                }
            }

        }


        [ResponseCache(NoStore = true, Duration = 0, VaryByHeader = "None")]
        [Route("api/web/client/{domainModelName}/{parameter?}/post")]
        [HttpDelete]
        public IActionResult Delete(string domainModelName, [FromBody] JsonElement domainModelData)
        {

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                SetTenantDetails();

                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker";


                Type modelType = Type.GetType($"{modelNamespace}");
                Type brokerType = Type.GetType($"{brokerNamespace}");

                if (modelType == null || brokerType == null)
                    return BadRequest("Type resolution failed.");

                if (!domainModelData.TryGetProperty("Model", out JsonElement modelElement))
                    return BadRequest("Missing 'Model'");

                object domainModelInstance = JsonConvert.DeserializeObject(modelElement.GetRawText(), modelType);


                //--------------Multitenant------------ -
                //_MasterDbContext ??= Utilities.GetDataContext(Utilities.MasterConfigDatabaseName, domainModelName);
                //ClientDbContextFactory _factory = new ClientDbContextFactory(_MasterDbContext as RTMasterConfigDataContext);
                //ClientDBContext _clientContext = _factory.CreateClientContext(BranchId, TreeId);
                //-------------------------------------------

                //Step 4: Create broker instance and invoke Add method
                 object brokerInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);

                MethodInfo deleteMethod = brokerType.GetMethod("Delete");
                if (deleteMethod == null)
                    return BadRequest("Delete method not found in broker");

                var result = deleteMethod.Invoke(brokerInstance, new object[] { domainModelInstance });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest($"Exception occurred: {e.Message}");
            }
            finally
            {
                watch.Stop();
                if (practiceDbContext != null)
                {
                    if (practiceDbContext.Database.GetDbConnection().State == System.Data.ConnectionState.Open)
                        practiceDbContext.Database.GetDbConnection().Close();

                    practiceDbContext.Dispose();
                }
            }
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
