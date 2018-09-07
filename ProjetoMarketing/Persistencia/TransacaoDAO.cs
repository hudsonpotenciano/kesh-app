﻿using Microsoft.EntityFrameworkCore;
using ProjetoMarketing.Contexts;
using ProjetoMarketing.Entidade;
using ProjetoMarketing.Entidade.Empresa;
using ProjetoMarketing.Entidade.Pessoa;
using ProjetoMarketing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Persistencia
{
    public class TransacaoDAO
    {
        private readonly PessoaEmpresaContext _context;

        public TransacaoDAO(PessoaEmpresaContext context)
        {
            _context = context;
            if (_context.Database.CurrentTransaction != null)
                _context.Database.CurrentTransaction.Commit();
            _context.Database.BeginTransaction();
        }

        public void GereCompartilhamento(ParametrosCompartilhamento parametros, out Compartilhamento compartilhamento)
        {
            compartilhamento = new Compartilhamento()
            {
                IdPerfilEmpresa = parametros.IdPerfilEmpresa,
                IdPessoa = parametros.IdPessoa,
                IdsPessoas = parametros.IdsPessoas,
                Data = DateTime.Today
            };

            _context.Compartilhamento.Add(compartilhamento);
            _context.SaveChanges();
        }

        public Task GereCupom(ParametrosCompartilhamento model, decimal desconto, out Cupom cupom, long idCompartilhamento)
        {

            cupom = new Cupom()
            {
                Desconto = desconto,
                IdPerfilEmpresa = model.IdPerfilEmpresa,
                IdPessoa = model.IdPessoa,
                Data = DateTime.Now,
                IdCompartilhamento = idCompartilhamento
            };

            _context.Cupom.Add(cupom);
            return _context.SaveChangesAsync();
        }

        public Task GereVendaComCupom(ParametrosCupomVenda model, Cupom cupom, out Venda venda)
        {
            venda = new Venda()
            {
                IdCupom = cupom.IdCupom,
                IdPessoa = cupom.IdPessoa,
                IdPerfilEmpresa = cupom.IdPerfilEmpresa,
                Valor = model.ValorDaVenda,
                Data = DateTime.Now
            };

            _context.Venda.Add(venda);
            return _context.SaveChangesAsync();
        }

        public Task<List<DTO.DTOCupomVenda>> ObtenhaCuponsEVendasPessoa(int idPessoa)
        {
            return (from cupom in _context.Cupom.Where(c => c.IdPessoa == idPessoa)
                    let venda = _context.Venda.FirstOrDefault(v => v.IdCupom == cupom.IdCupom)
                    let perfilEmpresa = _context.PerfilEmpresa.FirstOrDefault(e => e.IdPerfilEmpresa == cupom.IdPerfilEmpresa)
                    let valorPontos = _context.ContaEmpresa.FirstOrDefault(c => c.IdEmpresa == perfilEmpresa.IdEmpresa).ValorPontos
                    select new DTO.DTOCupomVenda()
                    {
                        Cupom = cupom,
                        Venda = venda,
                        NomeEmpresa = perfilEmpresa.Descricao,
                        Pontos = venda != null ? Venda.CalculePontos(venda.Valor,valorPontos) : 0
                    }).ToListAsync();
        }

        public Task<List<DTO.DTOCupomVenda>> ObtenhaCuponsEVendasEmpresa(long idPerfilEmpresa)
        {
            return (from cupom in _context.Cupom.Where(c => c.IdPerfilEmpresa == idPerfilEmpresa)
                    let venda = _context.Venda.FirstOrDefault(v => v.IdCupom == cupom.IdCupom)
                    let nomePessoa = _context.Pessoa.FirstOrDefault(p => p.IdPessoa == cupom.IdPessoa).Nome
                    let perfilEmpresa = _context.PerfilEmpresa.FirstOrDefault(e => e.IdPerfilEmpresa == cupom.IdPerfilEmpresa)
                    let valorPontos = _context.ContaEmpresa.FirstOrDefault(c => c.IdEmpresa == perfilEmpresa.IdEmpresa).ValorPontos
                    select new DTO.DTOCupomVenda()
                    {
                        Cupom = cupom,
                        Venda = venda,
                        NomePessoa = nomePessoa,
                        Pontos = venda != null ? Venda.CalculePontos(venda.Valor, valorPontos) : 0
                    }).ToListAsync();
        }

        public Task<Cupom> SelectCupom(Guid token)
        {
            return _context.Cupom.FirstOrDefaultAsync(a => a.Token.Equals(token));
        }

        public Task<Cupom> ObtenhaCupomPeloToken(ParametrosObtenhaCupom parametros)
        {
            return _context.Cupom.FromSql($@"select * from public.cupom
                                                 where cupom.token = {parametros.CupomToken}
                                                 and not exists (select idvenda from venda where venda.idcupom = cupom.idcupom)").FirstOrDefaultAsync();
        }

        public Task<bool> PessoaPodeCompartilhar(ParametrosObtenhaPessoasCompartilhamento parametros)
        {
            return _context.Compartilhamento.FromSql($@"select idcompartilhamento from compartilhamento where compartilhamento.idpessoa = {parametros.IdPessoa}
                                                          and compartilhamento.idperfilempresa = {parametros.IdPerfilEmpresa}
                                                          and compartilhamento.data >= {DateTime.Today}").AnyAsync();
        }
    }
}
