﻿using System;

namespace AppShowDoMilhao.Models.PerguntaModel
{
    public class PerguntaResponse
    {
        public int IdPergunta { get; set; }
        public string Enunciado { get; set; }
        public string AlternativaA { get; set; }
        public string AlternativaB { get; set; }
        public string AlternativaC { get; set; }
        public string AlternativaD { get; set; }
        public string RespostaCorreta { get; set; }
    }
}
