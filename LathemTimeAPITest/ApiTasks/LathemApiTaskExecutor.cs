using LathemAPIDAL.DAL;
using Newtonsoft.Json;
using System.Configuration;
using LathemAPIDAL.Models;
using Database;

namespace LathemTimeAPITest.ApiTasks
{
    public class LathemApiTaskExecutor
    {
        private ApiCaller LathemApiCaller = new ApiCaller();

        private List<Employee> employees;
        private IEmployeeDB db;

        public LathemApiTaskExecutor(IEmployeeDB _db)
        {
            employees = new List<Employee>();
            db = _db;
        }

        public void InitializeEmployees()
        {
            employees = db.GetAll();
        }

        public async Task DownloadUpdateReportEmployees()
        {
            List<Employee> newEmployees = await LathemApiCaller.GetEmployees();
            if(newEmployees.Count == 0)
            {
                Console.WriteLine("Failed to retrieve employees.");
            }
            string returnValue = String.Empty;
            CompareAndUpdate(newEmployees);
            returnValue = "Id\tFirstName\tLastName\r\n";
            foreach (Employee employee in employees)
            {
                returnValue += $"{employee.EmployeeId}\t{employee.FirstName}\t{employee.LastName}\r\n";
            }
            Console.WriteLine(returnValue);
        }

        public async Task RandomEmployeePunch(DateTime punchTime)
        {
            if(employees.Count == 0)
            {
                Console.WriteLine("No employees to punch in");
            }
            Random r = new Random();
            int employeeIndex = r.Next(0, employees.Count);
            ApiCaller caller = new ApiCaller();
            Employee ee = employees[employeeIndex];
            Punch punch = new Punch
            {
                EmployeeId = ee.EmployeeId,
                CurrentLocalTime = punchTime
            };
            PunchResponse punchResponse = await caller.SendPunch(punch);
            string inOrOut = punchResponse.IsPunchedIn ? "in" : "out";
            Console.WriteLine($"{ee.FirstName} {ee.LastName} clock {inOrOut} time was {punchTime.ToShortDateString()} {punchTime.ToShortTimeString()}");
            Console.WriteLine();
        }

        private void CompareAndUpdate(List<Employee> newEmployees)
        {
            bool deletedEmployees = false;
            bool updatedEmployees = false;
            bool createdEmployees = false;
            List<Employee> currentDeletedEmployees = employees.Where(e1 => newEmployees.All(e2 => e1.EmployeeId != e2.EmployeeId)).ToList();
            foreach (Employee employee in currentDeletedEmployees)
            {
                db.Delete(employee);
                deletedEmployees = true;
            }
            foreach(Employee newEmployee in newEmployees)
            {
                bool foundEmployee = false;
                foreach (Employee currentEmployee in employees)
                {
                    if(newEmployee.EmployeeId == currentEmployee.EmployeeId)
                    {
                        foundEmployee = true;
                        if (!newEmployee.Equals(currentEmployee))
                        {
                            db.Update(newEmployee);
                            updatedEmployees = true;
                        }
                    }
                }
                if (!foundEmployee)
                {
                    db.Create(newEmployee);
                    createdEmployees = true;
                }
            }
            // Get latest employees if any updating occurred
            if (deletedEmployees || updatedEmployees || createdEmployees)
            {
                employees = db.GetAll();
            }
        }
    }
}