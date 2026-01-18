using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using WebApi_Angular.DBContext;
using WebApi_Angular.Models;
using WebApi_Angular.Repository;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi_Angular.Service
{
    public class EmployeeBroker
    {
        #region Data members and properties
        PracticeDbContext _DbContext = null;
        private PracticeRepository practiceRepository { get; set; }
        private WebApi_Angular.Models.Employee _Employee = null;
        #endregion

        #region Constructors
        public EmployeeBroker()
        {
            

        }
        public EmployeeBroker(PracticeDbContext dbContext,PracticeRepository repository)
        {
            _DbContext = dbContext;
            practiceRepository = repository ?? throw new ArgumentNullException(nameof(repository));

        }
        #endregion Constructors


        #region Methods and functions
        public async Task<ResponseResult> GetAll(dynamic parameters)
        {
            Type type = parameters.GetType();
            PropertyInfo propertyInfo = type.GetProperty("ListType");
             string lstType= parameters["Model"]?["ListType"]?.ToString();
            WebApi_Angular.Models.ResponseResult response = new ResponseResult();
            if (lstType == "GetById")
            {
                int id = Convert.ToInt32(parameters["Model"]?["Id"]);
                response.Result = await practiceRepository.GetById(id);
                response.Message = "ok";
                response.Status = "SUCCESS";
                response.StatusCode = 201;
                return response;
            }
            else
            {
                 
                response.Result = await practiceRepository.GetAll(parameters);
                response.Message = "ok";
                response.Status = "SUCCESS";
                response.StatusCode = 201;
                return response;
            }
        }

        public ResponseResult Add(Employee domainModel)
        {
            _Employee = new WebApi_Angular.Models.Employee
            {

                Name = domainModel.Name,
                Id = domainModel.Id,
                Gender = domainModel.Gender,
                Salary = domainModel.Salary,
                Department = domainModel.Department,
                RecordUptoDate = DateTime.Now,
                
            };
            practiceRepository.Insert(_Employee);


            return new ResponseResult
            {
                Message = "Record Saved Successfully",
                Status = "ok",
                StatusCode = 200,
            };
        }
        public Task<ResponseResult> Edit(Employee domainModel)
        {
            var existingEmployee = await practiceRepository.GetEntity(domainModel);
            if (existingEmployee == null)
            {
                return new ResponseResult
                {
                    Message = "Record not exist",
                    Status = "FAIL",
                    StatusCode = 404,
                    Result = null
                };
            }

            existingEmployee.Name = domainModel.Name;
            existingEmployee.Id = domainModel.Id;
            existingEmployee.Gender = domainModel.Gender;
            existingEmployee.Salary = domainModel.Salary;
            existingEmployee.Department = domainModel.Department;
            existingEmployee.RecordUptoDate = DateTime.Now;

          var res= await practiceRepository.UpdateEntity(existingEmployee);
            if(res.StatusCode==200){
            return new ResponseResult
            {
                Message = "Record Updated Successfully",
                Status = "Ok",
                StatusCode = res.StatusCode,
            };
           }
            else
            {
                return new ResponseResult
                {
                    Message = "Record not updated",
                    Status = "Fail",
                    StatusCode = res.StatusCode,
                };
            }
        }
        public ResponseResult Delete(Employee domainModel)
        {
            bool existingEmployee = practiceRepository.DeleteEntity(domainModel.Id);

            if (existingEmployee == true)
            {
                return new ResponseResult
                {
                    Message = "Record Deleted Successfully",
                    Status = "Ok",
                    StatusCode = 200,
                    Result = null
                };
            }
            else
            {
                return new ResponseResult
                {
                    Message = "Record Not Deleted",
                    Status = "fail",
                    StatusCode = 400,
                    Result = null
                };
            }
        }

        #endregion Methods and functions

    }
}

