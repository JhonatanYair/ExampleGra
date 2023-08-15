using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleGra.Models
{
    public partial class Departament
    {
        public Departament()
        {
            Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Employee> Employee { get; set; }
    }
}
