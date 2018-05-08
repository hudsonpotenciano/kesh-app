﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoMarketing.Entidade.Empresa
{
    public class ImagensEmpresa
    {
        [Key]
        public string Id { get; set; }
        public int IdEmpresa { get; set; }
        public string Imagem { get; set; }
        public int Tipo { get; set; }
    }
}