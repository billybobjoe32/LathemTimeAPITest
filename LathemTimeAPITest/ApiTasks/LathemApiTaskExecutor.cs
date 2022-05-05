using LathemAPIDAL.DAL;
using Newtonsoft.Json;
using System.Configuration;
using LathemAPIDAL.Models;

namespace LathemTimeAPITest.ApiTasks
{
    public class LathemApiTaskExecutor
    {
        private ApiCaller LathemApiCaller = new ApiCaller();
        private string fileName = ConfigurationManager.AppSettings["EmployeeDataFileName"] ?? String.Empty;
        private List<Employee> employees;

        public LathemApiTaskExecutor()
        {
            employees = new List<Employee>();
        }

        public void InitializeEmployees()
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                if (File.Exists(fileName))
                {
                    string[] employeeData = File.ReadAllLines(fileName);
                    employees = JsonConvert.DeserializeObject<List<Employee>>(employeeData[0]);
                    if (employees == null)
                    {
                        employees = new List<Employee>();
                    }
                }
                else
                {
                    employees = new List<Employee>();
                    string fileStorageString = JsonConvert.SerializeObject(employees);
                    File.WriteAllText(fileName, fileStorageString);
                }
            }
        }

        public async Task DownloadUpdateReportEmployees()
        {
            List<Employee> newEmployees = await LathemApiCaller.GetEmployees();
            if(newEmployees.Count == 0)
            {
                Console.WriteLine("Failed to retrieve employees.");
            }
            string returnValue = String.Empty;
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("No file name specified in the configuration, failed to write data.");
            }
            else
            {
                if (CompareAndUpdate(newEmployees))
                {
                    string fileStorageString = JsonConvert.SerializeObject(employees);
                    File.WriteAllText(fileName, fileStorageString);
                }
                returnValue = "Id\tFirstName\tLastName\r\n";
                foreach (Employee employee in employees)
                {
                    returnValue += $"{employee.EmployeeId}\t{employee.FirstName}\t{employee.LastName}\r\n";
                }
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

        private bool CompareAndUpdate(List<Employee> newEmployees)
        {
            bool deletedEmployees = FindDeletedEmployeesAndRemove(newEmployees);
            bool addedEmployees = FindNewEmployeesAndAdd(newEmployees);
            bool updatedEmployees = FindExistingEmployeesAndUpdate(newEmployees);
            return deletedEmployees || addedEmployees || updatedEmployees;
        }

        private bool FindDeletedEmployeesAndRemove(List<Employee> newEmployees)
        {
            List<Employee> deletedEmployees = employees.Where(e1 => newEmployees.All(e2 => e1.EmployeeId != e2.EmployeeId)).ToList();
            foreach (Employee employee in deletedEmployees)
            {
                employees.Remove(employee);
            }
            return deletedEmployees.Count > 0;
        }

        private bool FindNewEmployeesAndAdd(List<Employee> newEmployees)
        {
            List<Employee> createdEmployees = newEmployees.Where(e1 => employees.All(e2 => e1.EmployeeId != e2.EmployeeId)).ToList();
            foreach (Employee employee in createdEmployees)
            {
                employees.Add(employee);
            }
            return createdEmployees.Count > 0;
        }

        private bool FindExistingEmployeesAndUpdate(List<Employee> newEmployees)
        {
            bool foundDifference = false;
            foreach(Employee employee in newEmployees)
            {
                Employee existingEmployee = employees.FirstOrDefault(ee => ee.EmployeeId == employee.EmployeeId);
                if(existingEmployee != null)
                {
                    if (!employee.Equals(existingEmployee))
                    {
                        existingEmployee.FirstName = employee.FirstName;
                        existingEmployee.LastName = employee.LastName;
                        foundDifference = true;
                    }
                }
            }
            return foundDifference;
        }
    }
}