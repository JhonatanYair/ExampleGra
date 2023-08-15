using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleGra.Models
{
    public partial class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int DepartamentId { get; set; }

        public virtual Departament Departament { get; set; }
    }
}
