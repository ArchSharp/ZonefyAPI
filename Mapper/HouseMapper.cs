using AutoMapper;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Entities;

namespace ZonefyDotnet.Mapper
{
    public class HouseMapper:Profile
    {
        public HouseMapper()
        {
            CreateMap<CreateHousePropertyDTO, HouseProperty>();
            CreateMap<HouseProperty, GetHousePropertyDTO>();
            CreateMap<PropertyStatistics, GetPropertyStatisticDTO>();
        }
    }
}
