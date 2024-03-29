﻿using System.ComponentModel.DataAnnotations;
using Models.Interfaces;

namespace Models.Entities;

public class EventStudentRelationship : IEntity
{
    [Key]
    public int Id { get; set; }
        
    public int StudentId { get; set; }
        
    public Student Student { get; set; }

    public int EventId { get; set; }
        
    public Event Event { get; set; }

    public override string ToString()
    {
        return ((IEntity)this).ToJsonString();
    }
}