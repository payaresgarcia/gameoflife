using AutoMapper;
using GameOfLife.Dtos;
using Newtonsoft.Json;

namespace GameOfLife.Api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DAL.Models.Board, Dtos.Board>()
                .ForMember(dest => dest.State, opt => opt.MapFrom<BoardStateResolver>());

            CreateMap<BoardDto, DAL.Models.Board>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CurrentState, opt => opt.MapFrom<BoardDtoStateResolver>());
        }

        /// <summary>
        /// Custom resolver from DAL.Models.Board to Dtos.Board for state property.
        /// </summary>
        private class BoardStateResolver : IValueResolver<DAL.Models.Board, Dtos.Board, int[,]>
        {
            public int[,] Resolve(DAL.Models.Board source, Dtos.Board destination, int[,] member, ResolutionContext context)
            {
                return JsonConvert.DeserializeObject<int[,]>(source.CurrentState)!;
            }
        }

        /// <summary>
        /// Custom resolver from Dtos.BoardDto to DAL.Models.Board for state property.
        /// </summary>
        private class BoardDtoStateResolver : IValueResolver<BoardDto, DAL.Models.Board, string>
        {
            public string Resolve(BoardDto source, DAL.Models.Board destination, string member, ResolutionContext context)
            {
                return JsonConvert.SerializeObject(source.State);
            }
        }
    }
}
