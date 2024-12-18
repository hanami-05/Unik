using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Linq
{
    public class BuisnesLogic
    {
        private List<User> users = new List<User>();
        private List<Record> records = new List<Record>();

        public BuisnesLogic()
        {
            /*Random r = new Random();


            for (int i = 0; i < 20; i++)
            {
                users.Add(new User(i + 1, $"User", $"{i}"));
            }

            for (int i = 0; i < 25; i++)
            {
                records.Add(new Record(users[r.Next(0, users.Count)], $"message number {i + 1}"));
            }*/

        }

        public void Access(List<User> users, List<Record> records) 
        {
            this.users = users;
            this.records = records;
        }

        public List<User> GetUsersBySurname(string surname) 
        {
            var selected = from user in users where user.Surname.Equals(surname) select user;
            return selected.ToList();
        }

        public User GetUserById(int id) 
        {
            var selected = from user in users where user.Id == id select user;
            return selected.Single();
        }

        public List<User> GetUserBySubstring(string sub) 
        {
            var selected = from user in users where user.Surname.Contains(sub) || user.Name.Contains(sub) select user;
            return selected.ToList();
        }

        public List<string> GetUniqueUserNames() 
        {
            var selected = from user in users select user.Name;
            return selected.Distinct().ToList();
        }

        public List<User> GetAllAuthors() 
        {
            var selected = from user in users join record in records on user equals record.Author select user;
            return selected.Distinct().ToList();   
        }

        public Dictionary<int, User> GetUsersDictionary()
        {
            return users.ToDictionary(user => user.Id, user => user); 
        }

        public int GetMaxID() 
        {
            var selected = from user in users select user.Id;
            return selected.Max();
        }

        public List<User> GetUserInOrder() 
        {
            var selected = from user in users orderby user.Id select user;
            return selected.ToList();
        }

        public List<User> GetUserInDescendingOrder()
        {
            var selected = from user in users orderby user.Id descending select user;
            return selected.ToList();
        }

        public List<User> GetReversedUsers() 
        {
            var selected = from user in users select user;
            return selected.Reverse().ToList();
        }

        public List<User> GetUserPages(int pageSize, int pageIndex) 
        {
            return users.Skip(pageSize*(pageIndex-1)).Take(pageSize).ToList();
        }
    }
}
