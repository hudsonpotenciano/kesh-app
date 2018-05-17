﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Entidade
{
    public class Cupom
    {
        [Key]
        public Guid Id { get; set; }
        public int IdPessoa { get; set; }
        public int IdCupom { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime Validade { get; set; }
        public Guid Token { get; set; }
    }
}
