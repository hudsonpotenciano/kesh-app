﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProjetoMarketing.Autentication.Context;
using Microsoft.AspNetCore.Authorization;
using ProjetoMarketing.Areas.Pessoa.Persistencia;
using ProjetoMarketing.Models;
using ProjetoMarketing.Controllers;
using ProjetoMarketing.Areas.Empresa.Models;
using ProjetoMarketing.Autentication;
using ProjetoMarketing.Data;
using System.Threading.Tasks;
using ProjetoMarketing.Contexts;

namespace ProjetoMarketing.Areas.Empresa.Controllers
{
    [Produces("application/json")]
    [Route("api/Empresa/Empresa")]
    public class EmpresaController : ControladorBase
    {
        private readonly PessoaEmpresaContext _context;
        private readonly UsuarioContext _contextUsuario;

        public EmpresaController(PessoaEmpresaContext context, UsuarioContext usuarioContext)
        {
            _context = context;
            _contextUsuario = usuarioContext;
        }

        [AllowAnonymous]
        [HttpPost("CadastreEmpresa")]
        public RetornoRequestModel CadastreEmpresa([FromBody]CadastroEmpresaModel model,
                                                [FromServices]SigningConfigurations signingConfigurations,
                                                [FromServices]TokenConfigurations tokenConfigurations)
        {
            if (_context.Empresa.Any(e => e.Cnpj == model.Cnpj || e.Email == model.Email))
            {
                return RetornoRequestModel.CrieFalhaDuplicidade();
            }

            var retorno = new RetornoRequestModel();

            var empresa = new Entidade.Empresa.Empresa();
            new Persistencia.EmpresaDAO(_context).AddEmpresa(model, out empresa);

            var usuario = new Entidade.Usuario()
            {
                IdEmpresa = empresa.IdEmpresa,
                Login = model.Email,
                Senha = model.Senha
            };

            if (empresa.IdEmpresa != 0)
            {
                new UsuarioDAO(_contextUsuario).Add(usuario);
                var user = new User(usuario.Login, usuario.Senha);

                retorno.Result = Projecoes.ProjecaoRetornoCadastroUsuarioEmpresa(usuario, GenerateAcessToken(user, signingConfigurations, tokenConfigurations));
            }

            return retorno;
        }

        [Authorize("Bearer")]
        [HttpPost("ObtenhaEmpresas")]
        public async Task<RetornoRequestModel> ObtenhaEmpresas([FromBody]ParametrosObtenhaEmpresas parametros)
        {
            if (!EstaAutenticado(_contextUsuario, parametros.Token))
                return RetornoRequestModel.CrieFalhaLogin();

            var retorno = new RetornoRequestModel
            {
                Result = Projecoes.ProjecaoEmpresas(await new Persistencia.EmpresaDAO(_context).SelectEmpresas())
            };

            return retorno;
        }
    }
}