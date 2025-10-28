using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace CareerPoint.Application.Mappings;

public class IFormFileToByteArrayConverter : IValueConverter<IFormFile?, byte[]?>
{
    public byte[]? Convert(IFormFile? sourceMember, ResolutionContext context)
    {
        if (sourceMember is null || sourceMember.Length == 0)
            return null;

        using MemoryStream memoryStream = new();
        sourceMember.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
