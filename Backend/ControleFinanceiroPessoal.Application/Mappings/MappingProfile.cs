using AutoMapper;
using ControleFinanceiro.API.DTOs;
using ControleFinanceiroPessoal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ControleFinanceiroPessoal.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movimentacao mappings
            CreateMap<Movimentacao, MovimentacaoDto>();
            CreateMap<CreateMovimentacaoDto, Movimentacao>();
            CreateMap<UpdateMovimentacaoDto, Movimentacao>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Inclusao, opt => opt.Ignore())
                .ForMember(dest => dest.Alteracao, opt => opt.Ignore());
        }
    }
}
