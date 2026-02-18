using MediatR;
using SwiftScale.BuildingBlocks;

namespace SwiftScale.Modules.Catalog.Application.UploadImage
{
    public record UploadProductImageCommand(Guid ProductId,
                                            Stream FileStream,
                                            string FileName) : IRequest<Result<string>>;
}
