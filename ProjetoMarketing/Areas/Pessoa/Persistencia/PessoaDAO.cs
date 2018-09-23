﻿using Microsoft.EntityFrameworkCore;
using ProjetoMarketing.Autentication;
using ProjetoMarketing.Contexts;
using ProjetoMarketing.DTO;
using ProjetoMarketing.Entidade;
using ProjetoMarketing.Entidade.Pessoa;
using ProjetoMarketing.Models;
using ProjetoMarketing.Negocio;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Areas.Pessoa.Persistencia
{
    public class PessoaDAO
    {
        private readonly PessoaEmpresaContext _context;

        public PessoaDAO(PessoaEmpresaContext context)
        {
            _context = context;
            if (_context.Database.CurrentTransaction != null)
            {
                _context.Database.CurrentTransaction.Commit();
            }

            _context.Database.BeginTransaction();
        }

        public Task AddPessoaUsuario(Models.ParametrosCadastroPessoa model, out Entidade.Pessoa.Pessoa pessoa, out Usuario usuario)
        {
            pessoa = new Entidade.Pessoa.Pessoa()
            {
                Email = model.Email,
                Nome = model.Nome,
            };

            //Necessário para obter o IDPESSOA
            _context.Pessoa.Add(pessoa);
            _context.SaveChanges();

            ImagemPerfil imagemPerfil = new ImagemPerfil()
            {
                Imagem = model.Foto != null ? Convert.FromBase64String(model.Foto) : null,
                IdPessoa = pessoa.IdPessoa
            };

            usuario = new Usuario()
            {
                IdPessoa = pessoa.IdPessoa,
                Login = model.Email,
                Token = Seguranca.GerarHashMd5(model.Email, model.Senha)
            };

            _context.Usuario.Add(usuario);
            _context.ImagemPerfil.Add(imagemPerfil);
            return _context.SaveChangesAsync();
        }

        public Task AddPessoaUsuario(Models.ParametrosCadastroPessoaRedeSocial model, out Entidade.Pessoa.Pessoa pessoa, out Usuario usuario)
        {
            pessoa = new Entidade.Pessoa.Pessoa()
            {
                Email = model.Email,
                Nome = model.Nome,
            };

            //Necessário para obter o IDPESSOA
            _context.Pessoa.Add(pessoa);
            _context.SaveChanges();

            ImagemPerfil imagemPerfil = new ImagemPerfil()
            {
                Imagem = model.Foto != null ? Convert.FromBase64String(model.Foto) : null,
                IdPessoa = pessoa.IdPessoa
            };

            usuario = new Usuario()
            {
                IdPessoa = pessoa.IdPessoa,
                Login = model.Email,
                Token = Seguranca.GerarHashMd5(model.Email, model.Id),
                RedeSocial = true
            };

            _context.Usuario.Add(usuario);
            _context.ImagemPerfil.Add(imagemPerfil);
            return _context.SaveChangesAsync();
        }

        public void Remove(Entidade.Pessoa.Pessoa pessoa)
        {
            _context.Pessoa.Remove(pessoa);
            _context.SaveChanges();
        }

        public Task<Entidade.Pessoa.Pessoa> Select(int idPessoa)
        {
            return _context.Pessoa.FirstOrDefaultAsync(p => p.IdPessoa == idPessoa);
        }

        public Task UpdatePessoaLocalizacao(ParametrosObtenhaPessoaEPerfilEmpresas parametros)
        {
            Entidade.Pessoa.Pessoa pessoa = _context.Pessoa.FirstOrDefault(p => p.IdPessoa == parametros.IdPessoa);
            if (pessoa == null)
            {
                return null;
            }

            pessoa.Latitude = parametros.Latitude;
            pessoa.Longitude = parametros.Longitude;

            _context.Pessoa.Update(pessoa);
            return _context.SaveChangesAsync();
        }

        public Task AddOrUpdatePessoaEmpresa(ParametrosAtualizeDadosPessoaEmpresa parametros)
        {
            PessoaEmpresa pessoaEmpresaBd = _context.PessoaEmpresa.FirstOrDefault(p => p.IdPessoa == parametros.IdPessoa && p.IdPerfilEmpresa == parametros.IdPerfilEmpresa);
            if (pessoaEmpresaBd != null)
            {
                pessoaEmpresaBd.Comentario = parametros.Comentario;
                pessoaEmpresaBd.Nota = parametros.Nota;
                pessoaEmpresaBd.DataAvaliacao = DateTime.Now;
                _context.PessoaEmpresa.Update(pessoaEmpresaBd);
                return _context.SaveChangesAsync();
            }
            else
            {
                PessoaEmpresa PessoaEmpresa = new PessoaEmpresa()
                {
                    IdPerfilEmpresa = parametros.IdPerfilEmpresa,
                    IdPessoa = parametros.IdPessoa,
                    Nota = parametros.Nota,
                    DataAvaliacao = DateTime.Now,
                    Comentario = parametros.Comentario
                };

                _context.PessoaEmpresa.Add(PessoaEmpresa);
                return _context.SaveChangesAsync();
            };
        }

        public Task<List<DTO.DTOPessoa>> ObtenhaPessoaEmpresas(ParametrosObtenhaPessoaEPerfilEmpresas parametros)
        {
            //OBTEM EMPRESAS NO RAIO DE 20KM
            return (from perfil in _context.PerfilEmpresa
                    let distancia = Negocio.Localizacao.DistanceTo(parametros.Latitude, parametros.Longitude, perfil.Latitude, perfil.Longitude, parametros.UnidadeDeMedida)
                    join pessoasEmpresa in _context.PessoaEmpresa on perfil.IdPerfilEmpresa equals pessoasEmpresa.IdPerfilEmpresa
                    join imagensCatalogo in _context.ImagemCatalogo on perfil.IdPerfilEmpresa equals imagensCatalogo.IdPerfilEmpresa
                    join empresa in _context.Empresa on perfil.IdEmpresa equals empresa.IdEmpresa
                    join conta in _context.ContaEmpresa on empresa.IdEmpresa equals conta.IdEmpresa
                    join pessoaEmpresa in _context.PessoaEmpresa on perfil.IdPerfilEmpresa equals pessoaEmpresa.IdPerfilEmpresa
                    //let countNota = _context.PessoaEmpresa.Count(p => p.IdPerfilEmpresa == idPerfilEmpresa && p.Nota != null)
                    //let notaGeral = _context.PessoaEmpresa
                    //                .Where(p => p.IdPerfilEmpresa == idPerfilEmpresa)
                    //                .Sum(p => p.Nota) / (countNota > 0 ? countNota : 1)
                    where pessoaEmpresa.IdPessoa == parametros.IdPessoa &&  distancia < 20
                    select new DTO.DTOPessoa()
                    {
                        Empresa = empresa,
                        Catalogo = imagensCatalogo,
                        ContaEmpresa = conta,
                        PerfilEmpresa = perfil,
                        PessoaEmpresa = pessoaEmpresa,
                        NotaGeral = 1,
                        Distancia = distancia
                    }).ToListAsync();
        }

        public Task<List<DTOPessoaLoja>> ObtenhaDadosPessoaLojas(ParametrosObtenhaDadosPessoa parametros)
        {
            return (from pessoaLoja in _context.PessoaLoja.Where(a => a.IdPessoa == parametros.IdPessoa)
                    let loja = _context.PerfilEmpresa.FirstOrDefault(a => a.IdPerfilEmpresa == pessoaLoja.IdPerfilEmpresa)
                    let conta = _context.ContaEmpresa.First(a => a.IdEmpresa == loja.IdEmpresa)
                    select new DTOPessoaLoja
                    {
                        Loja = loja,
                        Pontos = pessoaLoja.Pontos,
                        PontosEmDinheiro = Pontos.CalculePontos(pessoaLoja.Pontos,conta.ValorPontos)
                    }).ToListAsync();
        }

        public Task<List<DTO.DTONotasComentariosPessoasEmpresas>> ObtenhaComentarioENotaPessoasEmpresas(ParametrosObtenhaNotasComentarios parametros)
        {
            return (from pe in _context.PessoaEmpresa.Where(p => p.IdPerfilEmpresa == parametros.IdPerfilEmpresa)
                    join p in _context.Pessoa on pe.IdPessoa equals p.IdPessoa
                    select new DTO.DTONotasComentariosPessoasEmpresas()
                    {
                        Comentario = pe.Comentario,
                        Nota = pe.Nota,
                        IdPessoa = p.IdPessoa,
                        Nome = p.Nome,
                        DataAvaliacao = pe.DataAvaliacao
                    }).ToListAsync();
        }

        public Task<List<Entidade.Pessoa.Pessoa>> ObtenhaPessoasCompartilhamento(ParametrosObtenhaPessoasCompartilhamento parametros)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            RawSqlString sql = $@"select * from pessoa where (select public.geodistance(cast('{parametros.Latitude.ToString(nfi)}' as double precision), cast('{parametros.Longitude.ToString(nfi)}' as double precision),latitude,longitude) < 50)
                                                              and not exists (select idcupom from cupom where idpessoa = public.pessoa.idpessoa
                                                              and idperfilempresa = {parametros.IdPerfilEmpresa}
                                                              and cupom.data >= '{DateTime.Today.AddDays(-10).ToString("yyyy-MM-dd")}')";

            return (from a in _context.Pessoa.FromSql(sql)
                    select new Entidade.Pessoa.Pessoa()
                    {
                        Nome = a.Nome,
                        Email = a.Email,
                        IdPessoa = a.IdPessoa,
                    }).ToListAsync();
        }
    }
}
