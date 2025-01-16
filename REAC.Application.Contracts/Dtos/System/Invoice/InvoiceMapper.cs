using AutoMapper;
using REAC.EntityFrameworkCore.Entities.Invoice;
using System.Globalization;

namespace REAC.Application.Contracts.Dtos.System.Invoice
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<SysInvoice, InvoiceDto>().ReverseMap();
            CreateMap<SysInvoice, CreateInvoiceDto>().ReverseMap();
            CreateMap<SysInvoice, CreateInvoiceDto>().ReverseMap();
            CreateMap<SysInvoiceStatus, SysInvoiceStatusDto>()
                 .ForMember(des => des.Name,
                                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                     string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Name)) ?
                                     src.Name : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Name)).ReverseMap();
            CreateMap<SysInvoiceType, SysInvoiceTypeDto>()
                .ForMember(des => des.Name,
                                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                     string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Name)) ?
                                     src.Name : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Name)).ReverseMap();
        }
    }
}
