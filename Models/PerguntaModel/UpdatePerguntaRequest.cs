﻿using System.ComponentModel.DataAnnotations;

namespace AppShowDoMilhao.Models
{
    public class PerguntaUpdateRequest
    {
        [Required]
        public int IdPergunta { get; set; }

        [Required]
        public string? Enunciado { get; set; }

        [Required]
        [StringLength(255)]
        public string? AlternativaA { get; set; }

        [Required]
        [StringLength(255)]
        public string? AlternativaB { get; set; }

        [Required]
        [StringLength(255)]
        public string? AlternativaC { get; set; }

        [Required]
        [StringLength(255)]
        public string? AlternativaD { get; set; }

        [Required]
        [StringLength(255)]
        public string? RespostaCorreta { get; set; }
    }
}
