using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using Xamarin.Forms;
using LoginTest.Interface;
using System.IO;
using LoginTest.Models;

namespace LoginTest
{
    class DataAccess : IDisposable
    {
        private SQLiteConnection connection;

        public DataAccess()
        {
            var config = DependencyService.Get<IConfig>();
            connection = new SQLiteConnection(config.Plattform, Path.Combine(config.DirectoryDB, "TestDb.db3"));
            connection.CreateTable<Users>();
        }

        /// <summary>
        /// Método para insertar Usuario
        /// </summary>
        /// <param name="user">Objeto User</param>
        public void insertUser(Users user)
        {
            connection.Insert(user);
        }

        /// <summary>
        /// Método para Actualizar un Usuario
        /// </summary>
        /// <param name="user">Objeto User</param>
        public void updateUser(Users user)
        {
            connection.Update(user);
        }

        /// <summary>
        /// Método para Eliminar un Usuario
        /// </summary>
        /// <param name="user">Objeto User</param>
        public void deleteUser(Users user)
        {
            connection.Delete(user);
        }

        /// <summary>
        /// Métódo para buscar un Usuario por Número de Identificación
        /// </summary>
        /// <param name="IdentificationNumber">IdentificationNumber</param>
        /// <returns>Users</returns>
        public Users GetUser(string IdentificationNumber)
        {
            return connection.Table<Users>().FirstOrDefault(c => c.IdentificationNumber == IdentificationNumber);
        }

        /// <summary>
        /// Método para Listar todos los usuarios
        /// </summary>
        /// <returns>List<Users></returns>
        public List<Users> ListUser()
        {
           return connection.Table<Users>().ToList();   
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
