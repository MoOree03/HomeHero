﻿using HomeHero.Data;
using HomeHero.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace HomeHero.Services
{
    public class HHeroServices
    {
        readonly HomeHeroContext _context;
        public HHeroEncrypt HHeroEncrypt;
        public HHeroServices(HomeHeroContext context)
        {
            _context = context;
            HHeroEncrypt = new HHeroEncrypt(context);
        }
        
    }
}