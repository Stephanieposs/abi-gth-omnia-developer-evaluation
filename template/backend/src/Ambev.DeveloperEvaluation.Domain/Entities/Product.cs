﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }

    public string Image {  get; set; }

    public Rating Rating { get; set; }

    

    
}
