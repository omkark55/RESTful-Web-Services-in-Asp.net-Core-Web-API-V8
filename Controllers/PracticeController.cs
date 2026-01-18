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

            string Authorization = string.Empty;
 if (Request.Headers.TryGetValue("Authorization", out var headerBearer))
 {
     Authorization = headerBearer.FirstOrDefault();
 }
 if (string.IsNullOrEmpty(TreeId) || string.IsNullOrEmpty(BranchId))
 {
     throw new Exception("BranchId or TennantId not found");
 }
        }
        [HttpGet("Get")]
        [Route("api/web/client/{domainModelName}/{parameter?}/get")]
        public async Task<IActionResult> Get(string domainModelName, string parameter = "")
        {
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                SetTenantDetails();

                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker"; //WebApi_Angular.Service.WebApi_Angular.Models.EmployeeBroker

                Type brokerType = Type.GetType($"{brokerNamespace}");
                if (brokerType == null || brokerType == null)
                    return BadRequest("Broker not found.");
              
                dynamic paramObject = JsonConvert.DeserializeObject(parameter);

                object repositoryInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);

                MethodInfo getMethod = brokerType.GetMethod("GetAll");
                if (getMethod == null)
                    return BadRequest("Get method not found in broker");

                 var invokeResult = getMethod.Invoke(repositoryInstance, new object[] { paramObject });

 object result;

 if (invokeResult is Task task)
 {
     await task.ConfigureAwait(false);

     var resultProperty = task.GetType().GetProperty("Result");
     result = resultProperty?.GetValue(task);
 }
 else
 {
     result = invokeResult;
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
            }
        }
        [ResponseCache(NoStore = true, Duration = 0, VaryByHeader = "None")]
        [HttpPost("post")]
        [Route("api/web/client/{domainModelName}/{parameter?}/post")]
        public async Task<IActionResult> Post(string domainModelName, [FromBody] JsonElement domainModelData)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool bBulkSave = false;

            if (domainModelData.TryGetProperty("postType", out JsonElement postTypeElement) &&
                postTypeElement.GetString()?.Equals("Get", StringComparison.OrdinalIgnoreCase) == true)
            {
                return await Get(domainModelName, domainModelData.ToString());
            }
            if (domainModelData.TryGetProperty("postType", out JsonElement postTypeSaveElement) &&
              postTypeElement.GetString()?.Equals("BulkSave", StringComparison.OrdinalIgnoreCase) == true)
            {
                bBulkSave = true;
            }
            try
            {
                SetTenantDetails();
               
                string modelNamespace = $"WebApi_Angular.Models.{domainModelName}";
                string brokerNamespace = $"WebApi_Angular.Service.{domainModelName}Broker";

                Type modelType = Type.GetType($"{modelNamespace}");
                Type brokerType = Type.GetType($"{brokerNamespace}");

                if (brokerType == null || brokerType == null)
                    return BadRequest("Type resolution failed.");

                if (!domainModelData.TryGetProperty("Model", out JsonElement modelElement))
                    return BadRequest("Missing 'Model'");

                object brokerInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);
              
                object modelInstance = null;

                if (!bBulkSave)
                {
                    modelInstance = JsonConvert.DeserializeObject(modelElement.GetRawText(), modelType);
                }
              
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
        [Authorize(Roles = "developer")]
        [HttpPut]
        public async Task<IActionResult> Put(string domainModelName, [FromBody] JsonElement domainModelData)
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
              
                object brokerInstance = ActivatorUtilities.CreateInstance(HttpContext.RequestServices, brokerType);

                MethodInfo editMethod = brokerType.GetMethod("Edit");
                if (editMethod == null)
                    return BadRequest("Edit method not found in broker");

                var invokeResult = editMethod.Invoke(brokerInstance, new object[] { domainModelInstance });

object result;

if (invokeResult is Task task)
{
    await task.ConfigureAwait(false);
    
    var resultProperty = task.GetType().GetProperty("Result");
    result = resultProperty?.GetValue(task);
}
else
{
    result = invokeResult;
}

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
        [Authorize(Roles = "admin")]
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
      
    }
}
