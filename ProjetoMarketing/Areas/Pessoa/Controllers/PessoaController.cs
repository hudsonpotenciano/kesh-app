﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoMarketing.Areas.Pessoa.Models;
using ProjetoMarketing.Areas.Pessoa.Persistencia;
using ProjetoMarketing.Autentication;
using ProjetoMarketing.Contexts;
using ProjetoMarketing.Controllers;
using ProjetoMarketing.Data;
using ProjetoMarketing.Models;
using ProjetoMarketing.Servicos;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Areas.Pessoa.Controllers
{
    [Produces("application/json")]
    [Route("api/Pessoa/Pessoa")]
    public class PessoaController : ControladorBase
    {
        private readonly PessoaEmpresaContext _context;

        public PessoaController(PessoaEmpresaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("CadastrePessoa")]
        public async Task<RetornoRequestModel> CadastrePessoa([FromBody] ParametrosCadastroPessoa model,
                                                [FromServices]SigningConfigurations signingConfigurations,
                                                [FromServices]TokenConfigurations tokenConfigurations)
        {
            if (_context.Pessoa.Any(p => p.Email == model.Email))
            {
                return RetornoRequestModel.CrieFalhaDuplicidade();
            }

            Entidade.Pessoa.Pessoa pessoa = new Entidade.Pessoa.Pessoa();
            Entidade.ImagemPerfil imagemPerfil = new Entidade.ImagemPerfil();
            Entidade.Usuario usuario = new Entidade.Usuario();

            int resultado = await new PessoaDAO(_context).AddPessoaUsuario(model, out pessoa, out usuario, out imagemPerfil);

            if (resultado > 0)
            {
                new ImagemService(_context).SaveImagemPerfilPessoa(imagemPerfil);

                RetornoRequestModel retorno = new RetornoRequestModel
                {
                    Result = Projecoes.ProjecaoRetornoCadastroPessoaUsuario(usuario, GenerateAcessToken(usuario.Login, signingConfigurations, tokenConfigurations))
                };

                return retorno;
            }

            return RetornoRequestModel.CrieFalha();
        }

        [AllowAnonymous]
        [HttpPost("CadastrePessoaRedeSocial")]
        public async Task<RetornoRequestModel> CadastrePessoaRedeSocial([FromBody] ParametrosCadastroPessoaRedeSocial model,
                                              [FromServices]SigningConfigurations signingConfigurations,
                                              [FromServices]TokenConfigurations tokenConfigurations)
        {
            if (_context.Pessoa.Any(p => p.Email == model.Email))
            {
                return RetornoRequestModel.CrieFalhaDuplicidade();
            }


            Entidade.Pessoa.Pessoa pessoa = new Entidade.Pessoa.Pessoa();
            Entidade.Usuario usuario = new Entidade.Usuario();

            await new PessoaDAO(_context).AddPessoaUsuario(model, out pessoa, out usuario);

            if (usuario.Id > 0)
            {
                RetornoRequestModel retorno = new RetornoRequestModel
                {
                    Result = Projecoes.ProjecaoRetornoCadastroPessoaUsuario(usuario, GenerateAcessToken(usuario.Login, signingConfigurations, tokenConfigurations))
                };

                return retorno;
            }

            return RetornoRequestModel.CrieFalha();
        }

        [Authorize("Bearer")]
        [HttpPost("ObtenhaDadosPessoa")]
        public async Task<RetornoRequestModel> ObtenhaDadosPessoa([FromBody]ParametrosObtenhaDadosPessoa parametros)
        {
            try
            {
                RetornoRequestModel retorno = new RetornoRequestModel
                {
                    Result = Projecoes.DadosPessoa(await new PessoaDAO(_context).Selecione(parametros.IdPessoa))
                };

                return retorno;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        [Authorize("Bearer")]
        [HttpPost("ObtenhaDadosPessoaLojas")]
        public async Task<RetornoRequestModel> ObtenhaDadosPessoaLojas([FromBody]ParametrosObtenhaDadosPessoa parametros)
        {
            try
            {
                RetornoRequestModel retorno = new RetornoRequestModel
                {
                    Result = Projecoes.PessoaLojas(await new PessoaDAO(_context).ObtenhaDadosPessoaLojas(parametros))
                };

                return retorno;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        [Authorize("Bearer")]
        [HttpPost("ObtenhaPessoaEPerfilEmpresas")]
        public async Task<RetornoRequestModel> ObtenhaPessoaEPerfilEmpresas([FromBody]ParametrosObtenhaPessoaEPerfilEmpresas parametros)
        {
            await new PessoaDAO(_context).UpdatePessoaLocalizacao(parametros);

            try
            {
                System.Collections.Generic.List<DTO.DTOPessoa> pessoasEmpreas = await new PessoaDAO(_context).ObtenhaPessoaEmpresas(parametros);
                RetornoRequestModel retorno = new RetornoRequestModel
                {
                    Result = Projecoes.PessoaEmpresas(pessoasEmpreas, parametros.UnidadeDeMedida)
                };

                return retorno;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        [Authorize("Bearer")]
        [HttpPost("ObtenhaComentarioENotaPessoasEmpresas")]
        public async Task<RetornoRequestModel> ObtenhaComentarioENotaPessoasEmpresas([FromBody]ParametrosObtenhaNotasComentarios parametros)
        {

            System.Collections.Generic.List<DTO.DTONotasComentariosPessoasEmpresas> pessoaEmpresas = await new PessoaDAO(_context).ObtenhaComentarioENotaPessoasEmpresas(parametros);

            RetornoRequestModel retorno = new RetornoRequestModel
            {
                Result = Projecoes.NotasEComentariosPessoasEmpresas(pessoaEmpresas)
            };

            return retorno;
        }

        [Authorize("Bearer")]
        [HttpPost("AtualizeDadosPessoaEmpresa")]
        public async Task<RetornoRequestModel> AtualizeDadosPessoaEmpresa([FromBody]ParametrosAtualizeDadosPessoaEmpresa parametros)
        {
            try
            {
                await new PessoaDAO(_context).AddOrUpdatePessoaEmpresa(parametros);
                return RetornoRequestModel.CrieSucesso();
            }
            catch
            {
                return RetornoRequestModel.CrieFalha();
            }
        }

        [Authorize("Bearer")]
        [HttpPost("AltereSenhaPessoa")]
        public RetornoRequestModel AltereSenhaPessoa([FromBody] ParametrosAlteracaoDeSenha parametros)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parametros.NovaSenha))
                {
                    return RetornoRequestModel.CrieFalha();
                }

                RetornoRequestModel retorno = RetornoRequestModel.CrieSucesso();
                string novoToken = new UsuarioService(_context).AltereSenha(parametros, parametros.Token);
                retorno.Result = novoToken;
                return retorno;
            }
            catch
            {
                return RetornoRequestModel.CrieFalha();
            }
        }
    }
}