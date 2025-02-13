﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Dto;

public class OrderDTO
{
    [Required]
    public int ?IdOrder { get; set; }
    
    [Required]
    public int ?IdProduct { get; set; }
    
    [Required]
    public int ?Amount { get; set; }
    
    [Required]
    public DateTime ?CreatedAt { get; set; }
    
    public DateTime ?FulfilledAt { get; set; }
}