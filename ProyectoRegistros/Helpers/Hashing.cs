using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using ProyectoRegistros.Models;

namespace ProyectoRegistros.Helpers
{
    public class Hashing
    {
        public ProyectoregistroContext Context { get; }

        public Hashing(ProyectoregistroContext context)
        {
            Context = context;
        }
        public static string GenerarHash(string contraseña)
        {
            contraseña = contraseña + "V2025";
            byte[] datos = Encoding.UTF8.GetBytes(contraseña);
            var alg = SHA256.Create();
            byte[] encriptar = alg.ComputeHash(datos);
            string salida = BitConverter.ToString(encriptar).Replace("-", "");
            return salida;
        }
    }
}
