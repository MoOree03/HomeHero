﻿using HomeHero.Data;
using HomeHero.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace HomeHero.Services
{
    public class HomeHeroServices
    {
        readonly HomeHeroContext _context;
        public HomeHeroServices(HomeHeroContext context)
        {
            _context = context;
        }
        public async Task<bool> AddUser(string name, string surname, int location, string email, string password)
        {
            if (ExistEmail(email)) return false;
            else
            {
                byte[] salt = GenerateSalt();
                _context.Users.Add(new User
                {
                    NamesUser = name,
                    SurnamesUser = surname,
                    LocationResidenceID = location,
                    Email = email,
                    Salt = salt,
                    Password = HashPassword(password, salt)
                });
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public static byte[] GenerateSalt()
        {
            Random random = new Random();
            int number = random.Next(0, 255);
            var salt = new byte[number];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);
            return salt;
        }
        public static string HashPassword(string password, byte[] salt)
        {
            byte[] saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedPassword = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashedPassword) ;
            }
        }
        private bool ExistEmail(string email)
        {
            var query = from users in _context.Users
                        where users.Email == email
                        select users;
            if (query.Count() > 0) return true;
            else return false;
        }
        public User? LogInUser(string email, string password)
        {

            User user = _context.Users.SingleOrDefault(x => x.Email == email);
            if (user == null)
                return null;
            else
            {         
                // Codificamos la contraseña a bytes (asumimos que está en formato UTF-8)
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Concatenamos la contraseña y la sal
                byte[] saltedPassword = new byte[passwordBytes.Length + user.Salt.Length];
                passwordBytes.CopyTo(saltedPassword, 0);
                user.Salt.CopyTo(saltedPassword, passwordBytes.Length);

                // Usamos la función de hash SHA-256 para generar el hash
                SHA256Managed hasher = new SHA256Managed();
                byte[] hashedPassword = hasher.ComputeHash(saltedPassword);

                // Convertimos el hash a una cadena de texto en formato base64
                string encodedHash = Convert.ToBase64String(hashedPassword);

                // Comparamos el hash calculado con el almacenado en la BD
                if (encodedHash == user.Password) return user;
                else return null;
            }
        }
    }
}
