using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Rating
{
    
    public double Rate { get; set; }

    //[JsonIgnore]
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Count { get;  set; }

    public void IncrementCount()
    {
        Count++;
    }

    public void DecrementCount()
    {
        Count--;
    }

    public void ResetCount()
    {
        Count = 1;
    }
}
