﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;
using TOPDER.Service.IServices;

namespace TOPDER.Service.Services
{
    public class OrderTableService : IOrderTableService
    {
        private readonly IMapper _mapper;
        private readonly IOrderTableRepository _orderTableRepository;

        public OrderTableService(IOrderTableRepository orderTableRepository, IMapper mapper)
        {
            _orderTableRepository = orderTableRepository;
            _mapper = mapper;
        }
        public async Task<bool> AddAsync(List<CreateOrUpdateOrderTableDto> orderTableDtos)
        {
            var orderTables = _mapper.Map<List<OrderTable>>(orderTableDtos);
            return await _orderTableRepository.CreateRangeAsync(orderTables);
        }

        public async Task<List<OrderTableDto>> GetItemsByOrderAsync(int id)
        {
            var queryable = await _orderTableRepository.QueryableAsync();

            var query = queryable.Where(x => x.OrderId == id);

            var queryDTO = await query.Select(r => _mapper.Map<OrderTableDto>(r)).ToListAsync();

            return queryDTO; 
        }

    }
}