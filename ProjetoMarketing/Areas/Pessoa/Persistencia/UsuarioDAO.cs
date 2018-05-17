﻿
using ProjetoMarketing.Areas.Pessoa.Context;
using ProjetoMarketing.Autentication;
using ProjetoMarketing.Autentication.Context;
using ProjetoMarketing.Entidade;
using System.Linq;

namespace ProjetoMarketing.Areas.Pessoa.Persistencia
{
    public class UsuarioDAO
    {
        private readonly UsuarioContext _context;

        public UsuarioDAO(UsuarioContext context)
        {
            _context = context;
        }

        public Usuario Find(User usuario)
        {
            var token = Seguranca.GerarHashMd5(usuario.Login, usuario.Senha);
            return _context.Usuario.FirstOrDefault(u => u.Token == token);
        }

        public bool Validate(string token)
        {
            return _context.Usuario.FirstOrDefault(u => u.Token == token) != null;
        }


        public void Add(Usuario usuario)
        {
            usuario.Token = Seguranca.GerarHashMd5(usuario.Login, usuario.Senha);
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public void Remove(Usuario usuario)
        {
            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
        }
    }
}
