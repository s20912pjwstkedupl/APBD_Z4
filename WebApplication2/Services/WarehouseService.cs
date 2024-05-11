using System.ComponentModel.DataAnnotations;
using WebApplication2.Dto;
using WebApplication2.Exceptions;
using WebApplication2.Repositories;
using ValidationException = WebApplication2.Exceptions.ValidationException;

namespace WebApplication2.Services;

public interface IWarehouseService
{
    public Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto);
}

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    // private readonly IProductRepository _productRepository;
    public WarehouseService(IWarehouseRepository warehouseRepository, IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _warehouseRepository = warehouseRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }
    
    public async Task<int> RegisterProductInWarehouseAsync(RegisterProductInWarehouseRequestDTO dto)
    {
        if (!await _productRepository.HasProductWithIdAsync(dto.IdProduct!.Value))
           throw new NotFoundException("Product not found");
        
        if (!await _warehouseRepository.HasWarehouseWithIdAsync(dto.IdWarehouse!.Value))
           throw new NotFoundException("Warehouse not found");
        
        if(dto.Amount <= 0)
            throw new ValidationException("Amount must be greater than 0");

        var idOrder = await _orderRepository.GetByProductAndCountAndBeforeDateAsync(dto.IdProduct!.Value,
            dto.Amount!.Value, dto.CreatedAt!.Value);
        
        if (idOrder < 0)
            throw new NotFoundException("Order not found");
        
        if (await _warehouseRepository.HasOrderInWarehouseAsync(idOrder, dto.IdWarehouse!.Value))
            throw new ConflictException("Order was already fulfilled");
        
        var idProductWarehouse = await _warehouseRepository.RegisterProductInWarehouseAsync(
            idWarehouse: dto.IdWarehouse!.Value,
            idProduct: dto.IdProduct!.Value,
            idOrder: idOrder,
            createdAt: DateTime.UtcNow);
        
        if (!idProductWarehouse.HasValue)
            throw new Exception("Failed to register product in warehouse");

        var price = await _productRepository.GetPriceByIdAsync(dto.IdProduct!.Value);
        await _warehouseRepository.UpdateProductWarehousePriceAndAmount(idProductWarehouse!.Value, price, dto.Amount!.Value);
        
        return idProductWarehouse.Value;
    }
}