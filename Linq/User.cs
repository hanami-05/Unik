using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public User(int id, string name, string surname) 
        {
            Id = id;
            Name = name;
            Surname = surname;
        }

        public override string ToString()
        {
            return $"ID = {Id}: {Name} {Surname}";
        }
    }
}
