﻿using ProjetoMarketing.Contexts;
using ProjetoMarketing.Entidade;
using ProjetoMarketing.Models;
using ProjetoMarketing.Persistencia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Servicos
{
    public class TransacaoService
    {
        public static TransacaoService Instancia => new TransacaoService();

        public async Task<Cupom> GereCupomCompartilhamento(ParametrosCodigoCompartilhamento parametros, PessoaEmpresaContext _context)
        {
            if (string.IsNullOrEmpty(parametros.Codigo))
            {
                return null;
            }

            //Compartilhamento compartilhamento = _context.Compartilhamento.FirstOrDefault(c => c.Codigo == parametros.Codigo &&
            //                                                        c.IdPessoa != parametros.IdPessoaReceptor && !c.CupomFoiGerado);
            Compartilhamento compartilhamento = _context.Compartilhamento.FirstOrDefault(c => c.Codigo == parametros.Codigo);
            if (compartilhamento != null)
            {
                var transacaoDao = new TransacaoDAO(_context);

                Cupom cupom = new Cupom();
                await transacaoDao.GereCupom(parametros, out cupom, compartilhamento);

                if (cupom.IdCupom.Equals(Guid.Empty))
                {
                    return null;
                }

                compartilhamento.CupomFoiGerado = true;
                await transacaoDao.UpdateCompartilhamento(compartilhamento);
                NotificacaoService.Instancia.EnvieNotificacaoDeCompartilhamento(compartilhamento, _context);
                return cupom;
            }
            else
            {
                return null;
            }
        }

        public async Task<Compartilhamento> GereCompartilhamento(ParametrosCompartilhamento parametros, PessoaEmpresaContext _context)
        {
            try
            {
                Compartilhamento compartilhamento = new Compartilhamento();
                await new TransacaoDAO(_context).GereCompartilhamento(parametros, out compartilhamento);
                return compartilhamento;
            }
            catch (System.Exception)
            {
                return null;
                throw;
            }
        }
    }
}
