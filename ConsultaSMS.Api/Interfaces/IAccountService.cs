using System.Threading.Tasks;
using WebConsultaSMS.Models.Requests;
using WebConsultaSMS.Models.Responses;

namespace WebConsultaSMS.Interfaces
{
    public interface IRequestService
    {
        public Task<AccountProductResponse> GetProductAsync(ProductRequest filters);
        public Task<Response<DetailAccountResponse>> GetDetailAccount(ProductDetailRequest filters);
        public Task<Response<DetailCertificadoBankResponse>> GetDetailCertificados(ProductDetailRequest filters);
        public Task<Response<DetailPrestamosResponse>> GetDetailPrestamos(ProductDetailRequest filters);
        public Task<Response<DetailTarjetasCreditoResponse>> GetDetailTarjetaCreditos(ProductDetailRequest filters);
        public Task IncrementSMSCounter(string Telephone);
    }
}