using Newtonsoft.Json;

namespace LathemAPIDAL.Models
{
    public class Employee
    {
        [JsonProperty("EmployeeId")]
        public int EmployeeId { get; set; }
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        public string LastName { get; set; }

        public override bool Equals(object? obj)
        {
            Employee newEe = (Employee)obj;
            if(newEe == null) return false;
            return newEe.FirstName == FirstName && newEe.LastName == LastName;
        }
    }
}
