using System;
using BusinessLogic.Models;
using Mapster;
using SkyNile.DTO;

namespace SkyNile.MappingConfig;

public class FlightMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<FlightAdminUpdateDTO, Flight>()
            .Map(dest => dest.DepartureTime, src => src.DepartureTime)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.ArrivalTime, src => src.ArrivalTime);
    }
}
